using System.Collections;
using UnityEngine;

namespace Unitycoding
{
	public class LookAtMainCamera : MonoBehaviour
	{
		public bool ignoreRaycast = true;

		private Transform target;

		private Transform mTransform;

		private bool searchCamera;

		private void Start()
		{
			if (Camera.main != null)
			{
				target = Camera.main.transform;
			}
			else
			{
				StartCoroutine(SearchCamera());
			}
			mTransform = base.transform;
			if (ignoreRaycast)
			{
				base.gameObject.layer = 2;
			}
		}

		private void Update()
		{
			if (target != null)
			{
				mTransform.LookAt(mTransform.position + target.rotation * Vector3.back, target.rotation * Vector3.up);
			}
			else if (!searchCamera)
			{
				StartCoroutine(SearchCamera());
			}
		}

		private IEnumerator SearchCamera()
		{
			searchCamera = true;
			while (target == null)
			{
				if (Camera.main != null)
				{
					target = Camera.main.transform;
				}
				yield return new WaitForSeconds(2f);
			}
			searchCamera = false;
		}
	}
}
