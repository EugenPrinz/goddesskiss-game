using UnityEngine;

public class UIVIPSummary : MonoBehaviour
{
	public UILabel level;

	public UILabel exp;

	public UIProgressBar expProgress;

	public UILabel description;

	public UILabel[] bonusDescription;

	public void SetLevel()
	{
	}

	public void SetBuildingDescription(string buildingId)
	{
		UISetter.SetLabel(description, Localization.Get("Building." + buildingId + ".VIP.Description"));
		for (int i = 0; i < bonusDescription.Length; i++)
		{
			string key = "Building." + buildingId + ".VIP.Bonus." + i;
			if (Localization.Exists(key))
			{
				UISetter.SetLabel(bonusDescription[i], Localization.Get(key));
			}
			else
			{
				UISetter.SetLabel(bonusDescription[i], null);
			}
		}
	}
}
