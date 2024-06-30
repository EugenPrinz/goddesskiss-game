using UnityEngine;

[ExecuteInEditMode]
public class UIEffectRender : UIWidget
{
	public Renderer[] renderers;

	protected Renderer mainRenderer;

	protected int finalRenderQueue;

	public override Material material
	{
		get
		{
			if (!Application.isPlaying)
			{
				renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
				if (renderers.Length > 0)
				{
					mainRenderer = renderers[0];
				}
			}
			if (mainRenderer != null)
			{
				return mainRenderer.sharedMaterial;
			}
			return null;
		}
		set
		{
			if (mainRenderer != null)
			{
				mainRenderer.material = value;
			}
		}
	}

	protected override void OnStart()
	{
		if (Application.isPlaying)
		{
			renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
			if (renderers.Length > 0)
			{
				mainRenderer = renderers[0];
			}
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				Material[] sharedMaterials = renderer.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					Material material = sharedMaterials[j];
					if (material != null)
					{
						Material material2 = new Material(material);
						material2.name = material.name;
						if (material.shader != null)
						{
							material2.shader = material.shader;
						}
						sharedMaterials[j] = material2;
					}
				}
				renderer.sharedMaterials = sharedMaterials;
			}
		}
		base.OnStart();
	}

	public void UpdateMaterial()
	{
		base.width = 2;
		base.height = 2;
		if (!Application.isPlaying)
		{
			renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
		}
		if (renderers == null || drawCall == null || drawCall.finalRenderQueue == finalRenderQueue)
		{
			return;
		}
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			Material[] sharedMaterials = renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material != null)
				{
					if (drawCall != null)
					{
						material.renderQueue = drawCall.finalRenderQueue;
					}
					finalRenderQueue = material.renderQueue;
				}
			}
		}
	}

	public void RebuildMaterial()
	{
		finalRenderQueue = 0;
		if (!Application.isPlaying)
		{
			return;
		}
		renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
		if (renderers.Length > 0 && mainRenderer != renderers[0])
		{
			mainRenderer = renderers[0];
			if (drawCall != null)
			{
				drawCall.baseMaterial = this.material;
			}
		}
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material material = sharedMaterials[j];
				if (material != null)
				{
					Material material2 = new Material(material);
					material2.name = material.name;
					if (material.shader != null)
					{
						material2.shader = material.shader;
					}
					sharedMaterials[j] = material2;
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
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

	private void LateUpdate()
	{
		UpdateMaterial();
	}
}
