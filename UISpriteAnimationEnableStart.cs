using UnityEngine;

public class UISpriteAnimationEnableStart : MonoBehaviour
{
	public UISpriteAnimation spriteAnimation;

	private void OnEnable()
	{
		if (spriteAnimation != null)
		{
			spriteAnimation.ResetToBeginning();
			spriteAnimation.Play();
		}
	}
}
