using System.Collections.Generic;
using Spine.Unity.MeshGeneration;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[ExecuteInEditMode]
	[HelpURL("https://github.com/pharan/spine-unity-docs/blob/master/SkeletonRenderSeparator.md")]
	public class SkeletonRenderSeparator : MonoBehaviour
	{
		public const int DefaultSortingOrderIncrement = 5;

		[SerializeField]
		protected SkeletonRenderer skeletonRenderer;

		private MeshRenderer mainMeshRenderer;

		public bool copyPropertyBlock;

		[Tooltip("Copies MeshRenderer flags into ")]
		public bool copyMeshRendererFlags;

		public List<SkeletonPartsRenderer> partsRenderers = new List<SkeletonPartsRenderer>();

		private MaterialPropertyBlock block;

		public SkeletonRenderer SkeletonRenderer
		{
			get
			{
				return skeletonRenderer;
			}
			set
			{
				if (skeletonRenderer != null)
				{
					skeletonRenderer.GenerateMeshOverride -= HandleRender;
				}
				skeletonRenderer = value;
				base.enabled = false;
			}
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			mainMeshRenderer = skeletonRenderer.GetComponent<MeshRenderer>();
			skeletonRenderer.GenerateMeshOverride -= HandleRender;
			skeletonRenderer.GenerateMeshOverride += HandleRender;
			if (!copyMeshRendererFlags)
			{
				return;
			}
			bool useLightProbes = mainMeshRenderer.useLightProbes;
			bool receiveShadows = mainMeshRenderer.receiveShadows;
			for (int i = 0; i < partsRenderers.Count; i++)
			{
				SkeletonPartsRenderer skeletonPartsRenderer = partsRenderers[i];
				if (!(skeletonPartsRenderer == null))
				{
					MeshRenderer meshRenderer = skeletonPartsRenderer.MeshRenderer;
					meshRenderer.useLightProbes = useLightProbes;
					meshRenderer.receiveShadows = receiveShadows;
				}
			}
		}

		private void OnDisable()
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			skeletonRenderer.GenerateMeshOverride -= HandleRender;
			foreach (SkeletonPartsRenderer partsRenderer in partsRenderers)
			{
				partsRenderer.ClearMesh();
			}
		}

		private void HandleRender(SkeletonRenderer.SmartMesh.Instruction instruction)
		{
			int count = partsRenderers.Count;
			if (count <= 0)
			{
				return;
			}
			int i = 0;
			if (copyPropertyBlock)
			{
				mainMeshRenderer.GetPropertyBlock(block);
			}
			ExposedList<SubmeshInstruction> submeshInstructions = instruction.submeshInstructions;
			SubmeshInstruction[] items = submeshInstructions.Items;
			int num = submeshInstructions.Count - 1;
			SkeletonPartsRenderer skeletonPartsRenderer = partsRenderers[i];
			bool calculateNormals = skeletonRenderer.calculateNormals;
			int j = 0;
			int startSubmesh = 0;
			for (; j <= num; j++)
			{
				if (items[j].forceSeparate || j == num)
				{
					skeletonPartsRenderer.RenderParts(instruction.submeshInstructions, startSubmesh, j + 1);
					skeletonPartsRenderer.MeshGenerator.GenerateNormals = calculateNormals;
					if (copyPropertyBlock)
					{
						skeletonPartsRenderer.SetPropertyBlock(block);
					}
					startSubmesh = j + 1;
					i++;
					if (i >= count)
					{
						break;
					}
					skeletonPartsRenderer = partsRenderers[i];
				}
			}
			for (; i < count; i++)
			{
				partsRenderers[i].ClearMesh();
			}
		}
	}
}
