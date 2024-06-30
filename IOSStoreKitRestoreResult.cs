public class IOSStoreKitRestoreResult : ISN_Result
{
	public IOSTransactionErrorCode TransactionErrorCode
	{
		get
		{
			if (_Error != null)
			{
				return (IOSTransactionErrorCode)_Error.Code;
			}
			return IOSTransactionErrorCode.SKErrorNone;
		}
	}

	public IOSStoreKitRestoreResult(ISN_Error e)
		: base(e)
	{
	}

	public IOSStoreKitRestoreResult(bool IsResultSucceeded)
		: base(IsResultSucceeded)
	{
	}
}
