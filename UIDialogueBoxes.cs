using System.Collections.Generic;
using UnityEngine;

public class UIDialogueBoxes : MonoBehaviour
{
	[SerializeField]
	public GameObject contentsRoot;

	public UIDefaultListView DialgueListView;

	private UIDialogueBoxes _boxes;

	public void Init(List<string> _txt)
	{
		DialgueListView.SetDialougeText(_txt);
	}

	public static UIDialogueBoxes _Create()
	{
		Transform parent = UIManager.instance.DialogMrg.transform;
		GameObject gameObject = Utility.LoadAndInstantiateGameObject("Prefabs/UI/World/UIDialogueChoice", parent);
		if (gameObject == null)
		{
			return null;
		}
		UIDialogueBoxes component = gameObject.GetComponent<UIDialogueBoxes>();
		if (component == null)
		{
			return null;
		}
		component.gameObject.SetActive(value: true);
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		return component;
	}
}
