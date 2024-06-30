using Shared.Regulation;
using UnityEngine;

public class SelectChallengeListItem : UIItemBase
{
	public EBattleType type;

	public GameObject Lock;

	public UILabel locklevel;

	private int userLevel => RemoteObjectManager.instance.localUser.level;

	private Regulation regulation => RemoteObjectManager.instance.regulation;

	private void Start()
	{
		switch (type)
		{
		case EBattleType.Duel:
			UISetter.SetActive(Lock, active: false);
			break;
		case EBattleType.WaveDuel:
		{
			int num = int.Parse(regulation.defineDtbl["ARENA_3WAVE_OPEN_LEVEL"].value);
			if (userLevel < num)
			{
				UISetter.SetActive(Lock, active: true);
				UISetter.SetLabel(locklevel, Localization.Format("5040004", num));
			}
			else
			{
				UISetter.SetActive(Lock, active: false);
			}
			break;
		}
		default:
			UISetter.SetActive(Lock, active: true);
			UISetter.SetLabel(locklevel, Localization.Format("5040005"));
			break;
		}
	}
}
