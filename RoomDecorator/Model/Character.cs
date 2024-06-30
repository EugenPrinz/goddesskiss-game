using RoomDecorator.Data;
using Shared.Regulation;
using UnityEngine;
using UnityEngine.AI;

namespace RoomDecorator.Model
{
	public class Character : BaseUnit
	{
		public GameObject rewardPoint;

		[HideInInspector]
		public EMoveType moveType;

		[HideInInspector]
		public Vector3 destinationPosition;

		[HideInInspector]
		public Tile destinationTile;

		[HideInInspector]
		public BaseUnit destinationObject;

		private CharacterView _view;

		private NavMeshAgent _agent;

		private Sorter _sorter;

		private Tiles _tile;

		private RoCharacter _data;

		private EState _curState = EState.None;

		private State[] _state;

		private bool _pressed;

		private bool _isMaster;

		public override int sortingOrder
		{
			get
			{
				return base.sortingOrder;
			}
			set
			{
				_view.sortingOrder = value;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, (float)(-_view.sortingOrder) * 0.01f);
			}
		}

		public override string sortingLayerName
		{
			get
			{
				return base.sortingLayerName;
			}
			set
			{
				_view.sortingLayerName = value;
			}
		}

		public override bool isAttached => _curState == EState.Attach || _curState == EState.MoveToPosition || _curState == EState.Holding;

		public EState state => _curState;

		public CharacterView view => _view;

		public NavMeshAgent agent => _agent;

		public RoCharacter data => _data;

		public bool isPressed => _pressed;

		private void Awake()
		{
			_sorter = SingletonMonoBehaviour<Sorter>.Instance;
			_tile = SingletonMonoBehaviour<RoomDecorator>.Instance.GetTiles();
			_view = GetComponent<CharacterView>();
			_state = new State[7]
			{
				new Idle(this),
				new Move(this),
				new MoveToPosition(this),
				new Holding(this),
				new Attach(this),
				new Action(this, "wink", 1.9f, new EState[2]
				{
					EState.Idle,
					EState.Move
				}),
				new State(this)
			};
			_isMaster = SingletonMonoBehaviour<DormitoryData>.Instance.user.isMaster;
		}

		private void OnDestroy()
		{
			ChangeState(EState.None);
		}

		public void OnAttachPoint(Furniture furniture, AttachCharacterPoint point)
		{
			Attach attach = (Attach)_state[4];
			attach.target = furniture;
			attach.point = point;
			ChangeState(EState.Attach);
		}

		public void OnDetachPoint()
		{
			Attach attach = (Attach)_state[4];
			if (attach.target != null)
			{
				if (attach.target.fixedSkin)
				{
					view.EnableAccessory(enable: true);
				}
				attach.target = null;
				attach.point = null;
				Vector3 position = _agent.gameObject.transform.position;
				base.transform.position = new Vector3(position.x, position.z / 2f, base.transform.position.z);
				ChangeState(EState.Idle);
			}
			_sorter.SortUnit(this);
		}

		public void SetDirection(Vector3 direction)
		{
			if (direction.x > 0f && base.transform.localScale.x > 0f)
			{
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
			}
			else if (direction.x < 0f && base.transform.localScale.x < 0f)
			{
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
			}
		}

		public void ChangeState(EState nextState)
		{
			if (_curState != nextState)
			{
				_state[(int)_curState].OnExit();
				_curState = nextState;
				_state[(int)_curState].OnEnter();
			}
		}

		public void Init(NavMeshAgent agent, RoCharacter data)
		{
			id = data.id;
			_data = data;
			_agent = agent;
			InvalidCostume();
			InvalidRewardPoint();
			bool flag = false;
			Tile tileByPoint = _tile.GetTileByPoint(base.transform.position);
			if (tileByPoint.aboveObject != null && tileByPoint.aboveObject is Furniture)
			{
				Furniture furniture = (Furniture)tileByPoint.aboveObject;
				if (furniture.CanAttachCharacter(_data))
				{
					furniture.AttachCharacter(this);
					flag = true;
				}
			}
			if (!flag)
			{
				Vector3 position = _agent.gameObject.transform.position;
				base.transform.position = new Vector3(position.x, position.z / 2f, 0f);
				SetDirection(new Vector3(Random.Range(-1, 2), 0f, 0f));
				ChangeState(EState.Idle);
			}
		}

		public void InvalidCostume()
		{
			_renderers = null;
			DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = (DormitoryHeadCostumeDataRow)_data.head.data;
			DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = (DormitoryBodyCostumeDataRow)_data.body.data;
			_view.InitHead(dormitoryHeadCostumeDataRow.resource);
			_view.InitAccessory((!(dormitoryHeadCostumeDataRow.accessory != "0")) ? null : dormitoryHeadCostumeDataRow.accessory);
			_view.InitBody(dormitoryBodyCostumeDataRow.resource);
			ChangeState(EState.Idle);
		}

		public void InvalidRewardPoint()
		{
			rewardPoint.SetActive(_isMaster && _data.remain.GetCurrentProgress() > 1f);
		}

		public void UpdateTile(Tile tile)
		{
			if (base.origin != tile)
			{
				base.origin = tile;
			}
		}

		private void Update()
		{
			if (!(_agent == null))
			{
				if (_isMaster && !rewardPoint.activeSelf && _data.remain.GetCurrentProgress() > 1f)
				{
					rewardPoint.SetActive(value: true);
				}
				_state[(int)_curState].OnUpdate();
			}
		}

		private void OnClick()
		{
			if (_curState == EState.Idle || _curState == EState.Move || (_curState == EState.Holding && !_pressed))
			{
				ChangeState(EState.Wink);
			}
			if (_isMaster && rewardPoint.activeSelf)
			{
				RemoteObjectManager.instance.RequestGetDormitoryPoint(id);
			}
		}

		private void OnPress(bool pressed)
		{
			_pressed = pressed;
			if (_pressed)
			{
				ChangeState(EState.Holding);
			}
		}

		public override bool OrderCompare(Furniture target)
		{
			return !target.OrderCompare(this);
		}

		public override bool OrderCompare(Character target)
		{
			return base.transform.position.y < target.transform.position.y;
		}
	}
}
