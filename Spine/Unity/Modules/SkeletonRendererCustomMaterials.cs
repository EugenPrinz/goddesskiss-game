using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[ExecuteInEditMode]
	public class SkeletonRendererCustomMaterials : MonoBehaviour
	{
		[Serializable]
		public class MaterialOverride
		{
			public bool overrideDisabled;
		}

		[Serializable]
		public class SlotMaterialOverride : MaterialOverride
		{
			[SpineSlot("", "", false)]
			public string slotName;

			public Material material;
		}

		[Serializable]
		public class AtlasMaterialOverride : MaterialOverride
		{
			public Material originalMaterial;

			public Material replacementMaterial;
		}

		public SkeletonRenderer skeletonRenderer;

		[SerializeField]
		private List<SlotMaterialOverride> customSlotMaterials = new List<SlotMaterialOverride>();

		[SerializeField]
		private List<AtlasMaterialOverride> customMaterialOverrides = new List<AtlasMaterialOverride>();

		public List<SlotMaterialOverride> CustomSlotMaterials => customSlotMaterials;

		public List<AtlasMaterialOverride> CustomMaterialOverrides => customMaterialOverrides;

		public void ReapplyOverrides()
		{
			if (!(skeletonRenderer == null))
			{
				RemoveCustomMaterialOverrides();
				RemoveCustomSlotMaterials();
				SetCustomMaterialOverrides();
				SetCustomSlotMaterials();
			}
		}

		private void SetCustomSlotMaterials()
		{
			for (int i = 0; i < customSlotMaterials.Count; i++)
			{
				SlotMaterialOverride slotMaterialOverride = customSlotMaterials[i];
				if (!slotMaterialOverride.overrideDisabled && !string.IsNullOrEmpty(slotMaterialOverride.slotName))
				{
					Slot key = skeletonRenderer.skeleton.FindSlot(slotMaterialOverride.slotName);
					skeletonRenderer.CustomSlotMaterials[key] = slotMaterialOverride.material;
				}
			}
		}

		private void RemoveCustomSlotMaterials()
		{
			for (int i = 0; i < customSlotMaterials.Count; i++)
			{
				SlotMaterialOverride slotMaterialOverride = customSlotMaterials[i];
				if (!string.IsNullOrEmpty(slotMaterialOverride.slotName))
				{
					Slot key = skeletonRenderer.skeleton.FindSlot(slotMaterialOverride.slotName);
					if (skeletonRenderer.CustomSlotMaterials.TryGetValue(key, out var value) && !(value != slotMaterialOverride.material))
					{
						skeletonRenderer.CustomSlotMaterials.Remove(key);
					}
				}
			}
		}

		private void SetCustomMaterialOverrides()
		{
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (!atlasMaterialOverride.overrideDisabled)
				{
					skeletonRenderer.CustomMaterialOverride[atlasMaterialOverride.originalMaterial] = atlasMaterialOverride.replacementMaterial;
				}
			}
		}

		private void RemoveCustomMaterialOverrides()
		{
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (skeletonRenderer.CustomMaterialOverride.TryGetValue(atlasMaterialOverride.originalMaterial, out var value) && !(value != atlasMaterialOverride.replacementMaterial))
				{
					skeletonRenderer.CustomMaterialOverride.Remove(atlasMaterialOverride.originalMaterial);
				}
			}
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				skeletonRenderer = GetComponent<SkeletonRenderer>();
			}
			if (!(skeletonRenderer == null))
			{
				skeletonRenderer.Initialize(overwrite: false);
				SetCustomMaterialOverrides();
				SetCustomSlotMaterials();
			}
		}

		private void OnDisable()
		{
			if (!(skeletonRenderer == null))
			{
				RemoveCustomMaterialOverrides();
				RemoveCustomSlotMaterials();
			}
		}
	}
}
