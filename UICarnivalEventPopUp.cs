using Shared.Regulation;
using UnityEngine;

public class UICarnivalEventPopUp : UIPopup
{
	public UITimer remainTimer;

	public UILabel description;

	public GameObject moveBtn;

	private readonly string moveBtnIdPrefix = "Move-";

	private CarnivalTypeDataRow typeData;

	private bool state;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		typeData = _typeData;
		state = true;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		CarnivalDataRow carnivalDataRow = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.cTidx == typeData.idx);
		UISetter.SetLabel(description, Localization.Get(carnivalDataRow.explanation));
		UISetter.SetGameObjectName(moveBtn, $"{moveBtnIdPrefix}{carnivalDataRow.link}");
		SetTime();
	}

	public void SetTime()
	{
		TimeData remainTimeData = base.localUser.carnivalList.carnivalList[typeData.idx].remainTimeData;
		if (!(remainTimer == null))
		{
			remainTimer.SetLabelFormat(string.Format("{0}:", Localization.Get("4025")), string.Empty);
			remainTimer.RegisterOnFinished(delegate
			{
				state = false;
			});
			remainTimer.Set(remainTimeData);
		}
	}

	public override void OnRefresh()
	{
		Init(typeData);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith(moveBtnIdPrefix))
		{
			string destination = text.Substring(text.IndexOf("-") + 1);
			UIManager.instance.world.carnival.ClosePopup();
			UIManager.instance.world.camp.GoNavigation(destination);
		}
	}
}
