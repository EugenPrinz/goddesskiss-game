using System;

namespace Spine
{
	public class IkConstraint : IUpdatable
	{
		internal IkConstraintData data;

		internal ExposedList<Bone> bones = new ExposedList<Bone>();

		internal Bone target;

		internal int bendDirection;

		internal float mix;

		public IkConstraintData Data => data;

		public ExposedList<Bone> Bones => bones;

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

		public int BendDirection
		{
			get
			{
				return bendDirection;
			}
			set
			{
				bendDirection = value;
			}
		}

		public float Mix
		{
			get
			{
				return mix;
			}
			set
			{
				mix = value;
			}
		}

		public IkConstraint(IkConstraintData data, Skeleton skeleton)
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
			mix = data.mix;
			bendDirection = data.bendDirection;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData bone in data.bones)
			{
				bones.Add(skeleton.FindBone(bone.name));
			}
			target = skeleton.FindBone(data.target.name);
		}

		public void Update()
		{
			Apply();
		}

		public void Apply()
		{
			Bone bone = target;
			ExposedList<Bone> exposedList = bones;
			switch (exposedList.Count)
			{
			case 1:
				Apply(exposedList.Items[0], bone.worldX, bone.worldY, mix);
				break;
			case 2:
				Apply(exposedList.Items[0], exposedList.Items[1], bone.worldX, bone.worldY, bendDirection, mix);
				break;
			}
		}

		public override string ToString()
		{
			return data.name;
		}

		public static void Apply(Bone bone, float targetX, float targetY, float alpha)
		{
			float num = ((bone.parent != null) ? bone.parent.WorldRotationX : 0f);
			float rotation = bone.rotation;
			float num2 = MathUtils.Atan2(targetY - bone.worldY, targetX - bone.worldX) * (180f / (float)Math.PI) - num;
			if (bone.worldSignX != bone.worldSignY != (bone.skeleton.flipX != (bone.skeleton.flipY != Bone.yDown)))
			{
				num2 = 360f - num2;
			}
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			else if (num2 < -180f)
			{
				num2 += 360f;
			}
			bone.UpdateWorldTransform(bone.x, bone.y, rotation + (num2 - rotation) * alpha, bone.appliedScaleX, bone.appliedScaleY);
		}

		public static void Apply(Bone parent, Bone child, float targetX, float targetY, int bendDir, float alpha)
		{
			if (alpha == 0f)
			{
				return;
			}
			float x = parent.x;
			float y = parent.y;
			float num = parent.appliedScaleX;
			float num2 = parent.appliedScaleY;
			int num3;
			int num4;
			if (num < 0f)
			{
				num = 0f - num;
				num3 = 180;
				num4 = -1;
			}
			else
			{
				num3 = 0;
				num4 = 1;
			}
			if (num2 < 0f)
			{
				num2 = 0f - num2;
				num4 = -num4;
			}
			float x2 = child.x;
			float num5 = child.y;
			float num6 = child.appliedScaleX;
			bool flag = Math.Abs(num - num2) <= 0.0001f;
			if (!flag && num5 != 0f)
			{
				child.worldX = parent.a * x2 + parent.worldX;
				child.worldY = parent.c * x2 + parent.worldY;
				num5 = 0f;
			}
			int num7;
			if (num6 < 0f)
			{
				num6 = 0f - num6;
				num7 = 180;
			}
			else
			{
				num7 = 0;
			}
			Bone parent2 = parent.parent;
			float num8;
			float num9;
			float num10;
			float num11;
			if (parent2 == null)
			{
				num8 = targetX - x;
				num9 = targetY - y;
				num10 = child.worldX - x;
				num11 = child.worldY - y;
			}
			else
			{
				float a = parent2.a;
				float b = parent2.b;
				float c = parent2.c;
				float d = parent2.d;
				float num12 = 1f / (a * d - b * c);
				float worldX = parent2.worldX;
				float worldY = parent2.worldY;
				float num13 = targetX - worldX;
				float num14 = targetY - worldY;
				num8 = (num13 * d - num14 * b) * num12 - x;
				num9 = (num14 * a - num13 * c) * num12 - y;
				num13 = child.worldX - worldX;
				num14 = child.worldY - worldY;
				num10 = (num13 * d - num14 * b) * num12 - x;
				num11 = (num14 * a - num13 * c) * num12 - y;
			}
			float num15 = (float)Math.Sqrt(num10 * num10 + num11 * num11);
			float num16 = child.data.length * num6;
			float num21;
			float num18;
			if (flag)
			{
				num16 *= num;
				float num17 = (num8 * num8 + num9 * num9 - num15 * num15 - num16 * num16) / (2f * num15 * num16);
				if (num17 < -1f)
				{
					num17 = -1f;
				}
				else if (num17 > 1f)
				{
					num17 = 1f;
				}
				num18 = (float)Math.Acos(num17) * (float)bendDir;
				float num19 = num15 + num16 * num17;
				float num20 = num16 * MathUtils.Sin(num18);
				num21 = MathUtils.Atan2(num9 * num19 - num8 * num20, num8 * num19 + num9 * num20);
			}
			else
			{
				float num22 = num * num16;
				float num23 = num2 * num16;
				float num24 = MathUtils.Atan2(num9, num8);
				float num25 = num22 * num22;
				float num26 = num23 * num23;
				float num27 = num15 * num15;
				float num28 = num8 * num8 + num9 * num9;
				float num29 = num26 * num27 + num25 * num28 - num25 * num26;
				float num30 = -2f * num26 * num15;
				float num31 = num26 - num25;
				float num32 = num30 * num30 - 4f * num31 * num29;
				if (num32 >= 0f)
				{
					float num33 = (float)Math.Sqrt(num32);
					if (num30 < 0f)
					{
						num33 = 0f - num33;
					}
					num33 = (0f - (num30 + num33)) / 2f;
					float num34 = num33 / num31;
					float num35 = num29 / num33;
					float num36 = ((!(Math.Abs(num34) < Math.Abs(num35))) ? num35 : num34);
					if (num36 * num36 <= num28)
					{
						float num37 = (float)Math.Sqrt(num28 - num36 * num36) * (float)bendDir;
						num21 = num24 - MathUtils.Atan2(num37, num36);
						num18 = MathUtils.Atan2(num37 / num2, (num36 - num15) / num);
						goto IL_04f7;
					}
				}
				float num38 = 0f;
				float num39 = float.MaxValue;
				float x3 = 0f;
				float num40 = 0f;
				float num41 = 0f;
				float num42 = 0f;
				float x4 = 0f;
				float num43 = 0f;
				float num44 = num15 + num22;
				float num45 = num44 * num44;
				if (num45 > num42)
				{
					num41 = 0f;
					num42 = num45;
					x4 = num44;
				}
				num44 = num15 - num22;
				num45 = num44 * num44;
				if (num45 < num39)
				{
					num38 = (float)Math.PI;
					num39 = num45;
					x3 = num44;
				}
				float num46 = (float)Math.Acos((0f - num22) * num15 / (num25 - num26));
				num44 = num22 * MathUtils.Cos(num46) + num15;
				float num47 = num23 * MathUtils.Sin(num46);
				num45 = num44 * num44 + num47 * num47;
				if (num45 < num39)
				{
					num38 = num46;
					num39 = num45;
					x3 = num44;
					num40 = num47;
				}
				if (num45 > num42)
				{
					num41 = num46;
					num42 = num45;
					x4 = num44;
					num43 = num47;
				}
				if (num28 <= (num39 + num42) / 2f)
				{
					num21 = num24 - MathUtils.Atan2(num40 * (float)bendDir, x3);
					num18 = num38 * (float)bendDir;
				}
				else
				{
					num21 = num24 - MathUtils.Atan2(num43 * (float)bendDir, x4);
					num18 = num41 * (float)bendDir;
				}
			}
			goto IL_04f7;
			IL_04f7:
			float num48 = MathUtils.Atan2(num5, x2) * (float)num4;
			num21 = (num21 - num48) * (180f / (float)Math.PI) + (float)num3;
			num18 = (num18 + num48) * (180f / (float)Math.PI) * (float)num4 + (float)num7;
			if (num21 > 180f)
			{
				num21 -= 360f;
			}
			else if (num21 < -180f)
			{
				num21 += 360f;
			}
			if (num18 > 180f)
			{
				num18 -= 360f;
			}
			else if (num18 < -180f)
			{
				num18 += 360f;
			}
			float rotation = parent.rotation;
			parent.UpdateWorldTransform(x, y, rotation + (num21 - rotation) * alpha, parent.appliedScaleX, parent.appliedScaleY);
			rotation = child.rotation;
			child.UpdateWorldTransform(x2, num5, rotation + (num18 - rotation) * alpha, child.appliedScaleX, child.appliedScaleY);
		}
	}
}
