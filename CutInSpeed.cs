using UnityEngine;

public class CutInSpeed : AbstractCutInObject
{
	[Range(0f, 3f)]
	public float value = 1f;

	protected float speedValue;

	public override string ID => "Speed";

	public override bool CanEditModeUpdate()
	{
		return false;
	}

	public override void EnterStatus()
	{
		base.EnterStatus();
	}

	public override void UpdateStatus()
	{
		base.UpdateStatus();
	}

	public override void ExitStatus()
	{
		base.ExitStatus();
	}
}
