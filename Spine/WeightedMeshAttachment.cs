namespace Spine
{
	public class WeightedMeshAttachment : Attachment, IFfdAttachment
	{
		internal int[] bones;

		internal float[] weights;

		internal float[] uvs;

		internal float[] regionUVs;

		internal int[] triangles;

		internal float regionOffsetX;

		internal float regionOffsetY;

		internal float regionWidth;

		internal float regionHeight;

		internal float regionOriginalWidth;

		internal float regionOriginalHeight;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal WeightedMeshAttachment parentMesh;

		internal bool inheritFFD;

		public int HullLength { get; set; }

		public int[] Bones
		{
			get
			{
				return bones;
			}
			set
			{
				bones = value;
			}
		}

		public float[] Weights
		{
			get
			{
				return weights;
			}
			set
			{
				weights = value;
			}
		}

		public float[] RegionUVs
		{
			get
			{
				return regionUVs;
			}
			set
			{
				regionUVs = value;
			}
		}

		public float[] UVs
		{
			get
			{
				return uvs;
			}
			set
			{
				uvs = value;
			}
		}

		public int[] Triangles
		{
			get
			{
				return triangles;
			}
			set
			{
				triangles = value;
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

		public float RegionU { get; set; }

		public float RegionV { get; set; }

		public float RegionU2 { get; set; }

		public float RegionV2 { get; set; }

		public bool RegionRotate { get; set; }

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

		public bool InheritFFD
		{
			get
			{
				return inheritFFD;
			}
			set
			{
				inheritFFD = value;
			}
		}

		public WeightedMeshAttachment ParentMesh
		{
			get
			{
				return parentMesh;
			}
			set
			{
				parentMesh = value;
				if (value != null)
				{
					bones = value.bones;
					weights = value.weights;
					regionUVs = value.regionUVs;
					triangles = value.triangles;
					HullLength = value.HullLength;
					Edges = value.Edges;
					Width = value.Width;
					Height = value.Height;
				}
			}
		}

		public int[] Edges { get; set; }

		public float Width { get; set; }

		public float Height { get; set; }

		public WeightedMeshAttachment(string name)
			: base(name)
		{
		}

		public void UpdateUVs()
		{
			float regionU = RegionU;
			float regionV = RegionV;
			float num = RegionU2 - RegionU;
			float num2 = RegionV2 - RegionV;
			float[] array = regionUVs;
			if (uvs == null || uvs.Length != array.Length)
			{
				uvs = new float[array.Length];
			}
			float[] array2 = uvs;
			if (RegionRotate)
			{
				int i = 0;
				for (int num3 = array2.Length; i < num3; i += 2)
				{
					array2[i] = regionU + array[i + 1] * num;
					array2[i + 1] = regionV + num2 - array[i] * num2;
				}
			}
			else
			{
				int j = 0;
				for (int num4 = array2.Length; j < num4; j += 2)
				{
					array2[j] = regionU + array[j] * num;
					array2[j + 1] = regionV + array[j + 1] * num2;
				}
			}
		}

		public void ComputeWorldVertices(Slot slot, float[] worldVertices)
		{
			Skeleton skeleton = slot.bone.skeleton;
			ExposedList<Bone> exposedList = skeleton.bones;
			float x = skeleton.x;
			float y = skeleton.y;
			float[] array = weights;
			int[] array2 = bones;
			if (slot.attachmentVerticesCount == 0)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = array2.Length;
				while (num2 < num4)
				{
					float num5 = 0f;
					float num6 = 0f;
					int num7 = array2[num2++] + num2;
					while (num2 < num7)
					{
						Bone bone = exposedList.Items[array2[num2]];
						float num8 = array[num3];
						float num9 = array[num3 + 1];
						float num10 = array[num3 + 2];
						num5 += (num8 * bone.a + num9 * bone.b + bone.worldX) * num10;
						num6 += (num8 * bone.c + num9 * bone.d + bone.worldY) * num10;
						num2++;
						num3 += 3;
					}
					worldVertices[num] = num5 + x;
					worldVertices[num + 1] = num6 + y;
					num += 2;
				}
				return;
			}
			float[] attachmentVertices = slot.attachmentVertices;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = array2.Length;
			while (num12 < num15)
			{
				float num16 = 0f;
				float num17 = 0f;
				int num18 = array2[num12++] + num12;
				while (num12 < num18)
				{
					Bone bone2 = exposedList.Items[array2[num12]];
					float num19 = array[num13] + attachmentVertices[num14];
					float num20 = array[num13 + 1] + attachmentVertices[num14 + 1];
					float num21 = array[num13 + 2];
					num16 += (num19 * bone2.a + num20 * bone2.b + bone2.worldX) * num21;
					num17 += (num19 * bone2.c + num20 * bone2.d + bone2.worldY) * num21;
					num12++;
					num13 += 3;
					num14 += 2;
				}
				worldVertices[num11] = num16 + x;
				worldVertices[num11 + 1] = num17 + y;
				num11 += 2;
			}
		}

		public bool ApplyFFD(Attachment sourceAttachment)
		{
			return this == sourceAttachment || (inheritFFD && parentMesh == sourceAttachment);
		}
	}
}
