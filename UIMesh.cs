using UnityEngine;

[ExecuteInEditMode]
public class UIMesh : UIWidget
{
	[HideInInspector]
	public MeshRenderer meshRenderer;

	public override Material material
	{
		get
		{
			if (meshRenderer != null)
			{
				if (Application.isPlaying)
				{
					return meshRenderer.sharedMaterial;
				}
				return meshRenderer.sharedMaterial;
			}
			return null;
		}
		set
		{
			if (!(meshRenderer == null))
			{
				meshRenderer.material = value;
			}
		}
	}

	public MeshFilter meshFilter => GetComponent<MeshFilter>();

	public override void MakePixelPerfect()
	{
		if (meshFilter == null)
		{
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		Mesh sharedMesh = meshFilter.sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			if (vector.x < num)
			{
				num = vector.x;
			}
			else if (vector.x > num2)
			{
				num2 = vector.x;
			}
			if (vector.y < num3)
			{
				num3 = vector.y;
			}
			else if (vector.y > num4)
			{
				num4 = vector.y;
			}
		}
		base.width = Mathf.RoundToInt(num2 - num);
		base.height = Mathf.RoundToInt(num4 - num3);
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 vector = drawingDimensions;
		verts.Add(new Vector3(vector.x, vector.y, 0f));
		verts.Add(new Vector3(vector.x, vector.w, 0f));
		verts.Add(new Vector3(vector.z, vector.y, 0f));
		verts.Add(new Vector3(vector.z, vector.w, 0f));
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.zero);
		cols.Add(Color.clear);
		cols.Add(Color.clear);
		cols.Add(Color.clear);
		cols.Add(Color.clear);
		if (onPostFill != null)
		{
			onPostFill(this, verts.size, verts, uvs, cols);
		}
	}

	public void UpdateMaterial()
	{
		Material[] sharedMaterials = meshRenderer.sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			Material material = new Material(sharedMaterials[i]);
			material.SetVector("_ClipRange0", new Vector4(0f, 0f, 0f, 0f));
			material.SetVector("_ClipArgs0", Vector4.one * 10000f);
			material.SetColor("_Color", base.color);
			if (drawCall != null)
			{
				material.renderQueue = drawCall.finalRenderQueue;
			}
			sharedMaterials[i] = material;
		}
		meshRenderer.sharedMaterials = sharedMaterials;
	}

	private void LateUpdate()
	{
		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				return;
			}
		}
		UpdateMaterial();
	}
}
