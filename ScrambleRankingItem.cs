using UnityEngine;

public class ScrambleRankingItem : UIItemBase
{
	public UISprite icon;

	public UILabel nickName;

	public UILabel damage;

	public GameObject emptyCommander;

	public GameObject block;

	public UIProgressBar damageProgress;

	private int maxScore;

	public void Set(Protocols.ScrambleRankingData data)
	{
		UISetter.SetLabel(nickName, data.name);
		UISetter.SetLabel(damage, string.Format("{0}{1}", Localization.Get("110254"), data.score));
		UISetter.SetProgress(damageProgress, (float)data.score / (float)maxScore);
		UISetter.SetActive(block, data.role != 1);
		UISetter.SetActive(emptyCommander, data.role != 1);
	}

	public void SetMaxScore(int score)
	{
		maxScore = score;
	}
}
