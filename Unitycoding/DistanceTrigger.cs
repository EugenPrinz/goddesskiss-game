using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unitycoding
{
	public class DistanceTrigger : MonoBehaviour
	{
		[Serializable]
		public class TriggerEvent : UnityEvent<GameObject>
		{
		}

		[Tag]
		public string triggerTag = "Player";

		public float distance = 2f;

		public TriggerEvent onTriggerEnter;

		public TriggerEvent onTriggerExit;

		protected virtual void Start()
		{
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = distance;
			sphereCollider.isTrigger = true;
			base.gameObject.layer = 2;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == triggerTag)
			{
				onTriggerEnter.Invoke(other.gameObject);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == triggerTag)
			{
				onTriggerExit.Invoke(other.gameObject);
			}
		}
	}
}
