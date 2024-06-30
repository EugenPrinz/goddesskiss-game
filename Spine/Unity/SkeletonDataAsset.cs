using System;
using System.IO;
using UnityEngine;

namespace Spine.Unity
{
	public class SkeletonDataAsset : ScriptableObject
	{
		public AtlasAsset[] atlasAssets;

		public TextAsset skeletonJSON;

		public float scale = 0.01f;

		public string[] fromAnimation;

		public string[] toAnimation;

		public float[] duration;

		public float defaultMix;

		public RuntimeAnimatorController controller;

		private SkeletonData skeletonData;

		private AnimationStateData stateData;

		private void OnEnable()
		{
			if (atlasAssets == null)
			{
				atlasAssets = new AtlasAsset[0];
			}
		}

		public void Reset()
		{
			skeletonData = null;
			stateData = null;
		}

		public SkeletonData GetSkeletonData(bool quiet)
		{
			if (atlasAssets == null)
			{
				atlasAssets = new AtlasAsset[0];
				if (!quiet)
				{
				}
				Reset();
				return null;
			}
			if (skeletonJSON == null)
			{
				if (!quiet)
				{
				}
				Reset();
				return null;
			}
			if (atlasAssets.Length == 0)
			{
				Reset();
				return null;
			}
			Atlas[] array = new Atlas[atlasAssets.Length];
			for (int i = 0; i < atlasAssets.Length; i++)
			{
				if (atlasAssets[i] == null)
				{
					Reset();
					return null;
				}
				array[i] = atlasAssets[i].GetAtlas();
				if (array[i] == null)
				{
					Reset();
					return null;
				}
			}
			if (skeletonData != null)
			{
				return skeletonData;
			}
			AttachmentLoader attachmentLoader = new AtlasAttachmentLoader(array);
			float num = scale;
			try
			{
				if (skeletonJSON.name.ToLower().Contains(".skel"))
				{
					MemoryStream input = new MemoryStream(skeletonJSON.bytes);
					SkeletonBinary skeletonBinary = new SkeletonBinary(attachmentLoader);
					skeletonBinary.Scale = num;
					skeletonData = skeletonBinary.ReadSkeletonData(input);
				}
				else
				{
					StringReader reader = new StringReader(skeletonJSON.text);
					SkeletonJson skeletonJson = new SkeletonJson(attachmentLoader);
					skeletonJson.Scale = num;
					skeletonData = skeletonJson.ReadSkeletonData(reader);
				}
			}
			catch (Exception)
			{
				if (!quiet)
				{
				}
				return null;
			}
			stateData = new AnimationStateData(skeletonData);
			FillStateData();
			return skeletonData;
		}

		public void FillStateData()
		{
			if (stateData == null)
			{
				return;
			}
			stateData.DefaultMix = defaultMix;
			int i = 0;
			for (int num = fromAnimation.Length; i < num; i++)
			{
				if (fromAnimation[i].Length != 0 && toAnimation[i].Length != 0)
				{
					stateData.SetMix(fromAnimation[i], toAnimation[i], duration[i]);
				}
			}
		}

		public AnimationStateData GetAnimationStateData()
		{
			if (stateData != null)
			{
				return stateData;
			}
			GetSkeletonData(quiet: false);
			return stateData;
		}
	}
}
