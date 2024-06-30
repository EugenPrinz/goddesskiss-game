using Shared.Battle;
using UnityEngine;

public class CutInCamera : AbstractCutInObject
{
	protected Vector3 vecOffSet = Vector3.zero;

	protected Vector3 vecTargetPosition;

	protected Vector3 vecTargetAngle;

	protected Quaternion qTemp;

	public override string ID => string.Concat("CutInCamera", cutInData.cutInPlayer.Side, cutInData.cutInPlayer.type);

	public override void EnterStatus()
	{
		base.EnterStatus();
		if (!Option.Default.enableEffect)
		{
			return;
		}
		vecOffSet = Vector3.zero;
		if (cutInData.side == E_TARGET_SIDE.LEFT)
		{
			if (cutInData.cutInPlayer.type == CutInPlayer.E_TYPE.OWNER)
			{
				if (Option.Default.canLhsCutIn && cutInData.cutInPlayer.owner != null)
				{
					Vector3 vector = cutInData.cutInPlayer.owner.transform.position - cutInController.lCenter.position;
					vecOffSet = cutInController.leftCameraView.InverseTransformPoint(cutInController.leftCameraView.position + vector);
				}
			}
			else if (Option.Default.canRhsCutIn && cutInData.cutInPlayer.enemy != null)
			{
				Vector3 vector2 = cutInData.cutInPlayer.enemy.transform.position - cutInController.lCenter.position;
				vecOffSet = cutInController.leftCameraView.InverseTransformPoint(cutInController.leftCameraView.position + vector2);
			}
			return;
		}
		Lerp = 1f;
		if (cutInData.cutInPlayer.type == CutInPlayer.E_TYPE.OWNER)
		{
			if (Option.Default.canRhsCutIn && cutInData.cutInPlayer.owner != null)
			{
				Vector3 vector3 = cutInData.cutInPlayer.owner.transform.position - cutInController.rCenter.position;
				vecOffSet = cutInController.rightCameraView.InverseTransformPoint(cutInController.rightCameraView.position + vector3);
			}
		}
		else if (Option.Default.canLhsCutIn && Option.Default.canRhsCutIn && cutInData.cutInPlayer.enemy != null)
		{
			Vector3 vector4 = cutInData.cutInPlayer.enemy.transform.position - cutInController.rCenter.position;
			vecOffSet = cutInController.rightCameraView.InverseTransformPoint(cutInController.rightCameraView.position + vector4);
		}
	}

	public override void UpdateStatus()
	{
		base.UpdateStatus();
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
			vecTargetAngle = base.transform.localEulerAngles;
			vecTargetPosition = base.transform.localPosition + vecOffSet;
			cutInController.leftCamera.localRotation = Quaternion.Lerp(cutInController.leftCamera.localRotation, base.transform.localRotation, Lerp);
			cutInController.leftCamera.localPosition = Vector3.Lerp(cutInController.leftCamera.localPosition, vecTargetPosition, Lerp);
			return;
		}
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
		Vector3 vector = cutInController.leftCameraView.TransformPoint(base.transform.localPosition);
		Vector3 vector2 = cutInController.leftCameraView.TransformDirection(base.transform.localRotation.eulerAngles);
		Vector3 a = vector - cutInController.lCenter.position;
		a = Vector3.Scale(a, new Vector3(-1f, 1f, 1f));
		Vector3 position = cutInController.rCenter.position + a;
		Vector3 vector3 = cutInController.rightCameraView.InverseTransformPoint(position);
		vecTargetPosition = vector3 + vecOffSet;
		qTemp = cutInController.rightCamera.localRotation;
		cutInController.rightCamera.forward = cutInController.leftCameraView.forward;
		cutInController.rightCamera.localRotation *= base.transform.localRotation;
		cutInController.rightCamera.Rotate(0f, (0f - cutInController.rightCamera.eulerAngles.y) * 2f, 0f, Space.World);
		Quaternion localRotation = cutInController.rightCamera.localRotation;
		cutInController.rightCamera.localRotation = qTemp;
		cutInController.rightCamera.localRotation = Quaternion.Lerp(cutInController.rightCamera.localRotation, localRotation, Lerp);
		cutInController.rightCamera.localPosition = Vector3.Lerp(cutInController.rightCamera.localPosition, vecTargetPosition, Lerp);
	}

	public override void ExitStatus()
	{
		base.ExitStatus();
		if (cutInData.side == E_TARGET_SIDE.LEFT)
		{
			cutInController.leftCamera.localPosition = Vector3.zero;
			cutInController.leftCamera.localEulerAngles = Vector3.zero;
		}
		else
		{
			cutInController.rightCamera.localPosition = Vector3.zero;
			cutInController.rightCamera.localEulerAngles = Vector3.zero;
		}
	}
}
