using System;

public class ISN_Error
{
	protected int _Code;

	protected string _Description;

	public int Code => _Code;

	public string Description => _Description;

	public ISN_Error(int code, string description)
	{
		_Code = code;
		_Description = description;
	}

	public ISN_Error(string errorData)
	{
		string[] array = errorData.Split('|');
		_Code = Convert.ToInt32(array[0]);
		_Description = array[1];
	}
}
