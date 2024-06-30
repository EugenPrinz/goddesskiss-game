namespace Spine
{
	public interface AttachmentLoader
	{
		RegionAttachment NewRegionAttachment(Skin skin, string name, string path);

		MeshAttachment NewMeshAttachment(Skin skin, string name, string path);

		WeightedMeshAttachment NewWeightedMeshAttachment(Skin skin, string name, string path);

		BoundingBoxAttachment NewBoundingBoxAttachment(Skin skin, string name);
	}
}
