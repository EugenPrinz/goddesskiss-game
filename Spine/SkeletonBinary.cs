using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spine
{
	public class SkeletonBinary
	{
		public const int TIMELINE_SCALE = 0;

		public const int TIMELINE_ROTATE = 1;

		public const int TIMELINE_TRANSLATE = 2;

		public const int TIMELINE_ATTACHMENT = 3;

		public const int TIMELINE_COLOR = 4;

		public const int CURVE_LINEAR = 0;

		public const int CURVE_STEPPED = 1;

		public const int CURVE_BEZIER = 2;

		private AttachmentLoader attachmentLoader;

		private byte[] buffer = new byte[32];

		private List<SkeletonJson.LinkedMesh> linkedMeshes = new List<SkeletonJson.LinkedMesh>();

		public float Scale { get; set; }

		public SkeletonBinary(params Atlas[] atlasArray)
			: this(new AtlasAttachmentLoader(atlasArray))
		{
		}

		public SkeletonBinary(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(Stream input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			float scale = Scale;
			SkeletonData skeletonData = new SkeletonData();
			skeletonData.hash = ReadString(input);
			if (skeletonData.hash.Length == 0)
			{
				skeletonData.hash = null;
			}
			skeletonData.version = ReadString(input);
			if (skeletonData.version.Length == 0)
			{
				skeletonData.version = null;
			}
			skeletonData.width = ReadFloat(input);
			skeletonData.height = ReadFloat(input);
			bool flag = ReadBoolean(input);
			if (flag)
			{
				skeletonData.imagesPath = ReadString(input);
				if (skeletonData.imagesPath.Length == 0)
				{
					skeletonData.imagesPath = null;
				}
			}
			int i = 0;
			for (int num = ReadVarint(input, optimizePositive: true); i < num; i++)
			{
				string name = ReadString(input);
				BoneData parent = ((i != 0) ? skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)] : null);
				BoneData boneData = new BoneData(name, parent);
				boneData.x = ReadFloat(input) * scale;
				boneData.y = ReadFloat(input) * scale;
				boneData.scaleX = ReadFloat(input);
				boneData.scaleY = ReadFloat(input);
				boneData.rotation = ReadFloat(input);
				boneData.length = ReadFloat(input) * scale;
				boneData.inheritScale = ReadBoolean(input);
				boneData.inheritRotation = ReadBoolean(input);
				if (flag)
				{
					ReadInt(input);
				}
				skeletonData.bones.Add(boneData);
			}
			int j = 0;
			for (int num2 = ReadVarint(input, optimizePositive: true); j < num2; j++)
			{
				IkConstraintData ikConstraintData = new IkConstraintData(ReadString(input));
				int k = 0;
				for (int num3 = ReadVarint(input, optimizePositive: true); k < num3; k++)
				{
					ikConstraintData.bones.Add(skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)]);
				}
				ikConstraintData.target = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				ikConstraintData.mix = ReadFloat(input);
				ikConstraintData.bendDirection = ReadSByte(input);
				skeletonData.ikConstraints.Add(ikConstraintData);
			}
			int l = 0;
			for (int num4 = ReadVarint(input, optimizePositive: true); l < num4; l++)
			{
				TransformConstraintData transformConstraintData = new TransformConstraintData(ReadString(input));
				transformConstraintData.bone = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				transformConstraintData.target = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				transformConstraintData.translateMix = ReadFloat(input);
				transformConstraintData.x = ReadFloat(input) * scale;
				transformConstraintData.y = ReadFloat(input) * scale;
				skeletonData.transformConstraints.Add(transformConstraintData);
			}
			int m = 0;
			for (int num5 = ReadVarint(input, optimizePositive: true); m < num5; m++)
			{
				string name2 = ReadString(input);
				BoneData boneData2 = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				SlotData slotData = new SlotData(name2, boneData2);
				int num6 = ReadInt(input);
				slotData.r = (float)((num6 & 0xFF000000u) >> 24) / 255f;
				slotData.g = (float)((num6 & 0xFF0000) >> 16) / 255f;
				slotData.b = (float)((num6 & 0xFF00) >> 8) / 255f;
				slotData.a = (float)(num6 & 0xFF) / 255f;
				slotData.attachmentName = ReadString(input);
				slotData.blendMode = (BlendMode)ReadVarint(input, optimizePositive: true);
				skeletonData.slots.Add(slotData);
			}
			Skin skin = ReadSkin(input, "default", flag);
			if (skin != null)
			{
				skeletonData.defaultSkin = skin;
				skeletonData.skins.Add(skin);
			}
			int n = 0;
			for (int num7 = ReadVarint(input, optimizePositive: true); n < num7; n++)
			{
				skeletonData.skins.Add(ReadSkin(input, ReadString(input), flag));
			}
			int num8 = 0;
			for (int count = linkedMeshes.Count; num8 < count; num8++)
			{
				SkeletonJson.LinkedMesh linkedMesh = linkedMeshes[num8];
				Skin skin2 = ((linkedMesh.skin != null) ? skeletonData.FindSkin(linkedMesh.skin) : skeletonData.DefaultSkin);
				if (skin2 == null)
				{
					throw new Exception("Skin not found: " + linkedMesh.skin);
				}
				Attachment attachment = skin2.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (attachment == null)
				{
					throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				}
				if (linkedMesh.mesh is MeshAttachment)
				{
					MeshAttachment meshAttachment = (MeshAttachment)linkedMesh.mesh;
					meshAttachment.ParentMesh = (MeshAttachment)attachment;
					meshAttachment.UpdateUVs();
				}
				else
				{
					WeightedMeshAttachment weightedMeshAttachment = (WeightedMeshAttachment)linkedMesh.mesh;
					weightedMeshAttachment.ParentMesh = (WeightedMeshAttachment)attachment;
					weightedMeshAttachment.UpdateUVs();
				}
			}
			linkedMeshes.Clear();
			int num9 = 0;
			for (int num10 = ReadVarint(input, optimizePositive: true); num9 < num10; num9++)
			{
				EventData eventData = new EventData(ReadString(input));
				eventData.Int = ReadVarint(input, optimizePositive: false);
				eventData.Float = ReadFloat(input);
				eventData.String = ReadString(input);
				skeletonData.events.Add(eventData);
			}
			int num11 = 0;
			for (int num12 = ReadVarint(input, optimizePositive: true); num11 < num12; num11++)
			{
				ReadAnimation(ReadString(input), input, skeletonData);
			}
			skeletonData.bones.TrimExcess();
			skeletonData.slots.TrimExcess();
			skeletonData.skins.TrimExcess();
			skeletonData.events.TrimExcess();
			skeletonData.animations.TrimExcess();
			skeletonData.ikConstraints.TrimExcess();
			return skeletonData;
		}

		private Skin ReadSkin(Stream input, string skinName, bool nonessential)
		{
			int num = ReadVarint(input, optimizePositive: true);
			if (num == 0)
			{
				return null;
			}
			Skin skin = new Skin(skinName);
			for (int i = 0; i < num; i++)
			{
				int slotIndex = ReadVarint(input, optimizePositive: true);
				int j = 0;
				for (int num2 = ReadVarint(input, optimizePositive: true); j < num2; j++)
				{
					string text = ReadString(input);
					skin.AddAttachment(slotIndex, text, ReadAttachment(input, skin, slotIndex, text, nonessential));
				}
			}
			return skin;
		}

		private Attachment ReadAttachment(Stream input, Skin skin, int slotIndex, string attachmentName, bool nonessential)
		{
			float scale = Scale;
			string text = ReadString(input);
			if (text == null)
			{
				text = attachmentName;
			}
			switch ((AttachmentType)input.ReadByte())
			{
			case AttachmentType.region:
			{
				string text5 = ReadString(input);
				float num13 = ReadFloat(input);
				float num14 = ReadFloat(input);
				float scaleX = ReadFloat(input);
				float scaleY = ReadFloat(input);
				float rotation = ReadFloat(input);
				float num15 = ReadFloat(input);
				float num16 = ReadFloat(input);
				int num17 = ReadInt(input);
				if (text5 == null)
				{
					text5 = text;
				}
				RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, text, text5);
				if (regionAttachment == null)
				{
					return null;
				}
				regionAttachment.Path = text5;
				regionAttachment.x = num13 * scale;
				regionAttachment.y = num14 * scale;
				regionAttachment.scaleX = scaleX;
				regionAttachment.scaleY = scaleY;
				regionAttachment.rotation = rotation;
				regionAttachment.width = num15 * scale;
				regionAttachment.height = num16 * scale;
				regionAttachment.r = (float)((num17 & 0xFF000000u) >> 24) / 255f;
				regionAttachment.g = (float)((num17 & 0xFF0000) >> 16) / 255f;
				regionAttachment.b = (float)((num17 & 0xFF00) >> 8) / 255f;
				regionAttachment.a = (float)(num17 & 0xFF) / 255f;
				regionAttachment.UpdateOffset();
				return regionAttachment;
			}
			case AttachmentType.boundingbox:
			{
				float[] vertices = ReadFloatArray(input, ReadVarint(input, optimizePositive: true) * 2, scale);
				BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, text);
				if (boundingBoxAttachment == null)
				{
					return null;
				}
				boundingBoxAttachment.vertices = vertices;
				return boundingBoxAttachment;
			}
			case AttachmentType.mesh:
			{
				string text6 = ReadString(input);
				int num18 = ReadInt(input);
				int num19 = 0;
				int n = ReadVarint(input, optimizePositive: true) * 2;
				float[] regionUVs = ReadFloatArray(input, n, 1f);
				int[] triangles2 = ReadShortArray(input);
				float[] vertices2 = ReadFloatArray(input, n, scale);
				num19 = ReadVarint(input, optimizePositive: true);
				int[] edges2 = null;
				float num20 = 0f;
				float num21 = 0f;
				if (nonessential)
				{
					edges2 = ReadShortArray(input);
					num20 = ReadFloat(input);
					num21 = ReadFloat(input);
				}
				if (text6 == null)
				{
					text6 = text;
				}
				MeshAttachment meshAttachment2 = attachmentLoader.NewMeshAttachment(skin, text, text6);
				if (meshAttachment2 == null)
				{
					return null;
				}
				meshAttachment2.Path = text6;
				meshAttachment2.r = (float)((num18 & 0xFF000000u) >> 24) / 255f;
				meshAttachment2.g = (float)((num18 & 0xFF0000) >> 16) / 255f;
				meshAttachment2.b = (float)((num18 & 0xFF00) >> 8) / 255f;
				meshAttachment2.a = (float)(num18 & 0xFF) / 255f;
				meshAttachment2.vertices = vertices2;
				meshAttachment2.triangles = triangles2;
				meshAttachment2.regionUVs = regionUVs;
				meshAttachment2.UpdateUVs();
				meshAttachment2.HullLength = num19;
				if (nonessential)
				{
					meshAttachment2.Edges = edges2;
					meshAttachment2.Width = num20 * scale;
					meshAttachment2.Height = num21 * scale;
				}
				return meshAttachment2;
			}
			case AttachmentType.linkedmesh:
			{
				string text3 = ReadString(input);
				int num4 = ReadInt(input);
				string skin3 = ReadString(input);
				string parent2 = ReadString(input);
				bool inheritFFD2 = ReadBoolean(input);
				float num5 = 0f;
				float num6 = 0f;
				if (nonessential)
				{
					num5 = ReadFloat(input);
					num6 = ReadFloat(input);
				}
				if (text3 == null)
				{
					text3 = text;
				}
				MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, text, text3);
				if (meshAttachment == null)
				{
					return null;
				}
				meshAttachment.Path = text3;
				meshAttachment.r = (float)((num4 & 0xFF000000u) >> 24) / 255f;
				meshAttachment.g = (float)((num4 & 0xFF0000) >> 16) / 255f;
				meshAttachment.b = (float)((num4 & 0xFF00) >> 8) / 255f;
				meshAttachment.a = (float)(num4 & 0xFF) / 255f;
				meshAttachment.inheritFFD = inheritFFD2;
				if (nonessential)
				{
					meshAttachment.Width = num5 * scale;
					meshAttachment.Height = num6 * scale;
				}
				linkedMeshes.Add(new SkeletonJson.LinkedMesh(meshAttachment, skin3, slotIndex, parent2));
				return meshAttachment;
			}
			case AttachmentType.weightedmesh:
			{
				string text4 = ReadString(input);
				int num7 = ReadInt(input);
				int num8 = ReadVarint(input, optimizePositive: true);
				float[] array = ReadFloatArray(input, num8 * 2, 1f);
				int[] triangles = ReadShortArray(input);
				List<float> list = new List<float>(array.Length * 3 * 3);
				List<int> list2 = new List<int>(array.Length * 3);
				for (int i = 0; i < num8; i++)
				{
					int num9 = (int)ReadFloat(input);
					list2.Add(num9);
					for (int j = 0; j < num9; j++)
					{
						list2.Add((int)ReadFloat(input));
						list.Add(ReadFloat(input) * scale);
						list.Add(ReadFloat(input) * scale);
						list.Add(ReadFloat(input));
					}
				}
				int num10 = ReadVarint(input, optimizePositive: true);
				int[] edges = null;
				float num11 = 0f;
				float num12 = 0f;
				if (nonessential)
				{
					edges = ReadShortArray(input);
					num11 = ReadFloat(input);
					num12 = ReadFloat(input);
				}
				if (text4 == null)
				{
					text4 = text;
				}
				WeightedMeshAttachment weightedMeshAttachment2 = attachmentLoader.NewWeightedMeshAttachment(skin, text, text4);
				if (weightedMeshAttachment2 == null)
				{
					return null;
				}
				weightedMeshAttachment2.Path = text4;
				weightedMeshAttachment2.r = (float)((num7 & 0xFF000000u) >> 24) / 255f;
				weightedMeshAttachment2.g = (float)((num7 & 0xFF0000) >> 16) / 255f;
				weightedMeshAttachment2.b = (float)((num7 & 0xFF00) >> 8) / 255f;
				weightedMeshAttachment2.a = (float)(num7 & 0xFF) / 255f;
				weightedMeshAttachment2.bones = list2.ToArray();
				weightedMeshAttachment2.weights = list.ToArray();
				weightedMeshAttachment2.triangles = triangles;
				weightedMeshAttachment2.regionUVs = array;
				weightedMeshAttachment2.UpdateUVs();
				weightedMeshAttachment2.HullLength = num10 * 2;
				if (nonessential)
				{
					weightedMeshAttachment2.Edges = edges;
					weightedMeshAttachment2.Width = num11 * scale;
					weightedMeshAttachment2.Height = num12 * scale;
				}
				return weightedMeshAttachment2;
			}
			case AttachmentType.weightedlinkedmesh:
			{
				string text2 = ReadString(input);
				int num = ReadInt(input);
				string skin2 = ReadString(input);
				string parent = ReadString(input);
				bool inheritFFD = ReadBoolean(input);
				float num2 = 0f;
				float num3 = 0f;
				if (nonessential)
				{
					num2 = ReadFloat(input);
					num3 = ReadFloat(input);
				}
				if (text2 == null)
				{
					text2 = text;
				}
				WeightedMeshAttachment weightedMeshAttachment = attachmentLoader.NewWeightedMeshAttachment(skin, text, text2);
				if (weightedMeshAttachment == null)
				{
					return null;
				}
				weightedMeshAttachment.Path = text2;
				weightedMeshAttachment.r = (float)((num & 0xFF000000u) >> 24) / 255f;
				weightedMeshAttachment.g = (float)((num & 0xFF0000) >> 16) / 255f;
				weightedMeshAttachment.b = (float)((num & 0xFF00) >> 8) / 255f;
				weightedMeshAttachment.a = (float)(num & 0xFF) / 255f;
				weightedMeshAttachment.inheritFFD = inheritFFD;
				if (nonessential)
				{
					weightedMeshAttachment.Width = num2 * scale;
					weightedMeshAttachment.Height = num3 * scale;
				}
				linkedMeshes.Add(new SkeletonJson.LinkedMesh(weightedMeshAttachment, skin2, slotIndex, parent));
				return weightedMeshAttachment;
			}
			default:
				return null;
			}
		}

		private float[] ReadFloatArray(Stream input, int n, float scale)
		{
			float[] array = new float[n];
			if (scale == 1f)
			{
				for (int i = 0; i < n; i++)
				{
					array[i] = ReadFloat(input);
				}
			}
			else
			{
				for (int j = 0; j < n; j++)
				{
					array[j] = ReadFloat(input) * scale;
				}
			}
			return array;
		}

		private int[] ReadShortArray(Stream input)
		{
			int num = ReadVarint(input, optimizePositive: true);
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (input.ReadByte() << 8) | input.ReadByte();
			}
			return array;
		}

		private void ReadAnimation(string name, Stream input, SkeletonData skeletonData)
		{
			ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
			float scale = Scale;
			float num = 0f;
			int i = 0;
			for (int num2 = ReadVarint(input, optimizePositive: true); i < num2; i++)
			{
				int slotIndex = ReadVarint(input, optimizePositive: true);
				int j = 0;
				for (int num3 = ReadVarint(input, optimizePositive: true); j < num3; j++)
				{
					int num4 = input.ReadByte();
					int num5 = ReadVarint(input, optimizePositive: true);
					switch (num4)
					{
					case 4:
					{
						ColorTimeline colorTimeline = new ColorTimeline(num5);
						colorTimeline.slotIndex = slotIndex;
						for (int l = 0; l < num5; l++)
						{
							float time = ReadFloat(input);
							int num6 = ReadInt(input);
							float r = (float)((num6 & 0xFF000000u) >> 24) / 255f;
							float g = (float)((num6 & 0xFF0000) >> 16) / 255f;
							float b = (float)((num6 & 0xFF00) >> 8) / 255f;
							float a = (float)(num6 & 0xFF) / 255f;
							colorTimeline.SetFrame(l, time, r, g, b, a);
							if (l < num5 - 1)
							{
								ReadCurve(input, l, colorTimeline);
							}
						}
						exposedList.Add(colorTimeline);
						num = Math.Max(num, colorTimeline.frames[num5 * 5 - 5]);
						break;
					}
					case 3:
					{
						AttachmentTimeline attachmentTimeline = new AttachmentTimeline(num5);
						attachmentTimeline.slotIndex = slotIndex;
						for (int k = 0; k < num5; k++)
						{
							attachmentTimeline.SetFrame(k, ReadFloat(input), ReadString(input));
						}
						exposedList.Add(attachmentTimeline);
						num = Math.Max(num, attachmentTimeline.frames[num5 - 1]);
						break;
					}
					}
				}
			}
			int m = 0;
			for (int num7 = ReadVarint(input, optimizePositive: true); m < num7; m++)
			{
				int boneIndex = ReadVarint(input, optimizePositive: true);
				int n = 0;
				for (int num8 = ReadVarint(input, optimizePositive: true); n < num8; n++)
				{
					int num9 = input.ReadByte();
					int num10 = ReadVarint(input, optimizePositive: true);
					switch (num9)
					{
					case 1:
					{
						RotateTimeline rotateTimeline = new RotateTimeline(num10);
						rotateTimeline.boneIndex = boneIndex;
						for (int num13 = 0; num13 < num10; num13++)
						{
							rotateTimeline.SetFrame(num13, ReadFloat(input), ReadFloat(input));
							if (num13 < num10 - 1)
							{
								ReadCurve(input, num13, rotateTimeline);
							}
						}
						exposedList.Add(rotateTimeline);
						num = Math.Max(num, rotateTimeline.frames[num10 * 2 - 2]);
						break;
					}
					case 0:
					case 2:
					{
						float num11 = 1f;
						TranslateTimeline translateTimeline;
						if (num9 == 0)
						{
							translateTimeline = new ScaleTimeline(num10);
						}
						else
						{
							translateTimeline = new TranslateTimeline(num10);
							num11 = scale;
						}
						translateTimeline.boneIndex = boneIndex;
						for (int num12 = 0; num12 < num10; num12++)
						{
							translateTimeline.SetFrame(num12, ReadFloat(input), ReadFloat(input) * num11, ReadFloat(input) * num11);
							if (num12 < num10 - 1)
							{
								ReadCurve(input, num12, translateTimeline);
							}
						}
						exposedList.Add(translateTimeline);
						num = Math.Max(num, translateTimeline.frames[num10 * 3 - 3]);
						break;
					}
					}
				}
			}
			int num14 = 0;
			for (int num15 = ReadVarint(input, optimizePositive: true); num14 < num15; num14++)
			{
				IkConstraintData item = skeletonData.ikConstraints.Items[ReadVarint(input, optimizePositive: true)];
				int num16 = ReadVarint(input, optimizePositive: true);
				IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(num16);
				ikConstraintTimeline.ikConstraintIndex = skeletonData.ikConstraints.IndexOf(item);
				for (int num17 = 0; num17 < num16; num17++)
				{
					ikConstraintTimeline.SetFrame(num17, ReadFloat(input), ReadFloat(input), ReadSByte(input));
					if (num17 < num16 - 1)
					{
						ReadCurve(input, num17, ikConstraintTimeline);
					}
				}
				exposedList.Add(ikConstraintTimeline);
				num = Math.Max(num, ikConstraintTimeline.frames[num16 * 3 - 3]);
			}
			int num18 = 0;
			for (int num19 = ReadVarint(input, optimizePositive: true); num18 < num19; num18++)
			{
				Skin skin = skeletonData.skins.Items[ReadVarint(input, optimizePositive: true)];
				int num20 = 0;
				for (int num21 = ReadVarint(input, optimizePositive: true); num20 < num21; num20++)
				{
					int slotIndex2 = ReadVarint(input, optimizePositive: true);
					int num22 = 0;
					for (int num23 = ReadVarint(input, optimizePositive: true); num22 < num23; num22++)
					{
						Attachment attachment = skin.GetAttachment(slotIndex2, ReadString(input));
						int num24 = ReadVarint(input, optimizePositive: true);
						FfdTimeline ffdTimeline = new FfdTimeline(num24);
						ffdTimeline.slotIndex = slotIndex2;
						ffdTimeline.attachment = attachment;
						for (int num25 = 0; num25 < num24; num25++)
						{
							float time2 = ReadFloat(input);
							int num26 = ((!(attachment is MeshAttachment)) ? (((WeightedMeshAttachment)attachment).weights.Length / 3 * 2) : ((MeshAttachment)attachment).vertices.Length);
							int num27 = ReadVarint(input, optimizePositive: true);
							float[] array;
							if (num27 == 0)
							{
								array = ((!(attachment is MeshAttachment)) ? new float[num26] : ((MeshAttachment)attachment).vertices);
							}
							else
							{
								array = new float[num26];
								int num28 = ReadVarint(input, optimizePositive: true);
								num27 += num28;
								if (scale == 1f)
								{
									for (int num29 = num28; num29 < num27; num29++)
									{
										array[num29] = ReadFloat(input);
									}
								}
								else
								{
									for (int num30 = num28; num30 < num27; num30++)
									{
										array[num30] = ReadFloat(input) * scale;
									}
								}
								if (attachment is MeshAttachment)
								{
									float[] vertices = ((MeshAttachment)attachment).vertices;
									int num31 = 0;
									for (int num32 = array.Length; num31 < num32; num31++)
									{
										array[num31] += vertices[num31];
									}
								}
							}
							ffdTimeline.SetFrame(num25, time2, array);
							if (num25 < num24 - 1)
							{
								ReadCurve(input, num25, ffdTimeline);
							}
						}
						exposedList.Add(ffdTimeline);
						num = Math.Max(num, ffdTimeline.frames[num24 - 1]);
					}
				}
			}
			int num33 = ReadVarint(input, optimizePositive: true);
			if (num33 > 0)
			{
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(num33);
				int count = skeletonData.slots.Count;
				for (int num34 = 0; num34 < num33; num34++)
				{
					float time3 = ReadFloat(input);
					int num35 = ReadVarint(input, optimizePositive: true);
					int[] array2 = new int[count];
					for (int num36 = count - 1; num36 >= 0; num36--)
					{
						array2[num36] = -1;
					}
					int[] array3 = new int[count - num35];
					int num37 = 0;
					int num38 = 0;
					for (int num39 = 0; num39 < num35; num39++)
					{
						int num40 = ReadVarint(input, optimizePositive: true);
						while (num37 != num40)
						{
							array3[num38++] = num37++;
						}
						array2[num37 + ReadVarint(input, optimizePositive: true)] = num37++;
					}
					while (num37 < count)
					{
						array3[num38++] = num37++;
					}
					for (int num41 = count - 1; num41 >= 0; num41--)
					{
						if (array2[num41] == -1)
						{
							array2[num41] = array3[--num38];
						}
					}
					drawOrderTimeline.SetFrame(num34, time3, array2);
				}
				exposedList.Add(drawOrderTimeline);
				num = Math.Max(num, drawOrderTimeline.frames[num33 - 1]);
			}
			int num42 = ReadVarint(input, optimizePositive: true);
			if (num42 > 0)
			{
				EventTimeline eventTimeline = new EventTimeline(num42);
				for (int num43 = 0; num43 < num42; num43++)
				{
					float time4 = ReadFloat(input);
					EventData eventData = skeletonData.events.Items[ReadVarint(input, optimizePositive: true)];
					Event @event = new Event(time4, eventData);
					@event.Int = ReadVarint(input, optimizePositive: false);
					@event.Float = ReadFloat(input);
					@event.String = ((!ReadBoolean(input)) ? eventData.String : ReadString(input));
					eventTimeline.SetFrame(num43, @event);
				}
				exposedList.Add(eventTimeline);
				num = Math.Max(num, eventTimeline.frames[num42 - 1]);
			}
			exposedList.TrimExcess();
			skeletonData.animations.Add(new Animation(name, exposedList, num));
		}

		private void ReadCurve(Stream input, int frameIndex, CurveTimeline timeline)
		{
			switch (input.ReadByte())
			{
			case 1:
				timeline.SetStepped(frameIndex);
				break;
			case 2:
				timeline.SetCurve(frameIndex, ReadFloat(input), ReadFloat(input), ReadFloat(input), ReadFloat(input));
				break;
			}
		}

		private static sbyte ReadSByte(Stream input)
		{
			int num = input.ReadByte();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			return (sbyte)num;
		}

		private static bool ReadBoolean(Stream input)
		{
			return input.ReadByte() != 0;
		}

		private float ReadFloat(Stream input)
		{
			buffer[3] = (byte)input.ReadByte();
			buffer[2] = (byte)input.ReadByte();
			buffer[1] = (byte)input.ReadByte();
			buffer[0] = (byte)input.ReadByte();
			return BitConverter.ToSingle(buffer, 0);
		}

		private static int ReadInt(Stream input)
		{
			return (input.ReadByte() << 24) + (input.ReadByte() << 16) + (input.ReadByte() << 8) + input.ReadByte();
		}

		private static int ReadVarint(Stream input, bool optimizePositive)
		{
			int num = input.ReadByte();
			int num2 = num & 0x7F;
			if (((uint)num & 0x80u) != 0)
			{
				num = input.ReadByte();
				num2 |= (num & 0x7F) << 7;
				if (((uint)num & 0x80u) != 0)
				{
					num = input.ReadByte();
					num2 |= (num & 0x7F) << 14;
					if (((uint)num & 0x80u) != 0)
					{
						num = input.ReadByte();
						num2 |= (num & 0x7F) << 21;
						if (((uint)num & 0x80u) != 0)
						{
							num2 |= (input.ReadByte() & 0x7F) << 28;
						}
					}
				}
			}
			return (!optimizePositive) ? ((num2 >> 1) ^ -(num2 & 1)) : num2;
		}

		private string ReadString(Stream input)
		{
			int num = ReadVarint(input, optimizePositive: true);
			switch (num)
			{
			case 0:
				return null;
			case 1:
				return string.Empty;
			default:
			{
				num--;
				byte[] array = buffer;
				if (array.Length < num)
				{
					array = new byte[num];
				}
				ReadFully(input, array, 0, num);
				return Encoding.UTF8.GetString(array, 0, num);
			}
			}
		}

		private static void ReadFully(Stream input, byte[] buffer, int offset, int length)
		{
			while (length > 0)
			{
				int num = input.Read(buffer, offset, length);
				if (num <= 0)
				{
					throw new EndOfStreamException();
				}
				offset += num;
				length -= num;
			}
		}
	}
}
