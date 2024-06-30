using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmColor : FsmVariable
	{
		[SerializeField]
		private Color value;

		public Color Value
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

		public override Type VariableType => typeof(Color);

		public override void SetValue(object value)
		{
			this.value = (Color)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator Color(FsmColor value)
		{
			return value.Value;
		}

		public static implicit operator FsmColor(Color value)
		{
			FsmColor fsmColor = ScriptableObject.CreateInstance<FsmColor>();
			fsmColor.SetValue(value);
			return fsmColor;
		}
	}
}
