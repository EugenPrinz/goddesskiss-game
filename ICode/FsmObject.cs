using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmObject : FsmVariable
	{
		[SerializeField]
		private UnityEngine.Object value;

		public UnityEngine.Object Value
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

		public override Type VariableType => typeof(UnityEngine.Object);

		public override void SetValue(object value)
		{
			this.value = (UnityEngine.Object)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}
	}
}
