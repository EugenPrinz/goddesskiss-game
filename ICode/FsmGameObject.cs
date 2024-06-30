using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmGameObject : FsmVariable
	{
		[SerializeField]
		private GameObject value;

		[SerializeField]
		private string scenePath;

		public GameObject Value
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

		public string ScenePath
		{
			get
			{
				return scenePath;
			}
			set
			{
				scenePath = value;
			}
		}

		public override Type VariableType => typeof(GameObject);

		public override void SetValue(object value)
		{
			this.value = (GameObject)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator GameObject(FsmGameObject value)
		{
			return value.Value;
		}

		public static implicit operator FsmGameObject(GameObject value)
		{
			FsmGameObject fsmGameObject = ScriptableObject.CreateInstance<FsmGameObject>();
			fsmGameObject.SetValue(value);
			return fsmGameObject;
		}
	}
}
