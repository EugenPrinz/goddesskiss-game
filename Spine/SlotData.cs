using System;

namespace Spine
{
	public class SlotData
	{
		internal string name;

		internal BoneData boneData;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal string attachmentName;

		internal BlendMode blendMode;

		public string Name => name;

		public BoneData BoneData => boneData;

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

		public string AttachmentName
		{
			get
			{
				return attachmentName;
			}
			set
			{
				attachmentName = value;
			}
		}

		public BlendMode BlendMode
		{
			get
			{
				return blendMode;
			}
			set
			{
				blendMode = value;
			}
		}

		public SlotData(string name, BoneData boneData)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			if (boneData == null)
			{
				throw new ArgumentNullException("boneData cannot be null.");
			}
			this.name = name;
			this.boneData = boneData;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
