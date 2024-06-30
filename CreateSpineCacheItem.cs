using Shared.Battle;
using Shared.Regulation;
using Spine.Unity;
using UnityEngine;

public class CreateSpineCacheItem : CreateCacheItem
{
	public bool useInteraction;

	public bool animationLoop;

	public float timeScale = 1f;

	public int sortingOrder;

	public SpineClipper clipper;

	protected SkeletonAnimation spine;

	protected override void Create()
	{
		base.Create();
		if (!(targetItem != null))
		{
			return;
		}
		spine = targetItem.GetComponent<SkeletonAnimation>();
		if (spine != null)
		{
			if (!string.IsNullOrEmpty(animationName))
			{
				spine.AnimationName = animationName;
			}
			spine.loop = animationLoop;
			spine.timeScale = timeScale;
			Renderer component = spine.GetComponent<Renderer>();
			if (component != null)
			{
				component.sortingOrder = sortingOrder;
			}
			if (UIManager.instance.battle != null && UIManager.instance.battle.Main != null)
			{
				Unit unit = null;
				if (actor != null)
				{
					unit = actor.unit;
				}
				if (unit == null)
				{
					unit = UIManager.instance.battle.Simulator.FindLhsUnitByCmdResId(spine.name);
				}
				if (unit != null)
				{
					string skin = "1";
					if (unit._ctdri >= 0)
					{
						RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(unit.cid);
						if (roCommander != null && roCommander.isBasicCostume)
						{
							skin = roCommander.currentViewCostume;
						}
						else
						{
							CommanderCostumeDataRow commanderCostumeDataRow = RemoteObjectManager.instance.regulation.commanderCostumeDtbl[unit._ctdri];
							skin = commanderCostumeDataRow.skinName;
						}
					}
					spine.skeleton.SetSkin(skin);
					spine.skeleton.SetSlotsToSetupPose();
				}
				else
				{
					RoCommander roCommander2 = RemoteObjectManager.instance.localUser.FindCommanderResourceId(spine.name);
					if (roCommander2 != null && spine.skeleton.data.FindSkin(roCommander2.getCurrentCostumeName()) != null)
					{
						spine.skeleton.SetSkin(roCommander2.getCurrentCostumeName());
						spine.skeleton.SetSlotsToSetupPose();
					}
				}
			}
		}
		if (!useInteraction)
		{
			UIInteraction component2 = targetItem.GetComponent<UIInteraction>();
			if (component2 != null)
			{
				if (component2.rootInteraction != null)
				{
					Object.DestroyImmediate(component2.rootInteraction.gameObject);
				}
				Object.DestroyImmediate(component2);
			}
		}
		if (clipper != null)
		{
			SpineClipper spineClipper = targetItem.AddComponent<SpineClipper>();
			spineClipper.clipRange = clipper.clipRange;
			spineClipper.softness = clipper.softness;
			clipper.enabled = false;
		}
	}
}
