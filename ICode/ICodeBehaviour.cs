using System.Linq;
using UnityEngine;

namespace ICode
{
	[AddComponentMenu("ICode/ICodeBehaviour")]
	public class ICodeBehaviour : MonoBehaviour
	{
		public delegate void CustomEvent(string eventName, object parameter);

		public StateMachine stateMachine;

		public bool enableOnStart = true;

		public bool showStateGizmos;

		public bool showSceneIcon;

		public int group;

		[Multiline(5)]
		public string description;

		private Node actveNode;

		private AnyState anyState;

		private Node switchToNode;

		private bool isPaused;

		private bool isDisabled;

		public Node ActiveNode
		{
			get
			{
				return actveNode;
			}
			set
			{
				actveNode = value;
			}
		}

		public AnyState AnyState
		{
			get
			{
				return anyState;
			}
			set
			{
				anyState = value;
			}
		}

		public event CustomEvent onReceiveEvent;

		private void OnEnable()
		{
			if (enableOnStart)
			{
				EnableStateMachine();
			}
		}

		private void Update()
		{
			if (isPaused || isDisabled)
			{
				return;
			}
			if (ActiveNode != null)
			{
				if (!ActiveNode.IsEntered)
				{
					ActiveNode.OnEnter();
				}
				else if (!ActiveNode.IsFinished)
				{
					ActiveNode.OnUpdate();
					UpdateChanges(ActiveNode);
				}
			}
			if (AnyState != null)
			{
				if (!AnyState.IsEntered)
				{
					AnyState.OnEnter();
				}
				else if (!AnyState.IsFinished)
				{
					AnyState.OnUpdate();
					UpdateChanges(AnyState);
				}
			}
		}

		private void SwitchNode(Node toNode)
		{
			if (toNode == null)
			{
				return;
			}
			if (ActiveNode != null)
			{
				ActiveNode.OnExit();
				if (ActiveNode.Parent != null && ActiveNode.Parent != toNode.Parent)
				{
					ActiveNode.Parent.OnExit();
				}
			}
			ActiveNode = toNode;
			if (ActiveNode != null && ActiveNode.Parent != AnyState.Parent)
			{
				AnyState.OnExit();
				AnyState = ActiveNode.Parent.GetAnyState();
				AnyState.OnEnter();
			}
			switchToNode = null;
			ActiveNode.OnEnter();
		}

		private void UpdateChanges(Node node)
		{
			switchToNode = node.ValidateTransitions();
			SwitchNode(switchToNode);
		}

		public void EnableStateMachine()
		{
			if (stateMachine == null)
			{
				base.enabled = false;
				return;
			}
			if (!stateMachine.IsInitialized)
			{
				stateMachine = (StateMachine)FsmUtility.Copy(stateMachine);
				stateMachine.Init(this);
			}
			if (!isPaused)
			{
				ActiveNode = stateMachine.GetStartNode();
				AnyState = stateMachine.GetAnyState();
			}
			isPaused = false;
			isDisabled = false;
			base.enabled = true;
		}

		public void DisableStateMachine(bool pause)
		{
			isPaused = pause;
			isDisabled = true;
		}

		public void SetNode(string nodeName)
		{
			Node toNode = stateMachine.NodesRecursive.First((Node x) => x.Name == nodeName);
			SwitchNode(toNode);
		}

		public void SetNode(Node node)
		{
			SwitchNode(node);
		}

		public void SendEvent(string eventName, object parameter)
		{
			if (this.onReceiveEvent != null)
			{
				this.onReceiveEvent(eventName, parameter);
			}
		}

		public void EnableGroup(int group)
		{
			ICodeBehaviour[] components = base.gameObject.GetComponents<ICodeBehaviour>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].group == group)
				{
					components[i].enabled = true;
				}
			}
		}
	}
}
