using System;

namespace Spine
{
	public class TransformConstraint : IUpdatable
	{
		internal TransformConstraintData data;

		internal Bone bone;

		internal Bone target;

		internal float translateMix;

		internal float x;

		internal float y;

		public TransformConstraintData Data => data;

		public Bone Bone
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

		public Bone Target
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

		public TransformConstraint(TransformConstraintData data, Skeleton skeleton)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton cannot be null.");
			}
			this.data = data;
			translateMix = data.translateMix;
			x = data.x;
			y = data.y;
			bone = skeleton.FindBone(data.bone.name);
			target = skeleton.FindBone(data.target.name);
		}

		public void Update()
		{
			Apply();
		}

		public void Apply()
		{
			float num = translateMix;
			if (num > 0f)
			{
				Bone bone = this.bone;
				target.LocalToWorld(x, y, out var worldX, out var worldY);
				bone.worldX += (worldX - bone.worldX) * num;
				bone.worldY += (worldY - bone.worldY) * num;
			}
		}

		public override string ToString()
		{
			return data.name;
		}
	}
}
