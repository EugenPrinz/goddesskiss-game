using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmVector3 : FsmVariable
	{
		[SerializeField]
		private Vector3 value;

		public Vector3 Value
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

		public override Type VariableType => typeof(Vector3);

		public override void SetValue(object value)
		{
			this.value = (Vector3)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator Vector3(FsmVector3 value)
		{
			return value.Value;
		}

		public static implicit operator FsmVector3(Vector3 value)
		{
			FsmVector3 fsmVector = ScriptableObject.CreateInstance<FsmVector3>();
			fsmVector.SetValue(value);
			return fsmVector;
		}
	}
}
