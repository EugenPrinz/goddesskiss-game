using System.Collections.Generic;
using UnityEngine;

namespace ICode
{
	[AddComponentMenu("ICode/OverrideVariables")]
	public class OverrideVariables : MonoBehaviour
	{
		public ICodeBehaviour behaviour;

		public List<SerializedVariable> setVariables;

		private void Start()
		{
			if (!(behaviour.stateMachine != null))
			{
				return;
			}
			if (!behaviour.stateMachine.IsInitialized)
			{
				bool flag = behaviour.enabled;
				behaviour.EnableStateMachine();
				behaviour.enabled = flag;
			}
			for (int i = 0; i < setVariables.Count; i++)
			{
				FsmVariable variable = behaviour.stateMachine.GetVariable(setVariables[i].name);
				if (variable != null)
				{
					variable.SetValue(setVariables[i].GetValue());
				}
			}
		}
	}
}
