using System;

public class ISN_LocalReceiptResult
{
	private byte[] _Receipt;

	private string _ReceiptString = string.Empty;

	public byte[] Receipt => _Receipt;

	public string ReceiptString => _ReceiptString;

	public ISN_LocalReceiptResult(string data)
	{
		if (data.Length > 0)
		{
			_Receipt = Convert.FromBase64String(data);
			_ReceiptString = data;
		}
	}
}
