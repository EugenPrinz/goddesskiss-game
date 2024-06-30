using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmString : FsmVariable
	{
		[SerializeField]
		private string value;

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				SetValue(value);
			}
		}

		public override Type VariableType => typeof(string);

		public override void SetValue(object value)
		{
			this.value = (string)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator string(FsmString value)
		{
			return value.Value;
		}

		public static implicit operator FsmString(string value)
		{
			FsmString fsmString = ScriptableObject.CreateInstance<FsmString>();
			fsmString.SetValue(value);
			return fsmString;
		}
	}
}
