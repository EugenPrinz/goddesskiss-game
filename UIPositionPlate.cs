using UnityEngine;

public class UIPositionPlate : UIItemBase
{
	public UISprite thumbnail;

	public UILabel positionNumber;

	public GameObject normalRoot;

	public UILabel statusValue;

	public GameObject selectedEnableRoot;

	public GameObject selectedRoot;

	public GameObject removeUnitButton;

	public GameObject changeUnitButton;

	public GameObject addUnitButton;

	public GameObject hpRoot;

	public UISprite plate;

	public UISprite incompatible;

	public Collider plateCollider;

	public static readonly string commanderPlateSpriteName = "world_dispose_blue";

	public static readonly string unitPlateSpriteName = "world_dispose_green";

	public static readonly string enemyPlateSpriteName = "world_dispose_red";

	public static readonly string emptyPlateSpriteName = "world_dispose_brown";

	public static readonly string upIncompatibleSpriteName = "world_arrow_up";

	public static readonly string downIncompatibleSpriteName = "world_arrow_down";

	public static readonly string equalIncompatibleSpriteName = "world_arrow_equal";

	public void Reset()
	{
		UISetter.SetSprite(thumbnail, null);
		UISetter.SetActive(normalRoot, active: true);
		UISetter.SetActive(selectedRoot, active: false);
		UISetter.SetActive(removeUnitButton, active: false);
		UISetter.SetActive(changeUnitButton, active: false);
		UISetter.SetActive(addUnitButton, active: false);
		UISetter.SetActive(statusValue, active: false);
		UISetter.SetCollider(plateCollider, active: true);
		UISetter.SetActive(selectedEnableRoot, active: false);
		UISetter.SetActive(incompatible, active: false);
		UISetter.SetActive(hpRoot, active: false);
		UISetter.SetSprite(plate, emptyPlateSpriteName);
	}

	public void SetSelectedEnable(bool state)
	{
		UISetter.SetActive(selectedEnableRoot, state);
	}

	public void SetButtonId(string id)
	{
		UISetter.SetGameObjectName(removeUnitButton, "RemoveUnit-" + id);
		UISetter.SetGameObjectName(changeUnitButton, "ChangeUnit-" + id);
		UISetter.SetGameObjectName(addUnitButton, "AddUnit-" + id);
		UISetter.SetGameObjectName(plateCollider.gameObject, "Plate-" + id);
	}

	public void DeactivateAllButton()
	{
		UISetter.SetActive(removeUnitButton, active: false);
		UISetter.SetActive(changeUnitButton, active: false);
		UISetter.SetActive(addUnitButton, active: false);
	}

	public void SelectPlateState(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
		UISetter.SetSprite(plate, (!selected) ? emptyPlateSpriteName : commanderPlateSpriteName);
	}

	public void CopyThumbnail(UISprite target)
	{
		if (!(thumbnail == null) && !(target == null))
		{
			target.atlas = thumbnail.atlas;
			target.spriteName = thumbnail.spriteName;
		}
	}

	public void SetThumbnailAlpha(float alpha)
	{
		UISetter.SetAlpha(thumbnail, alpha);
	}

	public void SetPositionNumber(int num)
	{
		UISetter.SetLabel(positionNumber, num);
	}

	public void SetPlateColor(PlateType type)
	{
		switch (type)
		{
		case PlateType.Empty:
			UISetter.SetSprite(plate, emptyPlateSpriteName);
			break;
		case PlateType.Enemy:
			UISetter.SetSprite(plate, enemyPlateSpriteName);
			break;
		case PlateType.Unit:
			UISetter.SetSprite(plate, unitPlateSpriteName);
			break;
		case PlateType.Commander:
			UISetter.SetSprite(plate, commanderPlateSpriteName);
			break;
		}
	}

	public void SetIncompatible(IncompatibleType type)
	{
		UISetter.SetActive(incompatible, active: true);
		switch (type)
		{
		case IncompatibleType.Equal:
			UISetter.SetSprite(incompatible, equalIncompatibleSpriteName);
			break;
		case IncompatibleType.Up:
			UISetter.SetSprite(incompatible, upIncompatibleSpriteName);
			break;
		case IncompatibleType.Down:
			UISetter.SetSprite(incompatible, downIncompatibleSpriteName);
			break;
		}
	}

	public void ResetIncompatible()
	{
		UISetter.SetActive(incompatible, active: false);
	}

	public void IsActiveHpRoot(bool state)
	{
		UISetter.SetActive(hpRoot, state);
	}

	public void IsEnemyTargetPlate(bool state)
	{
		UISetter.SetSprite(plate, (!state) ? emptyPlateSpriteName : enemyPlateSpriteName);
	}

	public void IsTargetPlate(bool state)
	{
		UISetter.SetSprite(plate, (!state) ? emptyPlateSpriteName : unitPlateSpriteName);
	}
}
