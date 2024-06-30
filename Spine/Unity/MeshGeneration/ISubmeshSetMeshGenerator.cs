namespace Spine.Unity.MeshGeneration
{
	public interface ISubmeshSetMeshGenerator
	{
		bool GenerateNormals { get; set; }

		MeshAndMaterials GenerateMesh(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh);
	}
}
