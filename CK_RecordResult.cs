public class CK_RecordResult : ISN_Result
{
	private CK_Record _Record;

	private CK_Database _Database;

	public CK_Record Record => _Record;

	public CK_Database Database => _Database;

	public CK_RecordResult(int recordId)
		: base(IsResultSucceeded: true)
	{
		_Record = CK_Record.GetRecordByInternalId(recordId);
	}

	public CK_RecordResult(string errorData)
		: base(errorData)
	{
	}

	public void SetDatabase(CK_Database database)
	{
		_Database = database;
	}
}
