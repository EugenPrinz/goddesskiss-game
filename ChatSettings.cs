using UnityEngine;

public class ChatSettings : ScriptableObject
{
	public string AppId;

	[HideInInspector]
	public bool WizardDone;

	private static ChatSettings instance;

	public static ChatSettings Instance
	{
		get
		{
			if (instance != null)
			{
				return instance;
			}
			instance = Load();
			return instance;
		}
	}

	public static ChatSettings Load()
	{
		ChatSettings chatSettings = (ChatSettings)Resources.Load("ChatSettingsFile", typeof(ChatSettings));
		if (chatSettings != null)
		{
			return chatSettings;
		}
		return Create();
	}

	private static ChatSettings Create()
	{
		return (ChatSettings)ScriptableObject.CreateInstance("ChatSettings");
	}
}
