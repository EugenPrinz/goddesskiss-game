using UnityEngine;

public class BattleUnitSelectMark : MonoBehaviour
{
	public GameObject turn;

	public GameObject target;

	public GameObject targetCandidate;

	public void SetTurnUnit(bool active)
	{
		if (turn != null)
		{
			bool activeSelf = turn.activeSelf;
			turn.SetActive(active);
			if (!activeSelf)
			{
				turn.transform.localScale = Vector3.one * 2.5f;
				TweenScale.Begin(turn, 0.3f, Vector3.one);
			}
		}
	}

	public void SetAttackTargetCadidate(bool active)
	{
		if (targetCandidate != null)
		{
			targetCandidate.SetActive(active);
		}
	}

	public void SetAttackTarget(bool active)
	{
		if (target != null)
		{
			target.SetActive(active);
		}
	}
}
