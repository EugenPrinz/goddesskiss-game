using System;

public class GK_SavedGame
{
	private string _Id;

	private string _Name;

	private string _DeviceName;

	private DateTime _ModificationDate;

	private byte[] _Data;

	private bool _IsDataLoaded;

	public string Id => _Id;

	public string Name => _Name;

	public string DeviceName => _DeviceName;

	public DateTime ModificationDate => _ModificationDate;

	public byte[] Data => _Data;

	public bool IsDataLoaded => _IsDataLoaded;

	public event Action<GK_SaveDataLoaded> ActionDataLoaded = delegate
	{
	};

	public GK_SavedGame(string id, string name, string device, string dateString)
	{
		_Id = id;
		_Name = name;
		_DeviceName = device;
		_ModificationDate = DateTime.Parse(dateString);
	}

	public void LoadData()
	{
		ISN_Singleton<ISN_GameSaves>.Instance.LoadSaveData(this);
	}

	public void GenerateDataLoadEvent(string base64Data)
	{
		_Data = Convert.FromBase64String(base64Data);
		_IsDataLoaded = true;
		GK_SaveDataLoaded obj = new GK_SaveDataLoaded(this);
		this.ActionDataLoaded(obj);
	}

	public void GenerateDataLoadFailedEvent(string erorrData)
	{
		GK_SaveDataLoaded obj = new GK_SaveDataLoaded(erorrData);
		this.ActionDataLoaded(obj);
	}
}
