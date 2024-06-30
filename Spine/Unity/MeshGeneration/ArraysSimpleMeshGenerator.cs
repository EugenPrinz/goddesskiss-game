using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public class ArraysSimpleMeshGenerator : ArraysMeshGenerator, ISimpleMeshGenerator
	{
		protected float scale = 1f;

		private Mesh lastGeneratedMesh;

		private readonly DoubleBufferedMesh doubleBufferedMesh = new DoubleBufferedMesh();

		private int[] triangles;

		private int triangleBufferCount;

		public float Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public Mesh LastGeneratedMesh => lastGeneratedMesh;

		public Mesh GenerateMesh(Skeleton skeleton)
		{
			int num = 0;
			int num2 = 0;
			Slot[] items = skeleton.drawOrder.Items;
			int count = skeleton.drawOrder.Count;
			for (int i = 0; i < count; i++)
			{
				Slot slot = items[i];
				Attachment attachment = slot.attachment;
				int num3;
				int num4;
				if (attachment is RegionAttachment)
				{
					num3 = 4;
					num4 = 6;
				}
				else if (attachment is MeshAttachment meshAttachment)
				{
					num3 = meshAttachment.vertices.Length >> 1;
					num4 = meshAttachment.triangles.Length;
				}
				else
				{
					if (!(attachment is WeightedMeshAttachment weightedMeshAttachment))
					{
						continue;
					}
					num3 = weightedMeshAttachment.uvs.Length >> 1;
					num4 = weightedMeshAttachment.triangles.Length;
				}
				num2 += num4;
				num += num3;
			}
			ArraysMeshGenerator.EnsureSize(num, ref meshVertices, ref meshUVs, ref meshColors32);
			triangles = triangles ?? new int[num2];
			Vector3 boundsMin = default(Vector3);
			Vector3 boundsMax = default(Vector3);
			if (num == 0)
			{
				boundsMin = new Vector3(0f, 0f, 0f);
				boundsMax = new Vector3(0f, 0f, 0f);
			}
			else
			{
				boundsMin.x = 2.1474836E+09f;
				boundsMin.y = 2.1474836E+09f;
				boundsMax.x = -2.1474836E+09f;
				boundsMax.y = -2.1474836E+09f;
				boundsMin.z = -0.01f * scale;
				boundsMax.z = 0.01f * scale;
				int vertexIndex = 0;
				ArraysMeshGenerator.FillVerts(skeleton, 0, count, 0f, premultiplyVertexColors, meshVertices, meshUVs, meshColors32, ref vertexIndex, ref attachmentVertexBuffer, ref boundsMin, ref boundsMax);
				boundsMax.x *= scale;
				boundsMax.y *= scale;
				boundsMin.x *= scale;
				boundsMax.y *= scale;
				Vector3[] array = meshVertices;
				for (int j = 0; j < num; j++)
				{
					Vector3 vector = array[j];
					vector.x *= scale;
					vector.y *= scale;
					array[j] = vector;
				}
			}
			ArraysMeshGenerator.FillTriangles(skeleton, num2, 0, 0, count, ref triangles, isLastSubmesh: true);
			Mesh nextMesh = doubleBufferedMesh.GetNextMesh();
			nextMesh.vertices = meshVertices;
			nextMesh.colors32 = meshColors32;
			nextMesh.uv = meshUVs;
			nextMesh.bounds = ArraysMeshGenerator.ToBounds(boundsMin, boundsMax);
			nextMesh.triangles = triangles;
			lastGeneratedMesh = nextMesh;
			return nextMesh;
		}
	}
}
