using UnityEngine;

public class OceanWave : MonoBehaviour
{
	public Material material;

	public Vector2 velocity = Vector2.one;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(material == null))
		{
			string text = "_MainTex";
			Vector2 textureOffset = material.GetTextureOffset(text);
			textureOffset += Time.deltaTime * velocity;
			textureOffset.x %= 1f;
			textureOffset.y %= 1f;
			material.SetTextureOffset(text, textureOffset);
		}
	}
}
