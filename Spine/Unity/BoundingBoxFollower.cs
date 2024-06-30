using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteInEditMode]
	public class BoundingBoxFollower : MonoBehaviour
	{
		public SkeletonRenderer skeletonRenderer;

		[SpineSlot("", "skeletonRenderer", true)]
		public string slotName;

		[Tooltip("LOL JK, Someone else do it!")]
		public bool use3DMeshCollider;

		private Slot slot;

		private BoundingBoxAttachment currentAttachment;

		private PolygonCollider2D currentCollider;

		private string currentAttachmentName;

		private bool valid;

		private bool hasReset;

		public Dictionary<BoundingBoxAttachment, PolygonCollider2D> colliderTable = new Dictionary<BoundingBoxAttachment, PolygonCollider2D>();

		public Dictionary<BoundingBoxAttachment, string> attachmentNameTable = new Dictionary<BoundingBoxAttachment, string>();

		public string CurrentAttachmentName => currentAttachmentName;

		public BoundingBoxAttachment CurrentAttachment => currentAttachment;

		public PolygonCollider2D CurrentCollider => currentCollider;

		public Slot Slot => slot;

		private void OnEnable()
		{
			ClearColliders();
			if (skeletonRenderer == null)
			{
				skeletonRenderer = GetComponentInParent<SkeletonRenderer>();
			}
			if (skeletonRenderer != null)
			{
				SkeletonRenderer obj = skeletonRenderer;
				obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleReset));
				SkeletonRenderer obj2 = skeletonRenderer;
				obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleReset));
				if (hasReset)
				{
					HandleReset(skeletonRenderer);
				}
			}
		}

		private void OnDisable()
		{
			SkeletonRenderer obj = skeletonRenderer;
			obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleReset));
		}

		private void Start()
		{
			if (!hasReset && skeletonRenderer != null)
			{
				HandleReset(skeletonRenderer);
			}
		}

		public void HandleReset(SkeletonRenderer renderer)
		{
			if (slotName == null || slotName == string.Empty)
			{
				return;
			}
			hasReset = true;
			ClearColliders();
			colliderTable.Clear();
			if (skeletonRenderer.skeleton == null)
			{
				SkeletonRenderer obj = skeletonRenderer;
				obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleReset));
				skeletonRenderer.Initialize(overwrite: false);
				SkeletonRenderer obj2 = skeletonRenderer;
				obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleReset));
			}
			Skeleton skeleton = skeletonRenderer.skeleton;
			slot = skeleton.FindSlot(slotName);
			int slotIndex = skeleton.FindSlotIndex(slotName);
			foreach (Skin skin in skeleton.Data.Skins)
			{
				List<string> list = new List<string>();
				skin.FindNamesForSlot(slotIndex, list);
				foreach (string item in list)
				{
					Attachment attachment = skin.GetAttachment(slotIndex, item);
					if (attachment is BoundingBoxAttachment)
					{
						PolygonCollider2D polygonCollider2D = SkeletonUtility.AddBoundingBoxAsComponent((BoundingBoxAttachment)attachment, base.gameObject);
						polygonCollider2D.enabled = false;
						polygonCollider2D.hideFlags = HideFlags.HideInInspector;
						colliderTable.Add((BoundingBoxAttachment)attachment, polygonCollider2D);
						attachmentNameTable.Add((BoundingBoxAttachment)attachment, item);
					}
				}
			}
			if (colliderTable.Count == 0)
			{
				valid = false;
			}
			else
			{
				valid = true;
			}
			if (valid)
			{
			}
		}

		private void ClearColliders()
		{
			PolygonCollider2D[] components = GetComponents<PolygonCollider2D>();
			if (Application.isPlaying)
			{
				PolygonCollider2D[] array = components;
				foreach (PolygonCollider2D obj in array)
				{
					UnityEngine.Object.Destroy(obj);
				}
			}
			else
			{
				PolygonCollider2D[] array2 = components;
				foreach (PolygonCollider2D obj2 in array2)
				{
					UnityEngine.Object.DestroyImmediate(obj2);
				}
			}
			colliderTable.Clear();
			attachmentNameTable.Clear();
		}

		private void LateUpdate()
		{
			if (skeletonRenderer.valid && slot != null && slot.Attachment != currentAttachment)
			{
				SetCurrent((BoundingBoxAttachment)slot.Attachment);
			}
		}

		private void SetCurrent(BoundingBoxAttachment attachment)
		{
			if ((bool)currentCollider)
			{
				currentCollider.enabled = false;
			}
			if (attachment != null)
			{
				currentCollider = colliderTable[attachment];
				currentCollider.enabled = true;
			}
			else
			{
				currentCollider = null;
			}
			currentAttachment = attachment;
			currentAttachmentName = ((currentAttachment != null) ? attachmentNameTable[attachment] : null);
		}
	}
}
