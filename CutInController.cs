using System.Collections.Generic;
using Cache;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class CutInController : Manager<CutInController>
{
	public const string leftCameraRoot = "SceneRoot/Field/Left/Anchor/DefaultCameraTransform/Camera";

	public const string rightCameraRoot = "SceneRoot/Field/Right/Anchor/DefaultCameraTransform/Camera";

	public const string leftBackRoot = "SceneRoot/Field/Left/Anchor/DefaultCameraTransform/Camera/Black";

	public const string rightBackRoot = "SceneRoot/Field/Right/Anchor/DefaultCameraTransform/Camera/Black";

	public const string splitLineRoot = "SceneRoot/Main Camera/SplitLine";

	public const string lCenterRoot = "SceneRoot/Field/Left/Anchor/Units/04";

	public const string rCenterRoot = "SceneRoot/Field/Right/Anchor/Units/04";

	public const string leftCameraViewRoot = "SceneRoot/Field/Left/Anchor/DefaultCameraTransform";

	public const string rightCameraViewRoot = "SceneRoot/Field/Right/Anchor/DefaultCameraTransform";

	public Transform leftCamera;

	public Transform rightCamera;

	public Transform splitLine;

	public Transform leftBack;

	public Transform rightBack;

	public Transform leftCameraView;

	public Transform lCenter;

	public Transform rightCameraView;

	public Transform rCenter;

	public GameObject uiUnitPanel;

	protected int playCnt;

	protected Dictionary<string, AbstractCutInObject> datas = new Dictionary<string, AbstractCutInObject>();

	public virtual bool IsActive => playCnt > 0;

	protected override void Init()
	{
		base.Init();
		playCnt = 0;
	}

	private void Start()
	{
		if (leftCamera == null)
		{
		}
		if (rightCamera == null)
		{
		}
		if (!(splitLine == null))
		{
		}
	}

	public CutInDataSet Create(Unit unit, Skill skill, E_TARGET_SIDE side, UnitRenderer owner, UnitRenderer enemy, FireEvent fireEvent)
	{
		string key = "default";
		CacheElement element = CacheManager.instance.CutInEffectCache.GetElement(skill._skillDataRow.key);
		if (element == null)
		{
			element = CacheManager.instance.CutInEffectCache.GetElement(key);
			if (element == null)
			{
				return null;
			}
		}
		CutInDataSet cutInDataSet = CacheManager.instance.CutInEffectCache.Create<CutInDataSet>(element);
		if (cutInDataSet == null)
		{
			return null;
		}
		cutInDataSet.isCommander = false;
		cutInDataSet.commanderDri = 0;
		cutInDataSet.side = side;
		cutInDataSet.unit = unit;
		cutInDataSet.skill = skill;
		cutInDataSet.owner = owner;
		cutInDataSet.enemy = enemy;
		cutInDataSet.fireEvent = fireEvent;
		cutInDataSet.name = key;
		cutInDataSet.CacheID = element.ID;
		return cutInDataSet;
	}

	public void Release(CutInDataSet cutInSet)
	{
		cutInSet.Release();
	}

	protected void PlaySkillCut(CutInDataSet cutInData)
	{
		if (cutInData.commanderDri >= 0)
		{
			UIManager.instance.battle.MainUI.skillCut.Create(cutInData);
		}
	}

	public void Play(CutInDataSet cutInDataSet)
	{
		cutInDataSet.transform.parent = UIManager.instance.battle.MainUI.cutInPanel.transform;
		PlaySkillCut(cutInDataSet);
		if (cutInDataSet.side == E_TARGET_SIDE.LEFT)
		{
			if (Option.Default.canLhsCutIn)
			{
				UISetter.SetActive(UIManager.instance.battle.MainUI.uiBattleUnitPanel.gameObject, active: false);
			}
		}
		else if (Option.Default.canRhsCutIn)
		{
			UISetter.SetActive(UIManager.instance.battle.MainUI.uiBattleUnitPanel.gameObject, active: false);
		}
		cutInDataSet._Enter = delegate
		{
			playCnt++;
		};
		cutInDataSet._Exit = delegate
		{
			Release(cutInDataSet);
			playCnt--;
			if (playCnt <= 0)
			{
				UISetter.SetActive(UIManager.instance.battle.MainUI.uiBattleUnitPanel.gameObject, active: true);
			}
		};
		cutInDataSet.StartData();
	}

	public void Play(AbstractCutInObject cutInObject)
	{
		if (datas.ContainsKey(cutInObject.ID))
		{
			datas[cutInObject.ID].Stop();
		}
		cutInObject.Play();
	}

	public void OnPlay(AbstractCutInObject cutInObject)
	{
		datas.Add(cutInObject.ID, cutInObject);
	}

	public void OnStop(AbstractCutInObject cutInObject)
	{
		datas.Remove(cutInObject.ID);
	}
}
