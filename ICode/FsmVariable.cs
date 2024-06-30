using System;
using UnityEngine;
using UnityEngine.Events;

namespace ICode
{
	[Serializable]
	public abstract class FsmVariable : ScriptableObject, INameable
	{
		[Serializable]
		public class VariableChangedEvent : UnityEvent<object>
		{
		}

		protected VariableChangedEvent m_OnVariableChange;

		[SerializeField]
		private bool isHidden;

		[SerializeField]
		private bool isShared;

		[SerializeField]
		private new string name = "None";

		[SerializeField]
		private string group = "Default";

		public VariableChangedEvent onVariableChange
		{
			get
			{
				if (m_OnVariableChange == null)
				{
					m_OnVariableChange = new VariableChangedEvent();
				}
				return m_OnVariableChange;
			}
		}

		public bool IsHidden
		{
			get
			{
				return isHidden;
			}
			set
			{
				isHidden = value;
			}
		}

		public bool IsShared
		{
			get
			{
				return isShared;
			}
			set
			{
				isShared = value;
			}
		}

		public bool IsNone => IsShared && Name == "None";

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				base.name = name;
			}
		}

		public string Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public abstract Type VariableType { get; }

		public abstract void SetValue(object value);

		public abstract object GetValue();
	}
}
