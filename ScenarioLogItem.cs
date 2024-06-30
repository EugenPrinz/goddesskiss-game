using UnityEngine;

public class ScenarioLogItem : UIItemBase
{
	[SerializeField]
	private GameObject content;

	[SerializeField]
	private UILabel commander_name;

	[SerializeField]
	private UILabel contents;

	[SerializeField]
	private GameObject thumnail;

	[SerializeField]
	private UISprite commanderIcon;

	private string scenarioContent = string.Empty;

	public void SetActive(bool isActive)
	{
		UISetter.SetActive(content, isActive);
	}

	public void Set(ScenarioLogInfo logInfo)
	{
		UISetter.SetLabel(commander_name, logInfo.name);
		scenarioContent = string.Empty;
		if (logInfo.isScenario)
		{
			scenarioContent = Localization.GetScenario(logInfo.scenarioContent);
		}
		else
		{
			scenarioContent = Localization.GetEvent(logInfo.scenarioContent);
		}
		if (!string.IsNullOrEmpty(logInfo.userNickName) && !string.IsNullOrEmpty(scenarioContent))
		{
			UISetter.SetLabel(contents, string.Format(scenarioContent, logInfo.userNickName));
		}
		else
		{
			UISetter.SetLabel(contents, scenarioContent);
		}
		RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(logInfo.Id);
		if (roCommander != null)
		{
			UISetter.SetActive(thumnail, active: true);
			if (roCommander.isBasicCostume)
			{
				UISetter.SetSprite(commanderIcon, roCommander.resourceId + "_" + roCommander.currentViewCostume);
			}
			else
			{
				UISetter.SetSprite(commanderIcon, roCommander.getCurrentCostumeThumbnailName());
			}
		}
		else
		{
			UISetter.SetActive(thumnail, active: false);
		}
	}
}
