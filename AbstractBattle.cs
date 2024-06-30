using System;
using Cache;
using Shared.Battle;
using UnityEngine;

public abstract class AbstractBattle : MonoBehaviour
{
	[Serializable]
	public class BattleTroopViewData
	{
		public Transform troopAnchor;

		public Transform camera;

		public Transform cameraView;

		public UIShake cameraShake;

		public Transform black;

		public Transform troopCenterAnchor;

		public TerrainScroller terrainScroller;
	}

	[Serializable]
	public class ViewData
	{
		public Transform splitLine;

		public UISprite openingIcon;

		public UISprite waveIcon;

		public UILabel waveValue;

		public BattleTroopViewData leftView;

		public BattleTroopViewData rightView;
	}

	public ViewData viewData;

	public UIBattleMain ui;

	public Animator sceneAnimator;

	public AnimationCurve troopEntryAnimationCurve;

	public float defaultTimeScale = 1.2f;

	protected BattleData _battleData;

	public TimedGameObject _timeGameObject;

	[HideInInspector]
	public bool isReplayMode;

	protected SplitScreenManager _splitScreenManager;

	protected CacheWithPool _controllerCache;

	protected ProjectileCache _projectileCache;

	protected UnitCache _unitCache;

	protected TerrainCache _terrainCache;

	protected CacheWithPool _statusEffectCache;

	protected CacheWithPool _fireEffectCache;

	protected CacheWithPool _etcEffectCache;

	protected CacheWithPool _cutInEffectCache;

	protected Simulator _simulator;

	protected UnitRenderer[] _unitRenderers;

	protected bool _isPause;

	public bool _isAuto;

	protected float _timeScale;

	protected int _priorityTargetIndex;

	public Shared.Battle.Input _input;

	public int _timeStack;

	public int _enteringTroopCount;

	public Transform lhsTroopAnchor => viewData.leftView.troopAnchor;

	public Transform rhsTroopAnchor => viewData.rightView.troopAnchor;

	public virtual Simulator Simulator { get; set; }

	public virtual BattleData BattleData { get; set; }

	public virtual UnitRenderer[] UnitRenderers { get; set; }

	public virtual void PlayUnitEntry(UnitRenderer unitRenderer, float delay = 2f, float playTime = 2f)
	{
	}
}
