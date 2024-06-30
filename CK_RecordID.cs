using System.Collections.Generic;

public class CK_RecordID
{
	private int _internalId;

	private string _Name;

	private static Dictionary<int, CK_RecordID> _Ids = new Dictionary<int, CK_RecordID>();

	public string Name => _Name;

	public int Internal_Id => _internalId;

	public CK_RecordID(string recordName)
	{
		_internalId = SA_IdFactory.NextId;
		_Name = recordName;
		ISN_CloudKit.CreateRecordId_Object(_internalId, _Name);
		_Ids.Add(_internalId, this);
	}

	public static CK_RecordID GetRecordIdByInternalId(int id)
	{
		return _Ids[id];
	}
}
