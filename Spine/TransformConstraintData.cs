using System;

namespace Spine
{
	public class TransformConstraintData
	{
		internal string name;

		internal BoneData bone;

		internal BoneData target;

		internal float translateMix;

		internal float x;

		internal float y;

		public string Name => name;

		public BoneData Bone
		{
			get
			{
				return bone;
			}
			set
			{
				bone = value;
			}
		}

		public BoneData Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public float TranslateMix
		{
			get
			{
				return translateMix;
			}
			set
			{
				translateMix = value;
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

		public TransformConstraintData(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
