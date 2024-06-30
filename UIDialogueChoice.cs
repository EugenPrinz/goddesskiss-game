using UnityEngine;

public class UIDialogueChoice : UIItemBase
{
	[SerializeField]
	private UILabel DialogueText;

	[SerializeField]
	private UIDialogueBoxes parent;

	private int DialogueID;

	public void setText(string _txt, int _dialogueId)
	{
		if (_txt != null)
		{
			DialogueText.text = _txt;
			DialogueID = _dialogueId;
		}
	}

	public void SelectDialogue()
	{
		Dialoguer.ContinueDialogue(DialogueID);
		Object.Destroy(parent.gameObject);
	}
}
