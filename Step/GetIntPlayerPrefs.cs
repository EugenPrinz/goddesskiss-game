using UnityEngine;

namespace Step
{
	public class GetIntPlayerPrefs : AbstractStepAction
	{
		public string key;

		public IntData ret;

		public override bool Enter()
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			if (ret == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			ret.value = PlayerPrefs.GetInt(key);
		}
	}
}
