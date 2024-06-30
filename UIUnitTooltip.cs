using UnityEngine;

public class UIUnitTooltip : MonoBehaviour
{
	private UIUnit unit;

	private void Start()
	{
		unit = GetComponent<UIUnit>();
	}

	public void OnClick()
	{
		if (unit.target != null)
		{
			UITooltip.Show(unit.target.id, unit.target.level);
		}
	}
}
