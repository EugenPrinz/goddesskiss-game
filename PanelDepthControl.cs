using UnityEngine;

public class PanelDepthControl : MonoBehaviour
{
	private UIPanel panel;

	private int originalDepth;

	private void OnEnable()
	{
		if (panel == null)
		{
			panel = GetComponent<UIPanel>();
		}
		originalDepth = panel.depth;
	}

	public void OnDisable()
	{
		panel.depth = originalDepth;
	}
}
