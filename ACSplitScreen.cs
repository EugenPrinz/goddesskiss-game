using UnityEngine;

[ExecuteInEditMode]
public class ACSplitScreen : MonoBehaviour
{
	public SplitScreenManager ssm;

	public float barRotate;

	public float barDistance;

	public float barOffsetX;

	public float barOffsetY;

	public void Update()
	{
		if (!(ssm == null))
		{
			Transform clipLine = ssm.left.ClipLine;
			Transform clipLine2 = ssm.right.ClipLine;
			clipLine.localRotation = Quaternion.Euler(0f, 0f, barRotate);
			clipLine2.localRotation = Quaternion.Euler(0f, 0f, barRotate);
			Vector3 vector = new Vector3(barOffsetX, barOffsetY);
			clipLine.localPosition = vector + clipLine.right * (0f - barDistance) * 0.5f;
			clipLine2.localPosition = vector + clipLine.right * barDistance * 0.5f;
		}
	}
}
