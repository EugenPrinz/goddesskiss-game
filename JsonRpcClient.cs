using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BestHTTP;
using BestHTTP.Decompression.Zlib;
using Encryption_Rijndae;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

public class JsonRpcClient : MonoBehaviour
{
	public class Request
	{
		private static int _idSeed;

		public object sender { get; private set; }

		public string id { get; private set; }

		public MethodInfo methodInfo { get; private set; }

		public string jsonString { get; private set; }

		public bool isNotification => id == string.Empty;

		public Request(object sender, MethodInfo mi, params object[] args)
		{
			if (sender == null)
			{
				throw new ArgumentNullException("object sender");
			}
			if (mi == null)
			{
				throw new ArgumentNullException("MethodInfo mi");
			}
			if (sender is Type && !mi.IsStatic)
			{
				throw new ArgumentException(_GetMethodFullName(mi) + " is not static method.");
			}
			this.sender = sender;
			RequestAttribute requestAttribute = _GetAttribute<RequestAttribute>(mi);
			id = _NextId8Bytes();
			methodInfo = mi;
			ParameterInfo[] parameters = mi.GetParameters();
			if (args == null)
			{
				args = new object[1] { string.Empty };
			}
			else if (parameters.Length != args.Length)
			{
				throw new ArgumentException("parameters.Length != args.Length");
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				if (parameterType != args[i].GetType())
				{
					throw new ArgumentException("paramType != args[n].GetType()");
				}
			}
			JObject jObject = new JObject { { "id", id } };
			int result = 0;
			if (int.TryParse(requestAttribute.method, out result))
			{
				jObject.Add(new JProperty("method", result));
			}
			else
			{
				jObject.Add(new JProperty("method", requestAttribute.method));
			}
			if (requestAttribute.wrapParams)
			{
				JObject jObject2 = new JObject();
				ParameterAliasDictionaryAttribute parameterAliasDictionaryAttribute = _GetAttribute<ParameterAliasDictionaryAttribute>(mi);
				for (int j = 0; j < parameters.Length; j++)
				{
					string text = string.Empty;
					if (parameterAliasDictionaryAttribute != null)
					{
						text = parameterAliasDictionaryAttribute.GetName(parameters[j].Name);
					}
					if (text == string.Empty)
					{
						text = parameters[j].Name;
					}
					jObject2.Add(new JProperty(text, args[j]));
				}
				jObject.Add("params", jObject2);
			}
			else
			{
				jObject.Add(new JProperty("params", args[0]));
			}
			if (!string.IsNullOrEmpty(session))
			{
				jObject.Add("sess", session);
			}
			jsonString = jObject.ToString(Formatting.None);
		}

		private static int _NextId()
		{
			if (_idSeed < 0)
			{
				_idSeed = 0;
			}
			return ++_idSeed;
		}

		private static string _NextId8Bytes()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < array.Length; i++)
			{
				int num = UnityEngine.Random.Range(0, 256);
				array[i] = (byte)num;
			}
			return $"{array[0]:X2}{array[1]:X2}{array[2]:X2}{array[3]:X2}".ToLower();
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class RequestAttribute : Attribute
	{
		public string url { get; private set; }

		public string method { get; private set; }

		public bool wrapParams { get; private set; }

		public bool shouldLockUiCamera { get; private set; }

		public RequestAttribute(string url, string method, bool wrapParams = true, bool shouldLockUiCamera = true)
		{
			this.url = url;
			this.method = method;
			this.wrapParams = wrapParams;
			this.shouldLockUiCamera = shouldLockUiCamera;
			switch (method)
			{
			case "sendMsg":
			case "wait":
			case "waitChannel":
			case "waitGuild":
			case "checkMsg":
				this.url = RemoteObjectManager.instance.ChattingServerUrl;
				break;
			default:
				this.url = RemoteObjectManager.instance.GameServerUrl;
				break;
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class ParameterAliasDictionaryAttribute : Attribute
	{
		private Dictionary<string, string> _aliasDict = new Dictionary<string, string>();

		private Dictionary<string, string> _nameDict = new Dictionary<string, string>();

		public ParameterAliasDictionaryAttribute(params string[] args)
		{
			if (args.Length % 2 != 0)
			{
				throw new ArgumentException("args.Length % 2 != 0");
			}
			for (int i = 0; i < args.Length; i += 2)
			{
				string text = args[i];
				string text2 = args[i + 1];
				_aliasDict.Add(text2, text);
				_nameDict.Add(text, text2);
			}
		}

		public string GetAlias(string name)
		{
			string value = string.Empty;
			if (!_aliasDict.TryGetValue(name, out value))
			{
				return string.Empty;
			}
			return value;
		}

		public string GetName(string alias)
		{
			string value = string.Empty;
			if (!_nameDict.TryGetValue(alias, out value))
			{
				return string.Empty;
			}
			return value;
		}
	}

	public static string session;

	public static bool netStateIdle = true;

	public const string ResultMethodNameSuffix = "Result";

	public const string ErrorMethodNameSuffix = "Error";

	public const string DefaultMethodNameSuffix = "OnJsonRpcRequestError";

	public const string SystemMethodNameSuffix = "OnSystemMessage";

	public const BindingFlags methodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;

	private Dictionary<string, string> _sessionIdDict = new Dictionary<string, string>();

	private Dictionary<string, List<string>> _registeredClassDict = new Dictionary<string, List<string>>();

	private Dictionary<string, MethodInfo> _requestMethodInfoDict = new Dictionary<string, MethodInfo>();

	private Queue<Request> _requestQueue = new Queue<Request>();

	private int _uiCameraLockCount;

	private int networkingCount;

	private static JsonRpcClient _instance;

	public string systemRequestName;

	public static JsonRpcClient instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			JsonRpcClient[] array = UnityEngine.Object.FindObjectsOfType<JsonRpcClient>();
			JsonRpcClient[] array2 = array;
			foreach (JsonRpcClient jsonRpcClient in array2)
			{
				UnityEngine.Object.DestroyImmediate(jsonRpcClient.gameObject);
			}
			GameObject gameObject = new GameObject();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			_instance = gameObject.AddComponent<JsonRpcClient>();
			return _instance;
		}
	}

	public bool IsRegisteredClass(Type cls)
	{
		return _registeredClassDict.ContainsKey(cls.FullName);
	}

	public void RegisterClass(Type cls)
	{
		if (IsRegisteredClass(cls))
		{
			return;
		}
		try
		{
			Dictionary<string, MethodInfo> dictionary = _MakeRequestMethodInfoDict(cls);
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, MethodInfo> item in dictionary)
			{
				_requestMethodInfoDict.Add(item.Key, item.Value);
				list.Add(_GetMethodFullName(item.Value));
			}
			_registeredClassDict.Add(cls.FullName, list);
		}
		catch
		{
			throw;
		}
	}

	public void UnregisterClass(Type cls)
	{
		if (!IsRegisteredClass(cls))
		{
			return;
		}
		try
		{
			List<string> list = _registeredClassDict[cls.FullName];
			foreach (string item in list)
			{
				_requestMethodInfoDict.Remove(item);
			}
			_registeredClassDict.Remove(cls.FullName);
		}
		catch
		{
			throw;
		}
	}

	public Request SendRequest(object sender, string methodName, params object[] args)
	{
		try
		{
			Request request = _MakeRequest(sender, methodName, args);
			string url = _GetAttribute<RequestAttribute>(request.methodInfo).url;
			StartCoroutine(_SendRequests(url, false, request));
			return request;
		}
		catch
		{
			throw;
		}
	}

	public Request EnqueueRequest(object sender, string methodName, params object[] args)
	{
		try
		{
			Request request = _MakeRequest(sender, methodName, args);
			_requestQueue.Enqueue(request);
			return request;
		}
		catch
		{
			throw;
		}
	}

	public Request SendFirstRequest(object sender, string methodName, params object[] args)
	{
		try
		{
			Request request = _MakeRequest(sender, methodName, args);
			string url = _GetAttribute<RequestAttribute>(request.methodInfo).url;
			StartCoroutine(_SendRequests(url, true, request));
			return request;
		}
		catch
		{
			throw;
		}
	}

	private Request _MakeRequest(object sender, string methodName, params object[] args)
	{
		if (sender == null)
		{
			throw new ArgumentNullException("object sender");
		}
		if (methodName == null)
		{
			throw new ArgumentNullException("string methodName");
		}
		Type type = sender as Type;
		string key = _GetMethodFullName(type ?? sender.GetType(), methodName);
		MethodInfo methodInfo = _requestMethodInfoDict[key];
		methodInfo.Invoke(sender, args);
		return new Request(sender, methodInfo, args);
	}

	public int GetQueuedRequestCount()
	{
		return _requestQueue.Count;
	}

	public void SendAllQueuedRequests()
	{
		SendQueuedRequests(GetQueuedRequestCount());
	}

	public void SendQueuedRequests(int count)
	{
		Dictionary<string, List<Request>> dictionary = new Dictionary<string, List<Request>>();
		int num = count;
		while (num >= 0 && _requestQueue.Count > 0)
		{
			Request request = _requestQueue.Dequeue();
			RequestAttribute requestAttribute = _GetAttribute<RequestAttribute>(request.methodInfo);
			if (dictionary.ContainsKey(requestAttribute.url))
			{
				dictionary[requestAttribute.url].Add(request);
			}
			else
			{
				List<Request> list = new List<Request>();
				list.Add(request);
				dictionary.Add(requestAttribute.url, list);
			}
			num--;
		}
		foreach (KeyValuePair<string, List<Request>> item in dictionary)
		{
			StartCoroutine(_SendRequests(item.Key, isfirstPacket: false, item.Value.ToArray()));
		}
	}

	private string _GetSessionId(string domain)
	{
		if (_sessionIdDict.ContainsKey(domain))
		{
			return _sessionIdDict[domain];
		}
		return string.Empty;
	}

	private static Dictionary<string, MethodInfo> _MakeRequestMethodInfoDict(Type cls)
	{
		StringBuilder stringBuilder = new StringBuilder();
		MethodInfo methodInfo = _GetDefaultErrorMethodInfo(cls);
		if (methodInfo == null)
		{
			stringBuilder.AppendLine(cls.Name + ".OnJsonRpcRequestError not found.");
		}
		else
		{
			string value = _VerifyErrorMethodInfo(methodInfo);
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.AppendLine(value);
			}
		}
		Dictionary<string, MethodInfo> dictionary = new Dictionary<string, MethodInfo>();
		MethodInfo[] methods = cls.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
		foreach (MethodInfo methodInfo2 in methods)
		{
			RequestAttribute requestAttribute = _GetAttribute<RequestAttribute>(methodInfo2);
			if (requestAttribute != null)
			{
				dictionary.Add(_GetMethodFullName(methodInfo2), methodInfo2);
				string value2 = _VerifyRequestMethodInfo(methodInfo2, methodInfo);
				if (!string.IsNullOrEmpty(value2))
				{
					stringBuilder.AppendLine(value2);
				}
			}
		}
		if (stringBuilder.Length > 0)
		{
			throw new ArgumentException(stringBuilder.ToString());
		}
		return dictionary;
	}

	private static T _GetAttribute<T>(MethodInfo mi)
	{
		object[] customAttributes = mi.GetCustomAttributes(typeof(T), inherit: false);
		return (customAttributes.Length != 0) ? ((T)customAttributes[0]) : default(T);
	}

	private static string _GetMethodFullName(MethodInfo mi)
	{
		return _GetMethodFullName(mi.DeclaringType, mi.Name);
	}

	private static string _GetMethodFullName(Type cls, string methodName)
	{
		return cls.FullName + "." + methodName;
	}

	private static MethodInfo _GetDefaultErrorMethodInfo(Type cls)
	{
		return cls.GetMethod("OnJsonRpcRequestError", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
	}

	private static MethodInfo _GetResultMethodInfo(MethodInfo request)
	{
		return request.DeclaringType.GetMethod(request.Name + "Result", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
	}

	private static MethodInfo _GetErrorMethodInfo(MethodInfo request)
	{
		return request.DeclaringType.GetMethod(request.Name + "Error", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
	}

	private static MethodInfo _GetSystemMethodInfo(Type cls)
	{
		return cls.GetMethod("OnSystemMessage", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
	}

	private static string _VerifyRequestMethodInfo(MethodInfo request, MethodInfo defaultError)
	{
		StringBuilder stringBuilder = new StringBuilder();
		return stringBuilder.ToString();
	}

	private static string _VerifyErrorMethodInfo(MethodInfo mi)
	{
		StringBuilder stringBuilder = new StringBuilder();
		return stringBuilder.ToString();
	}

	private static string _VerifyResultMethodInfo(MethodInfo mi)
	{
		StringBuilder stringBuilder = new StringBuilder();
		return stringBuilder.ToString();
	}

	private static JObject _MakeInternalError(string message)
	{
		JObject jObject = new JObject();
		jObject.Add("id", null);
		jObject.Add("error", new JObject { { "code", -32603 } });
		return jObject;
	}

	private int _CalculateUiCameraLockCount(params Request[] requests)
	{
		int num = 0;
		foreach (Request request in requests)
		{
			RequestAttribute requestAttribute = _GetAttribute<RequestAttribute>(request.methodInfo);
			if (requestAttribute.shouldLockUiCamera)
			{
				num++;
			}
		}
		return num;
	}

	private void _LockUiCamera(params Request[] requests)
	{
		int num = _CalculateUiCameraLockCount(requests);
		_uiCameraLockCount += num;
		if (_uiCameraLockCount > 0 && UICamera.current != null)
		{
			UICamera.current.eventReceiverMask = LayerMask.NameToLayer("Nothing");
		}
	}

	private void _UnlockUiCamera(params Request[] requests)
	{
		int num = _CalculateUiCameraLockCount(requests);
		_uiCameraLockCount -= num;
		if (_uiCameraLockCount < 0)
		{
			throw new InvalidOperationException("_uiCameraLockCount must not be negative number.");
		}
		if (_uiCameraLockCount == 0 && UICamera.current != null)
		{
			UICamera.current.eventReceiverMask = LayerMask.NameToLayer("Everything");
		}
	}

	private static object[] _FindSendersOfRequests(params Request[] requests)
	{
		List<object> list = new List<object>();
		foreach (Request request in requests)
		{
			if (!list.Contains(request.sender))
			{
				list.Add(request.sender);
			}
		}
		return list.ToArray();
	}

	private IEnumerator _SendRequests(string url, bool isfirstPacket, params Request[] requests)
	{
		netStateIdle = false;
		_LockUiCamera(requests);
		string domain = new Uri(url).Host;
		string value = _GetSessionId(domain);
		Dictionary<string, Request> requestDict = new Dictionary<string, Request>();
		Request[] array = requests;
		foreach (Request request in array)
		{
			requestDict.Add(request.id, request);
		}
		string text = _ConvertRequestBatchToJsonString(requests);
		LoggerHelper.Error(text);
		text = Program.Encrypt(text, isfirstPacket);
		JObject[] responses = new JObject[0];
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		bool isChattingReq = url == RemoteObjectManager.instance.ChattingServerUrl;
		if (url != RemoteObjectManager.instance.ChattingServerUrl)
		{
			networkingCount++;
		}
		if (networkingCount > 0)
		{
			NetworkAnimation.Instance.On();
		}
		HTTPRequest hTTPRequest = new HTTPRequest(new Uri(url), HTTPMethods.Post, delegate(HTTPRequest req, HTTPResponse resp)
		{
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess)
				{
					string dataAsText = resp.DataAsText;
					byte[] data = resp.Data;
					string firstHeaderValue = resp.GetFirstHeaderValue("SET-COOKIE");
					if (!string.IsNullOrEmpty(firstHeaderValue))
					{
						_sessionIdDict[domain] = firstHeaderValue;
					}
					dataAsText = Program.Decrypt(dataAsText, isfirstPacket);
					data = Prune(Encoding.UTF8.GetBytes(dataAsText));
					responses = _ConvertResponseBatchToJsonObjects(data);
					Request request2 = null;
					JObject[] array2 = responses;
					foreach (JObject jObject in array2)
					{
						if (jObject.TryGetValue("id", out var value2))
						{
							if (!requestDict.ContainsKey(value2.ToString()))
							{
								if (value2.ToString() == "system" && jObject.TryGetValue("result", out value2))
								{
									systemRequestName = string.Empty;
									if (request2 != null)
									{
										systemRequestName = request2.methodInfo.Name;
									}
									_StartSystemCoroutine(RemoteObjectManager.instance, value2);
								}
							}
							else
							{
								Request request3 = requestDict[value2.ToString()];
								request2 = request3;
								if (jObject.TryGetValue("error", out value2))
								{
									_StartErrorCoroutine(request3, value2);
								}
								else if (jObject.TryGetValue("result", out value2))
								{
									_StartResultCoroutine(request3, value2);
								}
							}
						}
					}
				}
				else if (!isChattingReq)
				{
					JObject jObject2 = _MakeInternalError(resp.Message);
					JToken args = jObject2["error"];
					object[] array3 = _FindSendersOfRequests(requests);
					object[] array4 = array3;
					foreach (object sender2 in array4)
					{
						_StartErrorCoroutine(sender2, args);
					}
				}
				break;
			case HTTPRequestStates.Error:
			case HTTPRequestStates.Aborted:
			case HTTPRequestStates.ConnectionTimedOut:
			case HTTPRequestStates.TimedOut:
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("19303"), Localization.Get("19304"), string.Empty, Localization.Get("1001"), Localization.Get("1000"));
				bool needReSend = false;
				uISimplePopup.onClick = delegate(GameObject sender)
				{
					string text2 = sender.name;
					if (text2 == "OK")
					{
						needReSend = true;
					}
				};
				uISimplePopup.onClose = delegate
				{
					if (needReSend)
					{
						netStateIdle = false;
						_LockUiCamera(requests);
						byte[] bytes2 = GZipStream.UncompressBuffer(req.RawData);
						string @string = Encoding.UTF8.GetString(bytes2);
						@string = Program.Decrypt(@string, isfirstPacket);
						bytes2 = Prune(Encoding.UTF8.GetBytes(@string));
						JObject[] array5 = _ConvertResponseBatchToJsonObjects(bytes2);
						if (array5.Length > 0)
						{
							if (array5[0].Property("reload") == null)
							{
								array5[0].Add("reload", 1);
							}
							if (array5.Length == 1)
							{
								@string = array5[0].ToString(Formatting.None);
							}
							else
							{
								JArray jArray = new JArray();
								JObject[] array6 = array5;
								foreach (JObject item in array6)
								{
									jArray.Add(item);
								}
								@string = jArray.ToString(Formatting.None);
							}
						}
						@string = Program.Encrypt(@string, isfirstPacket);
						bytes2 = Encoding.UTF8.GetBytes(@string);
						bytes2 = GZipStream.CompressBuffer(bytes2);
						req.SetHeader("Content-Length", bytes2.Length.ToString());
						req.RawData = bytes2;
						if (url != RemoteObjectManager.instance.ChattingServerUrl)
						{
							networkingCount++;
						}
						if (networkingCount > 0)
						{
							NetworkAnimation.Instance.On();
						}
						req.Send();
					}
					else
					{
						Application.Quit();
					}
				};
				break;
			}
			}
			_UnlockUiCamera(requests);
			if (url != RemoteObjectManager.instance.ChattingServerUrl)
			{
				networkingCount--;
			}
			if (networkingCount <= 0)
			{
				networkingCount = 0;
				NetworkAnimation.Instance.Off();
			}
			netStateIdle = true;
		});
		if (!string.IsNullOrEmpty(value))
		{
			hTTPRequest.SetHeader("Cookie", value);
		}
		hTTPRequest.SetHeader("Content-Type", "application/json");
		hTTPRequest.SetHeader("Accept-Encoding", "gzip, deflate");
		bytes = GZipStream.CompressBuffer(bytes);
		hTTPRequest.SetHeader("Content-Length", bytes.Length.ToString());
		hTTPRequest.RawData = bytes;
		hTTPRequest.Send();
		yield break;
	}

	private string _ConvertRequestBatchToJsonString(params Request[] requests)
	{
		if (requests.Length == 1)
		{
			return requests[0].jsonString;
		}
		JArray jArray = new JArray();
		foreach (Request request in requests)
		{
			jArray.Add(JObject.Parse(request.jsonString));
		}
		return jArray.ToString(Formatting.None);
	}

	private JObject[] _ConvertResponseBatchToJsonObjects(byte[] bytes)
	{
		try
		{
			return JsonConvert.DeserializeObject<JObject[]>(Encoding.UTF8.GetString(bytes));
		}
		catch (Exception)
		{
		}
		JObject jObject = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(bytes));
		return new JObject[1] { jObject };
	}

	private void _StartErrorCoroutine(object sender, JToken args)
	{
		MethodInfo methodInfo = _GetDefaultErrorMethodInfo(sender.GetType());
		object[] array = _MakeResponseMethodParameters(methodInfo, null, args);
		object obj = ((!(sender is Type)) ? sender : null);
		if (methodInfo.GetParameters().Length == array.Length)
		{
			StartCoroutine((IEnumerator)methodInfo.Invoke(obj, array));
		}
	}

	private void _StartSystemCoroutine(Request request, JToken args)
	{
		MethodInfo methodInfo = null;
		if (request != null)
		{
			methodInfo = _GetSystemMethodInfo(request.sender.GetType());
		}
		if (methodInfo != null)
		{
			object[] array = _MakeResponseMethodParameters(methodInfo, null, args);
			object obj = ((!(request.sender is Type)) ? request.sender : null);
			if (methodInfo.GetParameters().Length == array.Length)
			{
				StartCoroutine((IEnumerator)methodInfo.Invoke(obj, array));
			}
		}
	}

	private void _StartSystemCoroutine(object sender, JToken args)
	{
		MethodInfo methodInfo = _GetSystemMethodInfo(sender.GetType());
		if (methodInfo != null)
		{
			object[] array = _MakeResponseMethodParameters(methodInfo, null, args);
			object obj = ((!(sender is Type)) ? sender : null);
			if (methodInfo.GetParameters().Length == array.Length)
			{
				StartCoroutine((IEnumerator)methodInfo.Invoke(obj, array));
			}
		}
	}

	private void _StartErrorCoroutine(Request request, JToken args)
	{
		MethodInfo methodInfo = _GetErrorMethodInfo(request.methodInfo);
		if (methodInfo == null)
		{
			methodInfo = _GetDefaultErrorMethodInfo(request.methodInfo.DeclaringType);
		}
		object[] array = _MakeResponseMethodParameters(methodInfo, request, args);
		object obj = ((!(request.sender is Type)) ? request.sender : null);
		if (methodInfo.GetParameters().Length == array.Length)
		{
			StartCoroutine((IEnumerator)methodInfo.Invoke(obj, array));
		}
	}

	private void _StartResultCoroutine(Request request, JToken args)
	{
		MethodInfo methodInfo = _GetResultMethodInfo(request.methodInfo);
		object[] array = _MakeResponseMethodParameters(methodInfo, request, args);
		object obj = ((!(request.sender is Type)) ? request.sender : null);
		if (methodInfo.GetParameters().Length == array.Length)
		{
			StartCoroutine((IEnumerator)methodInfo.Invoke(obj, array));
		}
	}

	private static object _DeserializeObject(string json, Type type)
	{
		try
		{
			if (type == typeof(string))
			{
				return json;
			}
			if (type == typeof(bool))
			{
				return json == "true" || json == "True";
			}
			return JsonConvert.DeserializeObject(json, type);
		}
		catch (Exception)
		{
		}
		return null;
	}

	private object[] _MakeResponseMethodParameters(MethodInfo mi, Request request, JToken args)
	{
		List<object> list = new List<object>();
		ParameterAliasDictionaryAttribute parameterAliasDictionaryAttribute = _GetAttribute<ParameterAliasDictionaryAttribute>(mi);
		ParameterInfo[] parameters = mi.GetParameters();
		bool flag = false;
		if (args.Type == JTokenType.Object)
		{
			int count = ((JObject)args).Count;
			if (count + 2 >= parameters.Length)
			{
				flag = true;
			}
		}
		list.Add(request);
		if (parameters.Length < 2)
		{
			throw new ArgumentException("Incorrect parameter count.");
		}
		list.Add(_DeserializeObject(args.ToString(), parameters[1].ParameterType));
		if (flag)
		{
			for (int i = 2; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				string text = string.Empty;
				if (parameterAliasDictionaryAttribute != null)
				{
					text = parameterAliasDictionaryAttribute.GetAlias(parameterInfo.Name);
				}
				if (string.IsNullOrEmpty(text))
				{
					text = parameterInfo.Name;
				}
				list.Add(_DeserializeObject(args[text].ToString(), parameterInfo.ParameterType));
			}
		}
		return list.ToArray();
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
}
