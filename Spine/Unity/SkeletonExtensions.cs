using System;
using UnityEngine;

namespace Spine.Unity
{
	public static class SkeletonExtensions
	{
		private const float ByteToFloat = 0.003921569f;

		public static Color GetColor(this Skeleton s)
		{
			return new Color(s.r, s.g, s.b, s.a);
		}

		public static Color GetColor(this RegionAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static Color GetColor(this MeshAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static Color GetColor(this WeightedMeshAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static void SetColor(this Skeleton skeleton, Color color)
		{
			skeleton.A = color.a;
			skeleton.R = color.r;
			skeleton.G = color.g;
			skeleton.B = color.b;
		}

		public static void SetColor(this Skeleton skeleton, Color32 color)
		{
			skeleton.A = (float)(int)color.a * 0.003921569f;
			skeleton.R = (float)(int)color.r * 0.003921569f;
			skeleton.G = (float)(int)color.g * 0.003921569f;
			skeleton.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this Slot slot, Color color)
		{
			slot.A = color.a;
			slot.R = color.r;
			slot.G = color.g;
			slot.B = color.b;
		}

		public static void SetColor(this Slot slot, Color32 color)
		{
			slot.A = (float)(int)color.a * 0.003921569f;
			slot.R = (float)(int)color.r * 0.003921569f;
			slot.G = (float)(int)color.g * 0.003921569f;
			slot.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this RegionAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this RegionAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this MeshAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this MeshAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this WeightedMeshAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this WeightedMeshAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetPosition(this Bone bone, Vector2 position)
		{
			bone.X = position.x;
			bone.Y = position.y;
		}

		public static void SetPosition(this Bone bone, Vector3 position)
		{
			bone.X = position.x;
			bone.Y = position.y;
		}

		public static Vector2 GetSkeletonSpacePosition(this Bone bone)
		{
			return new Vector2(bone.worldX, bone.worldY);
		}

		public static Vector3 GetWorldPosition(this Bone bone, Transform parentTransform)
		{
			return parentTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY));
		}

		public static void PoseWithAnimation(this Skeleton skeleton, string animationName, float time, bool loop)
		{
			skeleton.data.FindAnimation(animationName)?.Apply(skeleton, 0f, time, loop, null);
		}

		public static void SetDrawOrderToSetupPose(this Skeleton skeleton)
		{
			Slot[] items = skeleton.slots.Items;
			int count = skeleton.slots.Count;
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			drawOrder.Clear(clearArray: false);
			drawOrder.GrowIfNeeded(count);
			Array.Copy(items, drawOrder.Items, count);
		}

		public static void SetColorToSetupPose(this Slot slot)
		{
			slot.r = slot.data.r;
			slot.g = slot.data.g;
			slot.b = slot.data.b;
			slot.a = slot.data.a;
		}

		public static void SetAttachmentToSetupPose(this Slot slot)
		{
			SlotData data = slot.data;
			slot.Attachment = slot.bone.skeleton.GetAttachment(data.name, data.attachmentName);
		}

		public static void SetSlotAttachmentToSetupPose(this Skeleton skeleton, int slotIndex)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			Attachment attachment = skeleton.GetAttachment(slotIndex, slot.data.attachmentName);
			slot.Attachment = attachment;
		}

		public static void SetKeyedItemsToSetupPose(this Animation animation, Skeleton skeleton)
		{
			Timeline[] items = animation.timelines.Items;
			int i = 0;
			for (int num = items.Length; i < num; i++)
			{
				items[i].SetToSetupPose(skeleton);
			}
		}

		public static void SetToSetupPose(this Timeline timeline, Skeleton skeleton)
		{
			if (timeline != null)
			{
				if (timeline is RotateTimeline)
				{
					Bone bone = skeleton.bones.Items[((RotateTimeline)timeline).boneIndex];
					bone.rotation = bone.data.rotation;
				}
				else if (timeline is TranslateTimeline)
				{
					Bone bone2 = skeleton.bones.Items[((TranslateTimeline)timeline).boneIndex];
					bone2.x = bone2.data.x;
					bone2.y = bone2.data.y;
				}
				else if (timeline is ScaleTimeline)
				{
					Bone bone3 = skeleton.bones.Items[((ScaleTimeline)timeline).boneIndex];
					bone3.scaleX = bone3.data.scaleX;
					bone3.scaleY = bone3.data.scaleY;
				}
				else if (timeline is FfdTimeline)
				{
					Slot slot = skeleton.slots.Items[((FfdTimeline)timeline).slotIndex];
					slot.attachmentVerticesCount = 0;
				}
				else if (timeline is AttachmentTimeline)
				{
					skeleton.SetSlotAttachmentToSetupPose(((AttachmentTimeline)timeline).slotIndex);
				}
				else if (timeline is ColorTimeline)
				{
					skeleton.slots.Items[((ColorTimeline)timeline).slotIndex].SetColorToSetupPose();
				}
				else if (timeline is IkConstraintTimeline)
				{
					IkConstraintTimeline ikConstraintTimeline = (IkConstraintTimeline)timeline;
					IkConstraint ikConstraint = skeleton.ikConstraints.Items[ikConstraintTimeline.ikConstraintIndex];
					IkConstraintData data = ikConstraint.data;
					ikConstraint.bendDirection = data.bendDirection;
					ikConstraint.mix = data.mix;
				}
				else if (timeline is DrawOrderTimeline)
				{
					skeleton.SetDrawOrderToSetupPose();
				}
			}
		}
	}
}
