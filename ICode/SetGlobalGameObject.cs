using UnityEngine;

namespace ICode
{
	public class SetGlobalGameObject : MonoBehaviour
	{
		public string variableName;

		public GameObject target;

		private void Start()
		{
			if (target == null)
			{
				target = base.gameObject;
			}
			GlobalVariables.SetVariable(variableName, target);
		}
	}
}
