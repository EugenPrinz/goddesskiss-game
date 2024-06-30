using System.Collections.Generic;
using Cache;
using Shared.Battle;
using UnityEngine;

public class UnitRenderer : MonoBehaviour
{
	public enum EExplosionType
	{
		None,
		Default,
		Own
	}

	public const string PrefabFolderPath = "Assets/Prefabs/Units";

	public string unitName;

	public int modelType;

	public List<MeshRenderer> localMeshRenderers;

	public List<GameObject> typeMeshRendererObject;

	public int cacheID;

	public int unitIdx;

	public Unit unit;

	public UIBattleUnit ui;

	public UIBattleUnitControllerItem uiCommander;

	public UISkill uiSkill;

	public TimedGameObject timedGameObject;

	public SplitScreenDrawSide drawSide;

	public BattleUnitSelectMark selectedMark;

	public bool isAuto;

	public SplitScreenManager _ssm;

	private Animation _ani;

	private FlowAndDeactivate _flow;

	private string _idleAnimationName = "idle";

	public UnitPositionPingPong dummyMove;

	public GameObject injuryEffect;

	private GameObject _injuryEffectRoot;

	private GameObject _destroyEffectRoot;

	public GameObject phaseEffect;

	public EExplosionType explosionType;

	public MountPoints mountPosition;

	public Transform mainTransform;

	public Transform uiTransform;

	private Collider _collider;

	private Vector3 _orgScale;

	protected Dictionary<int, StatusEffectController> _statusRenderers;

	private string _nextAnimationName;

	private bool _isDead;

	private List<int> _renderQueueList;

	public float scale
	{
		set
		{
			base.transform.localScale = _orgScale * value;
		}
	}

	public ECharacterType charType
	{
		get
		{
			if (unit == null)
			{
				return ECharacterType.None;
			}
			return unit._charType;
		}
	}

	public bool IsDead => _isDead;

	public void SetUnit(Unit unit)
	{
		this.unit = unit;
		unitIdx = unit._unitIdx;
	}

	public void SetAttackTargetCandidate(bool isTarget)
	{
		if (selectedMark != null)
		{
			selectedMark.SetAttackTargetCadidate(isTarget);
		}
		if (_collider != null)
		{
			_collider.enabled = true;
		}
	}

	public void SetAttackTarget(bool isTarget)
	{
		if (selectedMark != null)
		{
			selectedMark.SetAttackTarget(isTarget);
		}
	}

	public void SetTurnUnit(bool isTurnUnit)
	{
		if (selectedMark != null)
		{
			selectedMark.SetTurnUnit(isTurnUnit);
		}
	}

	public void SetUIActive(bool active)
	{
		if (ui != null)
		{
			ui.root.SetActive(active);
		}
		if (selectedMark != null)
		{
			selectedMark.gameObject.SetActive(active);
		}
	}

	public void SetInjury(bool injury)
	{
		if (charType == ECharacterType.Raid)
		{
			return;
		}
		if (injury)
		{
			_idleAnimationName = "injury";
			if (!unit.isPlayingAction)
			{
				PlayAnimation("injury");
			}
		}
		else
		{
			_idleAnimationName = "idle";
		}
		if (_injuryEffectRoot != null)
		{
			_injuryEffectRoot.SetActive(injury);
		}
	}

	public void BeginDummyMove()
	{
		dummyMove = base.gameObject.GetComponent<UnitPositionPingPong>();
		if (charType == ECharacterType.RaidPart)
		{
			if (dummyMove != null)
			{
				Object.DestroyImmediate(dummyMove);
			}
		}
		else if (dummyMove == null)
		{
			dummyMove = base.gameObject.AddComponent<UnitPositionPingPong>();
		}
	}

	public void Init()
	{
		_ssm = SplitScreenManager.instance;
		_isDead = false;
	}

	private void Awake()
	{
		_orgScale = base.transform.localScale;
		if (timedGameObject == null)
		{
			timedGameObject = GetComponent<TimedGameObject>();
			if (timedGameObject == null)
			{
				timedGameObject = base.gameObject.AddComponent<TimedGameObject>();
				timedGameObject.groupType = ETimeGroupType.EtcGroup;
				timedGameObject.AssignedObjects = Utility.FindTimedObjects(base.transform);
				timedGameObject.SearchObjects = false;
			}
		}
		if (mountPosition == null)
		{
			mountPosition = GetComponent<MountPoints>();
		}
		if (mainTransform == null)
		{
			mainTransform = base.transform;
		}
		if (uiTransform == null)
		{
			uiTransform = mainTransform;
		}
		_collider = GetComponent<Collider>();
	}

	private void Start()
	{
		Init();
		_statusRenderers = new Dictionary<int, StatusEffectController>();
		_ani = GetComponent<Animation>();
		_ani.cullingType = AnimationCullingType.AlwaysAnimate;
		if (charType != ECharacterType.RaidPart)
		{
			_flow = base.gameObject.AddComponent<FlowAndDeactivate>();
			_flow.target = base.gameObject;
			_flow.enabled = false;
			_flow.OnEventTime = delegate
			{
				_Release();
			};
		}
		if (injuryEffect != null)
		{
			_injuryEffectRoot = Utility.LoadAndInstantiateGameObject("Prefabs/Effect/Injury", injuryEffect.transform);
			_destroyEffectRoot = Utility.LoadAndInstantiateGameObject("Prefabs/Effect/Destroy", injuryEffect.transform);
			_injuryEffectRoot.SetActive(value: false);
			_destroyEffectRoot.SetActive(value: false);
		}
		PlayAnimation(_idleAnimationName);
	}

	private void _Release()
	{
		SetTurnUnit(isTurnUnit: false);
		SetAttackTargetCandidate(isTarget: false);
		SetAttackTarget(isTarget: false);
		SetInjury(injury: false);
		UISetter.SetActive(_injuryEffectRoot, active: false);
		UISetter.SetActive(_destroyEffectRoot, active: false);
		if (timedGameObject != null)
		{
			timedGameObject.SetTimeSpeed(100f, update: false);
			timedGameObject.groupType = ETimeGroupType.EtcGroup;
		}
		if (charType == ECharacterType.RaidPart)
		{
			if (ui != null)
			{
				ui.gameObject.SetActive(value: false);
			}
			return;
		}
		if (ui != null)
		{
			UIManager.instance.battle.MainUI.uiBattleUnitPanel.Release(ui);
			ui = null;
		}
		uiCommander = null;
		unit = null;
		CacheManager.instance.UnitCache.Release(cacheID, base.gameObject);
	}

	private void OnEnable()
	{
		if (ui != null)
		{
			ui.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if (ui != null)
		{
			ui.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (ui != null)
		{
			ui.transform.position = _ssm.ConvertPosCutToUI(drawSide, uiTransform.position);
		}
		UpdateStatusPosition();
		if (!_ani.isPlaying)
		{
			if (_isDead)
			{
				return;
			}
			if (unit != null && unit.isDead)
			{
				if (unit.takenRevival)
				{
					return;
				}
				if (unit._hasEventDeathSkill)
				{
					Skill skill = unit.skills[unit._deathSkillIndex];
					if (skill._sp >= skill._skillDataRow.maxSp)
					{
						return;
					}
					if (skill.isMeleeDeathSkillType)
					{
						_isDead = true;
						if (ui != null)
						{
							ui.Dead();
						}
						_nextAnimationName = null;
						_Release();
						return;
					}
				}
				Dead("destroy");
			}
			else if (_nextAnimationName != null)
			{
				_ani.Play(_nextAnimationName);
				_nextAnimationName = null;
				if (dummyMove != null)
				{
					dummyMove.enabled = true;
				}
			}
		}
		else if (!_isDead && unit != null && unit.isDead && !unit._hasEventDeathSkill && !unit.takenRevival)
		{
			Dead("destroy");
		}
	}

	public Transform GetBone(string path)
	{
		return base.transform.Find(path);
	}

	public void PlayAnimation(string animationName, bool hasReturnMotion = false)
	{
		if (!string.IsNullOrEmpty(animationName))
		{
			_ani.Play(animationName);
			if (hasReturnMotion)
			{
				_nextAnimationName = null;
			}
			else
			{
				_nextAnimationName = _idleAnimationName;
			}
			if ((animationName == "passive" || animationName == "melee1") && dummyMove != null)
			{
				dummyMove.enabled = false;
			}
		}
	}

	public void Dead(string animationName)
	{
		if (_isDead)
		{
			return;
		}
		if (_destroyEffectRoot != null)
		{
			if (_injuryEffectRoot != null)
			{
				_injuryEffectRoot.SetActive(value: false);
			}
			_destroyEffectRoot.SetActive(value: true);
		}
		else if (_injuryEffectRoot != null)
		{
			_injuryEffectRoot.SetActive(value: true);
		}
		switch (explosionType)
		{
		case EExplosionType.Default:
		case EExplosionType.Own:
		{
			Transform transform = null;
			transform = ((drawSide != SplitScreenDrawSide.Left) ? UIManager.instance.battle.RightView.troopAnchor : UIManager.instance.battle.LeftView.troopAnchor);
			Animation animation = CacheManager.instance.EtcEffectCache.Create<Animation>("explosion_default", transform);
			animation.gameObject.transform.parent = base.transform;
			animation.gameObject.transform.localPosition = Vector3.zero;
			animation.Rewind();
			break;
		}
		}
		_isDead = true;
		if (_flow != null)
		{
			_flow.enabled = true;
		}
		if (dummyMove != null)
		{
			dummyMove.enabled = false;
		}
		if (ui != null)
		{
			ui.Dead();
		}
		_ani.Play(animationName);
		_nextAnimationName = null;
		UvAnimation[] componentsInChildren = base.gameObject.GetComponentsInChildren<UvAnimation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].speed = Vector2.zero;
		}
		if (phaseEffect != null)
		{
			phaseEffect.SetActive(value: false);
		}
		if (charType == ECharacterType.RaidPart)
		{
			_Release();
		}
	}

	public void Revival(string animationName)
	{
		if (_isDead)
		{
			base.gameObject.SetActive(value: true);
			_isDead = false;
			if (_flow != null)
			{
				_flow.enabled = false;
			}
			if (injuryEffect != null)
			{
				_injuryEffectRoot.SetActive(value: false);
				_destroyEffectRoot.SetActive(value: false);
			}
			PlayAnimation(_idleAnimationName);
			if (ui != null)
			{
				ui.gameObject.SetActive(value: true);
				ui.Revival();
			}
			UvAnimation[] componentsInChildren = base.gameObject.GetComponentsInChildren<UvAnimation>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].speed = new Vector3(0f, -2f);
			}
			if (phaseEffect != null)
			{
				phaseEffect.SetActive(value: true);
			}
			if (unit != null && charType != ECharacterType.RaidPart)
			{
				UIManager.instance.battle.Main.PlayUnitEntry(this);
			}
		}
	}

	public void SetRenderQueueForCutIn()
	{
		if (_renderQueueList == null)
		{
			_InitRenderQueueList();
		}
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				int renderQueue = material.renderQueue;
				int renderQueue2 = 3996;
				if (renderQueue >= 3000)
				{
					renderQueue2 = 3999;
				}
				else if (renderQueue >= 2450)
				{
					renderQueue2 = 3998;
				}
				else if (renderQueue >= 2000)
				{
					renderQueue2 = 3997;
				}
				material.renderQueue = renderQueue2;
			}
		}
	}

	public void ResetRenderQueue()
	{
		if (_renderQueueList == null)
		{
			_InitRenderQueueList();
		}
		int num = -1;
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				material.renderQueue = _renderQueueList[++num];
			}
		}
	}

	public void _InitRenderQueueList()
	{
		_renderQueueList = new List<int>();
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				_renderQueueList.Add(material.renderQueue);
			}
		}
	}

	public void AddStatus(Status status)
	{
		if (!_statusRenderers.ContainsKey(status.Dri) && !string.IsNullOrEmpty(status.DataRow.pfbName) && status.DataRow.pfbName != "-")
		{
			StatusEffectController statusEffectController = CacheManager.instance.ControllerCache.Create<StatusEffectController>("StatusEffectController");
			if (statusEffectController != null)
			{
				statusEffectController.name = status.DataRow.pfbName;
				if (statusEffectController.Create(status.DataRow.pfbName, Vector3.zero))
				{
					statusEffectController.transform.position = base.transform.position;
					_statusRenderers.Add(status.Dri, statusEffectController);
				}
			}
		}
		ui.AddStatus(status);
	}

	public void RemoveStatus(Status status)
	{
		if (_statusRenderers.ContainsKey(status.Dri))
		{
			StatusEffectController statusEffectController = _statusRenderers[status.Dri];
			statusEffectController.Release();
			_statusRenderers.Remove(status.Dri);
		}
		ui.RemoveStatus(status);
	}

	public void UpdateStatusPosition()
	{
		if (_statusRenderers != null)
		{
			StatusEffectController statusEffectController = null;
			Dictionary<int, StatusEffectController>.Enumerator enumerator = _statusRenderers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				statusEffectController = enumerator.Current.Value;
				statusEffectController.transform.position = base.transform.position;
			}
		}
	}

	public void CleanStatus()
	{
		StatusEffectController statusEffectController = null;
		Dictionary<int, StatusEffectController>.Enumerator enumerator = _statusRenderers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			statusEffectController = enumerator.Current.Value;
			statusEffectController.Release();
		}
		_statusRenderers.Clear();
	}

	public Transform GetMountPosition(string key)
	{
		Transform transform = null;
		if (mountPosition == null)
		{
			return base.transform;
		}
		transform = mountPosition.GetPosition(key);
		if (transform == null)
		{
			return base.transform;
		}
		return transform;
	}

	private Transform _FindChildByName(string targetName, Transform tfTarget)
	{
		if (tfTarget.name == targetName)
		{
			return tfTarget.transform;
		}
		foreach (Transform item in tfTarget)
		{
			Transform transform = _FindChildByName(targetName, item);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public void SetModelType(int type)
	{
		if (modelType == type || type <= 0)
		{
			return;
		}
		for (int i = 0; i < typeMeshRendererObject.Count; i++)
		{
			Object.DestroyImmediate(typeMeshRendererObject[i]);
		}
		typeMeshRendererObject.Clear();
		UnitCache unitCache = CacheManager.instance.UnitCache;
		string key = $"{unitName}@{type}";
		CacheElement element = unitCache.GetElement(key);
		if (element == null)
		{
			return;
		}
		GameObject gameObject = unitCache.Create(element, Vector3.zero, Quaternion.identity);
		SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		if (componentsInChildren.Length > 0)
		{
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				GameObject gameObject2 = new GameObject(skinnedMeshRenderer.gameObject.name);
				gameObject2.transform.parent = base.transform;
				SkinnedMeshRenderer skinnedMeshRenderer2 = gameObject2.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				Transform[] array = new Transform[skinnedMeshRenderer.bones.Length];
				for (int k = 0; k < skinnedMeshRenderer.bones.Length; k++)
				{
					array[k] = _FindChildByName(skinnedMeshRenderer.bones[k].name, base.transform);
				}
				skinnedMeshRenderer2.bones = array;
				skinnedMeshRenderer2.sharedMesh = skinnedMeshRenderer.sharedMesh;
				skinnedMeshRenderer2.materials = skinnedMeshRenderer.materials;
				skinnedMeshRenderer2.useLightProbes = true;
				skinnedMeshRenderer2.receiveShadows = true;
				skinnedMeshRenderer2.rootBone = _FindChildByName(skinnedMeshRenderer.rootBone.name, base.transform);
				typeMeshRendererObject.Add(gameObject2);
			}
			for (int l = 0; l < localMeshRenderers.Count; l++)
			{
				localMeshRenderers[l].materials = componentsInChildren[0].materials;
			}
		}
		Object.DestroyImmediate(gameObject);
		modelType = type;
	}
}
