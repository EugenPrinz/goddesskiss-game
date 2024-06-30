public class IOSStoreKitResult : ISN_Result
{
	private string _ProductIdentifier = string.Empty;

	private InAppPurchaseState _State = InAppPurchaseState.Failed;

	private string _Receipt = string.Empty;

	private string _TransactionIdentifier = string.Empty;

	private string _ApplicationUsername = string.Empty;

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

	public InAppPurchaseState State => _State;

	public string ProductIdentifier => _ProductIdentifier;

	public string ApplicationUsername => _ApplicationUsername;

	public string Receipt => _Receipt;

	public string TransactionIdentifier => _TransactionIdentifier;

	public IOSStoreKitResult(string productIdentifier, ISN_Error e)
		: base(e)
	{
		_ProductIdentifier = productIdentifier;
		_State = InAppPurchaseState.Failed;
	}

	public IOSStoreKitResult(string productIdentifier, InAppPurchaseState state, string applicationUsername = "", string receipt = "", string transactionIdentifier = "")
		: base(IsResultSucceeded: true)
	{
		_ProductIdentifier = productIdentifier;
		_State = state;
		_Receipt = receipt;
		_TransactionIdentifier = transactionIdentifier;
		_ApplicationUsername = applicationUsername;
	}
}
