using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmArray : FsmVariable
	{
		[SerializeField]
		private object[] value;

		public object[] Value
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

		public override Type VariableType => typeof(object[]);

		public override void SetValue(object value)
		{
			this.value = (value as IList).Cast<object>().ToArray();
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator object[](FsmArray value)
		{
			return value.Value;
		}

		public static implicit operator FsmArray(object[] value)
		{
			FsmArray fsmArray = ScriptableObject.CreateInstance<FsmArray>();
			fsmArray.SetValue(value);
			return fsmArray;
		}
	}
}
