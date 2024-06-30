using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmBool : FsmVariable
	{
		[SerializeField]
		private bool value;

		public bool Value
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

		public override Type VariableType => typeof(bool);

		public override void SetValue(object value)
		{
			this.value = (bool)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator bool(FsmBool value)
		{
			return value.Value;
		}

		public static implicit operator FsmBool(bool value)
		{
			FsmBool fsmBool = ScriptableObject.CreateInstance<FsmBool>();
			fsmBool.SetValue(value);
			return fsmBool;
		}
	}
}
