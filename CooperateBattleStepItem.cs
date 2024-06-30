using UnityEngine;

public class CooperateBattleStepItem : MonoBehaviour
{
	public GameObject readyView;

	public GameObject clearView;

	public GameObject receiveBtn;

	public GameObject completeView;

	[HideInInspector]
	public ECooperateStepState state;

	public void Set(ECooperateStepState state)
	{
		UISetter.SetActive(readyView, state == ECooperateStepState.Ready);
		UISetter.SetActive(clearView, state != ECooperateStepState.Ready);
		UISetter.SetActive(receiveBtn, state == ECooperateStepState.Clear);
		UISetter.SetActive(completeView, state == ECooperateStepState.Complete);
		this.state = state;
	}
}
