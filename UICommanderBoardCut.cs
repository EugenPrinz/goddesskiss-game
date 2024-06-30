using System.Collections;
using UnityEngine;

public class UICommanderBoardCut : MonoBehaviour
{
	public delegate void FinishDelegate();

	public UICommander uiCommander;

	public Animation animationRoot;

	public UISpineAnimation spine;

	public UIGrid gradeGrid;

	public GameObject fxFlash;

	[Range(0f, 8f)]
	public int commanderPosition;

	public FinishDelegate OnFinish;

	private void OnEnable()
	{
		animationRoot.Rewind();
		StopAllCoroutines();
		if (commanderPosition >= 0 && commanderPosition < 9)
		{
			StartCoroutine(StartAnimation());
		}
	}

	private void OnDisable()
	{
		fxFlash.SetActive(value: false);
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Set(RoCommander commander)
	{
		if (commander != null)
		{
			uiCommander.Set(commander);
		}
	}

	public void Show(string commanderId, int grade, int position)
	{
		if (position != -1)
		{
			commanderPosition = position;
			spine.spinePrefabName = commanderId;
			UISetter.SetRank(gradeGrid, grade);
			Show();
		}
	}

	public void SetUICommanderUnitPosition(Vector3 position)
	{
		fxFlash.transform.position = position;
	}

	private IEnumerator StartAnimation()
	{
		animationRoot.Play("Commander_board");
		while (animationRoot.IsPlaying("Commander_board"))
		{
			yield return null;
		}
		string aniName = "Commander_board_position_" + commanderPosition;
		animationRoot.Play(aniName);
		while (animationRoot.IsPlaying(aniName))
		{
			yield return null;
		}
		if (OnFinish != null)
		{
			OnFinish();
		}
		base.gameObject.SetActive(value: false);
	}
}
