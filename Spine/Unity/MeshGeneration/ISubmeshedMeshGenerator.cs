using System.Collections.Generic;

namespace Spine.Unity.MeshGeneration
{
	public interface ISubmeshedMeshGenerator
	{
		List<Slot> Separators { get; }

		SubmeshedMeshInstruction GenerateInstruction(Skeleton skeleton);

		MeshAndMaterials GenerateMesh(SubmeshedMeshInstruction wholeMeshInstruction);
	}
}
