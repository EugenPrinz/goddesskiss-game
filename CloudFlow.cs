using UnityEngine;

public class CloudFlow : MonoBehaviour
{
	[HideInInspector]
	public Cloud[] m_CloudList;

	public bool m_EnableLargeCloudLoop;

	public eCloudFlowBehavior m_Behavior = eCloudFlowBehavior.FlowTheSameWay;

	public float m_MinSpeed = 0.05f;

	public float m_MaxSpeed = 0.3f;

	public Camera m_Camera;

	private Vector3 LeftMostOfScreen;

	private Vector3 RightMostOfScreen;

	private void Start()
	{
		m_CloudList = new Cloud[base.transform.childCount];
		int num = 0;
		int num2 = 0;
		foreach (Transform item in base.transform)
		{
			m_CloudList[num2] = new Cloud();
			m_CloudList[num2].m_MoveSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);
			if (num == 0)
			{
				m_CloudList[num2].m_MoveSpeed *= -1f;
				if (m_Behavior != 0)
				{
				}
			}
			else if (m_Behavior != 0)
			{
			}
			m_CloudList[num2].m_Cloud = item.gameObject;
			if (m_EnableLargeCloudLoop)
			{
				m_CloudList[num2].m_CloudFollower = Object.Instantiate(item.gameObject);
			}
			m_CloudList[num2].m_OriginalLocalPos = m_CloudList[num2].m_Cloud.transform.localPosition;
			num2++;
		}
		if (m_EnableLargeCloudLoop)
		{
			Cloud[] cloudList = m_CloudList;
			foreach (Cloud cloud in cloudList)
			{
				cloud.m_CloudFollower.transform.parent = base.transform;
			}
		}
		FindTheOrthographicCamera();
	}

	private void Update()
	{
		if (m_Camera == null)
		{
			FindTheOrthographicCamera();
		}
		if (m_Camera == null)
		{
			return;
		}
		int num = 0;
		Cloud[] cloudList = m_CloudList;
		foreach (Cloud cloud in cloudList)
		{
			if (cloud.m_Cloud.activeSelf)
			{
				m_CloudList[num].m_Cloud.transform.localPosition = new Vector3(m_CloudList[num].m_Cloud.transform.localPosition.x + m_CloudList[num].m_MoveSpeed * Time.deltaTime, m_CloudList[num].m_Cloud.transform.localPosition.y, m_CloudList[num].m_Cloud.transform.localPosition.z);
				if (m_CloudList[num].m_MoveSpeed > 0f)
				{
					if (m_CloudList[num].m_CloudFollower != null)
					{
						m_CloudList[num].m_CloudFollower.transform.localPosition = new Vector3(m_CloudList[num].m_Cloud.transform.localPosition.x - m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.x, m_CloudList[num].m_Cloud.transform.localPosition.y, m_CloudList[num].m_Cloud.transform.localPosition.z);
					}
					if (m_CloudList[num].m_Cloud.transform.localPosition.x > RightMostOfScreen.x + m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.x / 2f)
					{
						if (m_EnableLargeCloudLoop)
						{
							GameObject cloud2 = m_CloudList[num].m_Cloud;
							m_CloudList[num].m_Cloud = m_CloudList[num].m_CloudFollower;
							m_CloudList[num].m_CloudFollower = cloud2;
						}
						else
						{
							m_CloudList[num].m_MoveSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);
							m_CloudList[num].m_Cloud.transform.localPosition = new Vector3(LeftMostOfScreen.x - m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.x, m_CloudList[num].m_OriginalLocalPos.y, m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.z);
						}
					}
				}
				else
				{
					if (m_CloudList[num].m_CloudFollower != null)
					{
						m_CloudList[num].m_CloudFollower.transform.localPosition = new Vector3(m_CloudList[num].m_Cloud.transform.localPosition.x + m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.x, m_CloudList[num].m_Cloud.transform.localPosition.y, m_CloudList[num].m_Cloud.transform.localPosition.z);
					}
					if (m_CloudList[num].m_Cloud.transform.localPosition.x < LeftMostOfScreen.x - m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.x / 2f)
					{
						if (m_EnableLargeCloudLoop)
						{
							GameObject cloud3 = m_CloudList[num].m_Cloud;
							m_CloudList[num].m_Cloud = m_CloudList[num].m_CloudFollower;
							m_CloudList[num].m_CloudFollower = cloud3;
						}
						else
						{
							m_CloudList[num].m_MoveSpeed = 0f - Random.Range(m_MinSpeed, m_MaxSpeed);
							m_CloudList[num].m_Cloud.transform.localPosition = new Vector3(RightMostOfScreen.x + m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.x, m_CloudList[num].m_OriginalLocalPos.y, m_CloudList[num].m_Cloud.GetComponent<UISprite>().CalculateBounds().size.z);
						}
					}
				}
			}
			num++;
		}
	}

	private void FindTheOrthographicCamera()
	{
		if (m_Camera == null)
		{
			Camera[] array = Object.FindObjectsOfType<Camera>();
			Camera[] array2 = array;
			foreach (Camera camera in array2)
			{
				if (camera.orthographic)
				{
					m_Camera = camera;
					break;
				}
			}
		}
		if (m_Camera != null)
		{
			LeftMostOfScreen = new Vector3(-1334f, 0f, 0f);
			RightMostOfScreen = new Vector3(1334f, 0f, 0f);
		}
	}
}
