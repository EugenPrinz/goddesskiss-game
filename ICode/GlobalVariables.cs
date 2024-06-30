using System;
using System.Collections.Generic;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class GlobalVariables : ScriptableObject
	{
		public const string assetName = "GlobalVariables";

		[SerializeField]
		private FsmVariable[] variables = new FsmVariable[0];

		private static GlobalVariables globalVariables;

		public FsmVariable[] Variables
		{
			get
			{
				return variables;
			}
			set
			{
				variables = value;
			}
		}

		public static GlobalVariables Load()
		{
			return Load("GlobalVariables");
		}

		public static GlobalVariables Load(string name)
		{
			if (globalVariables == null)
			{
				globalVariables = Resources.Load<GlobalVariables>(name);
			}
			return globalVariables;
		}

		public static FsmVariable GetVariable(string name)
		{
			GlobalVariables globalVariables = Load("GlobalVariables");
			if (globalVariables == null)
			{
				return null;
			}
			for (int i = 0; i < globalVariables.Variables.Length; i++)
			{
				FsmVariable fsmVariable = globalVariables.Variables[i];
				if (fsmVariable.Name == name)
				{
					return fsmVariable;
				}
			}
			return null;
		}

		public static bool SetVariable(string name, object value)
		{
			FsmVariable variable = GetVariable(name);
			if (variable != null && variable.VariableType == value.GetType())
			{
				variable.SetValue(value);
				return true;
			}
			return false;
		}

		public static FsmVariable[] GetVariables()
		{
			GlobalVariables globalVariables = Load("GlobalVariables");
			if (globalVariables == null)
			{
				return new FsmVariable[0];
			}
			return globalVariables.Variables;
		}

		public static string[] GetVariableNames(params Type[] types)
		{
			FsmVariable[] array = GetVariables();
			List<string> list = new List<string>();
			FsmVariable[] array2 = array;
			foreach (FsmVariable fsmVariable in array2)
			{
				if (types.Length == 0)
				{
					list.Add(fsmVariable.Name);
					continue;
				}
				foreach (Type type in types)
				{
					if (fsmVariable.GetType() == type)
					{
						list.Add(fsmVariable.Name);
					}
				}
			}
			return list.ToArray();
		}
	}
}
