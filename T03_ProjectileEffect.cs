using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class T03_ProjectileEffect : MonoBehaviour
{
	public Transform lhsTroopAnchor;

	public Transform rhsTroopAnchor;

	public Transform lhsCameraAnchor;

	public Transform rhsCameraAnchor;

	public List<UnitRenderer> leftUnitList;

	public List<UnitRenderer> rightUnitList;

	public string terrainName = "land_jungle";

	private BattleData _battleData;

	private SWP_TimeGroupController _timeGroupController;

	private SplitScreenManager _splitScreenManager;

	private UnitRendererCache _unitRendererCache;

	private CutInEffectCache _cutInEffectCache;

	private ProjectileRendererCache _projectileRendererCache;

	private bool _isAuto;

	public Regulation regulation;

	private bool _isInit;

	private UnitRenderer unit;

	private IEnumerator Start()
	{
		_SetTerrainTheme(terrainName);
		_SetTerrainScrollSpeed(ConstValue.battleTerrainScrollSpeed);
		_battleData = BattleData.Get();
		if (_battleData == null)
		{
		}
		_timeGroupController = Object.FindObjectOfType<SWP_TimeGroupController>();
		_splitScreenManager = Object.FindObjectOfType<SplitScreenManager>();
		_unitRendererCache = Object.FindObjectOfType<UnitRendererCache>();
		_cutInEffectCache = Object.FindObjectOfType<CutInEffectCache>();
		_projectileRendererCache = Object.FindObjectOfType<ProjectileRendererCache>();
		_isAuto = false;
		_isInit = true;
		yield return null;
	}

	private void Update()
	{
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

	private void _OnClickBattleResult(GameObject sender)
	{
		switch (sender.name)
		{
		case "RePlay":
			Loading.Load(Loading.WorldMap);
			break;
		case "WorldMap":
			_battleData.move = EBattleResultMove.WorldMap;
			Loading.Load(Loading.WorldMap);
			break;
		case "MyTown":
			_battleData.move = EBattleResultMove.MyTown;
			Loading.Load(Loading.WorldMap);
			break;
		case "Challenge":
			_battleData.move = EBattleResultMove.Challenge;
			Loading.Load(Loading.WorldMap);
			break;
		case "NextStage":
			_battleData.move = EBattleResultMove.NextStage;
			Loading.Load(Loading.WorldMap);
			break;
		}
	}

	private void _SetTerrainTheme(string theme)
	{
		TerrainScroller[] array = Object.FindObjectsOfType<TerrainScroller>();
		TerrainScroller[] array2 = array;
		foreach (TerrainScroller terrainScroller in array2)
		{
			string gameObjectPath = Utility.GetGameObjectPath(terrainScroller.gameObject);
			if (gameObjectPath.Contains("Left"))
			{
				terrainScroller.theme = theme;
			}
			else if (gameObjectPath.Contains("Right"))
			{
				terrainScroller.theme = theme;
			}
		}
	}

	private void _SetTerrainScrollSpeed(float speed)
	{
		TerrainScroller[] array = Object.FindObjectsOfType<TerrainScroller>();
		TerrainScroller[] array2 = array;
		foreach (TerrainScroller terrainScroller in array2)
		{
			terrainScroller.speed = speed;
		}
	}
}
