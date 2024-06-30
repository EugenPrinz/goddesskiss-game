using System;
using System.Collections.Generic;
using System.Linq;
using ICode.Actions;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class State : Node
	{
		[SerializeField]
		private bool isSequence;

		[SerializeField]
		private StateAction[] actions = new StateAction[0];

		private StateAction activeAction;

		private int activeActionIndex;

		private List<StateAction> activeActions = new List<StateAction>();

		private bool restart;

		public bool IsSequence
		{
			get
			{
				return isSequence;
			}
			set
			{
				isSequence = value;
			}
		}

		public StateAction[] Actions
		{
			get
			{
				return actions;
			}
			set
			{
				actions = value;
			}
		}

		public StateAction ActiveAction => activeAction;

		public override void OnEnter()
		{
			base.OnEnter();
			activeActions = new List<StateAction>(Actions.Where((StateAction action) => action.IsEnabled));
			for (int i = 0; i < activeActions.Count; i++)
			{
				StateAction stateAction = activeActions[i];
				stateAction.Init(this);
				stateAction.OnEnterState();
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			for (int i = 0; i < actions.Length; i++)
			{
				StateAction stateAction = actions[i];
				if (stateAction.IsEnabled && stateAction.IsEntered)
				{
					stateAction.OnExit();
					stateAction.Reset();
				}
			}
			activeActionIndex = 0;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (restart)
			{
				OnExit();
				OnEnter();
				restart = false;
			}
			if (IsSequence && activeActionIndex < activeActions.Count)
			{
				activeAction = activeActions[activeActionIndex];
				if (!activeAction.IsEntered)
				{
					activeAction.IsEntered = true;
					activeAction.OnEnter();
				}
				else if (!activeAction.IsFinished)
				{
					activeAction.OnUpdate();
				}
				if (activeAction.IsFinished)
				{
					activeActionIndex++;
				}
				if (activeActionIndex == activeActions.Count)
				{
					base.Owner.SendEvent("FINISHED", null);
				}
				return;
			}
			for (int i = 0; i < activeActions.Count; i++)
			{
				activeAction = activeActions[i];
				if (!activeAction.IsEntered)
				{
					activeAction.IsEntered = true;
					activeAction.OnEnter();
				}
				else if (!activeAction.IsFinished)
				{
					activeAction.OnUpdate();
				}
			}
		}

		public void Restart()
		{
			restart = true;
		}
	}
}
