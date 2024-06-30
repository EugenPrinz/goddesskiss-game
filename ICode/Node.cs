using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class Node : ScriptableObject, INameable
	{
		public Rect position;

		public int color;

		public string comment;

		public float time;

		[SerializeField]
		private new string name;

		[SerializeField]
		private bool isStartNode;

		private bool isFinished;

		[Reference]
		[SerializeField]
		private StateMachine parent;

		[SerializeField]
		private Transition[] transitions = new Transition[0];

		private bool isEntered;

		private ICodeBehaviour owner;

		private bool isInitialized;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				base.name = value;
			}
		}

		public bool IsStartNode
		{
			get
			{
				return isStartNode;
			}
			set
			{
				isStartNode = value;
			}
		}

		public bool IsFinished
		{
			get
			{
				return isFinished;
			}
			set
			{
				isFinished = value;
			}
		}

		public StateMachine Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		public StateMachine Root => (!(Parent == null) || GetType() != typeof(StateMachine)) ? Parent.Root : ((StateMachine)this);

		public Transition[] Transitions
		{
			get
			{
				return transitions;
			}
			set
			{
				transitions = value;
			}
		}

		public Transition[] InTransitions
		{
			get
			{
				List<Transition> list = new List<Transition>();
				Node[] nodes = Parent.Nodes;
				foreach (Node node in nodes)
				{
					list.AddRange(node.Transitions.Where((Transition x) => x.ToNode == this));
				}
				return list.ToArray();
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

		public ICodeBehaviour Owner => Root.owner;

		public bool IsInitialized => isInitialized;

		public Node ValidateTransitions()
		{
			if (Parent != null)
			{
				Node node = Parent.ValidateTransitions();
				if (node != null)
				{
					return node;
				}
			}
			for (int i = 0; i < Transitions.Length; i++)
			{
				Node node2 = transitions[i].Validate();
				if (node2 != null)
				{
					return node2;
				}
			}
			return null;
		}

		public void Init(ICodeBehaviour component)
		{
			owner = component;
			Root.SetVariable("Owner", owner.gameObject);
			isInitialized = true;
		}

		public virtual void OnUpdate()
		{
			time += Time.deltaTime;
		}

		public virtual void OnEnter()
		{
			IsEntered = true;
			time = 0f;
			for (int i = 0; i < Transitions.Length; i++)
			{
				Transition transition = Transitions[i];
				transition.OnEnter();
			}
		}

		public virtual void OnExit()
		{
			IsEntered = false;
			for (int i = 0; i < Transitions.Length; i++)
			{
				Transition transition = Transitions[i];
				transition.OnExit();
			}
		}
	}
}
