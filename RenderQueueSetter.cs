using UnityEngine;

public class RenderQueueSetter : MonoBehaviour
{
	public Renderer[] rendererList;

	public int targetRenderQueue = 3000;

	private void Start()
	{
		if (rendererList == null || rendererList.Length <= 0)
		{
			rendererList = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
			if (rendererList == null || rendererList.Length <= 0)
			{
				return;
			}
		}
		for (int i = 0; i < rendererList.Length; i++)
		{
			Renderer renderer = rendererList[i];
			for (int j = 0; j < renderer.materials.Length; j++)
			{
				renderer.materials[j].renderQueue = targetRenderQueue;
			}
		}
		Object.Destroy(this);
	}
}
