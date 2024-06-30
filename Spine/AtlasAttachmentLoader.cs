using System;

namespace Spine
{
	public class AtlasAttachmentLoader : AttachmentLoader
	{
		private Atlas[] atlasArray;

		public AtlasAttachmentLoader(params Atlas[] atlasArray)
		{
			if (atlasArray == null)
			{
				throw new ArgumentNullException("atlas array cannot be null.");
			}
			this.atlasArray = atlasArray;
		}

		public RegionAttachment NewRegionAttachment(Skin skin, string name, string path)
		{
			AtlasRegion atlasRegion = FindRegion(path);
			if (atlasRegion == null)
			{
				throw new Exception("Region not found in atlas: " + path + " (region attachment: " + name + ")");
			}
			RegionAttachment regionAttachment = new RegionAttachment(name);
			regionAttachment.RendererObject = atlasRegion;
			regionAttachment.SetUVs(atlasRegion.u, atlasRegion.v, atlasRegion.u2, atlasRegion.v2, atlasRegion.rotate);
			regionAttachment.regionOffsetX = atlasRegion.offsetX;
			regionAttachment.regionOffsetY = atlasRegion.offsetY;
			regionAttachment.regionWidth = atlasRegion.width;
			regionAttachment.regionHeight = atlasRegion.height;
			regionAttachment.regionOriginalWidth = atlasRegion.originalWidth;
			regionAttachment.regionOriginalHeight = atlasRegion.originalHeight;
			return regionAttachment;
		}

		public MeshAttachment NewMeshAttachment(Skin skin, string name, string path)
		{
			AtlasRegion atlasRegion = FindRegion(path);
			if (atlasRegion == null)
			{
				throw new Exception("Region not found in atlas: " + path + " (mesh attachment: " + name + ")");
			}
			MeshAttachment meshAttachment = new MeshAttachment(name);
			meshAttachment.RendererObject = atlasRegion;
			meshAttachment.RegionU = atlasRegion.u;
			meshAttachment.RegionV = atlasRegion.v;
			meshAttachment.RegionU2 = atlasRegion.u2;
			meshAttachment.RegionV2 = atlasRegion.v2;
			meshAttachment.RegionRotate = atlasRegion.rotate;
			meshAttachment.regionOffsetX = atlasRegion.offsetX;
			meshAttachment.regionOffsetY = atlasRegion.offsetY;
			meshAttachment.regionWidth = atlasRegion.width;
			meshAttachment.regionHeight = atlasRegion.height;
			meshAttachment.regionOriginalWidth = atlasRegion.originalWidth;
			meshAttachment.regionOriginalHeight = atlasRegion.originalHeight;
			return meshAttachment;
		}

		public WeightedMeshAttachment NewWeightedMeshAttachment(Skin skin, string name, string path)
		{
			AtlasRegion atlasRegion = FindRegion(path);
			if (atlasRegion == null)
			{
				throw new Exception("Region not found in atlas: " + path + " (weighted mesh attachment: " + name + ")");
			}
			WeightedMeshAttachment weightedMeshAttachment = new WeightedMeshAttachment(name);
			weightedMeshAttachment.RendererObject = atlasRegion;
			weightedMeshAttachment.RegionU = atlasRegion.u;
			weightedMeshAttachment.RegionV = atlasRegion.v;
			weightedMeshAttachment.RegionU2 = atlasRegion.u2;
			weightedMeshAttachment.RegionV2 = atlasRegion.v2;
			weightedMeshAttachment.RegionRotate = atlasRegion.rotate;
			weightedMeshAttachment.regionOffsetX = atlasRegion.offsetX;
			weightedMeshAttachment.regionOffsetY = atlasRegion.offsetY;
			weightedMeshAttachment.regionWidth = atlasRegion.width;
			weightedMeshAttachment.regionHeight = atlasRegion.height;
			weightedMeshAttachment.regionOriginalWidth = atlasRegion.originalWidth;
			weightedMeshAttachment.regionOriginalHeight = atlasRegion.originalHeight;
			return weightedMeshAttachment;
		}

		public BoundingBoxAttachment NewBoundingBoxAttachment(Skin skin, string name)
		{
			return new BoundingBoxAttachment(name);
		}

		public AtlasRegion FindRegion(string name)
		{
			for (int i = 0; i < atlasArray.Length; i++)
			{
				AtlasRegion atlasRegion = atlasArray[i].FindRegion(name);
				if (atlasRegion != null)
				{
					return atlasRegion;
				}
			}
			return null;
		}
	}
}
