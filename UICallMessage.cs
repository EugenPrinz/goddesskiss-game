using UnityEngine;

public class UICallMessage : MonoBehaviour
{
	public GameObject scrollView;

	public UILabel message;

	private const int startKey = 19301;

	private const int endKey = 19400;

	private RoLocalUser localUser;

	private Vector2 scrollSize;

	private Vector3 labelStartPosition;

	private int textLength;

	private float speed = 0.3f;

	public void Close()
	{
		UISetter.SetActive(base.gameObject, active: false);
	}

	public void Open()
	{
		UISetter.SetActive(base.gameObject, active: true);
	}

	public void Init()
	{
		localUser = RemoteObjectManager.instance.localUser;
		scrollSize = scrollView.GetComponent<UIPanel>().GetViewSize();
		labelStartPosition = message.transform.localPosition;
		SetNotice();
	}

	public void SetNotice()
	{
		if (localUser.noticeList.Count == 0)
		{
			if (base.gameObject.activeSelf)
			{
				UISetter.SetActive(base.gameObject, active: false);
			}
			return;
		}
		int[] array = new int[localUser.noticeList.Count];
		localUser.noticeList.Keys.CopyTo(array, 0);
		UISetter.SetLabel(message, localUser.noticeList[array[Random.Range(0, array.Length - 1)]]);
		textLength = message.width;
		message.transform.localPosition = labelStartPosition;
		if (!base.gameObject.activeSelf)
		{
			UISetter.SetActive(base.gameObject, active: true);
		}
	}

	private void Update()
	{
		if (base.gameObject.activeSelf)
		{
			if (message.transform.localPosition.x >= 0f - ((float)textLength + scrollSize.x / 2f))
			{
				message.transform.Translate((0f - speed) * Time.deltaTime, 0f, 0f);
			}
			else
			{
				message.transform.localPosition = labelStartPosition;
			}
		}
	}
}
