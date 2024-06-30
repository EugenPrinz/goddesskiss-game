using UnityEngine;

namespace Step
{
	public class SetStringPlayerPrefs : AbstractStepAction
	{
		public string key;

		public string data;

		public override bool Enter()
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			PlayerPrefs.SetString(key, data);
		}
	}
}
