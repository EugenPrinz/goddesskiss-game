using UnityEngine;

public class UICommanderTooltip : MonoBehaviour
{
	public ETooltipType tType;

	private UICommander commander;

	private void Start()
	{
		commander = GetComponent<UICommander>();
	}

	public void OnClick()
	{
		if (commander.commanderId != null)
		{
			UITooltip.Show(tType, commander.commanderId);
		}
	}
}
