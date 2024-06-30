using UnityEngine;

public class UIServerListItem : UIItemBase
{
	public UILabel serverName;

	public UILabel number;

	public UILabel status;

	public UISprite thumnail;

	public UILabel level;

	public GameObject selectedRoot;

	public GameObject existUser;

	public UISprite Select;

	private int _status;

	private bool _selected;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
		_selected = selected;
	}

	public void Set(Protocols.ServerData.ServerInfo data)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(existUser, data.level > 0);
		UISetter.SetLabel(number, $"{data.idx:D2}");
		if (data.status == 1)
		{
			UISetter.SetLabel(status, Localization.Get("19071"));
		}
		else if (data.status == 2)
		{
			UISetter.SetLabel(status, Localization.Get("19065"));
		}
		else if (data.status == 3)
		{
			UISetter.SetLabel(status, Localization.Get("19070"));
		}
		else if (data.status == 4)
		{
			UISetter.SetLabel(status, Localization.Get("19073"));
		}
		UISetter.SetLabel(serverName, Localization.Format("19067", data.idx));
		UISetter.SetLabel(level, "Lv." + data.level);
		if (data.thumnail > 0)
		{
			UISetter.SetSprite(thumnail, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(data.thumnail));
		}
		_status = data.status;
	}

	public int GetStatus()
	{
		return _status;
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
