using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SpriteAttacher : MonoBehaviour
	{
		public bool attachOnStart = true;

		public bool keepLoaderInMemory = true;

		public Sprite sprite;

		[SpineSlot("", "", false)]
		public string slot;

		private SpriteAttachmentLoader loader;

		private RegionAttachment attachment;

		private void Start()
		{
			if (attachOnStart)
			{
				Attach();
			}
		}

		public void Attach()
		{
			SkeletonRenderer component = GetComponent<SkeletonRenderer>();
			if (loader == null)
			{
				loader = new SpriteAttachmentLoader(sprite, Shader.Find("Spine/Skeleton"));
			}
			if (attachment == null)
			{
				attachment = loader.NewRegionAttachment(null, sprite.name, string.Empty);
			}
			component.skeleton.FindSlot(slot).Attachment = attachment;
			if (!keepLoaderInMemory)
			{
				loader = null;
			}
		}
	}
}
