using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SkeletonUtilityKinematicShadow : MonoBehaviour
	{
		public bool hideShadow = true;

		public Transform parent;

		private Dictionary<Transform, Transform> shadowTable;

		private GameObject shadowRoot;

		private void Start()
		{
			shadowRoot = Object.Instantiate(base.gameObject);
			if (hideShadow)
			{
				shadowRoot.hideFlags = HideFlags.HideInHierarchy;
			}
			if (parent == null)
			{
				shadowRoot.transform.parent = base.transform.root;
			}
			else
			{
				shadowRoot.transform.parent = parent;
			}
			shadowTable = new Dictionary<Transform, Transform>();
			Object.Destroy(shadowRoot.GetComponent<SkeletonUtilityKinematicShadow>());
			shadowRoot.transform.position = base.transform.position;
			shadowRoot.transform.rotation = base.transform.rotation;
			Vector3 b = base.transform.TransformPoint(Vector3.right);
			float num = Vector3.Distance(base.transform.position, b);
			shadowRoot.transform.localScale = Vector3.one;
			Joint[] componentsInChildren = shadowRoot.GetComponentsInChildren<Joint>();
			Joint[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].connectedAnchor *= num;
			}
			Joint[] componentsInChildren2 = GetComponentsInChildren<Joint>();
			Joint[] array2 = componentsInChildren2;
			foreach (Joint obj in array2)
			{
				Object.Destroy(obj);
			}
			Rigidbody[] componentsInChildren3 = GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array3 = componentsInChildren3;
			foreach (Rigidbody obj2 in array3)
			{
				Object.Destroy(obj2);
			}
			Collider[] componentsInChildren4 = GetComponentsInChildren<Collider>();
			Collider[] array4 = componentsInChildren4;
			foreach (Collider obj3 in array4)
			{
				Object.Destroy(obj3);
			}
			SkeletonUtilityBone[] componentsInChildren5 = shadowRoot.GetComponentsInChildren<SkeletonUtilityBone>();
			SkeletonUtilityBone[] componentsInChildren6 = GetComponentsInChildren<SkeletonUtilityBone>();
			SkeletonUtilityBone[] array5 = componentsInChildren6;
			foreach (SkeletonUtilityBone skeletonUtilityBone in array5)
			{
				if (skeletonUtilityBone.gameObject == base.gameObject)
				{
					continue;
				}
				SkeletonUtilityBone[] array6 = componentsInChildren5;
				foreach (SkeletonUtilityBone skeletonUtilityBone2 in array6)
				{
					if (!(skeletonUtilityBone2.GetComponent<Rigidbody>() == null) && skeletonUtilityBone2.boneName == skeletonUtilityBone.boneName)
					{
						shadowTable.Add(skeletonUtilityBone2.transform, skeletonUtilityBone.transform);
						break;
					}
				}
			}
			SkeletonUtilityBone[] array7 = componentsInChildren5;
			foreach (SkeletonUtilityBone obj4 in array7)
			{
				Object.Destroy(obj4);
			}
		}

		private void FixedUpdate()
		{
			shadowRoot.GetComponent<Rigidbody>().MovePosition(base.transform.position);
			shadowRoot.GetComponent<Rigidbody>().MoveRotation(base.transform.rotation);
			foreach (KeyValuePair<Transform, Transform> item in shadowTable)
			{
				item.Value.localPosition = item.Key.localPosition;
				item.Value.localRotation = item.Key.localRotation;
			}
		}
	}
}
