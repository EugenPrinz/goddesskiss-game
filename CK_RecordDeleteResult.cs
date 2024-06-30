public class CK_RecordDeleteResult : ISN_Result
{
	private CK_RecordID _RecordID;

	private CK_Database _Database;

	public CK_Database Database => _Database;

	public CK_RecordID RecordID => _RecordID;

	public CK_RecordDeleteResult(int recordId)
		: base(IsResultSucceeded: true)
	{
		_RecordID = CK_RecordID.GetRecordIdByInternalId(recordId);
	}

	public CK_RecordDeleteResult(string errorData)
		: base(errorData)
	{
	}

	public void SetDatabase(CK_Database database)
	{
		_Database = database;
	}
}
