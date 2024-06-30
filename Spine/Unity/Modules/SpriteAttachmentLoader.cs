using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SpriteAttachmentLoader : AttachmentLoader
	{
		public static Dictionary<int, AtlasRegion> atlasTable = new Dictionary<int, AtlasRegion>();

		public static List<int> premultipliedAtlasIds = new List<int>();

		private Sprite sprite;

		private Shader shader;

		public SpriteAttachmentLoader(Sprite sprite, Shader shader)
		{
			if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
			{
				return;
			}
			this.sprite = sprite;
			this.shader = shader;
			Texture2D texture = sprite.texture;
			int instanceID = texture.GetInstanceID();
			if (premultipliedAtlasIds.Contains(instanceID))
			{
				return;
			}
			try
			{
				Color[] pixels = texture.GetPixels();
				for (int i = 0; i < pixels.Length; i++)
				{
					Color color = pixels[i];
					float a = color.a;
					color.r *= a;
					color.g *= a;
					color.b *= a;
					pixels[i] = color;
				}
				texture.SetPixels(pixels);
				texture.Apply();
				premultipliedAtlasIds.Add(instanceID);
			}
			catch
			{
			}
		}

		public RegionAttachment NewRegionAttachment(Skin skin, string name, string path)
		{
			RegionAttachment regionAttachment = new RegionAttachment(name);
			Texture2D texture = sprite.texture;
			int instanceID = texture.GetInstanceID();
			AtlasRegion atlasRegion;
			if (atlasTable.ContainsKey(instanceID))
			{
				atlasRegion = atlasTable[instanceID];
			}
			else
			{
				Material material = new Material(shader);
				if (sprite.packed)
				{
					material.name = "Unity Packed Sprite Material";
				}
				else
				{
					material.name = sprite.name + " Sprite Material";
				}
				material.mainTexture = texture;
				atlasRegion = new AtlasRegion();
				AtlasPage atlasPage = new AtlasPage();
				atlasPage.rendererObject = material;
				atlasRegion.page = atlasPage;
				atlasTable[instanceID] = atlasRegion;
			}
			Rect textureRect = sprite.textureRect;
			textureRect.x = Mathf.InverseLerp(0f, texture.width, textureRect.x);
			textureRect.y = Mathf.InverseLerp(0f, texture.height, textureRect.y);
			textureRect.width = Mathf.InverseLerp(0f, texture.width, textureRect.width);
			textureRect.height = Mathf.InverseLerp(0f, texture.height, textureRect.height);
			Bounds bounds = sprite.bounds;
			Vector3 size = bounds.size;
			bool rotate = false;
			if (sprite.packed)
			{
				rotate = sprite.packingRotation == SpritePackingRotation.Any;
			}
			regionAttachment.SetUVs(textureRect.xMin, textureRect.yMax, textureRect.xMax, textureRect.yMin, rotate);
			regionAttachment.RendererObject = atlasRegion;
			regionAttachment.SetColor(Color.white);
			regionAttachment.ScaleX = 1f;
			regionAttachment.ScaleY = 1f;
			regionAttachment.RegionOffsetX = sprite.rect.width * (0.5f - Mathf.InverseLerp(bounds.min.x, bounds.max.x, 0f)) / sprite.pixelsPerUnit;
			regionAttachment.RegionOffsetY = sprite.rect.height * (0.5f - Mathf.InverseLerp(bounds.min.y, bounds.max.y, 0f)) / sprite.pixelsPerUnit;
			regionAttachment.Width = size.x;
			regionAttachment.Height = size.y;
			regionAttachment.RegionWidth = size.x;
			regionAttachment.RegionHeight = size.y;
			regionAttachment.RegionOriginalWidth = size.x;
			regionAttachment.RegionOriginalHeight = size.y;
			regionAttachment.UpdateOffset();
			return regionAttachment;
		}

		public MeshAttachment NewMeshAttachment(Skin skin, string name, string path)
		{
			throw new NotImplementedException();
		}

		public WeightedMeshAttachment NewWeightedMeshAttachment(Skin skin, string name, string path)
		{
			throw new NotImplementedException();
		}

		public BoundingBoxAttachment NewBoundingBoxAttachment(Skin skin, string name)
		{
			throw new NotImplementedException();
		}
	}
}
