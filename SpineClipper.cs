using UnityEngine;

[ExecuteInEditMode]
public class SpineClipper : MonoBehaviour
{
	public GameObject target;

	public Rect clipRange;

	public Vector2 softness;

	public MeshRenderer targetRenderer
	{
		get
		{
			if (target != null)
			{
				return target.GetComponent<MeshRenderer>();
			}
			return base.gameObject.GetComponent<MeshRenderer>();
		}
	}

	private void Awake()
	{
		if (target == null)
		{
			target = base.gameObject;
		}
	}

	private void LateUpdate()
	{
		if (!(targetRenderer == null))
		{
			UpdateMaterial();
		}
	}

	public void UpdateMaterial()
	{
		Material[] sharedMaterials = targetRenderer.sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			Material material = new Material(sharedMaterials[i]);
			_UpdateClipValue(material);
			sharedMaterials[i] = material;
		}
		targetRenderer.sharedMaterials = sharedMaterials;
	}

	private void _UpdateClipValue(Material mat)
	{
		if (!(mat == null))
		{
			Vector4 value = new Vector4(clipRange.x, clipRange.y, clipRange.width, clipRange.height);
			Vector4 vector = new Vector4(softness.x, softness.y);
			value.x -= target.transform.localPosition.x;
			value.y -= target.transform.localPosition.y;
			if (vector.x > 0f)
			{
				vector.x = value.z / vector.x;
			}
			else
			{
				vector.x = 0f;
			}
			if (vector.y > 0f)
			{
				vector.y = value.w / vector.y;
			}
			else
			{
				vector.y = 0f;
			}
			mat.SetVector("_ClipRange0", value);
		}
	}
}
