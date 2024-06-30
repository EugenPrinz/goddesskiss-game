using Shared.Regulation;
using UnityEngine;

public class UITranscendenceListItem : UIItemBase
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UIProgressBar tsdcProgress;

	[SerializeField]
	private UILabel tsdcProgressLabel;

	[SerializeField]
	private GameObject btnUpgrade;

	[SerializeField]
	private GameObject btnInfo;

	[SerializeField]
	private GameObject effect;

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		UISetter.SetActive(effect, active: false);
		if (selected)
		{
			UISetter.SetActive(effect, active: true);
			SoundManager.PlaySFX("SE_UpgradeSkill_001");
		}
	}

	public void Set(RoCommander comm, TranscendenceSlotDataRow slot)
	{
		int num = comm.CurrentTranscendenceStep();
		int num2 = comm.transcendence[slot.slot - 1];
		int num3 = slot.firstStepLimit + slot.firstStepLimit * num;
		UISetter.SetLabel(title, Localization.Get(slot.statString));
		UISetter.SetLabel(description, "+" + num2 * slot.addStat);
		UISetter.SetSprite(icon, slot.icon.Trim());
		UISetter.SetLabel(tsdcProgressLabel, num2 + " / " + num3);
		UISetter.SetProgress(tsdcProgress, (float)num2 / (float)num3);
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[comm.id];
		int num4 = ((commanderDataRow.vip != 1) ? int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["TRANSCRNDENCE_MEDALS_VALUE"].value) : int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["TRANSCRNDENCE_MEDALS_VALUE_VIP"].value));
		UISetter.SetActive(btnUpgrade, num2 < num3);
		UISetter.SetButtonGray(btnUpgrade, comm.medal >= num4);
		UISetter.SetGameObjectName(btnUpgrade, $"{_GetOriginalName(btnUpgrade)}-{slot.slot}");
		UISetter.SetGameObjectName(btnInfo, $"{_GetOriginalName(btnInfo)}-{slot.slot}");
	}

	private string _GetOriginalName(GameObject go)
	{
		if (go == null)
		{
			return string.Empty;
		}
		string text = go.name;
		if (!text.Contains("-"))
		{
			return text;
		}
		return text.Remove(text.IndexOf("-"));
	}
}
