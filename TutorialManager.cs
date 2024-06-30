using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public List<GameObject> TutorialPrefab = new List<GameObject>();

	public List<GameObject> TutorialPosition = new List<GameObject>();

	public GameObject uiView;

	public GameObject ActiveTutorial;

	public int Step;

	public bool isTutorial;

	private int MaxStep;

	public void initData()
	{
	}

	public void Next()
	{
	}

	private bool GetTutorialNextAble()
	{
		return true;
	}

	public void TutorialCameraMove()
	{
	}
}
