using UnityEngine;

namespace Step
{
	public class SetIntPlayerPrefs : AbstractStepAction
	{
		public string key;

		public IntData data;

		public override bool Enter()
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			if (data == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			PlayerPrefs.SetInt(key, data.value);
		}
	}
}
