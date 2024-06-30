using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DialoguerEditor;
using UnityEngine;

namespace DialoguerCore
{
	public class DialoguerDataManager
	{
		private static Dictionary<string, DialoguerData> elements;

		private static DialoguerData _data;

		public static void Load(string key)
		{
			if (elements == null)
			{
				elements = new Dictionary<string, DialoguerData>();
			}
			if (elements.ContainsKey(key))
			{
				return;
			}
			DialogueEditorMasterObject dialogueEditorMasterObject = null;
			string arg = $"{key}.assetbundle";
			string path = $"{PatchManager.Instance.DefaultDataPath}patch/prefab/{arg}";
			if (File.Exists(path))
			{
				AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
				if (assetBundle != null)
				{
					DialogueEditorMasterObjectWrapper dialogueEditorMasterObjectWrapper = assetBundle.LoadAsset(key + ".asset") as DialogueEditorMasterObjectWrapper;
					if (dialogueEditorMasterObjectWrapper != null)
					{
						dialogueEditorMasterObject = dialogueEditorMasterObjectWrapper.data;
						elements.Add(key, dialogueEditorMasterObject.getDialoguerData());
					}
					assetBundle.Unload(unloadAllLoadedObjects: false);
				}
			}
			if (dialogueEditorMasterObject == null && dialogueEditorMasterObject == null)
			{
				dialogueEditorMasterObject = (Resources.Load(key) as DialogueEditorMasterObjectWrapper).data;
				elements.Add(key, dialogueEditorMasterObject.getDialoguerData());
			}
		}

		public static void Initialize(string key)
		{
			Load(key);
			_data = elements[key];
		}

		public static string GetGlobalVariablesState()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DialoguerGlobalVariables));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, _data.globalVariables);
			return stringWriter.ToString();
		}

		public static void LoadGlobalVariablesState(string globalVariablesXml)
		{
			_data.loadGlobalVariablesState(globalVariablesXml);
		}

		public static float GetGlobalFloat(int floatId)
		{
			return _data.globalVariables.floats[floatId];
		}

		public static void SetGlobalFloat(int floatId, float floatValue)
		{
			_data.globalVariables.floats[floatId] = floatValue;
		}

		public static bool GetGlobalBoolean(int booleanId)
		{
			return _data.globalVariables.booleans[booleanId];
		}

		public static void SetGlobalBoolean(int booleanId, bool booleanValue)
		{
			_data.globalVariables.booleans[booleanId] = booleanValue;
		}

		public static string GetGlobalString(int stringId)
		{
			return _data.globalVariables.strings[stringId];
		}

		public static void SetGlobalString(int stringId, string stringValue)
		{
			_data.globalVariables.strings[stringId] = stringValue;
		}

		public static DialoguerDialogue GetDialogueById(int dialogueId)
		{
			if (_data.dialogues.Count <= dialogueId)
			{
				return null;
			}
			return _data.dialogues[dialogueId];
		}

		public static bool HasDialogue(string name)
		{
			if (_data == null)
			{
				return false;
			}
			if (_data.dicDialogues == null)
			{
				return false;
			}
			if (!_data.dicDialogues.ContainsKey(name))
			{
				return false;
			}
			return true;
		}

		public static bool HasDialogue(string key, string name)
		{
			Load(key);
			if (!elements.ContainsKey(key))
			{
				return false;
			}
			DialoguerData dialoguerData = elements[key];
			if (dialoguerData.dicDialogues == null)
			{
				return false;
			}
			if (!dialoguerData.dicDialogues.ContainsKey(name))
			{
				return false;
			}
			return true;
		}

		public static bool HasDialogue(DialogueType type, string name)
		{
			return type switch
			{
				DialogueType.Origin => HasDialogue("dialoguer_data_object", name), 
				DialogueType.Scenario => HasDialogue("scenario_dialoguer_data_object", name), 
				DialogueType.Event => HasDialogue("event_dialoguer_data_object", name), 
				DialogueType.Infinity => HasDialogue("infinity_scenario", name), 
				_ => false, 
			};
		}

		public static DialoguerDialogue GetDialogueByName(string name)
		{
			return _data.dicDialogues[name];
		}
	}
}
