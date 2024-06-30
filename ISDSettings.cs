using System.Collections.Generic;
using UnityEngine;

public class ISDSettings : ScriptableObject
{
	public const string VERSION_NUMBER = "2.2";

	public bool IsfwSettingOpen;

	public bool IsLibSettingOpen;

	public bool IslinkerSettingOpne;

	public bool IscompilerSettingsOpen;

	public bool IsPlistSettingsOpen;

	public bool IsLanguageSettingOpen = true;

	public List<ISD_Framework> Frameworks = new List<ISD_Framework>();

	public List<ISD_Lib> Libraries = new List<ISD_Lib>();

	public List<string> compileFlags = new List<string>();

	public List<string> linkFlags = new List<string>();

	public List<ISD_Variable> PlistVariables = new List<ISD_Variable>();

	public List<string> langFolders = new List<string>();

	private const string ISDAssetName = "ISDSettingsResource";

	private const string ISDAssetExtension = ".asset";

	private static ISDSettings instance;

	public static ISDSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load("ISDSettingsResource") as ISDSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<ISDSettings>();
				}
			}
			return instance;
		}
	}

	public bool ContainsFreamworkWithName(string name)
	{
		foreach (ISD_Framework framework in Instance.Frameworks)
		{
			if (framework.Name.Equals(name))
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsPlistVarkWithName(string name)
	{
		foreach (ISD_Variable plistVariable in Instance.PlistVariables)
		{
			if (plistVariable.Name.Equals(name))
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsLibWithName(string name)
	{
		foreach (ISD_Lib library in Instance.Libraries)
		{
			if (library.Name.Equals(name))
			{
				return true;
			}
		}
		return false;
	}
}
