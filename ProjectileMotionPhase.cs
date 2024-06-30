using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotionPhase : MonoBehaviour
{
	public enum EAnchor
	{
		NONE,
		TROOP_ROW_CENTER,
		TROOP_COLUMN_CENTER,
		TROOP_ROW_COLUMN_CENTER,
		TROOP_CAMERA_UI,
		MAIN_CAMERA,
		TROOP_CAMERA,
		UPDATE_TROOP_CAMERA,
		UPDATE_SPLIT
	}

	[Serializable]
	public struct ActivationEvent
	{
		public GameObject target;

		[Range(0f, 10000f)]
		public int time;

		public bool value;
	}

	[Serializable]
	public struct ExtraEffect
	{
		public GameObject prefab;

		[Tooltip("발생 위치: 지정한 객체의 위치에서 이펙트가 생성됨.\n특별한 이유가 없으면 지정할 필요 없음.\n지정되어 있지 않으면 페이즈 객체의 위치를 자동 지정.\n페이즈의 자식 객체가 아니면 정상작동 하지 않음.")]
		public Transform position;

		[Range(0f, 10000f)]
		public int creationTime;

		[Range(0f, 10000f)]
		public int destroyTime;
	}

	[HideInInspector]
	public int cacheID;

	public EAnchor anchor;

	[HideInInspector]
	public Vector3 anchorOffset;

	[HideInInspector]
	public Transform tfAnchor;

	[Tooltip("발사체 단계의 지속시간 시간이 지나면 삭제 된다.\n단위는 밀리초 (1초가 1000)")]
	[Range(0f, 10000f)]
	public int duration = 1000;

	[Tooltip("이벤트가 발생하는 시간.\n발사체의 페이즈에 따라 발생하는 이벤트 종류가 정해진다.\n")]
	[Range(-1f, 10000f)]
	public int eventTime = -1;

	[HideInInspector]
	public int eventDelayTime;

	[HideInInspector]
	public bool bEnabel;

	[HideInInspector]
	public bool bRender = true;

	[HideInInspector]
	public bool bFinishEventTime;

	[HideInInspector]
	public bool bInvers;

	[HideInInspector]
	public UnitRenderer target;

	public ActivationEvent[] activationEvents;

	public ExtraEffect[] extraEffects;

	public TimedGameObject timeGameObject;

	public List<ProjectileMotionPhase> childPhases;

	public List<CreateCacheItem> cacheItems;

	protected bool init;

	protected bool initAnchor;

	public int elapsedTime { get; set; }

	private void Awake()
	{
		if (eventTime == -1)
		{
			eventTime = duration;
		}
		if (!initAnchor)
		{
			anchorOffset = base.transform.localPosition;
			initAnchor = true;
		}
		if (timeGameObject == null)
		{
			timeGameObject = GetComponent<TimedGameObject>();
			if (timeGameObject == null)
			{
				timeGameObject = base.gameObject.AddComponent<TimedGameObject>();
				timeGameObject.TimeGroupController = TimeControllerManager.instance.Projectile;
				timeGameObject.groupType = ETimeGroupType.ProjectileGroup;
				timeGameObject.AssignedObjects = Utility.FindTimedObjects(base.transform);
				timeGameObject.SearchObjects = false;
			}
		}
	}

	private void Start()
	{
		init = true;
		OnEnable();
	}

	private void OnEnable()
	{
		if (!init)
		{
			return;
		}
		if (target != null)
		{
			Transform transform = ((target.drawSide != SplitScreenDrawSide.Left) ? SplitScreenManager.instance.right.troopAnchor : SplitScreenManager.instance.left.troopAnchor);
			Transform transform2 = ((target.drawSide != SplitScreenDrawSide.Left) ? SplitScreenManager.instance.right.ui : SplitScreenManager.instance.left.ui);
			int unitIdx = target.unitIdx;
			switch (anchor)
			{
			case EAnchor.TROOP_ROW_CENTER:
				if (transform != null && unitIdx >= 0)
				{
					unitIdx %= 9;
					switch (unitIdx)
					{
					case 0:
					case 2:
						unitIdx = 1;
						break;
					case 3:
					case 5:
						unitIdx = 4;
						break;
					case 6:
					case 8:
						unitIdx = 7;
						break;
					}
					tfAnchor = transform.GetChild(unitIdx);
					base.transform.position = tfAnchor.position;
					base.transform.localPosition += anchorOffset;
				}
				break;
			case EAnchor.TROOP_COLUMN_CENTER:
				if (transform != null && unitIdx >= 0)
				{
					unitIdx %= 9;
					switch (unitIdx)
					{
					case 0:
					case 6:
						unitIdx = 3;
						break;
					case 1:
					case 7:
						unitIdx = 4;
						break;
					case 2:
					case 8:
						unitIdx = 5;
						break;
					}
					tfAnchor = transform.GetChild(unitIdx);
					base.transform.position = tfAnchor.position;
					base.transform.localPosition += anchorOffset;
				}
				break;
			case EAnchor.TROOP_ROW_COLUMN_CENTER:
				if (transform != null)
				{
					unitIdx = 4;
					tfAnchor = transform.GetChild(unitIdx);
					base.transform.position = tfAnchor.position;
					base.transform.localPosition += anchorOffset;
				}
				break;
			case EAnchor.TROOP_CAMERA_UI:
				if (transform2 != null)
				{
					tfAnchor = transform2;
					base.transform.rotation = tfAnchor.rotation;
					base.transform.position = tfAnchor.TransformPoint(anchorOffset);
				}
				break;
			case EAnchor.MAIN_CAMERA:
				tfAnchor = SplitScreenManager.instance.transform;
				base.transform.rotation = tfAnchor.rotation;
				base.transform.position = tfAnchor.TransformPoint(anchorOffset);
				break;
			case EAnchor.TROOP_CAMERA:
				tfAnchor = ((target.drawSide != SplitScreenDrawSide.Left) ? SplitScreenManager.instance.right.camera.transform : SplitScreenManager.instance.left.camera.transform);
				base.transform.rotation = tfAnchor.rotation;
				base.transform.position = tfAnchor.TransformPoint(anchorOffset);
				break;
			case EAnchor.UPDATE_TROOP_CAMERA:
				if (target.drawSide == SplitScreenDrawSide.Left)
				{
					tfAnchor = transform.GetChild(4);
					if (tfAnchor != null)
					{
						Vector3 vector = target.transform.position - tfAnchor.position;
						Vector3 offset = SplitScreenManager.instance.left.cameraView.InverseTransformPoint(SplitScreenManager.instance.left.cameraView.position + vector);
						SplitScreenManager.instance.left.RegisterCamera(base.transform, offset, bInvers);
					}
				}
				else
				{
					tfAnchor = transform.GetChild(4);
					if (tfAnchor != null)
					{
						Vector3 vector2 = target.transform.position - tfAnchor.position;
						Vector3 offset2 = SplitScreenManager.instance.right.cameraView.InverseTransformPoint(SplitScreenManager.instance.right.cameraView.position + vector2);
						SplitScreenManager.instance.right.RegisterCamera(base.transform, offset2, bInvers);
					}
				}
				break;
			case EAnchor.UPDATE_SPLIT:
				SplitScreenManager.instance.RegisterSpline(base.transform, bInvers);
				break;
			}
		}
		elapsedTime = 0;
		bFinishEventTime = false;
		for (int i = 0; i < childPhases.Count; i++)
		{
			ProjectileMotionPhase projectileMotionPhase = childPhases[i];
			if (i == 0)
			{
				projectileMotionPhase.bEnabel = true;
				projectileMotionPhase.gameObject.SetActive(value: true);
			}
			else
			{
				projectileMotionPhase.bEnabel = false;
				projectileMotionPhase.gameObject.SetActive(value: false);
			}
		}
	}

	private void OnDisable()
	{
		if (!(SplitScreenManager.instance == null))
		{
			if (anchor == EAnchor.UPDATE_TROOP_CAMERA)
			{
				SplitScreenManager.instance.left.UnRegisterCamera(base.transform);
				SplitScreenManager.instance.right.UnRegisterCamera(base.transform);
			}
			else if (anchor == EAnchor.UPDATE_SPLIT)
			{
				SplitScreenManager.instance.UnRegisterSpline(base.transform);
			}
		}
	}

	public void Set(UnitRenderer actor, UnitRenderer target)
	{
		if (!initAnchor)
		{
			anchorOffset = base.transform.localPosition;
			initAnchor = true;
		}
		this.target = target;
		for (int i = 0; i < childPhases.Count; i++)
		{
			childPhases[i].bInvers = bInvers;
			childPhases[i].Set(actor, target);
		}
		for (int j = 0; j < cacheItems.Count; j++)
		{
			cacheItems[j].actor = actor;
		}
	}

	private void Update()
	{
		if (!init)
		{
			return;
		}
		if (anchor == EAnchor.TROOP_CAMERA_UI || anchor == EAnchor.TROOP_CAMERA || anchor == EAnchor.MAIN_CAMERA)
		{
			if (tfAnchor == null)
			{
				return;
			}
			base.transform.rotation = tfAnchor.rotation;
			base.transform.position = tfAnchor.TransformPoint(anchorOffset);
		}
		int num = 0;
		int num2 = (int)(1000f * timeGameObject.TimedDeltaTime());
		if (childPhases.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < childPhases.Count; i++)
		{
			ProjectileMotionPhase projectileMotionPhase = childPhases[i];
			if (!projectileMotionPhase.bEnabel)
			{
				continue;
			}
			if (projectileMotionPhase.elapsedTime == 0)
			{
				int num3 = -1;
			}
			projectileMotionPhase.elapsedTime += num + num2;
			if (!projectileMotionPhase.bFinishEventTime)
			{
				num = Mathf.Max(0, projectileMotionPhase.elapsedTime - (projectileMotionPhase.eventTime + projectileMotionPhase.eventDelayTime));
				if (num > 0)
				{
					projectileMotionPhase.bFinishEventTime = true;
					int num4 = i + 1;
					if (num4 < childPhases.Count)
					{
						childPhases[num4].bEnabel = true;
						childPhases[num4].gameObject.SetActive(value: true);
					}
				}
			}
			if (Mathf.Max(0, projectileMotionPhase.elapsedTime - projectileMotionPhase.duration) > 0)
			{
				projectileMotionPhase.gameObject.SetActive(value: false);
				if (projectileMotionPhase.bFinishEventTime)
				{
					projectileMotionPhase.bEnabel = false;
					projectileMotionPhase.elapsedTime = 0;
				}
			}
		}
	}

	public void SetEventDealy(int eventTime)
	{
		eventDelayTime = eventTime;
	}
}
