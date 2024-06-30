using System;
using System.Collections.Generic;
using System.IO;

namespace Spine
{
	public class SkeletonJson
	{
		internal class LinkedMesh
		{
			internal string parent;

			internal string skin;

			internal int slotIndex;

			internal Attachment mesh;

			public LinkedMesh(Attachment mesh, string skin, int slotIndex, string parent)
			{
				this.mesh = mesh;
				this.skin = skin;
				this.slotIndex = slotIndex;
				this.parent = parent;
			}
		}

		private AttachmentLoader attachmentLoader;

		private List<LinkedMesh> linkedMeshes = new List<LinkedMesh>();

		public float Scale { get; set; }

		public SkeletonJson(params Atlas[] atlasArray)
			: this(new AtlasAttachmentLoader(atlasArray))
		{
		}

		public SkeletonJson(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader cannot be null.");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader cannot be null.");
			}
			float scale = Scale;
			SkeletonData skeletonData = new SkeletonData();
			if (!(Json.Deserialize(reader) is Dictionary<string, object> dictionary))
			{
				throw new Exception("Invalid JSON.");
			}
			if (dictionary.ContainsKey("skeleton"))
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["skeleton"];
				skeletonData.hash = (string)dictionary2["hash"];
				skeletonData.version = (string)dictionary2["spine"];
				skeletonData.width = GetFloat(dictionary2, "width", 0f);
				skeletonData.height = GetFloat(dictionary2, "height", 0f);
			}
			foreach (Dictionary<string, object> item in (List<object>)dictionary["bones"])
			{
				BoneData boneData = null;
				if (item.ContainsKey("parent"))
				{
					boneData = skeletonData.FindBone((string)item["parent"]);
					if (boneData == null)
					{
						throw new Exception("Parent bone not found: " + item["parent"]);
					}
				}
				BoneData boneData2 = new BoneData((string)item["name"], boneData);
				boneData2.length = GetFloat(item, "length", 0f) * scale;
				boneData2.x = GetFloat(item, "x", 0f) * scale;
				boneData2.y = GetFloat(item, "y", 0f) * scale;
				boneData2.rotation = GetFloat(item, "rotation", 0f);
				boneData2.scaleX = GetFloat(item, "scaleX", 1f);
				boneData2.scaleY = GetFloat(item, "scaleY", 1f);
				boneData2.inheritScale = GetBoolean(item, "inheritScale", defaultValue: true);
				boneData2.inheritRotation = GetBoolean(item, "inheritRotation", defaultValue: true);
				skeletonData.bones.Add(boneData2);
			}
			if (dictionary.ContainsKey("ik"))
			{
				foreach (Dictionary<string, object> item2 in (List<object>)dictionary["ik"])
				{
					IkConstraintData ikConstraintData = new IkConstraintData((string)item2["name"]);
					foreach (string item3 in (List<object>)item2["bones"])
					{
						BoneData boneData3 = skeletonData.FindBone(item3);
						if (boneData3 == null)
						{
							throw new Exception("IK bone not found: " + item3);
						}
						ikConstraintData.bones.Add(boneData3);
					}
					string text2 = (string)item2["target"];
					ikConstraintData.target = skeletonData.FindBone(text2);
					if (ikConstraintData.target == null)
					{
						throw new Exception("Target bone not found: " + text2);
					}
					ikConstraintData.bendDirection = (GetBoolean(item2, "bendPositive", defaultValue: true) ? 1 : (-1));
					ikConstraintData.mix = GetFloat(item2, "mix", 1f);
					skeletonData.ikConstraints.Add(ikConstraintData);
				}
			}
			if (dictionary.ContainsKey("transform"))
			{
				foreach (Dictionary<string, object> item4 in (List<object>)dictionary["transform"])
				{
					TransformConstraintData transformConstraintData = new TransformConstraintData((string)item4["name"]);
					string text3 = (string)item4["bone"];
					transformConstraintData.bone = skeletonData.FindBone(text3);
					if (transformConstraintData.bone == null)
					{
						throw new Exception("Bone not found: " + text3);
					}
					string text4 = (string)item4["target"];
					transformConstraintData.target = skeletonData.FindBone(text4);
					if (transformConstraintData.target == null)
					{
						throw new Exception("Target bone not found: " + text4);
					}
					transformConstraintData.translateMix = GetFloat(item4, "translateMix", 1f);
					transformConstraintData.x = GetFloat(item4, "x", 0f) * scale;
					transformConstraintData.y = GetFloat(item4, "y", 0f) * scale;
					skeletonData.transformConstraints.Add(transformConstraintData);
				}
			}
			if (dictionary.ContainsKey("slots"))
			{
				foreach (Dictionary<string, object> item5 in (List<object>)dictionary["slots"])
				{
					string name = (string)item5["name"];
					string text5 = (string)item5["bone"];
					BoneData boneData4 = skeletonData.FindBone(text5);
					if (boneData4 == null)
					{
						throw new Exception("Slot bone not found: " + text5);
					}
					SlotData slotData = new SlotData(name, boneData4);
					if (item5.ContainsKey("color"))
					{
						string hexString = (string)item5["color"];
						slotData.r = ToColor(hexString, 0);
						slotData.g = ToColor(hexString, 1);
						slotData.b = ToColor(hexString, 2);
						slotData.a = ToColor(hexString, 3);
					}
					if (item5.ContainsKey("attachment"))
					{
						slotData.attachmentName = (string)item5["attachment"];
					}
					if (item5.ContainsKey("blend"))
					{
						slotData.blendMode = (BlendMode)Enum.Parse(typeof(BlendMode), (string)item5["blend"], ignoreCase: false);
					}
					else
					{
						slotData.blendMode = BlendMode.normal;
					}
					skeletonData.slots.Add(slotData);
				}
			}
			if (dictionary.ContainsKey("skins"))
			{
				foreach (KeyValuePair<string, object> item6 in (Dictionary<string, object>)dictionary["skins"])
				{
					Skin skin = new Skin(item6.Key);
					foreach (KeyValuePair<string, object> item7 in (Dictionary<string, object>)item6.Value)
					{
						int slotIndex = skeletonData.FindSlotIndex(item7.Key);
						foreach (KeyValuePair<string, object> item8 in (Dictionary<string, object>)item7.Value)
						{
							Attachment attachment = ReadAttachment(skin, slotIndex, item8.Key, (Dictionary<string, object>)item8.Value);
							if (attachment != null)
							{
								skin.AddAttachment(slotIndex, item8.Key, attachment);
							}
						}
					}
					skeletonData.skins.Add(skin);
					if (skin.name == "default")
					{
						skeletonData.defaultSkin = skin;
					}
				}
			}
			int i = 0;
			for (int count = linkedMeshes.Count; i < count; i++)
			{
				LinkedMesh linkedMesh = linkedMeshes[i];
				Skin skin2 = ((linkedMesh.skin != null) ? skeletonData.FindSkin(linkedMesh.skin) : skeletonData.defaultSkin);
				if (skin2 == null)
				{
					throw new Exception("Slot not found: " + linkedMesh.skin);
				}
				Attachment attachment2 = skin2.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (attachment2 == null)
				{
					throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				}
				if (linkedMesh.mesh is MeshAttachment)
				{
					MeshAttachment meshAttachment = (MeshAttachment)linkedMesh.mesh;
					meshAttachment.ParentMesh = (MeshAttachment)attachment2;
					meshAttachment.UpdateUVs();
				}
				else
				{
					WeightedMeshAttachment weightedMeshAttachment = (WeightedMeshAttachment)linkedMesh.mesh;
					weightedMeshAttachment.ParentMesh = (WeightedMeshAttachment)attachment2;
					weightedMeshAttachment.UpdateUVs();
				}
			}
			linkedMeshes.Clear();
			if (dictionary.ContainsKey("events"))
			{
				foreach (KeyValuePair<string, object> item9 in (Dictionary<string, object>)dictionary["events"])
				{
					Dictionary<string, object> map = (Dictionary<string, object>)item9.Value;
					EventData eventData = new EventData(item9.Key);
					eventData.Int = GetInt(map, "int", 0);
					eventData.Float = GetFloat(map, "float", 0f);
					eventData.String = GetString(map, "string", null);
					skeletonData.events.Add(eventData);
				}
			}
			if (dictionary.ContainsKey("animations"))
			{
				foreach (KeyValuePair<string, object> item10 in (Dictionary<string, object>)dictionary["animations"])
				{
					ReadAnimation(item10.Key, (Dictionary<string, object>)item10.Value, skeletonData);
				}
			}
			skeletonData.bones.TrimExcess();
			skeletonData.slots.TrimExcess();
			skeletonData.skins.TrimExcess();
			skeletonData.events.TrimExcess();
			skeletonData.animations.TrimExcess();
			skeletonData.ikConstraints.TrimExcess();
			return skeletonData;
		}

		private Attachment ReadAttachment(Skin skin, int slotIndex, string name, Dictionary<string, object> map)
		{
			if (map.ContainsKey("name"))
			{
				name = (string)map["name"];
			}
			float scale = Scale;
			AttachmentType attachmentType = AttachmentType.region;
			if (map.ContainsKey("type"))
			{
				string text = (string)map["type"];
				if (text == "skinnedmesh")
				{
					text = "weightedmesh";
				}
				attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), text, ignoreCase: false);
			}
			string path = name;
			if (map.ContainsKey("path"))
			{
				path = (string)map["path"];
			}
			switch (attachmentType)
			{
			case AttachmentType.region:
			{
				RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, name, path);
				if (regionAttachment == null)
				{
					return null;
				}
				regionAttachment.Path = path;
				regionAttachment.x = GetFloat(map, "x", 0f) * scale;
				regionAttachment.y = GetFloat(map, "y", 0f) * scale;
				regionAttachment.scaleX = GetFloat(map, "scaleX", 1f);
				regionAttachment.scaleY = GetFloat(map, "scaleY", 1f);
				regionAttachment.rotation = GetFloat(map, "rotation", 0f);
				regionAttachment.width = GetFloat(map, "width", 32f) * scale;
				regionAttachment.height = GetFloat(map, "height", 32f) * scale;
				regionAttachment.UpdateOffset();
				if (map.ContainsKey("color"))
				{
					string hexString2 = (string)map["color"];
					regionAttachment.r = ToColor(hexString2, 0);
					regionAttachment.g = ToColor(hexString2, 1);
					regionAttachment.b = ToColor(hexString2, 2);
					regionAttachment.a = ToColor(hexString2, 3);
				}
				return regionAttachment;
			}
			case AttachmentType.mesh:
			case AttachmentType.linkedmesh:
			{
				MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, name, path);
				if (meshAttachment == null)
				{
					return null;
				}
				meshAttachment.Path = path;
				if (map.ContainsKey("color"))
				{
					string hexString = (string)map["color"];
					meshAttachment.r = ToColor(hexString, 0);
					meshAttachment.g = ToColor(hexString, 1);
					meshAttachment.b = ToColor(hexString, 2);
					meshAttachment.a = ToColor(hexString, 3);
				}
				meshAttachment.Width = (float)GetInt(map, "width", 0) * scale;
				meshAttachment.Height = (float)GetInt(map, "height", 0) * scale;
				string @string = GetString(map, "parent", null);
				if (@string == null)
				{
					meshAttachment.vertices = GetFloatArray(map, "vertices", scale);
					meshAttachment.triangles = GetIntArray(map, "triangles");
					meshAttachment.regionUVs = GetFloatArray(map, "uvs", 1f);
					meshAttachment.UpdateUVs();
					meshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
					if (map.ContainsKey("edges"))
					{
						meshAttachment.Edges = GetIntArray(map, "edges");
					}
				}
				else
				{
					meshAttachment.InheritFFD = GetBoolean(map, "ffd", defaultValue: true);
					linkedMeshes.Add(new LinkedMesh(meshAttachment, GetString(map, "skin", null), slotIndex, @string));
				}
				return meshAttachment;
			}
			case AttachmentType.weightedmesh:
			case AttachmentType.weightedlinkedmesh:
			{
				WeightedMeshAttachment weightedMeshAttachment = attachmentLoader.NewWeightedMeshAttachment(skin, name, path);
				if (weightedMeshAttachment == null)
				{
					return null;
				}
				weightedMeshAttachment.Path = path;
				if (map.ContainsKey("color"))
				{
					string hexString3 = (string)map["color"];
					weightedMeshAttachment.r = ToColor(hexString3, 0);
					weightedMeshAttachment.g = ToColor(hexString3, 1);
					weightedMeshAttachment.b = ToColor(hexString3, 2);
					weightedMeshAttachment.a = ToColor(hexString3, 3);
				}
				weightedMeshAttachment.Width = (float)GetInt(map, "width", 0) * scale;
				weightedMeshAttachment.Height = (float)GetInt(map, "height", 0) * scale;
				string string2 = GetString(map, "parent", null);
				if (string2 == null)
				{
					float[] floatArray = GetFloatArray(map, "uvs", 1f);
					float[] floatArray2 = GetFloatArray(map, "vertices", 1f);
					List<float> list = new List<float>(floatArray.Length * 3 * 3);
					List<int> list2 = new List<int>(floatArray.Length * 3);
					int i = 0;
					int num = floatArray2.Length;
					while (i < num)
					{
						int num2 = (int)floatArray2[i++];
						list2.Add(num2);
						for (int num3 = i + num2 * 4; i < num3; i += 4)
						{
							list2.Add((int)floatArray2[i]);
							list.Add(floatArray2[i + 1] * scale);
							list.Add(floatArray2[i + 2] * scale);
							list.Add(floatArray2[i + 3]);
						}
					}
					weightedMeshAttachment.bones = list2.ToArray();
					weightedMeshAttachment.weights = list.ToArray();
					weightedMeshAttachment.triangles = GetIntArray(map, "triangles");
					weightedMeshAttachment.regionUVs = floatArray;
					weightedMeshAttachment.UpdateUVs();
					weightedMeshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
					if (map.ContainsKey("edges"))
					{
						weightedMeshAttachment.Edges = GetIntArray(map, "edges");
					}
				}
				else
				{
					weightedMeshAttachment.InheritFFD = GetBoolean(map, "ffd", defaultValue: true);
					linkedMeshes.Add(new LinkedMesh(weightedMeshAttachment, GetString(map, "skin", null), slotIndex, string2));
				}
				return weightedMeshAttachment;
			}
			case AttachmentType.boundingbox:
			{
				BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, name);
				if (boundingBoxAttachment == null)
				{
					return null;
				}
				boundingBoxAttachment.vertices = GetFloatArray(map, "vertices", scale);
				return boundingBoxAttachment;
			}
			default:
				return null;
			}
		}

		private float[] GetFloatArray(Dictionary<string, object> map, string name, float scale)
		{
			List<object> list = (List<object>)map[name];
			float[] array = new float[list.Count];
			if (scale == 1f)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					array[i] = (float)list[i];
				}
			}
			else
			{
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					array[j] = (float)list[j] * scale;
				}
			}
			return array;
		}

		private int[] GetIntArray(Dictionary<string, object> map, string name)
		{
			List<object> list = (List<object>)map[name];
			int[] array = new int[list.Count];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				array[i] = (int)(float)list[i];
			}
			return array;
		}

		private float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (float)map[name];
		}

		private int GetInt(Dictionary<string, object> map, string name, int defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (int)(float)map[name];
		}

		private bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (bool)map[name];
		}

		private string GetString(Dictionary<string, object> map, string name, string defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (string)map[name];
		}

		private float ToColor(string hexString, int colorIndex)
		{
			if (hexString.Length != 8)
			{
				throw new ArgumentException("Color hexidecimal length must be 8, recieved: " + hexString);
			}
			return (float)Convert.ToInt32(hexString.Substring(colorIndex * 2, 2), 16) / 255f;
		}

		private void ReadAnimation(string name, Dictionary<string, object> map, SkeletonData skeletonData)
		{
			ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
			float num = 0f;
			float scale = Scale;
			if (map.ContainsKey("slots"))
			{
				foreach (KeyValuePair<string, object> item2 in (Dictionary<string, object>)map["slots"])
				{
					string key = item2.Key;
					int slotIndex = skeletonData.FindSlotIndex(key);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)item2.Value;
					foreach (KeyValuePair<string, object> item3 in dictionary)
					{
						List<object> list = (List<object>)item3.Value;
						string key2 = item3.Key;
						if (key2 == "color")
						{
							ColorTimeline colorTimeline = new ColorTimeline(list.Count);
							colorTimeline.slotIndex = slotIndex;
							int num2 = 0;
							foreach (Dictionary<string, object> item4 in list)
							{
								float time = (float)item4["time"];
								string hexString = (string)item4["color"];
								colorTimeline.SetFrame(num2, time, ToColor(hexString, 0), ToColor(hexString, 1), ToColor(hexString, 2), ToColor(hexString, 3));
								ReadCurve(colorTimeline, num2, item4);
								num2++;
							}
							exposedList.Add(colorTimeline);
							num = Math.Max(num, colorTimeline.frames[colorTimeline.FrameCount * 5 - 5]);
							continue;
						}
						if (key2 == "attachment")
						{
							AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list.Count);
							attachmentTimeline.slotIndex = slotIndex;
							int num3 = 0;
							foreach (Dictionary<string, object> item5 in list)
							{
								float time2 = (float)item5["time"];
								attachmentTimeline.SetFrame(num3++, time2, (string)item5["name"]);
							}
							exposedList.Add(attachmentTimeline);
							num = Math.Max(num, attachmentTimeline.frames[attachmentTimeline.FrameCount - 1]);
							continue;
						}
						throw new Exception("Invalid timeline type for a slot: " + key2 + " (" + key + ")");
					}
				}
			}
			if (map.ContainsKey("bones"))
			{
				foreach (KeyValuePair<string, object> item6 in (Dictionary<string, object>)map["bones"])
				{
					string key3 = item6.Key;
					int num4 = skeletonData.FindBoneIndex(key3);
					if (num4 == -1)
					{
						throw new Exception("Bone not found: " + key3);
					}
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)item6.Value;
					foreach (KeyValuePair<string, object> item7 in dictionary4)
					{
						List<object> list2 = (List<object>)item7.Value;
						string key4 = item7.Key;
						switch (key4)
						{
						case "rotate":
						{
							RotateTimeline rotateTimeline = new RotateTimeline(list2.Count);
							rotateTimeline.boneIndex = num4;
							int num9 = 0;
							foreach (Dictionary<string, object> item8 in list2)
							{
								float time4 = (float)item8["time"];
								rotateTimeline.SetFrame(num9, time4, (float)item8["angle"]);
								ReadCurve(rotateTimeline, num9, item8);
								num9++;
							}
							exposedList.Add(rotateTimeline);
							num = Math.Max(num, rotateTimeline.frames[rotateTimeline.FrameCount * 2 - 2]);
							break;
						}
						case "translate":
						case "scale":
						{
							float num5 = 1f;
							TranslateTimeline translateTimeline;
							if (key4 == "scale")
							{
								translateTimeline = new ScaleTimeline(list2.Count);
							}
							else
							{
								translateTimeline = new TranslateTimeline(list2.Count);
								num5 = scale;
							}
							translateTimeline.boneIndex = num4;
							int num6 = 0;
							foreach (Dictionary<string, object> item9 in list2)
							{
								float time3 = (float)item9["time"];
								float num7 = ((!item9.ContainsKey("x")) ? 0f : ((float)item9["x"]));
								float num8 = ((!item9.ContainsKey("y")) ? 0f : ((float)item9["y"]));
								translateTimeline.SetFrame(num6, time3, num7 * num5, num8 * num5);
								ReadCurve(translateTimeline, num6, item9);
								num6++;
							}
							exposedList.Add(translateTimeline);
							num = Math.Max(num, translateTimeline.frames[translateTimeline.FrameCount * 3 - 3]);
							break;
						}
						default:
							throw new Exception("Invalid timeline type for a bone: " + key4 + " (" + key3 + ")");
						}
					}
				}
			}
			if (map.ContainsKey("ik"))
			{
				foreach (KeyValuePair<string, object> item10 in (Dictionary<string, object>)map["ik"])
				{
					IkConstraintData item = skeletonData.FindIkConstraint(item10.Key);
					List<object> list3 = (List<object>)item10.Value;
					IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(list3.Count);
					ikConstraintTimeline.ikConstraintIndex = skeletonData.ikConstraints.IndexOf(item);
					int num10 = 0;
					foreach (Dictionary<string, object> item11 in list3)
					{
						float time5 = (float)item11["time"];
						float mix = ((!item11.ContainsKey("mix")) ? 1f : ((float)item11["mix"]));
						bool flag = !item11.ContainsKey("bendPositive") || (bool)item11["bendPositive"];
						ikConstraintTimeline.SetFrame(num10, time5, mix, flag ? 1 : (-1));
						ReadCurve(ikConstraintTimeline, num10, item11);
						num10++;
					}
					exposedList.Add(ikConstraintTimeline);
					num = Math.Max(num, ikConstraintTimeline.frames[ikConstraintTimeline.FrameCount * 3 - 3]);
				}
			}
			if (map.ContainsKey("ffd"))
			{
				foreach (KeyValuePair<string, object> item12 in (Dictionary<string, object>)map["ffd"])
				{
					Skin skin = skeletonData.FindSkin(item12.Key);
					foreach (KeyValuePair<string, object> item13 in (Dictionary<string, object>)item12.Value)
					{
						int slotIndex2 = skeletonData.FindSlotIndex(item13.Key);
						foreach (KeyValuePair<string, object> item14 in (Dictionary<string, object>)item13.Value)
						{
							List<object> list4 = (List<object>)item14.Value;
							FfdTimeline ffdTimeline = new FfdTimeline(list4.Count);
							Attachment attachment = skin.GetAttachment(slotIndex2, item14.Key);
							if (attachment == null)
							{
								throw new Exception("FFD attachment not found: " + item14.Key);
							}
							ffdTimeline.slotIndex = slotIndex2;
							ffdTimeline.attachment = attachment;
							int num11 = ((!(attachment is MeshAttachment)) ? (((WeightedMeshAttachment)attachment).Weights.Length / 3 * 2) : ((MeshAttachment)attachment).vertices.Length);
							int num12 = 0;
							foreach (Dictionary<string, object> item15 in list4)
							{
								float[] array;
								if (!item15.ContainsKey("vertices"))
								{
									array = ((!(attachment is MeshAttachment)) ? new float[num11] : ((MeshAttachment)attachment).vertices);
								}
								else
								{
									List<object> list5 = (List<object>)item15["vertices"];
									array = new float[num11];
									int @int = GetInt(item15, "offset", 0);
									if (scale == 1f)
									{
										int i = 0;
										for (int count = list5.Count; i < count; i++)
										{
											array[i + @int] = (float)list5[i];
										}
									}
									else
									{
										int j = 0;
										for (int count2 = list5.Count; j < count2; j++)
										{
											array[j + @int] = (float)list5[j] * scale;
										}
									}
									if (attachment is MeshAttachment)
									{
										float[] vertices = ((MeshAttachment)attachment).vertices;
										for (int k = 0; k < num11; k++)
										{
											array[k] += vertices[k];
										}
									}
								}
								ffdTimeline.SetFrame(num12, (float)item15["time"], array);
								ReadCurve(ffdTimeline, num12, item15);
								num12++;
							}
							exposedList.Add(ffdTimeline);
							num = Math.Max(num, ffdTimeline.frames[ffdTimeline.FrameCount - 1]);
						}
					}
				}
			}
			if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
			{
				List<object> list6 = (List<object>)map[(!map.ContainsKey("drawOrder")) ? "draworder" : "drawOrder"];
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(list6.Count);
				int count3 = skeletonData.slots.Count;
				int num13 = 0;
				foreach (Dictionary<string, object> item16 in list6)
				{
					int[] array2 = null;
					if (item16.ContainsKey("offsets"))
					{
						array2 = new int[count3];
						for (int num14 = count3 - 1; num14 >= 0; num14--)
						{
							array2[num14] = -1;
						}
						List<object> list7 = (List<object>)item16["offsets"];
						int[] array3 = new int[count3 - list7.Count];
						int num15 = 0;
						int num16 = 0;
						foreach (Dictionary<string, object> item17 in list7)
						{
							int num17 = skeletonData.FindSlotIndex((string)item17["slot"]);
							if (num17 == -1)
							{
								throw new Exception("Slot not found: " + item17["slot"]);
							}
							while (num15 != num17)
							{
								array3[num16++] = num15++;
							}
							int num18 = num15 + (int)(float)item17["offset"];
							array2[num18] = num15++;
						}
						while (num15 < count3)
						{
							array3[num16++] = num15++;
						}
						for (int num19 = count3 - 1; num19 >= 0; num19--)
						{
							if (array2[num19] == -1)
							{
								array2[num19] = array3[--num16];
							}
						}
					}
					drawOrderTimeline.SetFrame(num13++, (float)item16["time"], array2);
				}
				exposedList.Add(drawOrderTimeline);
				num = Math.Max(num, drawOrderTimeline.frames[drawOrderTimeline.FrameCount - 1]);
			}
			if (map.ContainsKey("events"))
			{
				List<object> list8 = (List<object>)map["events"];
				EventTimeline eventTimeline = new EventTimeline(list8.Count);
				int num20 = 0;
				foreach (Dictionary<string, object> item18 in list8)
				{
					EventData eventData = skeletonData.FindEvent((string)item18["name"]);
					if (eventData == null)
					{
						throw new Exception("Event not found: " + item18["name"]);
					}
					Event @event = new Event((float)item18["time"], eventData);
					@event.Int = GetInt(item18, "int", eventData.Int);
					@event.Float = GetFloat(item18, "float", eventData.Float);
					@event.String = GetString(item18, "string", eventData.String);
					eventTimeline.SetFrame(num20++, @event);
				}
				exposedList.Add(eventTimeline);
				num = Math.Max(num, eventTimeline.frames[eventTimeline.FrameCount - 1]);
			}
			exposedList.TrimExcess();
			skeletonData.animations.Add(new Animation(name, exposedList, num));
		}

		private void ReadCurve(CurveTimeline timeline, int frameIndex, Dictionary<string, object> valueMap)
		{
			if (valueMap.ContainsKey("curve"))
			{
				object obj = valueMap["curve"];
				if (obj.Equals("stepped"))
				{
					timeline.SetStepped(frameIndex);
				}
				else if (obj is List<object>)
				{
					List<object> list = (List<object>)obj;
					timeline.SetCurve(frameIndex, (float)list[0], (float)list[1], (float)list[2], (float)list[3]);
				}
			}
		}
	}
}
