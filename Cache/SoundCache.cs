using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class SoundCache : AbstractCacheWithoutPool
	{
		[Serializable]
		public class SoundCacheElement : CacheElement
		{
			public AudioClip sourceClip;
		}

		public List<SoundCacheElement> elements;

		[HideInInspector]
		public bool isPaused;

		public AudioSource PlaySFX(string clipName)
		{
			return SoundManager.PlaySFX(clipName);
		}

		public void PlayBGM(string level, string clipName)
		{
			AudioClip audioClip = Create(clipName, isBgm: true);
			if (!(audioClip == null))
			{
				PlayConnection(new SoundConnection(level, SoundManager.PlayMethod.ContinuousPlayThrough, audioClip));
			}
		}

		public void PlayConnection(SoundConnection sc, string level = null)
		{
			if (!string.IsNullOrEmpty(level))
			{
				sc.level = level;
			}
			if (string.IsNullOrEmpty(sc.level))
			{
				return;
			}
			bool flag = false;
			SoundConnection soundConnectionForThisLevel = SoundManager.GetSoundConnectionForThisLevel(sc.level);
			if (soundConnectionForThisLevel == null)
			{
				SoundManager.AddSoundConnection(sc);
				SoundManager.PlayConnection(sc.level);
				return;
			}
			if (soundConnectionForThisLevel.soundsToPlay.Count != sc.soundsToPlay.Count)
			{
				flag = true;
			}
			else
			{
				for (int i = 0; i < soundConnectionForThisLevel.soundsToPlay.Count; i++)
				{
					if (soundConnectionForThisLevel.soundsToPlay[i].name == sc.soundsToPlay[i].name && SoundManager.Instance.crossingOut)
					{
						flag = true;
						break;
					}
					if (soundConnectionForThisLevel.soundsToPlay[i].name != sc.soundsToPlay[i].name || SoundManager.GetCurrentSong() == null)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				SoundManager.ReplaceSoundConnection(sc);
				SoundManager.PlayConnection(sc.level);
			}
		}

		public AudioClip Create(string key, bool isBgm = false, int prepool = 0, float baseVolume = 1f, float volumeVariation = 0f, float pitchVariation = 0f)
		{
			SoundCacheElement soundCacheElement = null;
			if (isPaused)
			{
				return null;
			}
			if (_dicElements == null)
			{
				RefreshElementDict();
			}
			if (!_dicElements.ContainsKey(key))
			{
				StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle", (!isBgm) ? ECacheType.Sound : ECacheType.Bgm));
				soundCacheElement = (SoundCacheElement)LoadAssetBundlePrefab(key);
				if (soundCacheElement == null)
				{
					return null;
				}
				if (soundCacheElement.sourceClip == null)
				{
					return null;
				}
				SoundManager.SaveSFX(soundCacheElement.sourceClip);
				SoundManager.ApplySFXAttributes(soundCacheElement.sourceClip, prepool, baseVolume, volumeVariation, pitchVariation);
				_usedElements.Add(soundCacheElement);
				return soundCacheElement.sourceClip;
			}
			soundCacheElement = (SoundCacheElement)_dicElements[key];
			if (soundCacheElement.sourceClip == null)
			{
				StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle", (!isBgm) ? ECacheType.Sound : ECacheType.Bgm));
				LoadAssetBundlePrefab(key, soundCacheElement);
				if (soundCacheElement.sourceClip == null)
				{
					if (string.IsNullOrEmpty(soundCacheElement.resPath))
					{
						return null;
					}
					soundCacheElement.sourceClip = LoadFromRes<AudioClip>(soundCacheElement.resPath);
				}
				if (soundCacheElement.sourceClip == null)
				{
					return null;
				}
				SoundManager.SaveSFX(soundCacheElement.sourceClip);
				SoundManager.ApplySFXAttributes(soundCacheElement.sourceClip, prepool, baseVolume, volumeVariation, pitchVariation);
				_usedElements.Add(soundCacheElement);
			}
			return soundCacheElement.sourceClip;
		}

		public override CacheElement GetElement(string key)
		{
			return null;
		}

		public override void CleanUp()
		{
			if (!setting.dontDestroyOnLoad && _usedElements != null)
			{
				while (_usedElements.Count > 0)
				{
					((SoundCacheElement)_usedElements[0]).sourceClip = null;
					_usedElements.RemoveAt(0);
				}
				_usedElements.Clear();
				SoundManager.DeleteSFX();
				SoundManager.Instance.currentPockets.Clear();
			}
		}

		public void CleanUp(string[] clipNames)
		{
			for (int i = 0; i < clipNames.Length; i++)
			{
				int num = _usedElements.FindIndex((CacheElement x) => x.key == clipNames[i]);
				if (num >= 0)
				{
					((SoundCacheElement)_usedElements[num]).sourceClip = null;
					_usedElements.RemoveAt(num);
				}
			}
			SoundManager.DeleteSFX(clipNames);
			Resources.UnloadUnusedAssets();
		}

		protected override void RefreshElementDict()
		{
			_usedElements = new List<CacheElement>();
			_dicElements = new Dictionary<string, CacheElement>();
			for (int i = 0; i < elements.Count; i++)
			{
				_dicElements.Add(elements[i].key, elements[i]);
				if (elements[i].sourceClip != null)
				{
					_usedElements.Add(elements[i]);
				}
			}
		}

		protected override void LoadElementFromAssetBundle()
		{
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			SoundCacheElement soundCacheElement = (SoundCacheElement)targetElement;
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(key + ".assetbundle");
			if (assetBundle != null)
			{
				AudioClip sourceClip = assetBundle.LoadAsset(key + ".ogg") as AudioClip;
				if (soundCacheElement != null)
				{
					soundCacheElement.sourceClip = sourceClip;
				}
				else
				{
					soundCacheElement = new SoundCacheElement();
					soundCacheElement.key = key;
					soundCacheElement.sourceClip = sourceClip;
					soundCacheElement.resPath = string.Empty;
					AddCacheElement(soundCacheElement);
				}
			}
			return soundCacheElement;
		}
	}
}
