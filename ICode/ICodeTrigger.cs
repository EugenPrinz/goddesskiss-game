using System;
using System.Collections.Generic;
using UnityEngine;

namespace ICode
{
	[AddComponentMenu("ICode/ICodeTrigger")]
	public class ICodeTrigger : MonoBehaviour
	{
		[Serializable]
		public class ComponentModel
		{
			public UnityEngine.Object mObject;

			public bool enable = true;

			public void Process(bool forward)
			{
				bool flag = ((!forward) ? (!enable) : enable);
				if (mObject is Behaviour)
				{
					(mObject as Behaviour).enabled = flag;
				}
				else if (mObject is GameObject)
				{
					(mObject as GameObject).SetActive(flag);
				}
			}
		}

		public string triggerTag = "Player";

		public float radius = 5f;

		public Color color = new Color(1f, 0f, 0f, 0.1f);

		public List<ComponentModel> components;

		public bool parent;

		private void Start()
		{
			if (!parent)
			{
				GameObject gameObject = new GameObject("Trigger");
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = radius;
				sphereCollider.isTrigger = true;
				gameObject.layer = 2;
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				ICodeTrigger codeTrigger = gameObject.AddComponent<ICodeTrigger>();
				codeTrigger.radius = radius;
				codeTrigger.color = color;
				codeTrigger.components = components;
				codeTrigger.parent = true;
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag.Equals(triggerTag))
			{
				for (int i = 0; i < components.Count; i++)
				{
					components[i].Process(forward: true);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag.Equals(triggerTag))
			{
				for (int i = 0; i < components.Count; i++)
				{
					components[i].Process(forward: false);
				}
			}
		}
	}
}
