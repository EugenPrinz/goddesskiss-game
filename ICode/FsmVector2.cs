using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmVector2 : FsmVariable
	{
		[SerializeField]
		private Vector2 value;

		public Vector2 Value
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

		public override Type VariableType => typeof(Vector2);

		public override void SetValue(object value)
		{
			this.value = (Vector2)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator Vector2(FsmVector2 value)
		{
			return value.Value;
		}

		public static implicit operator FsmVector2(Vector2 value)
		{
			FsmVector2 fsmVector = ScriptableObject.CreateInstance<FsmVector2>();
			fsmVector.SetValue(value);
			return fsmVector;
		}
	}
}
