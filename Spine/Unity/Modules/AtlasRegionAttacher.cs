using System;
using System.Collections;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class AtlasRegionAttacher : MonoBehaviour
	{
		[Serializable]
		public class SlotRegionPair
		{
			[SpineSlot("", "", false)]
			public string slot;

			[SpineAtlasRegion]
			public string region;
		}

		public AtlasAsset atlasAsset;

		public SlotRegionPair[] attachments;

		private Atlas atlas;

		private void Awake()
		{
			SkeletonRenderer component = GetComponent<SkeletonRenderer>();
			component.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(component.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(Apply));
		}

		private void Apply(SkeletonRenderer skeletonRenderer)
		{
			atlas = atlasAsset.GetAtlas();
			AtlasAttachmentLoader atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
			float scale = skeletonRenderer.skeletonDataAsset.scale;
			IEnumerator enumerator = attachments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SlotRegionPair slotRegionPair = (SlotRegionPair)enumerator.Current;
				RegionAttachment regionAttachment = atlasAttachmentLoader.NewRegionAttachment(null, slotRegionPair.region, slotRegionPair.region);
				regionAttachment.Width = regionAttachment.RegionOriginalWidth * scale;
				regionAttachment.Height = regionAttachment.RegionOriginalHeight * scale;
				regionAttachment.SetColor(new Color(1f, 1f, 1f, 1f));
				regionAttachment.UpdateOffset();
				Slot slot = skeletonRenderer.skeleton.FindSlot(slotRegionPair.slot);
				slot.Attachment = regionAttachment;
			}
		}
	}
}
