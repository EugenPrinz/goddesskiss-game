using System;

namespace Spine
{
	public class Bone : IUpdatable
	{
		public static bool yDown;

		internal BoneData data;

		internal Skeleton skeleton;

		internal Bone parent;

		internal ExposedList<Bone> children = new ExposedList<Bone>();

		internal float x;

		internal float y;

		internal float rotation;

		internal float scaleX;

		internal float scaleY;

		internal float appliedRotation;

		internal float appliedScaleX;

		internal float appliedScaleY;

		internal float a;

		internal float b;

		internal float worldX;

		internal float c;

		internal float d;

		internal float worldY;

		internal float worldSignX;

		internal float worldSignY;

		public BoneData Data => data;

		public Skeleton Skeleton => skeleton;

		public Bone Parent => parent;

		public ExposedList<Bone> Children => children;

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

		public float AppliedRotation
		{
			get
			{
				return appliedRotation;
			}
			set
			{
				appliedRotation = value;
			}
		}

		public float AppliedScaleX
		{
			get
			{
				return appliedScaleX;
			}
			set
			{
				appliedScaleX = value;
			}
		}

		public float AppliedScaleY
		{
			get
			{
				return appliedScaleY;
			}
			set
			{
				appliedScaleY = value;
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

		public float A => a;

		public float B => b;

		public float C => c;

		public float D => d;

		public float WorldX => worldX;

		public float WorldY => worldY;

		public float WorldSignX => worldSignX;

		public float WorldSignY => worldSignY;

		public float WorldRotationX => MathUtils.Atan2(c, a) * (180f / (float)Math.PI);

		public float WorldRotationY => MathUtils.Atan2(d, b) * (180f / (float)Math.PI);

		public float WorldScaleX => (float)Math.Sqrt(a * a + b * b) * worldSignX;

		public float WorldScaleY => (float)Math.Sqrt(c * c + d * d) * worldSignY;

		public Bone(BoneData data, Skeleton skeleton, Bone parent)
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
			this.skeleton = skeleton;
			this.parent = parent;
			SetToSetupPose();
		}

		public void Update()
		{
			UpdateWorldTransform(x, y, rotation, scaleX, scaleY);
		}

		public void UpdateWorldTransform()
		{
			UpdateWorldTransform(x, y, rotation, scaleX, scaleY);
		}

		public void UpdateWorldTransform(float x, float y, float rotation, float scaleX, float scaleY)
		{
			appliedRotation = rotation;
			appliedScaleX = scaleX;
			appliedScaleY = scaleY;
			float num = MathUtils.CosDeg(rotation);
			float num2 = MathUtils.SinDeg(rotation);
			float num3 = num * scaleX;
			float num4 = (0f - num2) * scaleY;
			float num5 = num2 * scaleX;
			float num6 = num * scaleY;
			Bone bone = parent;
			if (bone == null)
			{
				Skeleton skeleton = this.skeleton;
				if (skeleton.flipX)
				{
					x = 0f - x;
					num3 = 0f - num3;
					num4 = 0f - num4;
				}
				if (skeleton.flipY != yDown)
				{
					y = 0f - y;
					num5 = 0f - num5;
					num6 = 0f - num6;
				}
				a = num3;
				b = num4;
				c = num5;
				d = num6;
				worldX = x;
				worldY = y;
				worldSignX = Math.Sign(scaleX);
				worldSignY = Math.Sign(scaleY);
				return;
			}
			float num7 = bone.a;
			float num8 = bone.b;
			float num9 = bone.c;
			float num10 = bone.d;
			worldX = num7 * x + num8 * y + bone.worldX;
			worldY = num9 * x + num10 * y + bone.worldY;
			worldSignX = bone.worldSignX * (float)Math.Sign(scaleX);
			worldSignY = bone.worldSignY * (float)Math.Sign(scaleY);
			if (data.inheritRotation && data.inheritScale)
			{
				a = num7 * num3 + num8 * num5;
				b = num7 * num4 + num8 * num6;
				c = num9 * num3 + num10 * num5;
				d = num9 * num4 + num10 * num6;
				return;
			}
			if (data.inheritRotation)
			{
				num7 = 1f;
				num8 = 0f;
				num9 = 0f;
				num10 = 1f;
				do
				{
					num = MathUtils.CosDeg(bone.appliedRotation);
					num2 = MathUtils.SinDeg(bone.appliedRotation);
					float num11 = num7 * num + num8 * num2;
					num8 = num7 * (0f - num2) + num8 * num;
					num7 = num11;
					num11 = num9 * num + num10 * num2;
					num10 = num9 * (0f - num2) + num10 * num;
					num9 = num11;
					if (!bone.data.inheritRotation)
					{
						break;
					}
					bone = bone.parent;
				}
				while (bone != null);
				a = num7 * num3 + num8 * num5;
				b = num7 * num4 + num8 * num6;
				c = num9 * num3 + num10 * num5;
				d = num9 * num4 + num10 * num6;
			}
			else if (data.inheritScale)
			{
				num7 = 1f;
				num8 = 0f;
				num9 = 0f;
				num10 = 1f;
				do
				{
					float num12 = bone.rotation;
					num = MathUtils.CosDeg(num12);
					num2 = MathUtils.SinDeg(num12);
					float num13 = bone.appliedScaleX;
					float num14 = bone.appliedScaleY;
					float num15 = num * num13;
					float num16 = (0f - num2) * num14;
					float num17 = num2 * num13;
					float num18 = num * num14;
					float num19 = num7 * num15 + num8 * num17;
					num8 = num7 * num16 + num8 * num18;
					num7 = num19;
					num19 = num9 * num15 + num10 * num17;
					num10 = num9 * num16 + num10 * num18;
					num9 = num19;
					if (num13 < 0f)
					{
						num12 = 0f - num12;
					}
					num = MathUtils.CosDeg(0f - num12);
					num2 = MathUtils.SinDeg(0f - num12);
					num19 = num7 * num + num8 * num2;
					num8 = num7 * (0f - num2) + num8 * num;
					num7 = num19;
					num19 = num9 * num + num10 * num2;
					num10 = num9 * (0f - num2) + num10 * num;
					num9 = num19;
					if (!bone.data.inheritScale)
					{
						break;
					}
					bone = bone.parent;
				}
				while (bone != null);
				a = num7 * num3 + num8 * num5;
				b = num7 * num4 + num8 * num6;
				c = num9 * num3 + num10 * num5;
				d = num9 * num4 + num10 * num6;
			}
			else
			{
				a = num3;
				b = num4;
				c = num5;
				d = num6;
			}
			if (this.skeleton.flipX)
			{
				a = 0f - a;
				b = 0f - b;
			}
			if (this.skeleton.flipY != yDown)
			{
				c = 0f - c;
				d = 0f - d;
			}
		}

		public void SetToSetupPose()
		{
			BoneData boneData = data;
			x = boneData.x;
			y = boneData.y;
			rotation = boneData.rotation;
			scaleX = boneData.scaleX;
			scaleY = boneData.scaleY;
		}

		public void WorldToLocal(float worldX, float worldY, out float localX, out float localY)
		{
			float num = worldX - this.worldX;
			float num2 = worldY - this.worldY;
			float num3 = a;
			float num4 = b;
			float num5 = c;
			float num6 = d;
			float num7 = 1f / (num3 * num6 - num4 * num5);
			localX = num * num6 * num7 - num2 * num4 * num7;
			localY = num2 * num3 * num7 - num * num5 * num7;
		}

		public void LocalToWorld(float localX, float localY, out float worldX, out float worldY)
		{
			worldX = localX * a + localY * b + this.worldX;
			worldY = localX * c + localY * d + this.worldY;
		}

		public override string ToString()
		{
			return data.name;
		}
	}
}
