using System.Collections.Generic;
using UnityEngine;

public class TouchableReward : MonoBehaviour
{
	public struct Data
	{
		public delegate void OnFinished(Data data);

		public string itemType;

		public int value;

		public bool isWorldObject;

		public Transform from;

		public Transform to;

		public Transform parent;

		public OnFinished onFinished;

		public bool clickable;

		public int gold
		{
			get
			{
				return (itemType == "Gold") ? value : 0;
			}
			set
			{
				_SetValue("Gold", value);
			}
		}

		public int exp
		{
			get
			{
				return (itemType == "Exp") ? value : 0;
			}
			set
			{
				_SetValue("Exp", value);
			}
		}

		public int iron
		{
			get
			{
				return (itemType == "Iron") ? value : 0;
			}
			set
			{
				_SetValue("Iron", value);
			}
		}

		public int gas
		{
			get
			{
				return (itemType == "Gas") ? value : 0;
			}
			set
			{
				_SetValue("Gas", value);
			}
		}

		private void _SetValue(string type, int value)
		{
			itemType = type;
			this.value = value;
		}

		public void Generate()
		{
			TouchableReward.Generate(this);
		}
	}

	private static TouchableReward _singleton;

	private SortedDictionary<string, GameObject> prefabDict;

	private static TouchableReward _instance
	{
		get
		{
			if (_singleton == null)
			{
				_CreateInstanceAndInit();
			}
			return _singleton;
		}
	}

	private void OnDestroy()
	{
		if (_singleton == this)
		{
			_singleton = null;
		}
	}

	public static void PreLoad()
	{
		_CreateInstanceAndInit();
	}

	private static void _CreateInstanceAndInit()
	{
		if (_singleton != null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("/_Singleton");
		if (gameObject == null)
		{
			gameObject = new GameObject("_Singleton");
		}
		TouchableReward touchableReward = gameObject.GetComponent<TouchableReward>();
		if (touchableReward == null)
		{
			touchableReward = gameObject.AddComponent<TouchableReward>();
		}
		string text = "Prefabs/PopResource/";
		SortedDictionary<string, GameObject> sortedDictionary = new SortedDictionary<string, GameObject>();
		List<string> list = new List<string>();
		list.Add("Gold");
		list.Add("Gas");
		list.Add("Iron");
		list.Add("Exp");
		List<string> list2 = list;
		foreach (string item in list2)
		{
			GameObject gameObject2 = Resources.Load(text + item) as GameObject;
			if (!(gameObject2 == null))
			{
				sortedDictionary.Add(item, gameObject2);
			}
		}
		touchableReward.prefabDict = sortedDictionary;
		_singleton = touchableReward;
	}

	public static void Generate(Data src)
	{
		_instance.CreateItem(src);
	}

	private TouchableRewardItem CreateItem(Data data)
	{
		GameObject gameObject = prefabDict[data.itemType];
		GameObject gameObject2 = Object.Instantiate(gameObject);
		gameObject2.transform.parent = data.parent;
		gameObject2.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localScale = gameObject.transform.localScale;
		TouchableRewardItem component = gameObject2.GetComponent<TouchableRewardItem>();
		component.data = data;
		return component;
	}
}
