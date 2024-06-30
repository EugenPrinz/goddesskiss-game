using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class UvAnimation : MonoBehaviour
{
	private MeshRenderer _meshRenderer;

	public Vector2 speed;

	private void Awake()
	{
		_meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		Vector2 mainTextureOffset = _meshRenderer.material.mainTextureOffset;
		float deltaTime = Time.deltaTime;
		Vector2 mainTextureOffset2 = new Vector2(Mathf.Repeat(mainTextureOffset.x + speed.x * deltaTime, 1f), Mathf.Repeat(mainTextureOffset.y + speed.y * deltaTime, 1f));
		_meshRenderer.material.mainTextureOffset = mainTextureOffset2;
	}
}
