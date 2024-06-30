using System;

[Serializable]
public class ISD_VariableListed
{
	public bool IsOpen = true;

	public ISD_PlistValueTypes Type;

	public string DictKey = string.Empty;

	public string StringValue = string.Empty;

	public int IntegerValue;

	public float FloatValue;

	public bool BooleanValue = true;
}
