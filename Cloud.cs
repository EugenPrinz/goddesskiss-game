using System;
using UnityEngine;

[Serializable]
public class Cloud
{
	public float m_MoveSpeed;

	public GameObject m_Cloud;

	public GameObject m_CloudFollower;

	public Vector3 m_OriginalLocalPos;

	public bool bPos;

	public float lastPercentPosition;

	public void UpdatePos()
	{
	}
}
