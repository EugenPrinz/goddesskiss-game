using UnityEngine;

namespace Step
{
	public class CreatePrefab : AbstractStepAction
	{
		public GameObject prefab;

		public GameObjectData parnetView;

		public Vector3Data createPosition;

		public GameObjectData ret;

		public override bool Enter()
		{
			if (prefab == null)
			{
				return false;
			}
			if (parnetView != null && parnetView.value == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			GameObject gameObject = Object.Instantiate(prefab);
			if (parnetView != null)
			{
				gameObject.transform.parent = parnetView.value.transform;
			}
			if (createPosition != null)
			{
				gameObject.transform.position = createPosition.value;
			}
			gameObject.transform.localPosition = gameObject.transform.localPosition + prefab.transform.localPosition;
			gameObject.transform.localScale = prefab.transform.localScale;
			gameObject.transform.localRotation = prefab.transform.localRotation;
			if (ret != null)
			{
				ret.value = gameObject;
			}
		}
	}
}
