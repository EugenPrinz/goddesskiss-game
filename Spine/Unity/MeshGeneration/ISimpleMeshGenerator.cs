using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public interface ISimpleMeshGenerator
	{
		float Scale { set; }

		Mesh LastGeneratedMesh { get; }

		Mesh GenerateMesh(Skeleton skeleton);
	}
}
