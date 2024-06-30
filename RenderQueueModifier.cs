using UnityEngine;

[ExecuteInEditMode]
public class RenderQueueModifier : MonoBehaviour
{
	public enum renderType
	{
		front,
		back
	}

	public UIPanel mPanel;

	public renderType type;

	public int depth = 1;

	private void Update()
	{
		if (!(mPanel == null))
		{
			int startingRenderQueue = mPanel.startingRenderQueue;
			startingRenderQueue += ((type != 0) ? (-depth) : depth);
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.sharedMaterial.renderQueue = startingRenderQueue;
			}
		}
	}
}
