using Shared.Regulation;
using UnityEngine;

public class UIWorldMapNavigationItem : UIItemBase
{
	public UISprite state;

	public UILabel worldName;

	public GameObject lockRoot;

	public void Set(WorldMapDataRow world)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(state, localUser.canMoveWorldMap(world.id));
		UISetter.SetActive(lockRoot, !localUser.canMoveWorldMap(world.id));
		UISetter.SetLabel(worldName, Localization.Get(world.name));
	}
}
