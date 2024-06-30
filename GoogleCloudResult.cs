using System;
using System.Text;

public class GoogleCloudResult
{
	private GP_AppStateStatusCodes _response;

	private string _message;

	private int _stateKey;

	public byte[] stateData;

	public byte[] serverConflictData;

	public string resolvedVersion;

	public GP_AppStateStatusCodes response => _response;

	public string stateDataString
	{
		get
		{
			if (stateData == null)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(stateData);
		}
	}

	public string serverConflictDataString
	{
		get
		{
			if (serverConflictData == null)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(stateData);
		}
	}

	public string message => _message;

	public int stateKey => _stateKey;

	public bool isSuccess => _response == GP_AppStateStatusCodes.STATUS_OK;

	public bool isFailure => !isSuccess;

	public GoogleCloudResult(string code)
	{
		_response = (GP_AppStateStatusCodes)Convert.ToInt32(code);
		_message = _response.ToString();
	}

	public GoogleCloudResult(string code, string key)
	{
		_response = (GP_AppStateStatusCodes)Convert.ToInt32(code);
		_message = _response.ToString();
		_stateKey = Convert.ToInt32(key);
	}
}
