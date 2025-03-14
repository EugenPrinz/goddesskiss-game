using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public class ArraysSubmeshedMeshGenerator : ArraysMeshGenerator, ISubmeshedMeshGenerator
	{
		private class SmartMesh
		{
			public readonly Mesh mesh = SpineMesh.NewMesh();

			private readonly ExposedList<Attachment> attachmentsUsed = new ExposedList<Attachment>();

			private readonly ExposedList<SubmeshInstruction> instructionsUsed = new ExposedList<SubmeshInstruction>();

			public void Set(Vector3[] verts, Vector2[] uvs, Color32[] colors, SubmeshedMeshInstruction instruction)
			{
				mesh.vertices = verts;
				mesh.uv = uvs;
				mesh.colors32 = colors;
				attachmentsUsed.Clear(clearArray: false);
				attachmentsUsed.GrowIfNeeded(instruction.attachmentList.Capacity);
				attachmentsUsed.Count = instruction.attachmentList.Count;
				instruction.attachmentList.CopyTo(attachmentsUsed.Items);
				instructionsUsed.Clear(clearArray: false);
				instructionsUsed.GrowIfNeeded(instruction.submeshInstructions.Capacity);
				instructionsUsed.Count = instruction.submeshInstructions.Count;
				instruction.submeshInstructions.CopyTo(instructionsUsed.Items);
			}

			public bool StructureDoesntMatch(SubmeshedMeshInstruction instructions)
			{
				if (instructions.attachmentList.Count != attachmentsUsed.Count)
				{
					return true;
				}
				if (instructions.submeshInstructions.Count != instructionsUsed.Count)
				{
					return true;
				}
				Attachment[] items = instructions.attachmentList.Items;
				Attachment[] items2 = attachmentsUsed.Items;
				int i = 0;
				for (int count = attachmentsUsed.Count; i < count; i++)
				{
					if (items[i] != items2[i])
					{
						return true;
					}
				}
				SubmeshInstruction[] items3 = instructions.submeshInstructions.Items;
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

		private readonly List<Slot> separators = new List<Slot>();

		public float zSpacing;

		private readonly DoubleBuffered<SmartMesh> doubleBufferedSmartMesh = new DoubleBuffered<SmartMesh>();

		private readonly SubmeshedMeshInstruction currentInstructions = new SubmeshedMeshInstruction();

		private readonly ExposedList<SubmeshTriangleBuffer> submeshBuffers = new ExposedList<SubmeshTriangleBuffer>();

		private Material[] sharedMaterials = new Material[0];

		public List<Slot> Separators => separators;

		public SubmeshedMeshInstruction GenerateInstruction(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton");
			}
			int num = 0;
			int num2 = 0;
			int firstVertexIndex = 0;
			int num3 = 0;
			int startSlot = 0;
			Material material = null;
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			Slot[] items = drawOrder.Items;
			int count = drawOrder.Count;
			int count2 = separators.Count;
			ExposedList<SubmeshInstruction> submeshInstructions = currentInstructions.submeshInstructions;
			submeshInstructions.Clear(clearArray: false);
			currentInstructions.attachmentList.Clear(clearArray: false);
			for (int i = 0; i < count; i++)
			{
				Slot slot = items[i];
				Attachment attachment = slot.attachment;
				object rendererObject;
				int num4;
				int num5;
				if (attachment is RegionAttachment regionAttachment)
				{
					rendererObject = regionAttachment.RendererObject;
					num4 = 4;
					num5 = 6;
				}
				else if (attachment is MeshAttachment meshAttachment)
				{
					rendererObject = meshAttachment.RendererObject;
					num4 = meshAttachment.vertices.Length >> 1;
					num5 = meshAttachment.triangles.Length;
				}
				else
				{
					if (!(attachment is WeightedMeshAttachment weightedMeshAttachment))
					{
						continue;
					}
					rendererObject = weightedMeshAttachment.RendererObject;
					num4 = weightedMeshAttachment.uvs.Length >> 1;
					num5 = weightedMeshAttachment.triangles.Length;
				}
				Material material2 = (Material)((AtlasRegion)rendererObject).page.rendererObject;
				bool flag = count2 > 0 && separators.Contains(slot);
				if ((num > 0 && material.GetInstanceID() != material2.GetInstanceID()) || flag)
				{
					submeshInstructions.Add(new SubmeshInstruction
					{
						skeleton = skeleton,
						material = material,
						triangleCount = num2,
						vertexCount = num3,
						startSlot = startSlot,
						endSlot = i,
						firstVertexIndex = firstVertexIndex,
						forceSeparate = flag
					});
					num2 = 0;
					num3 = 0;
					firstVertexIndex = num;
					startSlot = i;
				}
				material = material2;
				num2 += num5;
				num3 += num4;
				num += num4;
				currentInstructions.attachmentList.Add(attachment);
			}
			submeshInstructions.Add(new SubmeshInstruction
			{
				skeleton = skeleton,
				material = material,
				triangleCount = num2,
				vertexCount = num3,
				startSlot = startSlot,
				endSlot = count,
				firstVertexIndex = firstVertexIndex,
				forceSeparate = false
			});
			currentInstructions.vertexCount = num;
			return currentInstructions;
		}

		public MeshAndMaterials GenerateMesh(SubmeshedMeshInstruction meshInstructions)
		{
			SmartMesh next = doubleBufferedSmartMesh.GetNext();
			Mesh mesh = next.mesh;
			int count = meshInstructions.submeshInstructions.Count;
			ExposedList<SubmeshInstruction> submeshInstructions = meshInstructions.submeshInstructions;
			bool flag = ArraysMeshGenerator.EnsureTriangleBuffersSize(submeshBuffers, count, submeshInstructions.Items);
			bool flag2 = ArraysMeshGenerator.EnsureSize(meshInstructions.vertexCount, ref meshVertices, ref meshUVs, ref meshColors32);
			Vector3[] verts = meshVertices;
			float num = zSpacing;
			int count2 = meshInstructions.attachmentList.Count;
			Vector3 boundsMin = default(Vector3);
			Vector3 boundsMax = default(Vector3);
			if (count2 <= 0)
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
				if (num > 0f)
				{
					boundsMin.z = 0f;
					boundsMax.z = num * (float)(count2 - 1);
				}
				else
				{
					boundsMin.z = num * (float)(count2 - 1);
					boundsMax.z = 0f;
				}
			}
			bool flag3 = flag2 || flag || next.StructureDoesntMatch(meshInstructions);
			int vertexIndex = 0;
			for (int i = 0; i < count; i++)
			{
				SubmeshInstruction submeshInstruction = submeshInstructions.Items[i];
				int startSlot = submeshInstruction.startSlot;
				int endSlot = submeshInstruction.endSlot;
				Skeleton skeleton = submeshInstruction.skeleton;
				ArraysMeshGenerator.FillVerts(skeleton, startSlot, endSlot, num, premultiplyVertexColors, verts, meshUVs, meshColors32, ref vertexIndex, ref attachmentVertexBuffer, ref boundsMin, ref boundsMax);
				if (flag3)
				{
					SubmeshTriangleBuffer submeshTriangleBuffer = submeshBuffers.Items[i];
					bool isLastSubmesh = i == count - 1;
					ArraysMeshGenerator.FillTriangles(skeleton, submeshInstruction.triangleCount, submeshInstruction.firstVertexIndex, startSlot, endSlot, ref submeshTriangleBuffer.triangles, isLastSubmesh);
				}
			}
			if (flag3)
			{
				mesh.Clear();
				sharedMaterials = meshInstructions.GetUpdatedMaterialArray(sharedMaterials);
			}
			next.Set(meshVertices, meshUVs, meshColors32, meshInstructions);
			mesh.bounds = ArraysMeshGenerator.ToBounds(boundsMin, boundsMax);
			if (flag3)
			{
				mesh.subMeshCount = count;
				for (int j = 0; j < count; j++)
				{
					mesh.SetTriangles(submeshBuffers.Items[j].triangles, j);
				}
			}
			return new MeshAndMaterials(next.mesh, sharedMaterials);
		}
	}
}
