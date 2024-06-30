using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmInt : FsmVariable
	{
		[SerializeField]
		private int value;

		public int Value
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

		public override Type VariableType => typeof(int);

		public override void SetValue(object value)
		{
			this.value = (int)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator int(FsmInt value)
		{
			return value.Value;
		}

		public static implicit operator FsmInt(int value)
		{
			FsmInt fsmInt = ScriptableObject.CreateInstance<FsmInt>();
			fsmInt.SetValue(value);
			return fsmInt;
		}
	}
}
