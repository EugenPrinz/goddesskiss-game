using System;
using ICode.Conditions;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class Transition : ScriptableObject
	{
		[Reference]
		[SerializeField]
		private Node toNode;

		[Reference]
		[SerializeField]
		private Node fromNode;

		[SerializeField]
		private bool mute;

		[SerializeField]
		private Condition[] conditions = new Condition[0];

		public Node ToNode
		{
			get
			{
				return toNode;
			}
			set
			{
				toNode = value;
			}
		}

		public Node FromNode
		{
			get
			{
				return fromNode;
			}
			set
			{
				fromNode = value;
			}
		}

		public bool Mute
		{
			get
			{
				return mute;
			}
			set
			{
				mute = value;
			}
		}

		public Condition[] Conditions
		{
			get
			{
				return conditions;
			}
			set
			{
				conditions = value;
			}
		}

		public void Init(Node toNode, Node fromNode)
		{
			this.toNode = toNode;
			this.fromNode = fromNode;
		}

		public Node Validate()
		{
			if (mute)
			{
				return null;
			}
			for (int i = 0; i < conditions.Length; i++)
			{
				if (conditions[i].IsEnabled && !conditions[i].Validate())
				{
					return null;
				}
			}
			return ToNode;
		}

		public virtual void OnEnter()
		{
			for (int i = 0; i < conditions.Length; i++)
			{
				Condition condition = conditions[i];
				if (!condition.IsEntered)
				{
					condition.IsEntered = true;
					condition.Init(FromNode);
					condition.OnEnter();
				}
			}
		}

		public virtual void OnExit()
		{
			for (int i = 0; i < conditions.Length; i++)
			{
				Condition condition = conditions[i];
				condition.IsEntered = false;
				condition.OnExit();
			}
		}
	}
}
