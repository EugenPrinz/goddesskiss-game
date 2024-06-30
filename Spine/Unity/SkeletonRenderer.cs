using System;
using System.Collections.Generic;
using Spine.Unity.MeshGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spine.Unity
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[DisallowMultipleComponent]
	[HelpURL("http://esotericsoftware.com/spine-unity-documentation#Rendering")]
	public class SkeletonRenderer : MonoBehaviour
	{
		public delegate void SkeletonRendererDelegate(SkeletonRenderer skeletonRenderer);

		public delegate void InstructionDelegate(SmartMesh.Instruction instruction);

		public class SmartMesh
		{
			public class Instruction
			{
				public bool immutableTriangles;

				public int vertexCount = -1;

				public readonly ExposedList<Attachment> attachments = new ExposedList<Attachment>();

				public readonly ExposedList<SubmeshInstruction> submeshInstructions = new ExposedList<SubmeshInstruction>();

				public bool frontFacing;

				public readonly ExposedList<bool> attachmentFlips = new ExposedList<bool>();

				public void Clear()
				{
					attachments.Clear(clearArray: false);
					submeshInstructions.Clear(clearArray: false);
					attachmentFlips.Clear(clearArray: false);
				}

				public void Set(Instruction other)
				{
					immutableTriangles = other.immutableTriangles;
					vertexCount = other.vertexCount;
					attachments.Clear(clearArray: false);
					attachments.GrowIfNeeded(other.attachments.Capacity);
					attachments.Count = other.attachments.Count;
					other.attachments.CopyTo(attachments.Items);
					frontFacing = other.frontFacing;
					attachmentFlips.Clear(clearArray: false);
					attachmentFlips.GrowIfNeeded(other.attachmentFlips.Capacity);
					attachmentFlips.Count = other.attachmentFlips.Count;
					if (frontFacing)
					{
						other.attachmentFlips.CopyTo(attachmentFlips.Items);
					}
					submeshInstructions.Clear(clearArray: false);
					submeshInstructions.GrowIfNeeded(other.submeshInstructions.Capacity);
					submeshInstructions.Count = other.submeshInstructions.Count;
					other.submeshInstructions.CopyTo(submeshInstructions.Items);
				}
			}

			public Mesh mesh = SpineMesh.NewMesh();

			public Instruction instructionUsed = new Instruction();
		}

		private class SubmeshTriangleBuffer
		{
			public int[] triangles = new int[0];

			public int triangleCount;

			public int firstVertex = -1;
		}

		public SkeletonRendererDelegate OnRebuild;

		public SkeletonDataAsset skeletonDataAsset;

		public string initialSkinName;

		[FormerlySerializedAs("submeshSeparators")]
		[SpineSlot("", "", false)]
		public string[] separatorSlotNames = new string[0];

		[NonSerialized]
		public readonly List<Slot> separatorSlots = new List<Slot>();

		public float zSpacing;

		public bool renderMeshes = true;

		public bool immutableTriangles;

		public bool pmaVertexColors = true;

		public bool calculateNormals;

		public bool calculateTangents;

		public bool frontFacing;

		public bool logErrors;

		public bool disableRenderingOnOverride = true;

		[NonSerialized]
		private readonly Dictionary<Material, Material> customMaterialOverride = new Dictionary<Material, Material>();

		[NonSerialized]
		private readonly Dictionary<Slot, Material> customSlotMaterials = new Dictionary<Slot, Material>();

		private MeshRenderer meshRenderer;

		private MeshFilter meshFilter;

		[NonSerialized]
		public bool valid;

		[NonSerialized]
		public Skeleton skeleton;

		private DoubleBuffered<SmartMesh> doubleBufferedMesh;

		private readonly SmartMesh.Instruction currentInstructions = new SmartMesh.Instruction();

		private readonly ExposedList<SubmeshTriangleBuffer> submeshes = new ExposedList<SubmeshTriangleBuffer>();

		private readonly ExposedList<Material> submeshMaterials = new ExposedList<Material>();

		private Material[] sharedMaterials = new Material[0];

		private float[] tempVertices = new float[8];

		private Vector3[] vertices;

		private Color32[] colors;

		private Vector2[] uvs;

		private Vector3[] normals;

		private Vector4[] tangents;

		public Dictionary<Material, Material> CustomMaterialOverride => customMaterialOverride;

		public Dictionary<Slot, Material> CustomSlotMaterials => customSlotMaterials;

		private event InstructionDelegate generateMeshOverride;

		public event InstructionDelegate GenerateMeshOverride
		{
			add
			{
				generateMeshOverride += value;
				if (disableRenderingOnOverride && this.generateMeshOverride != null)
				{
					Initialize(overwrite: false);
					meshRenderer.enabled = false;
				}
			}
			remove
			{
				generateMeshOverride -= value;
				if (disableRenderingOnOverride && this.generateMeshOverride == null)
				{
					Initialize(overwrite: false);
					meshRenderer.enabled = true;
				}
			}
		}

		public static T NewSpineGameObject<T>(SkeletonDataAsset skeletonDataAsset) where T : SkeletonRenderer
		{
			return AddSpineComponent<T>(new GameObject("New Spine GameObject"), skeletonDataAsset);
		}

		public static T AddSpineComponent<T>(GameObject gameObject, SkeletonDataAsset skeletonDataAsset) where T : SkeletonRenderer
		{
			T val = gameObject.AddComponent<T>();
			if (skeletonDataAsset != null)
			{
				val.skeletonDataAsset = skeletonDataAsset;
				val.Initialize(overwrite: false);
			}
			return val;
		}

		public virtual void Awake()
		{
			Initialize(overwrite: false);
		}

		public virtual void Initialize(bool overwrite)
		{
			if (valid && !overwrite)
			{
				return;
			}
			if (meshFilter != null)
			{
				meshFilter.sharedMesh = null;
			}
			meshRenderer = GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.sharedMaterial = null;
			}
			currentInstructions.Clear();
			vertices = null;
			colors = null;
			uvs = null;
			sharedMaterials = new Material[0];
			submeshMaterials.Clear();
			submeshes.Clear();
			skeleton = null;
			valid = false;
			if (!skeletonDataAsset)
			{
				if (!logErrors)
				{
				}
				return;
			}
			SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(quiet: false);
			if (skeletonData != null)
			{
				valid = true;
				meshFilter = GetComponent<MeshFilter>();
				meshRenderer = GetComponent<MeshRenderer>();
				doubleBufferedMesh = new DoubleBuffered<SmartMesh>();
				vertices = new Vector3[0];
				skeleton = new Skeleton(skeletonData);
				if (initialSkinName != null && initialSkinName.Length > 0 && initialSkinName != "default")
				{
					skeleton.SetSkin(initialSkinName);
				}
				separatorSlots.Clear();
				for (int i = 0; i < separatorSlotNames.Length; i++)
				{
					separatorSlots.Add(skeleton.FindSlot(separatorSlotNames[i]));
				}
				LateUpdate();
				if (OnRebuild != null)
				{
					OnRebuild(this);
				}
			}
		}

		public virtual void LateUpdate()
		{
			if (!valid || (!meshRenderer.enabled && this.generateMeshOverride == null))
			{
				return;
			}
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			Slot[] items = drawOrder.Items;
			int count = drawOrder.Count;
			int count2 = separatorSlots.Count;
			bool flag = renderMeshes;
			SmartMesh.Instruction instruction = currentInstructions;
			ExposedList<Attachment> attachments = instruction.attachments;
			attachments.Clear(clearArray: false);
			attachments.GrowIfNeeded(count);
			attachments.Count = count;
			Attachment[] items2 = instruction.attachments.Items;
			ExposedList<bool> attachmentFlips = instruction.attachmentFlips;
			attachmentFlips.Clear(clearArray: false);
			attachmentFlips.GrowIfNeeded(count);
			attachmentFlips.Count = count;
			bool[] items3 = attachmentFlips.Items;
			ExposedList<SubmeshInstruction> submeshInstructions = instruction.submeshInstructions;
			submeshInstructions.Clear(clearArray: false);
			bool flag2 = customSlotMaterials.Count > 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int firstVertexIndex = 0;
			int startSlot = 0;
			Material material = null;
			for (int i = 0; i < count; i++)
			{
				Slot slot = items[i];
				Attachment attachment = (items2[i] = slot.attachment);
				bool flag3 = frontFacing && slot.bone.WorldSignX != slot.bone.WorldSignY;
				items3[i] = flag3;
				object rendererObject;
				int num4;
				int num5;
				if (attachment is RegionAttachment regionAttachment)
				{
					rendererObject = regionAttachment.RendererObject;
					num4 = 4;
					num5 = 6;
				}
				else
				{
					if (!flag)
					{
						continue;
					}
					if (attachment is MeshAttachment meshAttachment)
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
				}
				Material value;
				if (flag2)
				{
					if (!customSlotMaterials.TryGetValue(slot, out value))
					{
						value = (Material)((AtlasRegion)rendererObject).page.rendererObject;
					}
				}
				else
				{
					value = (Material)((AtlasRegion)rendererObject).page.rendererObject;
				}
				bool flag4 = count2 > 0 && separatorSlots.Contains(slot);
				if ((num > 0 && material.GetInstanceID() != value.GetInstanceID()) || flag4)
				{
					submeshInstructions.Add(new SubmeshInstruction
					{
						skeleton = skeleton,
						material = material,
						startSlot = startSlot,
						endSlot = i,
						triangleCount = num3,
						firstVertexIndex = firstVertexIndex,
						vertexCount = num2,
						forceSeparate = flag4
					});
					num3 = 0;
					num2 = 0;
					firstVertexIndex = num;
					startSlot = i;
				}
				material = value;
				num3 += num5;
				num += num4;
				num2 += num4;
			}
			submeshInstructions.Add(new SubmeshInstruction
			{
				skeleton = skeleton,
				material = material,
				startSlot = startSlot,
				endSlot = count,
				triangleCount = num3,
				firstVertexIndex = firstVertexIndex,
				vertexCount = num2,
				forceSeparate = false
			});
			instruction.vertexCount = num;
			instruction.immutableTriangles = immutableTriangles;
			instruction.frontFacing = frontFacing;
			if (customMaterialOverride.Count > 0)
			{
				SubmeshInstruction[] items4 = submeshInstructions.Items;
				for (int j = 0; j < submeshInstructions.Count; j++)
				{
					Material material2 = items4[j].material;
					if (customMaterialOverride.TryGetValue(material2, out var value2))
					{
						items4[j].material = value2;
					}
				}
			}
			if (this.generateMeshOverride != null)
			{
				this.generateMeshOverride(instruction);
				if (disableRenderingOnOverride)
				{
					return;
				}
			}
			Vector3[] array = vertices;
			if (num > array.Length)
			{
				array = (vertices = new Vector3[num]);
				colors = new Color32[num];
				uvs = new Vector2[num];
				if (calculateNormals)
				{
					Vector3[] array2 = (normals = new Vector3[num]);
					Vector3 vector = new Vector3(0f, 0f, -1f);
					for (int k = 0; k < num; k++)
					{
						array2[k] = vector;
					}
				}
				if (calculateTangents)
				{
					Vector4[] array3 = (tangents = new Vector4[num]);
					Vector4 vector2 = new Vector4(1f, 0f, 0f, -1f);
					for (int l = 0; l < num; l++)
					{
						array3[l] = vector2;
					}
				}
			}
			else
			{
				Vector3 zero = Vector3.zero;
				int m = num;
				for (int num6 = array.Length; m < num6; m++)
				{
					array[m] = zero;
				}
			}
			float num7 = zSpacing;
			float[] array4 = tempVertices;
			Vector2[] array5 = uvs;
			Color32[] array6 = colors;
			int num8 = 0;
			bool flag5 = pmaVertexColors;
			float num9 = skeleton.a * 255f;
			float r = skeleton.r;
			float g = skeleton.g;
			float b = skeleton.b;
			Vector3 vector3 = default(Vector3);
			Vector3 vector4 = default(Vector3);
			if (num == 0)
			{
				vector3 = new Vector3(0f, 0f, 0f);
				vector4 = new Vector3(0f, 0f, 0f);
			}
			else
			{
				vector3.x = 2.1474836E+09f;
				vector3.y = 2.1474836E+09f;
				vector4.x = -2.1474836E+09f;
				vector4.y = -2.1474836E+09f;
				if (num7 > 0f)
				{
					vector3.z = 0f;
					vector4.z = num7 * (float)(count - 1);
				}
				else
				{
					vector3.z = num7 * (float)(count - 1);
					vector4.z = 0f;
				}
				int num10 = 0;
				Color32 color = default(Color32);
				do
				{
					Slot slot2 = items[num10];
					Attachment attachment2 = slot2.attachment;
					if (attachment2 is RegionAttachment regionAttachment2)
					{
						regionAttachment2.ComputeWorldVertices(slot2.bone, array4);
						float z = (float)num10 * num7;
						float num11 = array4[0];
						float num12 = array4[1];
						float num13 = array4[2];
						float num14 = array4[3];
						float num15 = array4[4];
						float num16 = array4[5];
						float num17 = array4[6];
						float num18 = array4[7];
						array[num8].x = num11;
						array[num8].y = num12;
						array[num8].z = z;
						array[num8 + 1].x = num17;
						array[num8 + 1].y = num18;
						array[num8 + 1].z = z;
						array[num8 + 2].x = num13;
						array[num8 + 2].y = num14;
						array[num8 + 2].z = z;
						array[num8 + 3].x = num15;
						array[num8 + 3].y = num16;
						array[num8 + 3].z = z;
						if (flag5)
						{
							color.a = (byte)(num9 * slot2.a * regionAttachment2.a);
							color.r = (byte)(r * slot2.r * regionAttachment2.r * (float)(int)color.a);
							color.g = (byte)(g * slot2.g * regionAttachment2.g * (float)(int)color.a);
							color.b = (byte)(b * slot2.b * regionAttachment2.b * (float)(int)color.a);
							if (slot2.data.blendMode == BlendMode.additive)
							{
								color.a = 0;
							}
						}
						else
						{
							color.a = (byte)(num9 * slot2.a * regionAttachment2.a);
							color.r = (byte)(r * slot2.r * regionAttachment2.r * 255f);
							color.g = (byte)(g * slot2.g * regionAttachment2.g * 255f);
							color.b = (byte)(b * slot2.b * regionAttachment2.b * 255f);
						}
						array6[num8] = color;
						array6[num8 + 1] = color;
						array6[num8 + 2] = color;
						array6[num8 + 3] = color;
						float[] array7 = regionAttachment2.uvs;
						array5[num8].x = array7[0];
						array5[num8].y = array7[1];
						array5[num8 + 1].x = array7[6];
						array5[num8 + 1].y = array7[7];
						array5[num8 + 2].x = array7[2];
						array5[num8 + 2].y = array7[3];
						array5[num8 + 3].x = array7[4];
						array5[num8 + 3].y = array7[5];
						if (num11 < vector3.x)
						{
							vector3.x = num11;
						}
						else if (num11 > vector4.x)
						{
							vector4.x = num11;
						}
						if (num13 < vector3.x)
						{
							vector3.x = num13;
						}
						else if (num13 > vector4.x)
						{
							vector4.x = num13;
						}
						if (num15 < vector3.x)
						{
							vector3.x = num15;
						}
						else if (num15 > vector4.x)
						{
							vector4.x = num15;
						}
						if (num17 < vector3.x)
						{
							vector3.x = num17;
						}
						else if (num17 > vector4.x)
						{
							vector4.x = num17;
						}
						if (num12 < vector3.y)
						{
							vector3.y = num12;
						}
						else if (num12 > vector4.y)
						{
							vector4.y = num12;
						}
						if (num14 < vector3.y)
						{
							vector3.y = num14;
						}
						else if (num14 > vector4.y)
						{
							vector4.y = num14;
						}
						if (num16 < vector3.y)
						{
							vector3.y = num16;
						}
						else if (num16 > vector4.y)
						{
							vector4.y = num16;
						}
						if (num18 < vector3.y)
						{
							vector3.y = num18;
						}
						else if (num18 > vector4.y)
						{
							vector4.y = num18;
						}
						num8 += 4;
					}
					else
					{
						if (!flag)
						{
							continue;
						}
						if (attachment2 is MeshAttachment meshAttachment2)
						{
							int num19 = meshAttachment2.vertices.Length;
							if (array4.Length < num19)
							{
								array4 = (tempVertices = new float[num19]);
							}
							meshAttachment2.ComputeWorldVertices(slot2, array4);
							if (flag5)
							{
								color.a = (byte)(num9 * slot2.a * meshAttachment2.a);
								color.r = (byte)(r * slot2.r * meshAttachment2.r * (float)(int)color.a);
								color.g = (byte)(g * slot2.g * meshAttachment2.g * (float)(int)color.a);
								color.b = (byte)(b * slot2.b * meshAttachment2.b * (float)(int)color.a);
								if (slot2.data.blendMode == BlendMode.additive)
								{
									color.a = 0;
								}
							}
							else
							{
								color.a = (byte)(num9 * slot2.a * meshAttachment2.a);
								color.r = (byte)(r * slot2.r * meshAttachment2.r * 255f);
								color.g = (byte)(g * slot2.g * meshAttachment2.g * 255f);
								color.b = (byte)(b * slot2.b * meshAttachment2.b * 255f);
							}
							float[] array8 = meshAttachment2.uvs;
							float z2 = (float)num10 * num7;
							int num20 = 0;
							while (num20 < num19)
							{
								float num21 = array4[num20];
								float num22 = array4[num20 + 1];
								array[num8].x = num21;
								array[num8].y = num22;
								array[num8].z = z2;
								array6[num8] = color;
								array5[num8].x = array8[num20];
								array5[num8].y = array8[num20 + 1];
								if (num21 < vector3.x)
								{
									vector3.x = num21;
								}
								else if (num21 > vector4.x)
								{
									vector4.x = num21;
								}
								if (num22 < vector3.y)
								{
									vector3.y = num22;
								}
								else if (num22 > vector4.y)
								{
									vector4.y = num22;
								}
								num20 += 2;
								num8++;
							}
						}
						else
						{
							if (!(attachment2 is WeightedMeshAttachment weightedMeshAttachment2))
							{
								continue;
							}
							int num23 = weightedMeshAttachment2.uvs.Length;
							if (array4.Length < num23)
							{
								array4 = (tempVertices = new float[num23]);
							}
							weightedMeshAttachment2.ComputeWorldVertices(slot2, array4);
							if (flag5)
							{
								color.a = (byte)(num9 * slot2.a * weightedMeshAttachment2.a);
								color.r = (byte)(r * slot2.r * weightedMeshAttachment2.r * (float)(int)color.a);
								color.g = (byte)(g * slot2.g * weightedMeshAttachment2.g * (float)(int)color.a);
								color.b = (byte)(b * slot2.b * weightedMeshAttachment2.b * (float)(int)color.a);
								if (slot2.data.blendMode == BlendMode.additive)
								{
									color.a = 0;
								}
							}
							else
							{
								color.a = (byte)(num9 * slot2.a * weightedMeshAttachment2.a);
								color.r = (byte)(r * slot2.r * weightedMeshAttachment2.r * 255f);
								color.g = (byte)(g * slot2.g * weightedMeshAttachment2.g * 255f);
								color.b = (byte)(b * slot2.b * weightedMeshAttachment2.b * 255f);
							}
							float[] array9 = weightedMeshAttachment2.uvs;
							float z3 = (float)num10 * num7;
							int num24 = 0;
							while (num24 < num23)
							{
								float num25 = array4[num24];
								float num26 = array4[num24 + 1];
								array[num8].x = num25;
								array[num8].y = num26;
								array[num8].z = z3;
								array6[num8] = color;
								array5[num8].x = array9[num24];
								array5[num8].y = array9[num24 + 1];
								if (num25 < vector3.x)
								{
									vector3.x = num25;
								}
								else if (num25 > vector4.x)
								{
									vector4.x = num25;
								}
								if (num26 < vector3.y)
								{
									vector3.y = num26;
								}
								else if (num26 > vector4.y)
								{
									vector4.y = num26;
								}
								num24 += 2;
								num8++;
							}
						}
					}
				}
				while (++num10 < count);
			}
			SmartMesh next = doubleBufferedMesh.GetNext();
			Mesh mesh = next.mesh;
			mesh.vertices = array;
			mesh.colors32 = array6;
			mesh.uv = array5;
			Vector3 vector5 = vector4 - vector3;
			Vector3 center = vector3 + vector5 * 0.5f;
			mesh.bounds = new Bounds(center, vector5);
			SmartMesh.Instruction instructionUsed = next.instructionUsed;
			if (instructionUsed.vertexCount < num)
			{
				if (calculateNormals)
				{
					mesh.normals = normals;
				}
				if (calculateTangents)
				{
					mesh.tangents = tangents;
				}
			}
			bool flag6 = CheckIfMustUpdateMeshStructure(instruction, instructionUsed);
			if (flag6)
			{
				ExposedList<Material> exposedList = submeshMaterials;
				exposedList.Clear(clearArray: false);
				int count3 = submeshInstructions.Count;
				int count4 = submeshes.Count;
				submeshes.Capacity = count3;
				for (int n = count4; n < count3; n++)
				{
					submeshes.Items[n] = new SubmeshTriangleBuffer();
				}
				bool flag7 = !instruction.immutableTriangles;
				int num27 = 0;
				int num28 = count3 - 1;
				for (; num27 < count3; num27++)
				{
					SubmeshInstruction submeshInstructions2 = submeshInstructions.Items[num27];
					if (flag7 || num27 >= count4)
					{
						SetSubmesh(num27, submeshInstructions2, currentInstructions.attachmentFlips, num27 == num28);
					}
					exposedList.Add(submeshInstructions2.material);
				}
				mesh.subMeshCount = count3;
				for (int num29 = 0; num29 < count3; num29++)
				{
					mesh.SetTriangles(submeshes.Items[num29].triangles, num29);
				}
			}
			Material[] array10 = sharedMaterials;
			bool flag8 = flag6 || array10.Length != submeshInstructions.Count;
			if (!flag8)
			{
				SubmeshInstruction[] items5 = submeshInstructions.Items;
				int num30 = 0;
				for (int num31 = array10.Length; num30 < num31; num30++)
				{
					if (array10[num30].GetInstanceID() != items5[num30].material.GetInstanceID())
					{
						flag8 = true;
						break;
					}
				}
			}
			if (flag8)
			{
				if (submeshMaterials.Count == sharedMaterials.Length)
				{
					submeshMaterials.CopyTo(sharedMaterials);
				}
				else
				{
					sharedMaterials = submeshMaterials.ToArray();
				}
				meshRenderer.sharedMaterials = sharedMaterials;
			}
			meshFilter.sharedMesh = mesh;
			next.instructionUsed.Set(instruction);
		}

		private static bool CheckIfMustUpdateMeshStructure(SmartMesh.Instruction a, SmartMesh.Instruction b)
		{
			if (a.vertexCount != b.vertexCount)
			{
				return true;
			}
			if (a.immutableTriangles != b.immutableTriangles)
			{
				return true;
			}
			int count = b.attachments.Count;
			if (a.attachments.Count != count)
			{
				return true;
			}
			Attachment[] items = a.attachments.Items;
			Attachment[] items2 = b.attachments.Items;
			for (int i = 0; i < count; i++)
			{
				if (items[i] != items2[i])
				{
					return true;
				}
			}
			if (a.frontFacing != b.frontFacing)
			{
				return true;
			}
			if (a.frontFacing)
			{
				bool[] items3 = a.attachmentFlips.Items;
				bool[] items4 = b.attachmentFlips.Items;
				for (int j = 0; j < count; j++)
				{
					if (items3[j] != items4[j])
					{
						return true;
					}
				}
			}
			int count2 = a.submeshInstructions.Count;
			int count3 = b.submeshInstructions.Count;
			if (count2 != count3)
			{
				return true;
			}
			SubmeshInstruction[] items5 = a.submeshInstructions.Items;
			SubmeshInstruction[] items6 = b.submeshInstructions.Items;
			for (int k = 0; k < count3; k++)
			{
				SubmeshInstruction submeshInstruction = items5[k];
				SubmeshInstruction submeshInstruction2 = items6[k];
				if (submeshInstruction.vertexCount != submeshInstruction2.vertexCount || submeshInstruction.startSlot != submeshInstruction2.startSlot || submeshInstruction.endSlot != submeshInstruction2.endSlot || submeshInstruction.triangleCount != submeshInstruction2.triangleCount || submeshInstruction.firstVertexIndex != submeshInstruction2.firstVertexIndex)
				{
					return true;
				}
			}
			return false;
		}

		private void SetSubmesh(int submeshIndex, SubmeshInstruction submeshInstructions, ExposedList<bool> flipStates, bool isLastSubmesh)
		{
			SubmeshTriangleBuffer submeshTriangleBuffer = submeshes.Items[submeshIndex];
			int[] array = submeshTriangleBuffer.triangles;
			int triangleCount = submeshInstructions.triangleCount;
			int num = submeshInstructions.firstVertexIndex;
			int num2 = array.Length;
			if (isLastSubmesh && num2 > triangleCount)
			{
				for (int i = triangleCount; i < num2; i++)
				{
					array[i] = 0;
				}
				submeshTriangleBuffer.triangleCount = triangleCount;
			}
			else if (num2 != triangleCount)
			{
				array = (submeshTriangleBuffer.triangles = new int[triangleCount]);
				submeshTriangleBuffer.triangleCount = 0;
			}
			if (!renderMeshes && !frontFacing)
			{
				if (submeshTriangleBuffer.firstVertex != num || submeshTriangleBuffer.triangleCount < triangleCount)
				{
					submeshTriangleBuffer.triangleCount = triangleCount;
					submeshTriangleBuffer.firstVertex = num;
					int num3 = 0;
					while (num3 < triangleCount)
					{
						array[num3] = num;
						array[num3 + 1] = num + 2;
						array[num3 + 2] = num + 1;
						array[num3 + 3] = num + 2;
						array[num3 + 4] = num + 3;
						array[num3 + 5] = num + 1;
						num3 += 6;
						num += 4;
					}
				}
				return;
			}
			bool[] items = flipStates.Items;
			Slot[] items2 = skeleton.DrawOrder.Items;
			int num4 = 0;
			int j = submeshInstructions.startSlot;
			for (int endSlot = submeshInstructions.endSlot; j < endSlot; j++)
			{
				Attachment attachment = items2[j].attachment;
				bool flag = frontFacing && items[j];
				if (attachment is RegionAttachment)
				{
					if (!flag)
					{
						array[num4] = num;
						array[num4 + 1] = num + 2;
						array[num4 + 2] = num + 1;
						array[num4 + 3] = num + 2;
						array[num4 + 4] = num + 3;
						array[num4 + 5] = num + 1;
					}
					else
					{
						array[num4] = num + 1;
						array[num4 + 1] = num + 2;
						array[num4 + 2] = num;
						array[num4 + 3] = num + 1;
						array[num4 + 4] = num + 3;
						array[num4 + 5] = num + 2;
					}
					num4 += 6;
					num += 4;
					continue;
				}
				int num5;
				int[] triangles;
				if (attachment is MeshAttachment meshAttachment)
				{
					num5 = meshAttachment.vertices.Length >> 1;
					triangles = meshAttachment.triangles;
				}
				else
				{
					if (!(attachment is WeightedMeshAttachment weightedMeshAttachment))
					{
						continue;
					}
					num5 = weightedMeshAttachment.uvs.Length >> 1;
					triangles = weightedMeshAttachment.triangles;
				}
				if (flag)
				{
					int num6 = 0;
					int num7 = triangles.Length;
					while (num6 < num7)
					{
						array[num4 + 2] = num + triangles[num6];
						array[num4 + 1] = num + triangles[num6 + 1];
						array[num4] = num + triangles[num6 + 2];
						num6 += 3;
						num4 += 3;
					}
				}
				else
				{
					int num8 = 0;
					int num9 = triangles.Length;
					while (num8 < num9)
					{
						array[num4] = num + triangles[num8];
						num8++;
						num4++;
					}
				}
				num += num5;
			}
		}
	}
}
