using System;
using Shared.Regulation;
using UnityEngine;

[Serializable]
public class UIInnerPartBase
{
	public delegate void OnClickDelegate(GameObject sender, UIPanelBase parent);

	public GameObject root;

	protected RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	protected Regulation regulation => RemoteObjectManager.instance.regulation;

	protected RemoteObjectManager network => RemoteObjectManager.instance;

	protected UIManager.World uiWorld => UIManager.instance.world;

	protected UIManager.Battle uiBattle => UIManager.instance.battle;

	protected UIPanelBase parentPanelBase { get; set; }

	public virtual bool isActive => root.activeSelf;

	public virtual void OnClick(GameObject sender, UIPanelBase parent)
	{
	}

	public virtual void OnRefresh()
	{
	}

	public virtual void OnInit(UIPanelBase parent)
	{
		parentPanelBase = parent;
	}
}
