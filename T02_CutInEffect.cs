using System;
using System.Collections;
using System.Collections.Generic;
using Shared;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

[ExecuteInEditMode]
public class T02_CutInEffect : MonoBehaviour
{
	private enum State
	{
		Unknown,
		Opening,
		Playing,
		InputWait,
		CutIn,
		Result
	}

	public enum E_CUTIN_STATE
	{
		IDLE,
		PLAYING
	}

	private class _UnitRendererCreator : FrameAccessor
	{
		public SWP_TimeGroupController timeGroupController;

		public UnitRendererCache cache;

		public UnitRenderer[] renderers;

		public Transform lhsTroopAnchor;

		public Transform rhsTroopAnchor;

		public UIBattleMain ui;

		public int createdLhsTroopIndex;

		public int createdRhsTroopIndex;

		public T02_CutInEffect battle;

		public static _UnitRendererCreator Create(T02_CutInEffect battle)
		{
			_UnitRendererCreator unitRendererCreator = new _UnitRendererCreator();
			unitRendererCreator.battle = battle;
			unitRendererCreator.timeGroupController = battle._timeGroupController;
			unitRendererCreator.cache = battle._unitRendererCache;
			unitRendererCreator.renderers = battle._unitRenderers;
			unitRendererCreator.lhsTroopAnchor = battle.lhsTroopAnchor;
			unitRendererCreator.rhsTroopAnchor = battle.rhsTroopAnchor;
			unitRendererCreator.ui = battle.ui;
			unitRendererCreator.createdLhsTroopIndex = -1;
			unitRendererCreator.createdRhsTroopIndex = -1;
			return unitRendererCreator;
		}

		public override bool OnFrameAccessStart()
		{
			createdLhsTroopIndex = -1;
			createdRhsTroopIndex = -1;
			return true;
		}

		public override bool OnUnitAccessStart()
		{
			int num = base.unitIndex;
			if (!base.frame.IsUnitInBattle(num))
			{
				return false;
			}
			UnitRenderer unitRenderer = renderers[num];
			if (unitRenderer != null)
			{
				return false;
			}
			int lhsTroopIndex = base.simulator.GetLhsTroopIndex(num);
			int rhsTroopIndex = base.simulator.GetRhsTroopIndex(num);
			Transform transform = null;
			SplitScreenDrawSide drawSide = SplitScreenDrawSide.Unknown;
			if (lhsTroopIndex >= 0)
			{
				createdLhsTroopIndex = lhsTroopIndex;
				transform = lhsTroopAnchor;
				drawSide = SplitScreenDrawSide.Left;
			}
			if (rhsTroopIndex >= 0)
			{
				createdRhsTroopIndex = rhsTroopIndex;
				transform = rhsTroopAnchor;
				drawSide = SplitScreenDrawSide.Right;
			}
			int index = num % 9;
			transform = transform.GetChild(index);
			unitRenderer = cache.Create(base.unitDr.resourceName);
			renderers[num] = unitRenderer;
			SWP_TimedGameObject sWP_TimedGameObject = unitRenderer.gameObject.AddComponent<SWP_TimedGameObject>();
			sWP_TimedGameObject.ControllerGroupID = timeGroupController.GroupID;
			sWP_TimedGameObject.TimeGroupController = timeGroupController;
			Vector3 localScale = unitRenderer.transform.localScale;
			unitRenderer.transform.parent = transform;
			unitRenderer.transform.localPosition = Vector3.zero;
			unitRenderer.transform.localRotation = Quaternion.identity;
			unitRenderer.transform.localScale = localScale;
			unitRenderer.gameObject.name = "Unit-" + num;
			unitRenderer.SetUnit(base.unit);
			unitRenderer.ui = battle.ui.uiBattleUnitPanel.CreateUnitUI();
			unitRenderer.ui.gameObject.name = unitRenderer.gameObject.name;
			unitRenderer.drawSide = drawSide;
			GameObject gameObject = Utility.LoadAndInstantiateGameObject("Prefabs/UI/Battle/SelectMark", unitRenderer.transform);
			BattleUnitSelectMark battleUnitSelectMark = (unitRenderer.selectedMark = gameObject.GetComponent<BattleUnitSelectMark>());
			battleUnitSelectMark.SetTurnUnit(active: false);
			battleUnitSelectMark.SetAttackTargetCadidate(active: false);
			battleUnitSelectMark.SetAttackTarget(active: false);
			return false;
		}

		private UIBattleUnit _CreateUnitUi(GameObject parent, GameObject prefab)
		{
			GameObject gameObject = NGUITools.AddChild(parent, prefab);
			gameObject.SetActive(value: true);
			return gameObject.GetComponent<UIBattleUnit>();
		}
	}

	private class _ProjectileRendererCreator : FrameAccessor
	{
		public SWP_TimeGroupController timeGroupController;

		public ProjectileRendererCache cache;

		public UnitRenderer[] unitRenderers;

		public Shared.Battle.Random random;

		public static _ProjectileRendererCreator Create(T02_CutInEffect battle)
		{
			_ProjectileRendererCreator projectileRendererCreator = new _ProjectileRendererCreator();
			projectileRendererCreator.timeGroupController = battle._timeGroupController;
			projectileRendererCreator.cache = battle._projectileRendererCache;
			projectileRendererCreator.unitRenderers = battle._unitRenderers;
			return projectileRendererCreator;
		}

		public override bool OnFrameAccessStart()
		{
			random = new Shared.Battle.Random(base.frame.randomSeed);
			return true;
		}

		public override void OnProjectileAccessStart()
		{
			if (!base.frame.IsUnitInBattle(base.unitIndex) || base.projectile.elapsedTime != 0)
			{
				return;
			}
			bool shouldRenderSplash = base.projectileDr.shouldRenderSplash;
			if (!base.projectile.isSplash || shouldRenderSplash)
			{
				UnitRenderer unitRenderer = unitRenderers[base.unitIndex];
				int fireEventIndex = base.projectile.fireEventIndex;
				FireEvent fireEvent = base.unitMotionDr.fireEvents[fireEventIndex];
				string firePointBonePath = fireEvent.firePointBonePath;
				Transform bone = unitRenderer.GetBone(firePointBonePath);
				Vector3 position = unitRenderers[base.projectile.targetIndex].transform.position;
				Vector3 vector = bone.position - unitRenderer.transform.position;
				if (base.isMissedProjectile)
				{
					float num = 1.5f + 0.5f * (0.01f * (float)random.Next(0, 100));
					float f = (float)random.Next(0, 360) * ((float)Math.PI / 180f);
					vector.x = num * Mathf.Cos(f);
					vector.y = num * Mathf.Sin(f);
				}
				vector.y = 0f;
				position += vector;
				ProjectileRenderer projectileRenderer = cache.Create(base.projectile.id, bone.position, position);
				SWP_TimedGameObject component = projectileRenderer.GetComponent<SWP_TimedGameObject>();
				if (component == null)
				{
					component = projectileRenderer.gameObject.AddComponent<SWP_TimedGameObject>();
					component.ControllerGroupID = timeGroupController.GroupID;
					component.TimeGroupController = timeGroupController;
					component.AssignedObjects = projectileRenderer.FindTimedObjects();
					component.SearchObjects = false;
				}
			}
		}
	}

	private class _UnitRendererUpdater : FrameAccessor
	{
		public UIBattleMain ui;

		public SplitScreenManager splitScreenManager;

		public CutInEffectCache cutInEffectCache;

		public CutInEffect cutInEffect;

		public UnitRenderer[] renderers;

		public float delay;

		public static _UnitRendererUpdater Create(T02_CutInEffect battle)
		{
			_UnitRendererUpdater unitRendererUpdater = new _UnitRendererUpdater();
			unitRendererUpdater.ui = battle.ui;
			unitRendererUpdater.splitScreenManager = battle._splitScreenManager;
			unitRendererUpdater.cutInEffectCache = battle._cutInEffectCache;
			unitRendererUpdater.renderers = battle._unitRenderers;
			return unitRendererUpdater;
		}

		public override bool OnUnitAccessStart()
		{
			int num = base.unitIndex;
			SplitScreenManager splitScreenManager = this.splitScreenManager;
			UnitRenderer unitRenderer = renderers[num];
			if (unitRenderer == null)
			{
				return false;
			}
			if ((float)base.unit.health <= 0f)
			{
				unitRenderer.Dead("destroy");
			}
			else
			{
				unitRenderer.ui.SetAnimateHp(base.unit.maxHealth, base.unit.health);
				Vector3 position = unitRenderer.transform.position;
				position += UnityEngine.Random.insideUnitSphere * 0.5f;
				position = splitScreenManager.ConvertPosCutToUI(unitRenderer.drawSide, position);
				if (base.unit.takenDamage > 0)
				{
					unitRenderer.PlayAnimation("behit");
					float num2 = (float)base.unit.health / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num2 <= 0.3f);
					ui.damageBubble.Add(position, base.unit.takenDamage, Color.white);
				}
				if (base.unit.takenCriticalDamage > 0)
				{
					unitRenderer.PlayAnimation("behit");
					float num3 = (float)base.unit.health / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num3 <= 0.3f);
					ui.damageBubble.Add(position, base.unit.takenCriticalDamage, Color.red);
				}
				if (base.unit.takenHealing > 0)
				{
					float num4 = (float)base.unit.health / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num4 <= 0.3f);
					ui.damageBubble.Add(position, base.unit.takenHealing, Color.green);
				}
				if (base.unit._takenCriticalHealing > 0)
				{
					float num5 = (float)base.unit.health / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num5 <= 0.3f);
					ui.damageBubble.Add(position, base.unit._takenCriticalHealing, Color.green);
				}
				if (base.unit.avoidanceCount > 0)
				{
					unitRenderer.PlayAnimation("avoid");
					ui.missBubble.Add(position);
				}
			}
			return true;
		}

		public override bool OnSkillAccessStart()
		{
			UnitRenderer unitRenderer = renderers[base.unitIndex];
			if (base.skillIndex == 1)
			{
				float max = base.skillDr.maxSp;
				float curr = base.skill.sp;
				unitRenderer.ui.SetAnimateSkill(max, curr);
			}
			int num = base.unitMotionDr.playTime;
			string cutInEffectId = base.skillDr.cutInEffectId;
			FireEvent fireEvent = base.unitMotionDr.fireEvents[0];
			bool flag = false;
			if (cutInEffect == null && !string.IsNullOrEmpty(cutInEffectId) && fireEvent != null)
			{
				num += 66;
				flag = true;
			}
			bool flag2 = false;
			int num2 = base.skill.remainedMotionTime + 66;
			for (int i = 0; i < base.unitMotionDr.atkSwitchEvents.Count; i++)
			{
				flag2 = true;
				AttackSwitchEvent attackSwitchEvent = base.unitMotionDr.atkSwitchEvents[i];
				if (attackSwitchEvent == null)
				{
					break;
				}
				int num3 = base.unitMotionDr.playTime - attackSwitchEvent.time;
				if (num3 >= num2 || num3 < base.skill.remainedMotionTime)
				{
					continue;
				}
				switch (attackSwitchEvent.eType)
				{
				case AttackSwitchEvent.E_SWITCH_TYPE.TARGET:
				{
					Transform transform = unitRenderer.transform.Find("root_node/_TP");
					if (transform != null)
					{
						unitRenderer.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
						unitRenderer.transform.position = unitRenderer.transform.position + (renderers[base.skill._targetIndex].transform.position - transform.position);
					}
					break;
				}
				case AttackSwitchEvent.E_SWITCH_TYPE.LOCAL:
					unitRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
					unitRenderer.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
					break;
				}
			}
			if (num != base.skill.remainedMotionTime)
			{
				return false;
			}
			float num4 = 0f;
			if (flag)
			{
				bool flag3 = cutInEffectCache.HasEffect(cutInEffectId);
				CutInEffect.Side side = CutInEffect.Side.Left;
				if (base.frame.IsRhsUnitInBattle(base.unitIndex))
				{
					side = CutInEffect.Side.Right;
				}
				if (flag3 && cutInEffectId != "Default")
				{
					cutInEffect = cutInEffectCache.Create(cutInEffectId, side, unitRenderer);
					if (flag2)
					{
						cutInEffect.isDefault = false;
					}
					float num5 = (float)fireEvent.time * 0.001f;
					if (cutInEffect.eventDelay == -1)
					{
						num4 = cutInEffect.duration - num5;
						delay = cutInEffect.duration * 1000f - (float)fireEvent.time;
					}
					else
					{
						num4 = (float)cutInEffect.eventDelay * 0.001f;
						delay = cutInEffect.eventDelay;
					}
				}
				else
				{
					num4 = 0.5f;
					float num6 = (float)fireEvent.time * 0.001f + num4;
					delay = 300f;
					cutInEffect = cutInEffectCache.Create("Default", side, unitRenderer);
					if (flag2)
					{
						cutInEffect.isDefault = false;
					}
					Animation component = cutInEffect.GetComponent<Animation>();
					component[component.clip.name].speed /= num6;
					cutInEffect.RefreshDuration();
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
			}
			string unitMotionDrk = base.skillDr.unitMotionDrk;
			unitMotionDrk = unitMotionDrk.Substring(unitMotionDrk.LastIndexOf("/") + 1);
			unitRenderer.StartCoroutine(_PlayUnitMotion(unitRenderer, unitMotionDrk, num4));
			return false;
		}

		private IEnumerator _PlayUnitMotion(UnitRenderer renderer, string motionName, float delay)
		{
			if (delay > 0f)
			{
				yield return new WaitForSeconds(delay);
			}
			renderer.PlayAnimation(motionName);
		}
	}

	private class _MainUiUpdater : FrameAccessor
	{
	}

	private class _SkillIconUpdater : FrameAccessor
	{
		public override bool OnSkillAccessStart()
		{
			return false;
		}
	}

	private class _TurnUiUpdater : FrameAccessor
	{
		public UIBattleMain ui;

		public UnitRenderer[] renderers;

		public Shared.Battle.Input input;

		public override bool OnFrameAccessStart()
		{
			for (int i = 0; i < base.frame.units.Count; i++)
			{
				UnitRenderer unitRenderer = renderers[i];
				if (!(unitRenderer == null))
				{
					if (i == base.frame.turnUnitIndex)
					{
						unitRenderer.ui.SetTurn(0);
						unitRenderer.SetTurnUnit(isTurnUnit: true);
					}
					else
					{
						unitRenderer.SetTurnUnit(isTurnUnit: false);
					}
					unitRenderer.SetAttackTargetCandidate(isTarget: false);
					unitRenderer.SetAttackTarget(isTarget: false);
				}
			}
			if (!base.frame.isWaitingInput)
			{
				Manager<UISkillView>.GetInstance().SetInputFlag(bFlag: false);
				return true;
			}
			if (base.frame.IsLhsUnitInBattle(base.frame.turnUnitIndex))
			{
				Manager<UISkillView>.GetInstance().SetInputFlag(bFlag: true);
				Manager<UISkillView>.GetInstance().Set(base.frame.units[base.frame.turnUnitIndex]);
			}
			else
			{
				Manager<UISkillView>.GetInstance().SetInputFlag(bFlag: false);
				Manager<UISkillView>.GetInstance().Set(null);
			}
			return false;
		}

		public override bool OnSkillAccessStart()
		{
			if (base.unitIndex != base.frame.turnUnitIndex)
			{
				return false;
			}
			if (base.skill.remainedMotionTime == 0)
			{
				return false;
			}
			UnitRenderer unitRenderer = renderers[base.skill.targetIndex];
			if (unitRenderer != null)
			{
				unitRenderer.SetAttackTarget(isTarget: true);
			}
			return false;
		}
	}

	private class _InputCorrector : FrameAccessor
	{
		public Shared.Battle.Input input;

		public bool isAuto;
	}

	public E_CUTIN_STATE eCutInState;

	public UIBattleMain ui;

	public List<AudioClip> bgmList;

	public AudioClip winAudioClip;

	public AudioClip loseAudioClip;

	public Animator sceneAnimator;

	public Transform lhsTroopAnchor;

	public Transform rhsTroopAnchor;

	public AnimationCurve troopEntryAnimationCurve;

	private BattleData _battleData;

	private SWP_TimeGroupController _timeGroupController;

	private SplitScreenManager _splitScreenManager;

	private UnitRendererCache _unitRendererCache;

	private CutInEffectCache _cutInEffectCache;

	private ProjectileRendererCache _projectileRendererCache;

	private Simulator _simulator;

	private UnitRenderer[] _unitRenderers;

	private bool _isAuto;

	private State _state;

	private Shared.Battle.Input _input;

	private int _timeStack;

	private int _enteringTroopCount;

	private _UnitRendererCreator _unitRendererCreator;

	private _ProjectileRendererCreator _projectileRendererCreator;

	private _UnitRendererUpdater _unitRendererUpdater;

	private _TurnUiUpdater _turnUiUpdater;

	protected UISkillView _uiSkillView;

	protected List<int> candidateUnitIdxs = new List<int>();

	private void Start()
	{
		Time.timeScale = 1f;
		_HideTroopAnchor(lhsTroopAnchor);
		_HideTroopAnchor(rhsTroopAnchor);
		int index = UnityEngine.Random.Range(0, bgmList.Count);
		SoundPlayer.PlayBGM(bgmList[index]);
		_SetTerrainTheme("Land");
		_SetTerrainScrollSpeed(ConstValue.battleTerrainScrollSpeed);
		_battleData = BattleData.Get();
		if (_battleData != null)
		{
			_battleData = null;
		}
		_timeGroupController = UnityEngine.Object.FindObjectOfType<SWP_TimeGroupController>();
		_splitScreenManager = UnityEngine.Object.FindObjectOfType<SplitScreenManager>();
		_unitRendererCache = UnityEngine.Object.FindObjectOfType<UnitRendererCache>();
		_cutInEffectCache = UnityEngine.Object.FindObjectOfType<CutInEffectCache>();
		_projectileRendererCache = UnityEngine.Object.FindObjectOfType<ProjectileRendererCache>();
		Regulation rg = Regulation.FromLocalResources();
		_simulator = _CreateSimulator(_battleData, rg);
		_unitRenderers = new UnitRenderer[_simulator.unitCount];
		_isAuto = false;
		_state = State.Unknown;
		_input = null;
		_timeStack = 0;
		_unitRendererCreator = _UnitRendererCreator.Create(this);
		_projectileRendererCreator = _ProjectileRendererCreator.Create(this);
		_unitRendererUpdater = _UnitRendererUpdater.Create(this);
		ui.onClick = _OnClickMain;
		ui.SetRemainTime(GetRemainedTime());
		_simulator.AccessFrame(_unitRendererCreator);
		StartCoroutine(_PlayOpeningAnimation());
		if (Application.isPlaying)
		{
			_uiSkillView = Manager<UISkillView>.GetInstance();
			_uiSkillView._Click += OnSelectSkill;
		}
	}

	public float GetRemainedTime()
	{
		Frame frame = _simulator.frame;
		int timeLimit = _simulator.record.option.timeLimit;
		return (float)(timeLimit - frame.time) / 1000f;
	}

	private string _VerifyBattle(Record record)
	{
		return record.result.checksum;
	}

	private void _UpdateCutInEffectPrefab()
	{
		string text = "SceneRoot/UI Root/Battle/CutInEffect/Prefab";
		Transform transform = GameObject.Find(text).transform;
		if (transform == null || transform.transform.childCount == 0)
		{
			return;
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				transform = child;
				break;
			}
		}
		if (transform == null)
		{
			return;
		}
		Animation component = transform.GetComponent<Animation>();
		if (component == null)
		{
			return;
		}
		bool flag = component.clip.name.EndsWith("R");
		SplitScreenManager splitScreenManager = UnityEngine.Object.FindObjectOfType<SplitScreenManager>();
		Transform transform2 = transform.transform.Find("Camera").transform;
		Transform transform3 = transform.transform.Find("SplitLine").transform;
		if (!(splitScreenManager == null) && !(transform2 == null) && !(transform3 == null))
		{
			Transform transform4 = splitScreenManager.left.camera.transform;
			if (flag)
			{
				transform4 = splitScreenManager.right.camera.transform;
			}
			Transform transform5 = GameObject.Find("SceneRoot/Main Camera/SplitLine").transform;
			transform4.localPosition = transform2.localPosition;
			transform4.localEulerAngles = transform2.localEulerAngles;
			transform5.localPosition = transform3.localPosition;
			transform5.localEulerAngles = transform3.localEulerAngles;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			_UpdateCutInEffectPrefab();
			return;
		}
		ui.SetRemainTime(GetRemainedTime());
		switch (_state)
		{
		case State.Unknown:
		case State.Opening:
		case State.CutIn:
		case State.Result:
			return;
		}
		if (_enteringTroopCount > 0)
		{
			return;
		}
		if (_simulator.isEnded)
		{
			if (_state == State.Playing || _state == State.InputWait)
			{
				StartCoroutine(_PlayResultAnimation(_simulator.result));
				string empty = string.Empty;
				for (int i = 0; i < 1; i++)
				{
					empty = _VerifyBattle(_simulator.record);
				}
			}
			return;
		}
		if (!_TryPickTarget() || _isAuto)
		{
		}
		Shared.Battle.Input input = _GetCorrectedInput();
		Shared.Battle.Input rhsInput = null;
		if (!_isAuto && _state == State.InputWait && input == null)
		{
			return;
		}
		if (input != null)
		{
			_state = State.Playing;
		}
		float deltaTime = Time.deltaTime;
		_timeStack += (int)(deltaTime * 1000f);
		bool flag = false;
		while (!flag && _timeStack >= 66)
		{
			_timeStack -= 66;
			if (_unitRendererUpdater.delay > 0f)
			{
				_unitRendererUpdater.delay -= 66f;
				continue;
			}
			_simulator.Step(input, rhsInput);
			_input = null;
			Frame frame = _simulator.frame;
			_simulator.AccessFrame(_unitRendererCreator);
			if (_TryPlayTroopEntryAnimation(_unitRendererCreator))
			{
				flag = true;
			}
			_simulator.AccessFrame(_projectileRendererCreator);
			_simulator.AccessFrame(_unitRendererUpdater);
			if (_simulator.isEnded)
			{
				flag = true;
			}
			_UpdateTurnUi(null);
			if (frame.isWaitingLhsInput)
			{
				_state = State.InputWait;
				flag = true;
			}
			if (eCutInState == E_CUTIN_STATE.IDLE && _unitRendererUpdater.cutInEffect != null)
			{
				StartCoroutine(_PlayCutInEffect());
			}
		}
	}

	private IEnumerator _PlayCutInEffect()
	{
		eCutInState = E_CUTIN_STATE.PLAYING;
		yield return null;
		SplitScreenManager ssm = _splitScreenManager;
		CutInEffect cie = _unitRendererUpdater.cutInEffect;
		GameObject black = cie.transform.Find("Black").gameObject;
		string parentPath = "SceneRoot/UI Root/Battle/CutInEffect/" + cie.side;
		cie.transform.parent = GameObject.Find(parentPath).transform;
		cie.transform.localScale = Vector3.one;
		cie.unitRenderer.SetRenderQueueForCutIn();
		Vector3 prevUnitScale = cie.unitRenderer.transform.localScale;
		if (cie.isDefault)
		{
			cie.unitRenderer.transform.localScale *= 1.3f;
		}
		GameObject unitPanel = GameObject.Find("SceneRoot/UI Root/Battle/UnitPanel");
		unitPanel.SetActive(value: false);
		Transform cieCamera = cie.transform.Find("Camera");
		Transform srcCamera = ssm.left.camera.transform;
		if (cie.side == CutInEffect.Side.Right)
		{
			srcCamera = ssm.right.camera.transform;
		}
		int TargetSlot = 7;
		Vector3 cameraOffSet = Vector3.zero;
		int slotIdx = cie.unitRenderer.unitIdx % 9;
		Transform cieSplitLine = cie.transform.Find("SplitLine");
		Transform srcSplitLine = GameObject.Find("SceneRoot/Main Camera/SplitLine").transform;
		while (!cie.isEnded)
		{
			srcCamera.localPosition = cieCamera.localPosition + cameraOffSet;
			srcCamera.localEulerAngles = cieCamera.localEulerAngles;
			srcSplitLine.localPosition = cieSplitLine.localPosition;
			srcSplitLine.localEulerAngles = cieSplitLine.localEulerAngles;
			ssm.left.black.SetActive(black.activeSelf);
			ssm.right.black.SetActive(black.activeSelf);
			yield return null;
		}
		yield return null;
		srcCamera.localPosition = Vector3.zero;
		srcCamera.localEulerAngles = Vector3.zero;
		srcSplitLine.localPosition = Vector3.zero;
		srcSplitLine.localEulerAngles = Vector3.zero;
		unitPanel.SetActive(value: true);
		ssm.left.black.SetActive(value: false);
		ssm.right.black.SetActive(value: false);
		cie.unitRenderer.transform.localScale = prevUnitScale;
		cie.unitRenderer.ResetRenderQueue();
		cie.transform.parent = null;
		UnityEngine.Object.Destroy(cie.gameObject);
		_unitRendererUpdater.cutInEffect = null;
		eCutInState = E_CUTIN_STATE.IDLE;
	}

	private void _UpdateTurnUi(Shared.Battle.Input input)
	{
		if (_simulator.option.playMode != Option.PlayMode.RealTime)
		{
			_TurnUiUpdater turnUiUpdater = new _TurnUiUpdater();
			turnUiUpdater.ui = ui;
			turnUiUpdater.renderers = _unitRenderers;
			turnUiUpdater.input = input;
			_simulator.AccessFrame(turnUiUpdater);
		}
	}

	private void _HideTroopAnchor(Transform troopAnchor)
	{
		for (int i = 0; i < troopAnchor.childCount; i++)
		{
			Transform child = troopAnchor.GetChild(i);
			Renderer[] componentsInChildren = child.GetComponentsInChildren<Renderer>(includeInactive: true);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
	}

	private IEnumerator _PlayOpeningAnimation()
	{
		_state = State.Opening;
		if (sceneAnimator != null)
		{
			sceneAnimator.Play("Opening");
		}
		float playTime = 5f;
		StartCoroutine(_PlayTroopEntryAnimation(lhsTroopAnchor, 10f, 0f, playTime));
		StartCoroutine(_PlayTroopEntryAnimation(rhsTroopAnchor, 10f, 0f, playTime));
		float speed = ConstValue.battleTerrainScrollSpeed;
		StartCoroutine(_ScrollSpeedTo(0.25f * speed, speed, playTime));
		yield return new WaitForSeconds(playTime);
		sceneAnimator.enabled = false;
		_state = State.Playing;
	}

	private IEnumerator _ScrollSpeedTo(float from, float to, float duration)
	{
		yield return null;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float speed = Mathf.Lerp(from, to, elapsedTime / duration);
			_SetTerrainScrollSpeed(speed);
			yield return null;
		}
	}

	private IEnumerator _PlayResultAnimation(Result result)
	{
		_state = State.Result;
		yield return new WaitForSeconds(2f);
		Time.timeScale = 1f;
		bool isWin = _simulator.result.winSide < 0;
		UIBattleResult uiResult = UIManager.instance.battle.BattleResult;
		AudioClip audioClip = loseAudioClip;
		if (_battleData != null)
		{
			_battleData.isWin = result.IsWin;
		}
		if (isWin)
		{
			if (_battleData != null)
			{
				_battleData.isWin = result.IsWin;
			}
			audioClip = winAudioClip;
		}
		SoundPlayer.PlaySE(audioClip);
		UISetter.SetActive(ui.main, active: false);
		uiResult.onClick = _OnClickBattleResult;
		uiResult.root.SetActive(value: true);
	}

	private void _OnClickBattleResult(GameObject sender)
	{
		string text = sender.name;
		if (text == "GiveUp")
		{
			Loading.Load(Loading.WorldMap);
		}
		else if (text == "OccupyAfterOrganize")
		{
			Loading.Load(Loading.WorldMap);
		}
	}

	private bool _TryPickTarget()
	{
		if (!UnityEngine.Input.GetMouseButtonDown(0))
		{
			return false;
		}
		SplitScreenManager splitScreenManager = _splitScreenManager;
		SplitScreenManager.PickingResult pickingResult = splitScreenManager.PickObject(UnityEngine.Input.mousePosition);
		if (pickingResult == null)
		{
			return false;
		}
		string text = pickingResult.target.name;
		if (!text.StartsWith("Unit-"))
		{
			return false;
		}
		int turnUnitIndex = _simulator.frame.turnUnitIndex;
		int selectedSkillIdx = Manager<UISkillView>.GetInstance().GetSelectedSkillIdx();
		int targetIndex = int.Parse(text.Substring("Unit-".Length));
		_input = new Shared.Battle.Input(turnUnitIndex, selectedSkillIdx, targetIndex);
		return true;
	}

	private Shared.Battle.Input _GetCorrectedInput()
	{
		_InputCorrector inputCorrector = new _InputCorrector();
		inputCorrector.input = ((_input != null) ? Shared.Battle.Input.Copy(_input) : null);
		inputCorrector.isAuto = _isAuto;
		_simulator.AccessFrame(inputCorrector);
		return inputCorrector.input;
	}

	private bool _TryPlayTroopEntryAnimation(_UnitRendererCreator urc)
	{
		bool result = false;
		if (urc.createdLhsTroopIndex >= 0)
		{
			StartCoroutine(_PlayTroopEntryAnimation(lhsTroopAnchor, 10f, 2f, 2f));
			result = true;
		}
		if (urc.createdRhsTroopIndex >= 0)
		{
			StartCoroutine(_PlayTroopEntryAnimation(rhsTroopAnchor, 10f, 2f, 2f));
			result = true;
		}
		return result;
	}

	private IEnumerator _PlayTroopEntryAnimation(Transform anchor, float entryDistance, float delay, float playTime)
	{
		AnimationCurve curve = troopEntryAnimationCurve;
		float elapsedTime = 0f;
		bool isEnded = false;
		_enteringTroopCount++;
		while (!isEnded)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > playTime)
			{
				isEnded = true;
			}
			if (isEnded)
			{
				elapsedTime = playTime;
			}
			float z2 = 0f - entryDistance;
			float t = elapsedTime / playTime;
			z2 += entryDistance * curve.Evaluate(t);
			for (int i = 0; i < anchor.childCount; i++)
			{
				Transform child = anchor.GetChild(i);
				if (child.childCount != 0)
				{
					int index = child.childCount - 1;
					UnitRenderer component = child.GetChild(index).GetComponent<UnitRenderer>();
					Vector3 localPosition = component.transform.localPosition;
					localPosition.z = z2;
					component.transform.localPosition = localPosition;
					if (isEnded)
					{
						component.BeginDummyMove();
					}
				}
			}
			yield return null;
		}
		_enteringTroopCount--;
	}

	private void _SetTerrainTheme(string theme)
	{
		TerrainScroller[] array = UnityEngine.Object.FindObjectsOfType<TerrainScroller>();
		TerrainScroller[] array2 = array;
		foreach (TerrainScroller terrainScroller in array2)
		{
			terrainScroller.theme = theme;
		}
	}

	private void _SetTerrainScrollSpeed(float speed)
	{
		TerrainScroller[] array = UnityEngine.Object.FindObjectsOfType<TerrainScroller>();
		TerrainScroller[] array2 = array;
		foreach (TerrainScroller terrainScroller in array2)
		{
			terrainScroller.speed = speed;
		}
	}

	private Simulator _CreateSimulator(BattleData bd, Regulation rg)
	{
		EBattleType battleType = EBattleType.Undefined;
		List<string> battleItemDrks = new List<string>();
		List<Troop> list = null;
		List<Troop> list2 = null;
		if (bd != null)
		{
			throw new NotImplementedException();
		}
		string[] unitNames = new string[4] { "104", "103", "303", "405" };
		list = _CreateTestTroops("Army", new string[1] { "5001" }, unitNames, 1, 1, 0, 0);
		list2 = _CreateTestTroops("Army", new string[1] { "5002" }, unitNames, 1, 1, 0, 0);
		int randomSeed = UnityEngine.Random.Range(0, 30000);
		InitState initState = InitState.Create(battleType, list, list2, battleItemDrks, randomSeed);
		return Simulator.Create(rg, initState);
	}

	private void _OnClickMain(GameObject sender)
	{
		string text = sender.name;
		Frame frame = _simulator.frame;
		float num = -1f;
		Shared.Battle.Input input = null;
		switch (text)
		{
		case "Speed-x1":
			num = 1f;
			break;
		case "Speed-x2":
			num = 2f;
			break;
		case "Speed-x4":
			num = 4f;
			break;
		case "Auto":
			_isAuto = true;
			break;
		case "Manual":
			_isAuto = false;
			break;
		}
		if (num > 0f)
		{
			Time.timeScale = num;
			ui.SelectSpeed(text);
		}
		ui.SelectAutoBattle(_isAuto);
		if (input != null)
		{
			_input = input;
		}
		if (!_isAuto && _state == State.InputWait && !_simulator.frame.isWaitingInput)
		{
			_state = State.Playing;
		}
	}

	private void OnSelectSkill(int skillIdx)
	{
		Frame frame = _simulator.frame;
		int turnUnitIndex = frame.turnUnitIndex;
		Skill skill = frame.units[turnUnitIndex].skills[skillIdx];
		SkillDataRow skillDataRow = _simulator.regulation.skillDtbl[skill.dri];
		if (skill.sp < skillDataRow.maxSp)
		{
			Shared.Logger.Log("Need sp point");
			return;
		}
		for (int i = 0; i < candidateUnitIdxs.Count; i++)
		{
			int num = candidateUnitIdxs[i];
			UnitRenderer unitRenderer = _unitRenderers[num];
			if (!(unitRenderer == null))
			{
				unitRenderer.SetAttackTargetCandidate(isTarget: false);
			}
		}
		candidateUnitIdxs = frame.FindSkillTargetCandidates(_simulator.regulation, frame.turnUnitIndex, skillIdx);
		for (int j = 0; j < candidateUnitIdxs.Count; j++)
		{
			int num2 = candidateUnitIdxs[j];
			UnitRenderer unitRenderer2 = _unitRenderers[num2];
			if (!(unitRenderer2 == null))
			{
				unitRenderer2.SetAttackTargetCandidate(isTarget: true);
			}
		}
	}

	private List<Troop> _CreateTestTroops(string branch, string[] commanderNames, string[] unitNames, int minTroopCount, int maxTroopCount, int minSlotCount, int maxSlotCount)
	{
		int num = UnityEngine.Random.Range(minTroopCount, maxTroopCount + 1);
		int num2 = UnityEngine.Random.Range(minSlotCount, maxSlotCount + 1);
		List<Troop> list = new List<Troop>();
		for (int i = 0; i < num; i++)
		{
			Troop troop = new Troop();
			troop._slots = new List<Troop.Slot>();
			for (int j = 0; j < 9; j++)
			{
				Troop.Slot slot = new Troop.Slot();
				if (j >= 3 && j - 3 < num2 - 1)
				{
					int num3 = UnityEngine.Random.Range(0, unitNames.Length);
					slot.id = unitNames[num3];
					slot.health = int.MaxValue;
				}
				troop._slots.Add(slot);
			}
			list.Add(troop);
		}
		return list;
	}
}
