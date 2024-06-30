using System;

namespace Spine
{
	public class RegionAttachment : Attachment
	{
		public const int X1 = 0;

		public const int Y1 = 1;

		public const int X2 = 2;

		public const int Y2 = 3;

		public const int X3 = 4;

		public const int Y3 = 5;

		public const int X4 = 6;

		public const int Y4 = 7;

		internal float x;

		internal float y;

		internal float rotation;

		internal float scaleX = 1f;

		internal float scaleY = 1f;

		internal float width;

		internal float height;

		internal float regionOffsetX;

		internal float regionOffsetY;

		internal float regionWidth;

		internal float regionHeight;

		internal float regionOriginalWidth;

		internal float regionOriginalHeight;

		internal float[] offset = new float[8];

		internal float[] uvs = new float[8];

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

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

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

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

		public string Path { get; set; }

		public object RendererObject { get; set; }

		public float RegionOffsetX
		{
			get
			{
				return regionOffsetX;
			}
			set
			{
				regionOffsetX = value;
			}
		}

		public float RegionOffsetY
		{
			get
			{
				return regionOffsetY;
			}
			set
			{
				regionOffsetY = value;
			}
		}

		public float RegionWidth
		{
			get
			{
				return regionWidth;
			}
			set
			{
				regionWidth = value;
			}
		}

		public float RegionHeight
		{
			get
			{
				return regionHeight;
			}
			set
			{
				regionHeight = value;
			}
		}

		public float RegionOriginalWidth
		{
			get
			{
				return regionOriginalWidth;
			}
			set
			{
				regionOriginalWidth = value;
			}
		}

		public float RegionOriginalHeight
		{
			get
			{
				return regionOriginalHeight;
			}
			set
			{
				regionOriginalHeight = value;
			}
		}

		public float[] Offset => offset;

		public float[] UVs => uvs;

		public RegionAttachment(string name)
			: base(name)
		{
		}

		public void SetUVs(float u, float v, float u2, float v2, bool rotate)
		{
			float[] array = uvs;
			if (rotate)
			{
				array[2] = u;
				array[3] = v2;
				array[4] = u;
				array[5] = v;
				array[6] = u2;
				array[7] = v;
				array[0] = u2;
				array[1] = v2;
			}
			else
			{
				array[0] = u;
				array[1] = v2;
				array[2] = u;
				array[3] = v;
				array[4] = u2;
				array[5] = v;
				array[6] = u2;
				array[7] = v2;
			}
		}

		public void UpdateOffset()
		{
			float num = width;
			float num2 = height;
			float num3 = scaleX;
			float num4 = scaleY;
			float num5 = num / regionOriginalWidth * num3;
			float num6 = num2 / regionOriginalHeight * num4;
			float num7 = (0f - num) / 2f * num3 + regionOffsetX * num5;
			float num8 = (0f - num2) / 2f * num4 + regionOffsetY * num6;
			float num9 = num7 + regionWidth * num5;
			float num10 = num8 + regionHeight * num6;
			float num11 = rotation * (float)Math.PI / 180f;
			float num12 = (float)Math.Cos(num11);
			float num13 = (float)Math.Sin(num11);
			float num14 = x;
			float num15 = y;
			float num16 = num7 * num12 + num14;
			float num17 = num7 * num13;
			float num18 = num8 * num12 + num15;
			float num19 = num8 * num13;
			float num20 = num9 * num12 + num14;
			float num21 = num9 * num13;
			float num22 = num10 * num12 + num15;
			float num23 = num10 * num13;
			float[] array = offset;
			array[0] = num16 - num19;
			array[1] = num18 + num17;
			array[2] = num16 - num23;
			array[3] = num22 + num17;
			array[4] = num20 - num23;
			array[5] = num22 + num21;
			array[6] = num20 - num19;
			array[7] = num18 + num21;
		}

		public void ComputeWorldVertices(Bone bone, float[] worldVertices)
		{
			float num = bone.skeleton.x + bone.worldX;
			float num2 = bone.skeleton.y + bone.worldY;
			float num3 = bone.a;
			float num4 = bone.b;
			float c = bone.c;
			float d = bone.d;
			float[] array = offset;
			worldVertices[0] = array[0] * num3 + array[1] * num4 + num;
			worldVertices[1] = array[0] * c + array[1] * d + num2;
			worldVertices[2] = array[2] * num3 + array[3] * num4 + num;
			worldVertices[3] = array[2] * c + array[3] * d + num2;
			worldVertices[4] = array[4] * num3 + array[5] * num4 + num;
			worldVertices[5] = array[4] * c + array[5] * d + num2;
			worldVertices[6] = array[6] * num3 + array[7] * num4 + num;
			worldVertices[7] = array[6] * c + array[7] * d + num2;
		}
	}
}
