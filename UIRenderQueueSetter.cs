using UnityEngine;

public class UIRenderQueueSetter : MonoBehaviour
{
	public UIPanel targetPanel;

	public Renderer[] rendererList;

	private void LateUpdate()
	{
		if (rendererList == null || rendererList.Length <= 0)
		{
			rendererList = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
			if (rendererList == null || rendererList.Length <= 0)
			{
				return;
			}
		}
		if (targetPanel == null)
		{
			targetPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
			if (targetPanel == null)
			{
				return;
			}
		}
		int num = 0;
		if (targetPanel.drawCalls.Count == 0)
		{
			num = targetPanel.startingRenderQueue;
		}
		else
		{
			int index = targetPanel.drawCalls.Count - 1;
			num = targetPanel.drawCalls[index].finalRenderQueue;
		}
		for (int i = 0; i < rendererList.Length; i++)
		{
			Renderer renderer = rendererList[i];
			for (int j = 0; j < renderer.materials.Length; j++)
			{
				renderer.materials[j].renderQueue = num;
			}
		}
	}
}
