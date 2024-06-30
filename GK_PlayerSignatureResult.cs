using System;
using System.Collections.Generic;

public class GK_PlayerSignatureResult : ISN_Result
{
	public string _PublicKeyUrl;

	public byte[] _Signature;

	public byte[] _Salt;

	public long _Timestamp;

	public string PublicKeyUrl => _PublicKeyUrl;

	public byte[] Signature => _Signature;

	public byte[] Salt => _Salt;

	public long Timestamp => _Timestamp;

	public GK_PlayerSignatureResult(ISN_Error er)
		: base(IsResultSucceeded: false)
	{
		_Error = er;
	}

	public GK_PlayerSignatureResult(string publicKeyUrl, string signature, string salt, string timestamp)
		: base(IsResultSucceeded: true)
	{
		_PublicKeyUrl = publicKeyUrl;
		string[] array = signature.Split(',');
		List<byte> list = new List<byte>();
		string[] array2 = array;
		foreach (string value in array2)
		{
			list.Add(Convert.ToByte(value));
		}
		_Signature = list.ToArray();
		array = salt.Split(',');
		list = new List<byte>();
		string[] array3 = array;
		foreach (string value2 in array3)
		{
			list.Add(Convert.ToByte(value2));
		}
		_Salt = list.ToArray();
		_Timestamp = Convert.ToInt64(timestamp);
	}
}
