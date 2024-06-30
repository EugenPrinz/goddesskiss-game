using System;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteInEditMode]
	[AddComponentMenu("Spine/BoneFollower")]
	public class BoneFollower : MonoBehaviour
	{
		public SkeletonRenderer skeletonRenderer;

		[SpineBone("", "skeletonRenderer")]
		public string boneName;

		public bool followZPosition = true;

		public bool followBoneRotation = true;

		public bool resetOnAwake = true;

		[NonSerialized]
		public bool valid;

		[NonSerialized]
		public Bone bone;

		private Transform skeletonTransform;

		public SkeletonRenderer SkeletonRenderer
		{
			get
			{
				return skeletonRenderer;
			}
			set
			{
				skeletonRenderer = value;
				Reset();
			}
		}

		public void HandleResetRenderer(SkeletonRenderer skeletonRenderer)
		{
			Reset();
		}

		public void Reset()
		{
			bone = null;
			valid = skeletonRenderer != null && skeletonRenderer.valid;
			if (valid)
			{
				skeletonTransform = skeletonRenderer.transform;
				SkeletonRenderer obj = skeletonRenderer;
				obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleResetRenderer));
				SkeletonRenderer obj2 = skeletonRenderer;
				obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleResetRenderer));
			}
		}

		private void OnDestroy()
		{
			if (skeletonRenderer != null)
			{
				SkeletonRenderer obj = skeletonRenderer;
				obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleResetRenderer));
			}
		}

		public void Awake()
		{
			if (resetOnAwake)
			{
				Reset();
			}
		}

		private void LateUpdate()
		{
			DoUpdate();
		}

		public void DoUpdate()
		{
			if (!valid)
			{
				Reset();
				return;
			}
			if (bone == null)
			{
				if (boneName == null || boneName.Length == 0)
				{
					return;
				}
				bone = skeletonRenderer.skeleton.FindBone(boneName);
				if (bone == null)
				{
					return;
				}
			}
			Skeleton skeleton = skeletonRenderer.skeleton;
			float num = ((!(skeleton.flipX ^ skeleton.flipY)) ? 1f : (-1f));
			Transform transform = base.transform;
			if (transform.parent == skeletonTransform)
			{
				transform.localPosition = new Vector3(bone.worldX, bone.worldY, (!followZPosition) ? transform.localPosition.z : 0f);
				if (followBoneRotation)
				{
					Vector3 eulerAngles = transform.localRotation.eulerAngles;
					transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, bone.WorldRotationX * num);
				}
				return;
			}
			Vector3 position = skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
			if (!followZPosition)
			{
				position.z = transform.position.z;
			}
			transform.position = position;
			if (followBoneRotation)
			{
				Vector3 eulerAngles2 = skeletonTransform.rotation.eulerAngles;
				transform.rotation = Quaternion.Euler(eulerAngles2.x, eulerAngles2.y, skeletonTransform.rotation.eulerAngles.z + bone.WorldRotationX * num);
			}
		}
	}
}
