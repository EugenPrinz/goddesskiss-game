using System;
using System.Collections.Generic;
using System.Linq;
using ICode.Actions;
using ICode.Conditions;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class StateMachine : Node
	{
		[SerializeField]
		private FsmVariable[] variables = new FsmVariable[0];

		[SerializeField]
		private Node[] nodes;

		public FsmVariable[] Variables
		{
			get
			{
				return base.Root.variables;
			}
			set
			{
				base.Root.variables = value;
			}
		}

		public FsmVariable[] VisibleVariables => base.Root.Variables.Where((FsmVariable x) => !x.IsHidden).ToArray();

		public Node[] Nodes
		{
			get
			{
				if (nodes == null)
				{
					nodes = new Node[0];
				}
				return nodes;
			}
			set
			{
				nodes = value;
			}
		}

		public Node[] NodesRecursive
		{
			get
			{
				List<Node> list = new List<Node>();
				if (Nodes.Length > 0)
				{
					list.AddRange(Nodes);
				}
				StateMachine[] stateMachines = StateMachines;
				foreach (StateMachine stateMachine in stateMachines)
				{
					list.AddRange(stateMachine.NodesRecursive);
				}
				return list.ToArray();
			}
		}

		public StateMachine[] StateMachines => nodes.Where((Node node) => node.GetType() == typeof(StateMachine)).Cast<StateMachine>().ToArray();

		public StateMachine[] StateMachinesRecursive => NodesRecursive.Where((Node node) => node.GetType() == typeof(StateMachine)).Cast<StateMachine>().ToArray();

		public State[] States => nodes.Where((Node node) => typeof(State).IsAssignableFrom(node.GetType())).Cast<State>().ToArray();

		public State[] StatesRecursive => NodesRecursive.Where((Node node) => typeof(State).IsAssignableFrom(node.GetType())).Cast<State>().ToArray();

		public StateAction[] ActionsRecursive => NodesRecursive.Where((Node node) => typeof(State).IsAssignableFrom(node.GetType())).Cast<State>().SelectMany((State x) => x.Actions)
			.ToArray();

		public Transition[] TransitionsRecursive => NodesRecursive.SelectMany((Node x) => x.Transitions).ToArray();

		public Condition[] ConditionsRecursive => TransitionsRecursive.SelectMany((Transition x) => x.Conditions).ToArray();

		public ExecutableNode[] ExecutableNodesRecursive
		{
			get
			{
				List<ExecutableNode> list = new List<ExecutableNode>(ActionsRecursive);
				list.AddRange(ConditionsRecursive);
				return list.ToArray();
			}
		}

		public FsmVariable GetVariable(string name)
		{
			for (int i = 0; i < base.Root.Variables.Length; i++)
			{
				FsmVariable fsmVariable = base.Root.Variables[i];
				if (fsmVariable.Name == name)
				{
					return fsmVariable;
				}
			}
			return null;
		}

		public bool SetVariable(string name, object value)
		{
			FsmVariable variable = GetVariable(name);
			if (variable != null && variable.VariableType.IsAssignableFrom(value.GetType()))
			{
				variable.SetValue(value);
				return true;
			}
			if (value != null)
			{
				variable = (FsmVariable)ScriptableObject.CreateInstance(FsmUtility.GetVariableType(value.GetType()));
				variable.hideFlags = HideFlags.HideInHierarchy;
				variable.Name = name;
				variable.SetValue(value);
				Variables = ArrayUtility.Add(Variables, variable);
			}
			return false;
		}

		public State GetState(string stateName)
		{
			for (int i = 0; i < States.Length; i++)
			{
				State state = States[i];
				if (state.Name == stateName)
				{
					return state;
				}
			}
			return null;
		}

		public Node GetNode(string nodeName)
		{
			Node[] nodesRecursive = NodesRecursive;
			for (int i = 0; i < nodesRecursive.Length; i++)
			{
				Node node = nodes[i];
				if (node.Name == nodeName)
				{
					return node;
				}
			}
			return null;
		}

		public Node GetStartNode()
		{
			for (int i = 0; i < Nodes.Length; i++)
			{
				Node node = Nodes[i];
				if (node.IsStartNode)
				{
					return node;
				}
			}
			return null;
		}

		public AnyState GetAnyState()
		{
			for (int i = 0; i < States.Length; i++)
			{
				State state = States[i];
				if (state is AnyState)
				{
					return state as AnyState;
				}
			}
			return null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			base.Owner.ActiveNode = GetStartNode();
			base.Owner.AnyState = GetAnyState();
		}
	}
}
