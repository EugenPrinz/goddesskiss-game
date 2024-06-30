using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[RequireComponent(typeof(SkeletonRenderer))]
	public class SkeletonRagdoll2D : MonoBehaviour
	{
		private static Transform helper;

		[Header("Hierarchy")]
		[SpineBone("", "")]
		public string startingBoneName = string.Empty;

		[SpineBone("", "")]
		public List<string> stopBoneNames = new List<string>();

		[Header("Parameters")]
		public bool applyOnStart;

		[Tooltip("Set RootRigidbody IsKinematic to true when Apply is called.")]
		public bool pinStartBone;

		public float gravityScale = 1f;

		[Tooltip("Warning!  You will have to re-enable and tune mix values manually if attempting to remove the ragdoll system.")]
		public bool disableIK = true;

		[Tooltip("If no BoundingBox Attachment is attached to a bone, this becomes the default Width or Radius of a Bone's ragdoll Rigidbody")]
		public float thickness = 0.125f;

		[Tooltip("Default rotational limit value.  Min is negative this value, Max is this value.")]
		public float rotationLimit = 20f;

		public float rootMass = 20f;

		[Tooltip("If your ragdoll seems unstable or uneffected by limits, try lowering this value.")]
		[Range(0.01f, 1f)]
		public float massFalloffFactor = 0.4f;

		[Tooltip("The layer assigned to all of the rigidbody parts.")]
		public int colliderLayer;

		[Range(0f, 1f)]
		public float mix = 1f;

		private Rigidbody2D rootRigidbody;

		private ISkeletonAnimation skeletonAnim;

		private Skeleton skeleton;

		private Dictionary<Bone, Transform> boneTable = new Dictionary<Bone, Transform>();

		private Bone startingBone;

		private Transform ragdollRoot;

		private Vector2 rootOffset;

		private bool isActive;

		public Rigidbody2D RootRigidbody => rootRigidbody;

		public Vector3 RootOffset => rootOffset;

		public Vector3 EstimatedSkeletonPosition => rootRigidbody.position - rootOffset;

		public bool IsActive => isActive;

		private IEnumerator Start()
		{
			skeletonAnim = (ISkeletonAnimation)GetComponent<SkeletonRenderer>();
			if (helper == null)
			{
				helper = new GameObject("Helper").transform;
				helper.hideFlags = HideFlags.HideInHierarchy;
			}
			if (applyOnStart)
			{
				yield return null;
				Apply();
			}
		}

		public Coroutine SmoothMix(float target, float duration)
		{
			return StartCoroutine(SmoothMixCoroutine(target, duration));
		}

		private IEnumerator SmoothMixCoroutine(float target, float duration)
		{
			float startTime = Time.time;
			float startMix = mix;
			while (mix > 0f)
			{
				mix = Mathf.SmoothStep(startMix, target, (Time.time - startTime) / duration);
				yield return null;
			}
		}

		public void SetSkeletonPosition(Vector3 worldPosition)
		{
			if (!isActive)
			{
				return;
			}
			Vector3 vector = worldPosition - base.transform.position;
			base.transform.position = worldPosition;
			foreach (Transform value in boneTable.Values)
			{
				value.position -= vector;
			}
			UpdateWorld(null);
			skeleton.UpdateWorldTransform();
		}

		public Rigidbody2D[] GetRigidbodyArray()
		{
			if (!isActive)
			{
				return new Rigidbody2D[0];
			}
			Rigidbody2D[] array = new Rigidbody2D[boneTable.Count];
			int num = 0;
			foreach (Transform value in boneTable.Values)
			{
				array[num] = value.GetComponent<Rigidbody2D>();
				num++;
			}
			return array;
		}

		public Rigidbody2D GetRigidbody(string boneName)
		{
			Bone bone = skeleton.FindBone(boneName);
			if (bone == null)
			{
				return null;
			}
			if (boneTable.ContainsKey(bone))
			{
				return boneTable[bone].GetComponent<Rigidbody2D>();
			}
			return null;
		}

		public void Remove()
		{
			isActive = false;
			foreach (Transform value in boneTable.Values)
			{
				Object.Destroy(value.gameObject);
			}
			Object.Destroy(ragdollRoot.gameObject);
			boneTable.Clear();
			skeletonAnim.UpdateWorld -= UpdateWorld;
		}

		public void Apply()
		{
			isActive = true;
			skeleton = skeletonAnim.Skeleton;
			mix = 1f;
			Bone bone = (startingBone = skeleton.FindBone(startingBoneName));
			RecursivelyCreateBoneProxies(bone);
			rootRigidbody = boneTable[bone].GetComponent<Rigidbody2D>();
			rootRigidbody.isKinematic = pinStartBone;
			rootRigidbody.mass = rootMass;
			List<Collider2D> list = new List<Collider2D>();
			foreach (KeyValuePair<Bone, Transform> item in boneTable)
			{
				Bone key = item.Key;
				Transform value = item.Value;
				Bone bone2 = null;
				Transform transform = base.transform;
				list.Add(value.GetComponent<Collider2D>());
				if (key != startingBone)
				{
					bone2 = key.Parent;
					transform = boneTable[bone2];
				}
				else
				{
					ragdollRoot = new GameObject("RagdollRoot").transform;
					ragdollRoot.parent = base.transform;
					if (key == skeleton.RootBone)
					{
						ragdollRoot.localPosition = new Vector3(key.WorldX, key.WorldY, 0f);
						ragdollRoot.localRotation = Quaternion.Euler(0f, 0f, GetCompensatedRotationIK(key));
						transform = ragdollRoot;
					}
					else
					{
						ragdollRoot.localPosition = new Vector3(key.Parent.WorldX, key.Parent.WorldY, 0f);
						ragdollRoot.localRotation = Quaternion.Euler(0f, 0f, GetCompensatedRotationIK(key.Parent));
						transform = ragdollRoot;
					}
					rootOffset = value.position - base.transform.position;
				}
				Rigidbody2D component = transform.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					HingeJoint2D hingeJoint2D = value.gameObject.AddComponent<HingeJoint2D>();
					hingeJoint2D.connectedBody = component;
					Vector3 vector = transform.InverseTransformPoint(value.position);
					vector.x *= 1f;
					hingeJoint2D.connectedAnchor = vector;
					hingeJoint2D.GetComponent<Rigidbody2D>().mass = hingeJoint2D.connectedBody.mass * massFalloffFactor;
					JointAngleLimits2D limits = default(JointAngleLimits2D);
					limits.min = 0f - rotationLimit;
					limits.max = rotationLimit;
					hingeJoint2D.limits = limits;
					hingeJoint2D.useLimits = true;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (i != j)
					{
						Physics2D.IgnoreCollision(list[i], list[j]);
					}
				}
			}
			SkeletonUtilityBone[] componentsInChildren = GetComponentsInChildren<SkeletonUtilityBone>();
			if (componentsInChildren.Length > 0)
			{
				List<string> list2 = new List<string>();
				SkeletonUtilityBone[] array = componentsInChildren;
				foreach (SkeletonUtilityBone skeletonUtilityBone in array)
				{
					if (skeletonUtilityBone.mode == SkeletonUtilityBone.Mode.Override)
					{
						list2.Add(skeletonUtilityBone.gameObject.name);
						Object.Destroy(skeletonUtilityBone.gameObject);
					}
				}
				if (list2.Count > 0)
				{
					string text = "Destroyed Utility Bones: ";
					for (int l = 0; l < list2.Count; l++)
					{
						text += list2[l];
						if (l != list2.Count - 1)
						{
							text += ",";
						}
					}
				}
			}
			if (disableIK)
			{
				foreach (IkConstraint ikConstraint in skeleton.IkConstraints)
				{
					ikConstraint.Mix = 0f;
				}
			}
			skeletonAnim.UpdateWorld += UpdateWorld;
		}

		private void RecursivelyCreateBoneProxies(Bone b)
		{
			if (stopBoneNames.Contains(b.Data.Name))
			{
				return;
			}
			GameObject gameObject = new GameObject(b.Data.Name);
			gameObject.layer = colliderLayer;
			Transform transform = gameObject.transform;
			boneTable.Add(b, transform);
			transform.parent = base.transform;
			transform.localPosition = new Vector3(b.WorldX, b.WorldY, 0f);
			transform.localRotation = Quaternion.Euler(0f, 0f, b.WorldRotationX);
			transform.localScale = new Vector3(b.WorldScaleX, b.WorldScaleY, 0f);
			float length = b.Data.Length;
			List<Collider2D> list = AttachBoundingBoxRagdollColliders(b);
			if (length == 0f)
			{
				if (list.Count == 0)
				{
					CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
					circleCollider2D.radius = thickness / 2f;
				}
			}
			else if (list.Count == 0)
			{
				BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
				boxCollider2D.size = new Vector2(length, thickness);
				boxCollider2D.offset = new Vector2(length / 2f, 0f);
			}
			Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.gravityScale = gravityScale;
			foreach (Bone child in b.Children)
			{
				RecursivelyCreateBoneProxies(child);
			}
		}

		private List<Collider2D> AttachBoundingBoxRagdollColliders(Bone b)
		{
			List<Collider2D> list = new List<Collider2D>();
			Transform transform = boneTable[b];
			GameObject gameObject = transform.gameObject;
			Skin skin = skeleton.Skin;
			if (skin == null)
			{
				skin = skeleton.Data.DefaultSkin;
			}
			bool flag = false;
			bool flag2 = false;
			List<Attachment> list2 = new List<Attachment>();
			foreach (Slot slot in skeleton.Slots)
			{
				if (slot.Bone != b)
				{
					continue;
				}
				skin.FindAttachmentsForSlot(skeleton.Slots.IndexOf(slot), list2);
				foreach (Attachment item in list2)
				{
					if (!(item is BoundingBoxAttachment) || !item.Name.ToLower().Contains("ragdoll"))
					{
						continue;
					}
					PolygonCollider2D polygonCollider2D = SkeletonUtility.AddBoundingBoxAsComponent((BoundingBoxAttachment)item, gameObject, isTrigger: false);
					if (flag || flag2)
					{
						Vector2[] points = polygonCollider2D.points;
						for (int i = 0; i < points.Length; i++)
						{
							if (flag)
							{
								points[i].x *= -1f;
							}
							if (flag2)
							{
								points[i].y *= -1f;
							}
						}
						polygonCollider2D.points = points;
					}
					list.Add(polygonCollider2D);
				}
			}
			return list;
		}

		private void UpdateWorld(ISkeletonAnimation skeletonRenderer)
		{
			foreach (KeyValuePair<Bone, Transform> item in boneTable)
			{
				Bone key = item.Key;
				Transform value = item.Value;
				bool flag = false;
				bool flag2 = false;
				Bone bone = null;
				Transform transform = base.transform;
				if (key != startingBone)
				{
					bone = key.Parent;
					transform = boneTable[bone];
				}
				else
				{
					bone = key.Parent;
					transform = ragdollRoot;
					if (key.Parent == null)
					{
						flag = key.Skeleton.FlipX;
						flag2 = key.Skeleton.FlipY;
					}
				}
				helper.position = transform.position;
				helper.rotation = transform.rotation;
				helper.localScale = new Vector3((!flag) ? transform.localScale.x : (0f - transform.localScale.x), (!flag2) ? transform.localScale.y : (0f - transform.localScale.y), 1f);
				Vector3 position = value.position;
				position = helper.InverseTransformPoint(position);
				key.X = Mathf.Lerp(key.X, position.x, mix);
				key.Y = Mathf.Lerp(key.Y, position.y, mix);
				Vector3 vector = helper.InverseTransformDirection(value.right);
				float b = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (bone != null)
				{
				}
				key.Rotation = Mathf.Lerp(key.Rotation, b, mix);
			}
		}

		private float GetCompensatedRotationIK(Bone b)
		{
			Bone parent = b.Parent;
			float num = b.AppliedRotation;
			while (parent != null)
			{
				num += parent.AppliedRotation;
				parent = parent.parent;
			}
			return num;
		}

		private void OnDrawGizmosSelected()
		{
			if (isActive)
			{
				Gizmos.DrawWireSphere(base.transform.position, thickness * 1.2f);
				Vector3 vector = rootRigidbody.position - rootOffset;
				Gizmos.DrawLine(base.transform.position, vector);
				Gizmos.DrawWireSphere(vector, thickness * 1.2f);
			}
		}
	}
}
