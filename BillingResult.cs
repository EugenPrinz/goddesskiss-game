public class BillingResult
{
	private int _response;

	private string _message;

	private GooglePurchaseTemplate _purchase;

	public GooglePurchaseTemplate purchase => _purchase;

	public int response => _response;

	public string message => _message;

	public bool isSuccess => _response == 0;

	public bool isFailure => !isSuccess;

	public BillingResult(int code, string msg, GooglePurchaseTemplate p)
		: this(code, msg)
	{
		_purchase = p;
	}

	public BillingResult(int code, string msg)
	{
		_response = code;
		_message = msg;
	}
}
