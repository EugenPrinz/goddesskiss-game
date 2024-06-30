using Shared.Battle;
using UnityEngine;

public class CutInSplitLine : AbstractCutInObject
{
	protected Vector3 vecTargetPosition;

	protected Vector3 vecTargetAngle;

	public override string ID => "CutInSplitLine";

	public override void EnterStatus()
	{
		base.EnterStatus();
		if (Option.Default.enableEffect)
		{
			Lerp = 0f;
		}
	}

	public override void UpdateStatus()
	{
		base.UpdateStatus();
		vecTargetAngle = base.transform.localEulerAngles;
		vecTargetPosition = base.transform.localPosition;
		if (cutInData.side == E_TARGET_SIDE.LEFT)
		{
			if (Application.isPlaying)
			{
				if (cutInData.cutInPlayer.type == CutInPlayer.E_TYPE.OWNER)
				{
					if (!Option.Default.canLhsCutIn)
					{
						return;
					}
				}
				else if (!Option.Default.canRhsCutIn)
				{
					return;
				}
			}
		}
		else
		{
			if (Application.isPlaying)
			{
				if (cutInData.cutInPlayer.type == CutInPlayer.E_TYPE.OWNER)
				{
					if (!Option.Default.canRhsCutIn)
					{
						return;
					}
				}
				else if (!Option.Default.canLhsCutIn || !Option.Default.canRhsCutIn)
				{
					return;
				}
			}
			vecTargetPosition = new Vector3(0f - base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z);
		}
		cutInController.splitLine.localEulerAngles = Vector3.Lerp(cutInController.splitLine.localEulerAngles, vecTargetAngle, Lerp);
		cutInController.splitLine.localPosition = Vector3.Lerp(cutInController.splitLine.localPosition, vecTargetPosition, Lerp);
	}

	public override void ExitStatus()
	{
		base.ExitStatus();
		cutInController.splitLine.localPosition = Vector3.zero;
		cutInController.splitLine.localEulerAngles = Vector3.zero;
	}
}
