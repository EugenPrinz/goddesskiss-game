using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	public int RenderQueue = 4000;

	private Material mMat;

	private void Start()
	{
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			Renderer renderer = particleSystem.gameObject.GetComponent<Renderer>() ?? particleSystem.GetComponent<Renderer>();
			if (renderer != null)
			{
				mMat = new Material(renderer.sharedMaterial)
				{
					renderQueue = RenderQueue
				};
				renderer.material = mMat;
			}
		}
	}

	private void OnDestroy()
	{
		if (mMat != null)
		{
			Object.Destroy(mMat);
		}
	}
}
