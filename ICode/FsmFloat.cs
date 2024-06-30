using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmFloat : FsmVariable
	{
		[SerializeField]
		private float value;

		public float Value
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

		public override Type VariableType => typeof(float);

		public override void SetValue(object value)
		{
			this.value = (float)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator float(FsmFloat value)
		{
			return value.Value;
		}

		public static implicit operator FsmFloat(float value)
		{
			FsmFloat fsmFloat = ScriptableObject.CreateInstance<FsmFloat>();
			fsmFloat.SetValue(value);
			return fsmFloat;
		}
	}
}
