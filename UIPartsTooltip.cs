using UnityEngine;

public class UIPartsTooltip : MonoBehaviour
{
	private string strName;

	private string strDesc;

	public void SetTooltipDesc(string n, string s)
	{
		strName = n;
		strDesc = s;
	}

	private void OnPress(bool show)
	{
		if (show)
		{
			if (strName != null || strDesc != null)
			{
				UITooltip.Show(strName, strDesc);
			}
		}
		else
		{
			UITooltip.Hide();
		}
	}
}
