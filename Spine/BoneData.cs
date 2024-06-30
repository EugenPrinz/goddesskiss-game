using System;

namespace Spine
{
	public class BoneData
	{
		internal BoneData parent;

		internal string name;

		internal float length;

		internal float x;

		internal float y;

		internal float rotation;

		internal float scaleX = 1f;

		internal float scaleY = 1f;

		internal bool inheritScale = true;

		internal bool inheritRotation = true;

		public BoneData Parent => parent;

		public string Name => name;

		public float Length
		{
			get
			{
				return length;
			}
			set
			{
				length = value;
			}
		}

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public float Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}

		public float ScaleX
		{
			get
			{
				return scaleX;
			}
			set
			{
				scaleX = value;
			}
		}

		public float ScaleY
		{
			get
			{
				return scaleY;
			}
			set
			{
				scaleY = value;
			}
		}

		public bool InheritScale
		{
			get
			{
				return inheritScale;
			}
			set
			{
				inheritScale = value;
			}
		}

		public bool InheritRotation
		{
			get
			{
				return inheritRotation;
			}
			set
			{
				inheritRotation = value;
			}
		}

		public BoneData(string name, BoneData parent)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			this.name = name;
			this.parent = parent;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
