using UnityEngine;

public class CutInBgColor : AbstractCutInObject
{
	public enum E_TYPE
	{
		OWNER,
		ENEMY
	}

	public E_TYPE type;

	protected GameObject targetObject;

	protected Material thisMaterial;

	protected Color targetCol;

	protected Material targetMaterial;

	public override string ID => string.Concat("CutInBg", cutInData.side, type);

	public override void StartEdit()
	{
		base.StartEdit();
		EnterStatus();
	}

	public override void EnterStatus()
	{
		base.EnterStatus();
		thisMaterial = GetComponent<MeshRenderer>().material;
		if (cutInData.side == E_TARGET_SIDE.LEFT)
		{
			if (type == E_TYPE.OWNER)
			{
				targetObject = cutInController.leftBack.gameObject;
				targetMaterial = cutInController.leftBack.GetComponent<MeshRenderer>().material;
			}
			else
			{
				targetObject = cutInController.rightBack.gameObject;
				targetMaterial = cutInController.rightBack.GetComponent<MeshRenderer>().material;
			}
		}
		else if (type == E_TYPE.OWNER)
		{
			targetObject = cutInController.rightBack.gameObject;
			targetMaterial = cutInController.rightBack.GetComponent<MeshRenderer>().material;
		}
		else
		{
			targetObject = cutInController.leftBack.gameObject;
			targetMaterial = cutInController.leftBack.GetComponent<MeshRenderer>().material;
		}
	}

	public override void UpdateStatus()
	{
		base.UpdateStatus();
		targetObject.SetActive(base.gameObject.activeSelf);
	}

	public override void ExitStatus()
	{
		targetObject.SetActive(value: false);
		base.ExitStatus();
	}
}
