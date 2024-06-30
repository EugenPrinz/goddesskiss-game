using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public class ArraysSubmeshSetMeshGenerator : ArraysMeshGenerator, ISubmeshSetMeshGenerator
	{
		private class SmartMesh
		{
			public readonly Mesh mesh = SpineMesh.NewMesh();

			private readonly ExposedList<Attachment> attachmentsUsed = new ExposedList<Attachment>();

			private readonly ExposedList<SubmeshInstruction> instructionsUsed = new ExposedList<SubmeshInstruction>();

			public void Set(Vector3[] verts, Vector2[] uvs, Color32[] colors, ExposedList<Attachment> attachments, ExposedList<SubmeshInstruction> instructions)
			{
				mesh.vertices = verts;
				mesh.uv = uvs;
				mesh.colors32 = colors;
				attachmentsUsed.Clear(clearArray: false);
				attachmentsUsed.GrowIfNeeded(attachments.Capacity);
				attachmentsUsed.Count = attachments.Count;
				attachments.CopyTo(attachmentsUsed.Items);
				instructionsUsed.Clear(clearArray: false);
				instructionsUsed.GrowIfNeeded(instructions.Capacity);
				instructionsUsed.Count = instructions.Count;
				instructions.CopyTo(instructionsUsed.Items);
			}

			public bool StructureDoesntMatch(ExposedList<Attachment> attachments, ExposedList<SubmeshInstruction> instructions)
			{
				if (attachments.Count != attachmentsUsed.Count)
				{
					return true;
				}
				if (instructions.Count != instructionsUsed.Count)
				{
					return true;
				}
				Attachment[] items = attachments.Items;
				Attachment[] items2 = attachmentsUsed.Items;
				int i = 0;
				for (int count = attachmentsUsed.Count; i < count; i++)
				{
					if (items[i] != items2[i])
					{
						return true;
					}
				}
				SubmeshInstruction[] items3 = instructions.Items;
				SubmeshInstruction[] items4 = instructionsUsed.Items;
				int j = 0;
				for (int count2 = instructionsUsed.Count; j < count2; j++)
				{
					SubmeshInstruction submeshInstruction = items3[j];
					SubmeshInstruction submeshInstruction2 = items4[j];
					if (submeshInstruction.material.GetInstanceID() != submeshInstruction2.material.GetInstanceID() || submeshInstruction.startSlot != submeshInstruction2.startSlot || submeshInstruction.endSlot != submeshInstruction2.endSlot || submeshInstruction.triangleCount != submeshInstruction2.triangleCount || submeshInstruction.vertexCount != submeshInstruction2.vertexCount || submeshInstruction.firstVertexIndex != submeshInstruction2.firstVertexIndex)
					{
						return true;
					}
				}
				return false;
			}
		}

		public float zSpacing;

		private readonly DoubleBuffered<SmartMesh> doubleBufferedSmartMesh = new DoubleBuffered<SmartMesh>();

		private readonly ExposedList<SubmeshInstruction> currentInstructions = new ExposedList<SubmeshInstruction>();

		private readonly ExposedList<Attachment> attachmentBuffer = new ExposedList<Attachment>();

		private readonly ExposedList<SubmeshTriangleBuffer> submeshBuffers = new ExposedList<SubmeshTriangleBuffer>();

		private Material[] sharedMaterials = new Material[0];

		public MeshAndMaterials GenerateMesh(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh)
		{
			SubmeshInstruction[] items = instructions.Items;
			currentInstructions.Clear(clearArray: false);
			for (int i = startSubmesh; i < endSubmesh; i++)
			{
				currentInstructions.Add(items[i]);
			}
			SmartMesh next = doubleBufferedSmartMesh.GetNext();
			Mesh mesh = next.mesh;
			int count = currentInstructions.Count;
			SubmeshInstruction[] items2 = currentInstructions.Items;
			int num = 0;
			for (int j = 0; j < count; j++)
			{
				items2[j].firstVertexIndex = num;
				num += items2[j].vertexCount;
			}
			bool flag = ArraysMeshGenerator.EnsureSize(num, ref meshVertices, ref meshUVs, ref meshColors32);
			bool flag2 = ArraysMeshGenerator.EnsureTriangleBuffersSize(submeshBuffers, count, items2);
			float num2 = zSpacing;
			Vector3 boundsMin = default(Vector3);
			Vector3 boundsMax = default(Vector3);
			if (num <= 0)
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
				int endSlot = items2[count - 1].endSlot;
				if (num2 > 0f)
				{
					boundsMin.z = 0f;
					boundsMax.z = num2 * (float)endSlot;
				}
				else
				{
					boundsMin.z = num2 * (float)endSlot;
					boundsMax.z = 0f;
				}
			}
			ExposedList<Attachment> exposedList = attachmentBuffer;
			exposedList.Clear(clearArray: false);
			int vertexIndex = 0;
			for (int k = 0; k < count; k++)
			{
				SubmeshInstruction submeshInstruction = items2[k];
				int startSlot = submeshInstruction.startSlot;
				int endSlot2 = submeshInstruction.endSlot;
				Skeleton skeleton = submeshInstruction.skeleton;
				Slot[] items3 = skeleton.DrawOrder.Items;
				for (int l = startSlot; l < endSlot2; l++)
				{
					Attachment attachment = items3[l].attachment;
					if (attachment != null)
					{
						exposedList.Add(attachment);
					}
				}
				ArraysMeshGenerator.FillVerts(skeleton, startSlot, endSlot2, num2, premultiplyVertexColors, meshVertices, meshUVs, meshColors32, ref vertexIndex, ref attachmentVertexBuffer, ref boundsMin, ref boundsMax);
			}
			bool flag3 = flag || flag2 || next.StructureDoesntMatch(exposedList, currentInstructions);
			for (int m = 0; m < count; m++)
			{
				SubmeshInstruction submeshInstruction2 = items2[m];
				if (flag3)
				{
					SubmeshTriangleBuffer submeshTriangleBuffer = submeshBuffers.Items[m];
					bool isLastSubmesh = m == count - 1;
					ArraysMeshGenerator.FillTriangles(submeshInstruction2.skeleton, submeshInstruction2.triangleCount, submeshInstruction2.firstVertexIndex, submeshInstruction2.startSlot, submeshInstruction2.endSlot, ref submeshTriangleBuffer.triangles, isLastSubmesh);
				}
			}
			if (flag3)
			{
				mesh.Clear();
				sharedMaterials = currentInstructions.GetUpdatedMaterialArray(sharedMaterials);
			}
			next.Set(meshVertices, meshUVs, meshColors32, exposedList, currentInstructions);
			mesh.bounds = ArraysMeshGenerator.ToBounds(boundsMin, boundsMax);
			TryAddNormalsTo(mesh, num);
			if (flag3)
			{
				mesh.subMeshCount = count;
				for (int n = 0; n < count; n++)
				{
					mesh.SetTriangles(submeshBuffers.Items[n].triangles, n);
				}
			}
			return new MeshAndMaterials(next.mesh, sharedMaterials);
		}
	}
}
