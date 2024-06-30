namespace Spine
{
	public class BoundingBoxAttachment : Attachment
	{
		internal float[] vertices;

		public float[] Vertices
		{
			get
			{
				return vertices;
			}
			set
			{
				vertices = value;
			}
		}

		public BoundingBoxAttachment(string name)
			: base(name)
		{
		}

		public void ComputeWorldVertices(Bone bone, float[] worldVertices)
		{
			float num = bone.skeleton.x + bone.worldX;
			float num2 = bone.skeleton.y + bone.worldY;
			float a = bone.a;
			float b = bone.b;
			float c = bone.c;
			float d = bone.d;
			float[] array = vertices;
			int i = 0;
			for (int num3 = array.Length; i < num3; i += 2)
			{
				float num4 = array[i];
				float num5 = array[i + 1];
				worldVertices[i] = num4 * a + num5 * b + num;
				worldVertices[i + 1] = num4 * c + num5 * d + num2;
			}
		}
	}
}
