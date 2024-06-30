using System;
using System.Collections.Generic;

[Serializable]
public class ISD_Variable
{
	public bool IsOpen = true;

	public bool IsListOpen = true;

	public string Name;

	public ISD_PlistValueTypes Type;

	public ISD_PlistValueTypes ArrayType;

	public string StringValue = string.Empty;

	public int IntegerValue;

	public float FloatValue;

	public bool BooleanValue = true;

	public List<ISD_VariableListed> ArrayValue = new List<ISD_VariableListed>();

	public List<ISD_VariableListed> DictValues = new List<ISD_VariableListed>();

	public Dictionary<string, ISD_VariableListed> DictionaryValue
	{
		get
		{
			Dictionary<string, ISD_VariableListed> dictionary = new Dictionary<string, ISD_VariableListed>();
			foreach (ISD_VariableListed dictValue in DictValues)
			{
				dictionary.Add(dictValue.DictKey, dictValue);
			}
			return dictionary;
		}
	}

	public void AddVarToDictionary(ISD_VariableListed v)
	{
		bool flag = true;
		foreach (ISD_VariableListed dictValue in DictValues)
		{
			if (dictValue.DictKey.Equals(v.DictKey))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			DictValues.Add(v);
		}
	}
}
