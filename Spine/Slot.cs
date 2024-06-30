using System;

namespace Spine
{
	public class Slot
	{
		internal SlotData data;

		internal Bone bone;

		internal float r;

		internal float g;

		internal float b;

		internal float a;

		internal Attachment attachment;

		internal float attachmentTime;

		internal float[] attachmentVertices = new float[0];

		internal int attachmentVerticesCount;

		public SlotData Data => data;

		public Bone Bone => bone;

		public Skeleton Skeleton => bone.skeleton;

		public float R
		{
			get
			{
				return r;
			}
			set
			{
				r = value;
			}
		}

		public float G
		{
			get
			{
				return g;
			}
			set
			{
				g = value;
			}
		}

		public float B
		{
			get
			{
				return b;
			}
			set
			{
				b = value;
			}
		}

		public float A
		{
			get
			{
				return a;
			}
			set
			{
				a = value;
			}
		}

		public Attachment Attachment
		{
			get
			{
				return attachment;
			}
			set
			{
				if (attachment != value)
				{
					attachment = value;
					attachmentTime = bone.skeleton.time;
					attachmentVerticesCount = 0;
				}
			}
		}

		public float AttachmentTime
		{
			get
			{
				return bone.skeleton.time - attachmentTime;
			}
			set
			{
				attachmentTime = bone.skeleton.time - value;
			}
		}

		public float[] AttachmentVertices
		{
			get
			{
				return attachmentVertices;
			}
			set
			{
				attachmentVertices = value;
			}
		}

		public int AttachmentVerticesCount
		{
			get
			{
				return attachmentVerticesCount;
			}
			set
			{
				attachmentVerticesCount = value;
			}
		}

		public Slot(SlotData data, Bone bone)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			if (bone == null)
			{
				throw new ArgumentNullException("bone cannot be null.");
			}
			this.data = data;
			this.bone = bone;
			SetToSetupPose();
		}

		internal void SetToSetupPose(int slotIndex)
		{
			r = data.r;
			g = data.g;
			b = data.b;
			a = data.a;
			if (data.attachmentName == null)
			{
				Attachment = null;
				return;
			}
			attachment = null;
			Attachment = bone.skeleton.GetAttachment(slotIndex, data.attachmentName);
		}

		public void SetToSetupPose()
		{
			SetToSetupPose(bone.skeleton.data.slots.IndexOf(data));
		}

		public override string ToString()
		{
			return data.name;
		}
	}
}
