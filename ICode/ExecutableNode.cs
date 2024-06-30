using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class ExecutableNode : ScriptableObject
	{
		[SerializeField]
		[HideInInspector]
		private bool enabled = true;

		[SerializeField]
		[HideInInspector]
		private bool isOpen = true;

		private bool isEntered;

		private StateMachine root;

		private bool initialized;

		public bool IsEnabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
			set
			{
				isOpen = value;
			}
		}

		public bool IsEntered
		{
			get
			{
				return isEntered;
			}
			set
			{
				isEntered = value;
			}
		}

		public StateMachine Root => root;

		public void Init(Node node)
		{
			if (initialized)
			{
				return;
			}
			initialized = true;
			root = node.Root;
			FieldInfo[] publicFields = GetType().GetPublicFields();
			for (int i = 0; i < publicFields.Length; i++)
			{
				if (!typeof(FsmVariable).IsAssignableFrom(publicFields[i].FieldType))
				{
					continue;
				}
				FsmVariable fsmVariable = (FsmVariable)publicFields[i].GetValue(this);
				if (fsmVariable != null && fsmVariable.IsShared)
				{
					FsmVariable fsmVariable2 = node.Root.GetVariable(fsmVariable.Name) ?? GlobalVariables.GetVariable(fsmVariable.Name);
					if (fsmVariable2 != null)
					{
						publicFields[i].SetValue(this, fsmVariable2);
					}
				}
			}
		}

		public FsmVariable[] GetSharedVariables(Node node)
		{
			List<FsmVariable> list = new List<FsmVariable>();
			FieldInfo[] publicFields = GetType().GetPublicFields();
			for (int i = 0; i < publicFields.Length; i++)
			{
				if (!typeof(FsmVariable).IsAssignableFrom(publicFields[i].FieldType))
				{
					continue;
				}
				FsmVariable fsmVariable = (FsmVariable)publicFields[i].GetValue(this);
				if (fsmVariable != null && fsmVariable.IsShared)
				{
					FsmVariable variable = node.Root.GetVariable(fsmVariable.Name);
					if (variable != null)
					{
						list.Add(variable);
					}
				}
			}
			return list.ToArray();
		}

		public Type CheckForComponents(GameObject gameObject)
		{
			object[] customAttributes = GetType().GetCustomAttributes(inherit: true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i] is RequireComponent requireComponent)
				{
					Type type = ((requireComponent.m_Type0 == null || !(gameObject.GetComponent(requireComponent.m_Type0) == null)) ? null : requireComponent.m_Type0);
					if (type != null)
					{
						return type;
					}
					Type type2 = ((requireComponent.m_Type1 == null || !(gameObject.GetComponent(requireComponent.m_Type1) == null)) ? null : requireComponent.m_Type1);
					if (type2 != null)
					{
						return type2;
					}
					Type type3 = ((requireComponent.m_Type2 == null || !(gameObject.GetComponent(requireComponent.m_Type2) == null)) ? null : requireComponent.m_Type2);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			return null;
		}

		public virtual void OnEnterState()
		{
		}

		public virtual void OnEnter()
		{
		}

		public virtual void OnExit()
		{
		}
	}
}
