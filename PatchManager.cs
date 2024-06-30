using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encryption_Rijndae;
using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class PatchManager : MonoBehaviour
{
	private struct PatchInfo
	{
		public string address;

		public string fileName;

		public int version;

		public long size;
	}

	private readonly string PATCHFILE_NAME = "patchlist.assetbundle";

	private readonly string ZIPPATCHFILE_NAME = "zippatchlist.assetbundle";

	private readonly string PATCHDBFILE_NAME = "DBList.txt";

	private readonly string PATCHBADWORDFILE_NAME = "BadWordList.json";

	private string _PATCH_URL = "http://gkcdn.dbros.co.kr/";

	private string _PATCH_URL_SUB = "http://gk.flerogames.com:8080/cdn/";

	private string _serverUrl;

	public readonly string PLATFROM = "Android/";

	public string DB_DIRECTORY = "Test_Patch_DB/";

	private Dictionary<string, PatchInfo> _dicPatchInfo = new Dictionary<string, PatchInfo>();

	private Dictionary<string, PatchInfo> _dicVoicePatchInfo = new Dictionary<string, PatchInfo>();

	private byte[] _bytesPatchList;

	private byte[] _bytesZipList;

	private int _nLocalVersion;

	private int _nNextLocalVersion;

	private int _nZipLocalVersion;

	private int _nZipNextVersion;

	private bool _isFinished;

	private bool _isSuccess;

	private int _nRepatchCount;

	private long patchFileSize;

	private List<string> arrDBFileName = new List<string>();

	private Dictionary<string, int> arrDBPatchFileName = new Dictionary<string, int>();

	private UIProgressBar progress;

	private UILabel labelName;

	private float fDownCount;

	private static PatchManager _instance;

	private string _strDefaultDataPath = string.Empty;

	public string SERVER_URL
	{
		get
		{
			return _serverUrl;
		}
		set
		{
			_serverUrl = value;
			_PATCH_URL = value;
		}
	}

	public static PatchManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(PatchManager)) as PatchManager;
				if (_instance == null)
				{
					_instance = new GameObject("PatchManager").AddComponent<PatchManager>();
					_instance.SERVER_URL = _instance._PATCH_URL;
					if (RemoteObjectManager.instance.localUser != null)
					{
						if (RemoteObjectManager.instance.localUser.channel > 1)
						{
							_instance.DB_DIRECTORY = "Test_Patch_DB_" + RemoteObjectManager.instance.localUser.channel + "/";
						}
						else
						{
							_instance.DB_DIRECTORY = "Test_Patch_DB/";
						}
					}
				}
			}
			return _instance;
		}
	}

	public string GetPatchUrl => SERVER_URL;

	public string DefaultDataPath
	{
		get
		{
			if (_strDefaultDataPath == string.Empty)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (Application.isEditor)
				{
					stringBuilder.Append(Application.dataPath);
					stringBuilder.Append("/../Androidcache/");
				}
				else
				{
					stringBuilder.Append(Application.persistentDataPath + "/");
				}
				_strDefaultDataPath = stringBuilder.ToString();
			}
			return _strDefaultDataPath;
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public IEnumerator StartPatch()
	{
		StartCoroutine("RunPatch");
		while (!_isFinished)
		{
			yield return null;
		}
	}

	public IEnumerator RunPatch(UILabel label, UIProgressBar progressBar)
	{
		_PATCH_URL = SERVER_URL;
		labelName = label;
		progress = progressBar;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		yield return StartCoroutine(CheckLocalVersion(isZip: false));
		_dicPatchInfo.Clear();
		_dicVoicePatchInfo.Clear();
		yield return StartCoroutine(CheckServerVersion($"{_PATCH_URL}{PLATFROM}{PATCHFILE_NAME}", isZip: false));
		bool bStopCoroutine = false;
		if (_dicPatchInfo.Count > 0)
		{
			bStopCoroutine = true;
			UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("19005"), Localization.Get("19004"), (patchFileSize <= 0) ? null : Localization.Format("28070", GetFileSize(patchFileSize)), Localization.Get("19003"), Localization.Get("1000"));
			UIEventTrigger component = uISimplePopup.transform.Find("Block").GetComponent<UIEventTrigger>();
			if (component != null)
			{
				component.onClick.Clear();
			}
			uISimplePopup.onClick = delegate(GameObject go)
			{
				string text2 = go.name;
				if (text2 == "OK")
				{
					bStopCoroutine = false;
					AdjustManager.Instance.SimpleEvent("5t9xhs");
				}
			};
			uISimplePopup.onClose = delegate
			{
				if (bStopCoroutine)
				{
					Application.Quit();
				}
			};
		}
		while (bStopCoroutine)
		{
			yield return null;
		}
		yield return StartCoroutine(StartPatchUpdate(isZip: false));
		AdjustManager.Instance.SimpleEvent("yh5kvw");
		bool bVoiceDown = false;
		bStopCoroutine = false;
		if (_dicVoicePatchInfo.Count > 0)
		{
			bStopCoroutine = true;
			UISimplePopup uISimplePopup2 = UISimplePopup.CreateBool(localization: true, "19005", "19083", null, "19003", "1000");
			UIEventTrigger component2 = uISimplePopup2.transform.Find("Block").GetComponent<UIEventTrigger>();
			if (component2 != null)
			{
				component2.onClick.Clear();
			}
			uISimplePopup2.onClick = delegate(GameObject go)
			{
				string text = go.name;
				if (text == "OK")
				{
					PlayerPrefs.SetInt("VoiceDownState", 1);
					bVoiceDown = true;
					bStopCoroutine = false;
				}
			};
			uISimplePopup2.onClose = delegate
			{
				if (bStopCoroutine)
				{
					PlayerPrefs.SetInt("VoiceDownState", 2);
					bVoiceDown = false;
					bStopCoroutine = false;
				}
			};
		}
		while (bStopCoroutine)
		{
			yield return null;
		}
		if (bVoiceDown)
		{
			yield return StartCoroutine(StartPatchUpdate(isZip: false, isVoice: true));
		}
		UpdateListFile(isZip: false);
		Resources.UnloadUnusedAssets();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		_isFinished = true;
		yield return null;
	}

	private IEnumerator CheckLocalVersion(bool isZip)
	{
		string url = string.Empty;
		string filePath2 = string.Empty;
		if (isZip)
		{
			url = $"file://{DefaultDataPath}/{ZIPPATCHFILE_NAME}";
			filePath2 = $"{DefaultDataPath}/{ZIPPATCHFILE_NAME}";
		}
		else
		{
			url = $"file://{DefaultDataPath}/{PATCHFILE_NAME}";
			filePath2 = $"{DefaultDataPath}/{PATCHFILE_NAME}";
		}
		if (File.Exists(filePath2))
		{
			AssetBundle bundle = null;
			using (WWW www = new WWW(url))
			{
				yield return www;
				if (www.error == null)
				{
					bundle = www.assetBundle;
				}
			}
			if (bundle != null)
			{
				try
				{
					TextAsset textAsset = UnityEngine.Object.Instantiate(bundle.mainAsset) as TextAsset;
					string[] array = textAsset.text.Split('\n');
					if (array.Length > 1)
					{
						if (isZip)
						{
							_nZipLocalVersion = int.Parse(array[0]);
						}
						else
						{
							_nLocalVersion = int.Parse(array[0]);
						}
					}
				}
				finally
				{
				}
				bundle.Unload(unloadAllLoadedObjects: true);
			}
		}
		else
		{
			_nLocalVersion = 1;
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	private IEnumerator CheckServerVersion(string url, bool isZip)
	{
		using (WWW www = new WWW(URLAntiCacheRandomizer(url)))
		{
			yield return www;
			if (www.error == null)
			{
				try
				{
					TextAsset textAsset = UnityEngine.Object.Instantiate(www.assetBundle.mainAsset) as TextAsset;
					string[] array = textAsset.text.Split('\n');
					if (array.Length > 1)
					{
						if (isZip)
						{
							_bytesZipList = www.bytes;
							_nZipNextVersion = int.Parse(array[0]);
						}
						else
						{
							_bytesPatchList = www.bytes;
							_nNextLocalVersion = int.Parse(array[0]);
						}
						MakePatchList(array, isZip);
					}
					www.assetBundle.Unload(unloadAllLoadedObjects: true);
				}
				catch
				{
					if (!isZip)
					{
					}
				}
			}
			else
			{
				StopCoroutine(CheckServerVersion(url, isZip));
				yield return new WaitForSeconds(2f);
				yield return StartCoroutine(CheckServerVersion(url, isZip));
			}
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	private void MakePatchList(string[] data, bool isZip)
	{
		int num = 0;
		int num2 = 0;
		if (isZip)
		{
			num = _nZipLocalVersion;
			num2 = _nZipNextVersion;
		}
		else
		{
			num = _nLocalVersion;
			num2 = _nNextLocalVersion;
		}
		patchFileSize = 0L;
		string[] array = Application.version.Split('.');
		if (array.Length > 1)
		{
			int num3 = int.Parse(array[1]);
			if (num3 > PlayerPrefs.GetInt("GameVersion", 0))
			{
				RemoveAllPatchFile();
				PlayerPrefs.SetInt("GameVersion", num3);
			}
		}
		if (num > num2)
		{
			RemoveAllPatchFile();
		}
		for (int i = 1; i < data.Length; i++)
		{
			string text = data[i];
			text = text.Replace("\r", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			PatchInfo value = default(PatchInfo);
			string[] array2 = text.Split('\t');
			value.address = array2[0];
			value.fileName = array2[1];
			value.version = int.Parse(array2[2]);
			if (array2.Length > 3 && !string.IsNullOrEmpty(array2[3]))
			{
				value.size = long.Parse(array2[3]);
			}
			string path = $"{DefaultDataPath}{value.address}/{value.fileName}";
			if (!File.Exists(path))
			{
				_dicPatchInfo.Add(value.fileName, value);
			}
			else
			{
				if (num2 <= num || num2 < value.version || (isZip && num >= value.version) || (!isZip && ((_nZipNextVersion <= _nLocalVersion) ? _nLocalVersion : _nZipNextVersion) >= value.version))
				{
					continue;
				}
				if (_dicPatchInfo.ContainsKey(value.fileName))
				{
					if (value.version > _dicPatchInfo[value.fileName].version)
					{
						_dicPatchInfo.Remove(value.fileName);
						_dicPatchInfo.Add(value.fileName, value);
					}
				}
				else
				{
					_dicPatchInfo.Add(value.fileName, value);
				}
			}
		}
		foreach (KeyValuePair<string, PatchInfo> item in _dicPatchInfo)
		{
			patchFileSize += item.Value.size;
		}
	}

	private IEnumerator StartPatchUpdate(bool isZip, bool isVoice = false)
	{
		fDownCount = 0f;
		if (!isVoice)
		{
			if (_dicPatchInfo.Count <= 0)
			{
				yield break;
			}
			if (progress != null)
			{
				progress.value = 0f;
			}
			int index2 = 1;
			int retryCoutn = 0;
			Dictionary<string, PatchInfo>.Enumerator itr = _dicPatchInfo.GetEnumerator();
			bool bNext = itr.MoveNext();
			while (bNext)
			{
				_isSuccess = false;
				PatchInfo info = itr.Current.Value;
				if (progress != null)
				{
					progress.value = (float)index2 / (float)_dicPatchInfo.Values.Count;
				}
				labelName.text = Localization.Get("19300") + "(" + $"{(float)index2 / (float)_dicPatchInfo.Values.Count:P1}" + ")\n";
				yield return StartCoroutine(RequestFileDownLoad(info));
				if (_isSuccess)
				{
					index2++;
					bNext = itr.MoveNext();
					retryCoutn = 0;
					continue;
				}
				if (retryCoutn == 0)
				{
					bool bRetry2 = false;
					UISimplePopup popup = UISimplePopup.CreateOK(localization: false, Localization.Get("1310"), info.fileName, Localization.Get("19085"), Localization.Get("1001"));
					if (popup != null)
					{
						popup.onClose = delegate
						{
							bRetry2 = true;
						};
					}
					while (!bRetry2)
					{
						yield return null;
					}
				}
				else if (retryCoutn >= 3)
				{
					UISimplePopup popup2 = UISimplePopup.CreateOK(localization: false, Localization.Get("19013"), info.fileName, Localization.Get("19086"), Localization.Get("5133"));
					if (popup2 != null)
					{
						popup2.onClose = delegate
						{
							Application.Quit();
						};
					}
					while (true)
					{
						yield return null;
					}
				}
				retryCoutn++;
				yield return new WaitForSeconds(2f);
			}
		}
		else
		{
			if (_dicVoicePatchInfo.Count <= 0)
			{
				yield break;
			}
			if (progress != null)
			{
				progress.value = 0f;
			}
			int index = 1;
			int retryCoutn2 = 0;
			Dictionary<string, PatchInfo>.Enumerator itr2 = _dicVoicePatchInfo.GetEnumerator();
			bool bNext2 = itr2.MoveNext();
			while (bNext2)
			{
				_isSuccess = false;
				PatchInfo info2 = itr2.Current.Value;
				if (progress != null)
				{
					progress.value = (float)index / (float)_dicVoicePatchInfo.Values.Count;
				}
				labelName.text = Localization.Get("19300") + "(" + $"{(float)index / (float)_dicVoicePatchInfo.Values.Count:P1}" + ")\n";
				yield return StartCoroutine(RequestFileDownLoad(info2));
				if (_isSuccess)
				{
					index++;
					bNext2 = itr2.MoveNext();
					retryCoutn2 = 0;
					continue;
				}
				if (retryCoutn2 == 0)
				{
					bool bRetry = false;
					UISimplePopup popup3 = UISimplePopup.CreateOK(localization: false, Localization.Get("1310"), info2.fileName, Localization.Get("19085"), Localization.Get("1001"));
					if (popup3 != null)
					{
						popup3.onClose = delegate
						{
							bRetry = true;
						};
					}
					while (!bRetry)
					{
						yield return null;
					}
				}
				else if (retryCoutn2 >= 3)
				{
					UISimplePopup popup4 = UISimplePopup.CreateOK(localization: false, Localization.Get("19013"), info2.fileName, Localization.Get("19086"), Localization.Get("5133"));
					if (popup4 != null)
					{
						popup4.onClose = delegate
						{
							Application.Quit();
						};
					}
					while (true)
					{
						yield return null;
					}
				}
				retryCoutn2++;
				yield return new WaitForSeconds(2f);
			}
		}
	}

	private IEnumerator RequestFileDownLoad(PatchInfo sInfo)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		string FullAddress = $"{_PATCH_URL}{PLATFROM}{sInfo.version}/{sInfo.address}/{sInfo.fileName}";
		using (WWW www = new WWW(URLAntiCacheRandomizer(FullAddress)))
		{
			while (!www.isDone)
			{
				yield return null;
				if (www.error != null)
				{
					break;
				}
			}
			if (www.error == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(DefaultDataPath);
				stringBuilder.Append("/");
				if (sInfo.address != string.Empty)
				{
					stringBuilder.Append(sInfo.address);
					if (!Directory.Exists(stringBuilder.ToString()))
					{
						Directory.CreateDirectory(stringBuilder.ToString());
					}
					stringBuilder.Append("/");
				}
				stringBuilder.Append(sInfo.fileName);
				if (www.bytes.Length > 0)
				{
					if (sInfo.address != "zip")
					{
						SaveFiles(www.bytes, stringBuilder.ToString());
					}
					else
					{
						UnZipFiles(www.bytes);
					}
				}
				if (localUser.bDownLoadFileCheck)
				{
					AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(sInfo.fileName);
					if (assetBundle == null)
					{
						assetBundle = www.assetBundle;
						if (assetBundle != null)
						{
							_isSuccess = true;
							www.assetBundle.Unload(unloadAllLoadedObjects: true);
						}
						else if (File.Exists(stringBuilder.ToString()))
						{
							File.Delete(stringBuilder.ToString());
						}
					}
					else
					{
						_isSuccess = true;
					}
				}
				else
				{
					_isSuccess = true;
				}
			}
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	public void UnZipFiles(byte[] btData)
	{
	}

	private void UpdateListFile(bool isZip)
	{
		string empty = string.Empty;
		try
		{
			if (isZip)
			{
				empty = $"{DefaultDataPath}/{ZIPPATCHFILE_NAME}";
				if (_bytesZipList.Length > 0)
				{
					SaveFiles(_bytesZipList, empty);
				}
			}
			else
			{
				empty = $"{DefaultDataPath}{PATCHFILE_NAME}";
				if (_bytesPatchList.Length > 0)
				{
					SaveFiles(_bytesPatchList, empty);
				}
			}
		}
		finally
		{
		}
	}

	public void RetryPatch(GameObject obj)
	{
		StartCoroutine("RunPatch");
	}

	private void PatchError(string strMsg)
	{
		StopCoroutine("RunPatch");
		_dicPatchInfo.Clear();
		_bytesPatchList = null;
		_bytesZipList = null;
		_nLocalVersion = 0;
		_nNextLocalVersion = 0;
		_nZipLocalVersion = 0;
		_nZipNextVersion = 0;
		_nRepatchCount++;
		if (_nRepatchCount >= 3)
		{
			_nRepatchCount = 0;
		}
		else
		{
			StartCoroutine(RetryPatch_Coroutine());
		}
	}

	private IEnumerator RetryPatch_Coroutine()
	{
		yield return new WaitForSeconds(5f * Time.timeScale);
		RetryPatch(null);
	}

	public IEnumerator ELoadCostumeBundle()
	{
		string url = string.Empty;
		string filePath = string.Empty;
		fDownCount = 0f;
		url = $"file://{DefaultDataPath}/{PATCHFILE_NAME}";
		filePath = $"{DefaultDataPath}/{PATCHFILE_NAME}";
		if (!File.Exists(filePath))
		{
			yield break;
		}
		AssetBundle bundle = null;
		using (WWW www = new WWW(url))
		{
			yield return www;
			if (www.error == null)
			{
				bundle = www.assetBundle;
			}
		}
		if (!(bundle != null))
		{
			yield break;
		}
		TextAsset textAsset = UnityEngine.Object.Instantiate(bundle.mainAsset) as TextAsset;
		string[] _RowDatas = textAsset.text.Split('\n');
		for (int i = 1; i < _RowDatas.Length - 1; i++)
		{
			string[] fileinfo = _RowDatas[i].Split('\t');
			string path = fileinfo[0];
			string fileName = fileinfo[1];
			labelName.text = "-Resource Loading(" + i + "/" + (_RowDatas.Length - 2) + ")-\n";
			string fullPath = $"{DefaultDataPath}{path}/{fileName}";
			yield return StartCoroutine(LoadAssetBundle(fullPath));
			if (fileName.EndsWith("_skeletonanimation.assetbundle"))
			{
				RemoteObjectManager.instance.localUser.assetbundleSpineKey.Add(fileName.Replace("_skeletonanimation.assetbundle", string.Empty));
			}
		}
		bundle.Unload(unloadAllLoadedObjects: false);
	}

	private IEnumerator LoadAssetBundle(string path)
	{
		AssetBundle bundle = null;
		WWW assetBundle = new WWW("file://" + path);
		while (!assetBundle.isDone)
		{
			yield return new WaitForSeconds(0.01f);
			if (progress != null)
			{
				progress.value = assetBundle.progress;
			}
		}
		yield return assetBundle;
		if (assetBundle.isDone)
		{
			bundle = assetBundle.assetBundle;
		}
		if (bundle != null)
		{
			string[] array = path.Split('/');
			AssetBundleManager.AddAssetBundle(array[array.Length - 1], bundle);
		}
	}

	public Font LoadFontFromAssetBundle(string fileName)
	{
		string arg = "patch/texture";
		string path = $"{DefaultDataPath}{arg}/{fileName}";
		AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
		return assetBundle.LoadAsset(fileName.Replace(".assetbundle", ".otf")) as Font;
	}

	public IEnumerator LoadPrefabAssetBundle(string fileName, ECacheType type = ECacheType.None)
	{
		string arg = "patch/prefab";
		switch (type)
		{
		case ECacheType.Sound:
		case ECacheType.Bgm:
			arg = "patch/sound";
			break;
		case ECacheType.Texture:
			arg = "patch/texture";
			break;
		}
		string path = $"{DefaultDataPath}{arg}/{fileName}";
		if (!AssetBundleManager.HasAssetBundle(fileName) && File.Exists(path))
		{
			AssetBundleManager.AddAssetBundle(fileName, AssetBundle.LoadFromFile(path), type);
		}
		yield break;
	}

	public IEnumerator RunBadWordPatch()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		bool updateListFile = true;
		List<string> patchLangs = new List<string>();
		string FullAddress = DefaultDataPath + "Regulation/" + PATCHBADWORDFILE_NAME;
		if (!File.Exists(FullAddress))
		{
			Dictionary<string, double>.Enumerator enumerator = localUser.badWordVersions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				patchLangs.Add(enumerator.Current.Key);
			}
		}
		else
		{
			try
			{
				Dictionary<string, double> dictionary = (Dictionary<string, double>)Program.Object_DecryptFromFile(FullAddress);
				Dictionary<string, double>.Enumerator enumerator2 = localUser.badWordVersions.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (!dictionary.ContainsKey(enumerator2.Current.Key))
					{
						patchLangs.Add(enumerator2.Current.Key);
					}
					else if (dictionary[enumerator2.Current.Key] < enumerator2.Current.Value)
					{
						patchLangs.Add(enumerator2.Current.Key);
					}
				}
			}
			catch (Exception)
			{
				patchLangs = new List<string>();
				Dictionary<string, double>.Enumerator enumerator3 = localUser.badWordVersions.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					patchLangs.Add(enumerator3.Current.Key);
				}
			}
		}
		if (patchLangs.Count > 0)
		{
			updateListFile = true;
			RemoteObjectManager.instance.RequestBadWordList(patchLangs);
			while (localUser.badWords == null)
			{
				yield return null;
			}
			if (localUser.badWords.Count > 0)
			{
				Dictionary<string, List<string>>.Enumerator enumerator4 = localUser.badWords.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(DefaultDataPath);
					stringBuilder.Append("Regulation");
					if (!Directory.Exists(stringBuilder.ToString()))
					{
						Directory.CreateDirectory(stringBuilder.ToString());
					}
					stringBuilder.Append("/");
					stringBuilder.Append(enumerator4.Current.Key + ".json");
					Program.Object_EncryptToFile(stringBuilder.ToString(), enumerator4.Current.Value);
				}
			}
		}
		Dictionary<string, double>.Enumerator readItr = localUser.badWordVersions.GetEnumerator();
		while (readItr.MoveNext())
		{
			if (localUser.badWords == null)
			{
				localUser.badWords = new Dictionary<string, List<string>>();
			}
			if (!localUser.badWords.ContainsKey(readItr.Current.Key))
			{
				string filePath = DefaultDataPath + "Regulation/" + readItr.Current.Key + ".json";
				List<string> value = (List<string>)Program.Object_DecryptFromFile(filePath);
				localUser.badWords.Add(readItr.Current.Key, value);
			}
		}
		if (updateListFile)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(DefaultDataPath);
			stringBuilder2.Append("Regulation");
			if (!Directory.Exists(stringBuilder2.ToString()))
			{
				Directory.CreateDirectory(stringBuilder2.ToString());
			}
			stringBuilder2.Append("/");
			stringBuilder2.Append(PATCHBADWORDFILE_NAME);
			Program.Object_EncryptToFile(stringBuilder2.ToString(), localUser.badWordVersions);
		}
	}

	public IEnumerator FileDownLoad(UILabel label, UIProgressBar progressBar, bool bUpdate)
	{
		labelName = label;
		progress = progressBar;
		yield return StartCoroutine("CheckFileList");
		if (bUpdate && arrDBPatchFileName.Count > 0)
		{
			if (progress != null)
			{
				progress.value = fDownCount / (float)arrDBPatchFileName.Count;
			}
			if (labelName != null && progress != null)
			{
				labelName.text = Localization.Get("19301") + "(" + $"{progress.value:P1}" + ")\n";
			}
			Dictionary<string, int>.Enumerator itr = arrDBPatchFileName.GetEnumerator();
			bool bNext = itr.MoveNext();
			while (bNext)
			{
				yield return StartCoroutine(RequestFileDownLoad(itr.Current.Key, itr.Current.Value));
				fDownCount += 1f;
				bNext = itr.MoveNext();
				if (progress != null)
				{
					progress.value = fDownCount / (float)arrDBPatchFileName.Count;
				}
				if (labelName != null && progress != null)
				{
					labelName.text = Localization.Get("19301") + "(" + $"{progress.value:P1}" + ")\n";
				}
			}
		}
		for (int i = 0; i < arrDBFileName.Count; i++)
		{
			if (progress != null)
			{
				progress.value = (float)(i + 1) / (float)arrDBFileName.Count;
			}
			if (labelName != null && progress != null)
			{
				labelName.text = Localization.Get("19302") + "(" + $"{progress.value:P1}" + ")\n";
			}
			yield return StartCoroutine(LoadDBFile(arrDBFileName[i]));
		}
	}

	private byte[] Prune(byte[] bytes)
	{
		if (bytes.Length == 0)
		{
			return bytes;
		}
		int num = bytes.Length - 1;
		while (bytes[num] == 0)
		{
			num--;
		}
		byte[] array = new byte[num + 1];
		Array.Copy(bytes, array, num + 1);
		return array;
	}

	private IEnumerator CheckFileList()
	{
		bool isUpdateDbList = false;
		if (double.Parse(PlayerPrefs.GetString("DBVersion", "0")) < RemoteObjectManager.instance.localUser.serverDBVersion || !File.Exists(DefaultDataPath + "Regulation/" + PATCHDBFILE_NAME))
		{
			isUpdateDbList = true;
		}
		Dictionary<string, List<string>> fileInfo = null;
		bool isSuccess = false;
		while (!isSuccess)
		{
			if (!isUpdateDbList)
			{
				string FullAddress = DefaultDataPath + "Regulation/" + PATCHDBFILE_NAME;
				fileInfo = (Dictionary<string, List<string>>)Program.Object_DecryptFromFile(FullAddress);
			}
			else
			{
				string FullAddress = URLAntiCacheRandomizer(_PATCH_URL + DB_DIRECTORY + PATCHDBFILE_NAME);
				using WWW www = new WWW(FullAddress);
				while (!www.isDone)
				{
					yield return null;
				}
				yield return www;
				if (www.error == null)
				{
					try
					{
						string s = Program.JSON_Decrypt(www.text);
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						string @string = Encoding.UTF8.GetString(Prune(bytes));
						fileInfo = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(@string);
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(DefaultDataPath);
						stringBuilder.Append("/");
						stringBuilder.Append("Regulation");
						if (!Directory.Exists(stringBuilder.ToString()))
						{
							Directory.CreateDirectory(stringBuilder.ToString());
						}
						stringBuilder.Append("/");
						stringBuilder.Append(PATCHDBFILE_NAME);
						Program.Object_EncryptToFile(stringBuilder.ToString(), fileInfo);
					}
					catch (Exception)
					{
						_PATCH_URL = _PATCH_URL_SUB;
						isUpdateDbList = true;
						continue;
					}
				}
			}
			if (fileInfo != null)
			{
				Dictionary<string, List<string>>.Enumerator enumerator = fileInfo.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string key = enumerator.Current.Key;
					double num = double.Parse(enumerator.Current.Value[0].ToString());
					int value = int.Parse(enumerator.Current.Value[1].ToString());
					if (!arrDBFileName.Contains(key))
					{
						arrDBFileName.Add(key);
					}
					string path = string.Format("{0}{1}/{2}", DefaultDataPath, "Regulation", key);
					if (!File.Exists(path))
					{
						if (!arrDBPatchFileName.ContainsKey(key))
						{
							arrDBPatchFileName.Add(key, value);
						}
					}
					else if (num > double.Parse(PlayerPrefs.GetString("DBVersion", "0")) && !arrDBPatchFileName.ContainsKey(key))
					{
						arrDBPatchFileName.Add(key, value);
					}
				}
				isSuccess = true;
			}
			if (!isSuccess)
			{
				yield return new WaitForSeconds(2f);
			}
		}
	}

	private IEnumerator RequestFileDownLoad(string fileName, int fileSize)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		int retryCount = 0;
		bool isSuccess = false;
		while (!isSuccess)
		{
			string FullAddress = _PATCH_URL + DB_DIRECTORY + fileName;
			using (WWW www = new WWW(URLAntiCacheRandomizer(FullAddress)))
			{
				yield return www;
				if (www.error == null)
				{
					if (localUser.bDownLoadFileCheck && www.size != fileSize)
					{
						if (retryCount == 0)
						{
							bool bRetry = false;
							UISimplePopup popup = UISimplePopup.CreateOK(localization: false, Localization.Get("1310"), fileName + " file size : " + fileSize, Localization.Get("19085"), Localization.Get("1001"));
							if (popup != null)
							{
								popup.onClose = delegate
								{
									bRetry = true;
								};
							}
							while (!bRetry)
							{
								yield return null;
							}
						}
						else if (retryCount >= 3)
						{
							UISimplePopup popup2 = UISimplePopup.CreateOK(localization: false, Localization.Get("19013"), fileName + " file size : " + fileSize, Localization.Get("19086"), Localization.Get("5133"));
							if (popup2 != null)
							{
								popup2.onClose = delegate
								{
									Application.Quit();
								};
							}
							while (true)
							{
								yield return null;
							}
						}
						retryCount++;
						yield return new WaitForSeconds(1f);
						continue;
					}
					try
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(DefaultDataPath);
						stringBuilder.Append("/");
						stringBuilder.Append("Regulation");
						if (!Directory.Exists(stringBuilder.ToString()))
						{
							Directory.CreateDirectory(stringBuilder.ToString());
						}
						stringBuilder.Append("/");
						stringBuilder.Append(fileName);
						if (www.bytes.Length > 0)
						{
							string text = fileName.Substring(0, 1);
							text = text.ToLower();
							text += fileName.Substring(1);
							text = text.Replace("DataTable.json", string.Empty);
							text = text.Replace(".json", string.Empty);
							if (regulation.HasTable(text))
							{
								string s = Program.JSON_Decrypt(www.text);
								byte[] bytes = Encoding.UTF8.GetBytes(s);
								string @string = Encoding.UTF8.GetString(Prune(bytes));
								Program.Object_EncryptToFile(stringBuilder.ToString(), regulation.SetTable(text, @string));
							}
							else
							{
								SaveFiles(www.bytes, stringBuilder.ToString());
							}
						}
						isSuccess = true;
					}
					catch (Exception)
					{
						_PATCH_URL = _PATCH_URL_SUB;
					}
				}
			}
			if (!isSuccess)
			{
				yield return new WaitForSeconds(2f);
			}
		}
	}

	private IEnumerator LoadDBFile(string filename)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		string key5 = filename.Substring(0, 1);
		key5 = key5.ToLower();
		key5 += filename.Substring(1);
		key5 = key5.Replace("DataTable.json", string.Empty);
		key5 = key5.Replace(".json", string.Empty);
		if (regulation.HasTable(key5))
		{
			if (regulation.GetTable(key5) == null)
			{
				regulation.SetTable(key5, Program.Object_DecryptFromFile(DefaultDataPath + "/Regulation/" + filename));
			}
			yield break;
		}
		string text2 = string.Empty;
		WWW www = new WWW("file://" + DefaultDataPath + "/Regulation/" + filename);
		yield return www;
		if (www.isDone)
		{
			text2 = www.text;
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = Program.JSON_Decrypt(text2);
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			string @string = Encoding.UTF8.GetString(Prune(bytes));
			Regulation.RegulationFile.Add(key5, @string);
			regulation.SetFromLocalResources(key5, @string);
		}
	}

	private void SaveFiles(byte[] b, string str)
	{
		FileStream fileStream = new FileStream(str, FileMode.Create);
		fileStream.Seek(0L, SeekOrigin.Begin);
		fileStream.Write(b, 0, b.Length);
		fileStream.Dispose();
		fileStream.Close();
		fileStream = null;
	}

	public void RemoveAllDBFile()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(DefaultDataPath + "Regulation");
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			fileInfo.Delete();
		}
	}

	public void RemoveAllPatchFile()
	{
		if (Directory.Exists(DefaultDataPath + "patch"))
		{
			Directory.Delete(DefaultDataPath + "patch", recursive: true);
		}
	}

	public void RemoveAll()
	{
		Directory.Delete(DefaultDataPath, recursive: true);
		Application.Quit();
	}

	public static string URLAntiCacheRandomizer(string url)
	{
		string empty = string.Empty;
		empty += UnityEngine.Random.Range(1000000, 8000000);
		empty += UnityEngine.Random.Range(1000000, 8000000);
		return url + "?p=" + empty;
	}

	private string GetFileSize(long byteCount)
	{
		string result = "0 Bytes";
		if ((double)byteCount >= 1073741824.0)
		{
			result = $"{(double)byteCount / 1073741824.0:##.##}" + " GB";
		}
		else if ((double)byteCount >= 1048576.0)
		{
			result = $"{(double)byteCount / 1048576.0:##.##}" + " MB";
		}
		else if ((double)byteCount >= 1024.0)
		{
			result = $"{(double)byteCount / 1024.0:##.##}" + " KB";
		}
		else if (byteCount > 0 && (double)byteCount < 1024.0)
		{
			result = byteCount + " Bytes";
		}
		return result;
	}
}
