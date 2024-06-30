using System;
using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using RoomDecorator.Model;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator
{
	public class GridManager : SingletonMonoBehaviour<GridManager>
	{
		public GameObject editBtnGroup;

		public UIButton placeBtn;

		public UIButton undoBtn;

		private Tiles _tiles;

		private Sorter _sorter;

		private Furniture _selectedFurniture;

		private bool dragging;

		private DormitoryData _data;

		private bool _isEditMode;

		private LinkedList<Furniture> _furnitures = new LinkedList<Furniture>();

		private Dictionary<int, LinkedListNode<Furniture>> _furnitureMap = new Dictionary<int, LinkedListNode<Furniture>>();

		private void Start()
		{
			_tiles = UnityEngine.Object.FindObjectOfType<Tiles>();
			_sorter = SingletonMonoBehaviour<Sorter>.Instance;
		}

		private void OnEnable()
		{
			Message.AddListener("Room.Update.Mode", InvalidMode);
		}

		private void OnDisable()
		{
			Message.RemoveListener("Room.Update.Mode", InvalidMode);
		}

		private void InvalidMode()
		{
			_isEditMode = SingletonMonoBehaviour<DormitoryData>.Instance.isEditMode;
			if (_isEditMode)
			{
				UICamera uICamera = UICamera.list[1];
				uICamera.eventReceiverMask = (int)uICamera.eventReceiverMask | LayerMask.GetMask("Furniture");
			}
			else
			{
				UICamera uICamera2 = UICamera.list[1];
				uICamera2.eventReceiverMask = (int)uICamera2.eventReceiverMask ^ ((int)UICamera.list[1].eventReceiverMask & LayerMask.GetMask("Furniture"));
			}
			UndoSelectedFurniture();
			SetGridActive(_isEditMode);
		}

		private void SetGridActive(bool value)
		{
			for (LinkedListNode<Furniture> linkedListNode = _furnitures.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.SetGridActive(_isEditMode);
			}
		}

		public void UndoSelectedFurniture()
		{
			if (!(_selectedFurniture == null))
			{
				editBtnGroup.SetActive(value: false);
				if (_selectedFurniture.previous == null)
				{
					int instanceID = _selectedFurniture.gameObject.GetInstanceID();
					LinkedListNode<Furniture> node = _furnitureMap[instanceID];
					_furnitures.Remove(node);
					_furnitureMap.Remove(instanceID);
					UnityEngine.Object.DestroyImmediate(_selectedFurniture.gameObject);
					Resources.UnloadUnusedAssets();
				}
				else
				{
					OnUndo(_selectedFurniture);
					_sorter.SortAll();
				}
			}
		}

		public void Init()
		{
			_data = SingletonMonoBehaviour<DormitoryData>.Instance;
			_tiles = SingletonMonoBehaviour<RoomDecorator>.Instance.GetTiles();
			InitFurnitures();
		}

		private void ReleaseAll()
		{
		}

		public Tiles GetTiles()
		{
			return _tiles;
		}

		private GridObject CreateGridObject(string resourceName)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/GridObjects/" + resourceName, typeof(GameObject));
			GameObject gameObject = UnityEngine.Object.Instantiate(original);
			return gameObject.GetComponent<GridObject>();
		}

		private Furniture CreateFurniture(string resourceName)
		{
			GameObject gameObject = null;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceName + ".assetbundle"));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resourceName + ".assetbundle");
			if (assetBundle != null)
			{
				gameObject = assetBundle.LoadAsset(resourceName + ".prefab") as GameObject;
			}
			if (gameObject == null)
			{
				gameObject = (GameObject)Resources.Load("Prefabs/Cache/Dormitory/Furnitures/" + resourceName, typeof(GameObject));
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.name = resourceName;
			gameObject2.transform.parent = _sorter.transform;
			return gameObject2.GetComponent<Furniture>();
		}

		private void InitFurnitures()
		{
			ReleaseAll();
			List<Protocols.Dormitory.FloorDecoInfo>.Enumerator enumerator = _data.room.decos.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CreateFurniture(enumerator.Current);
			}
			_sorter.SortAll();
		}

		private Furniture CreateFurniture(Protocols.Dormitory.FloorDecoInfo data)
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = RemoteObjectManager.instance.regulation.dormitoryDecorationDtbl[data.id];
			Tile tileByCoordinate = _tiles.GetTileByCoordinate(data.px, data.py);
			Furniture furniture = CreateFurniture(dormitoryDecorationDataRow.resource);
			furniture.id = dormitoryDecorationDataRow.id;
			furniture.Set(dormitoryDecorationDataRow);
			furniture.Move(tileByCoordinate);
			furniture.Rotate((Direction)data.rotation);
			GridObject gridObject = CreateGridObject($"GridObject_{Math.Max(dormitoryDecorationDataRow.xSize, dormitoryDecorationDataRow.ySize)}x{Math.Min(dormitoryDecorationDataRow.xSize, dormitoryDecorationDataRow.ySize)}");
			gridObject.transform.parent = furniture.transform;
			gridObject.transform.localPosition = Vector3.zero;
			gridObject.transform.localScale = new Vector3((dormitoryDecorationDataRow.xSize >= dormitoryDecorationDataRow.ySize) ? 1 : (-1), 1f, 1f);
			furniture.grid = gridObject;
			furniture.SetGridActive(_isEditMode);
			if (!OnInvalid(furniture, out var area))
			{
				furniture.Place(area);
				furniture.SetColor(Color.white);
				furniture.SetGridColor(Color.green);
			}
			LinkedListNode<Furniture> value = _furnitures.AddLast(furniture);
			_furnitureMap.Add(furniture.gameObject.GetInstanceID(), value);
			return furniture;
		}

		public void CreateFurniture(RoDormitory.Item item)
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = (DormitoryDecorationDataRow)item.data;
			Tile tileByCoordinate = _tiles.GetTileByCoordinate(5, 5);
			Furniture furniture = CreateFurniture(dormitoryDecorationDataRow.resource);
			furniture.Set(dormitoryDecorationDataRow);
			furniture.id = item.id;
			furniture.Move(tileByCoordinate);
			furniture.Rotate(Direction.South);
			GridObject gridObject = CreateGridObject($"GridObject_{Math.Max(dormitoryDecorationDataRow.xSize, dormitoryDecorationDataRow.ySize)}x{Math.Min(dormitoryDecorationDataRow.xSize, dormitoryDecorationDataRow.ySize)}");
			gridObject.transform.parent = furniture.transform;
			gridObject.transform.localPosition = Vector3.zero;
			gridObject.transform.localScale = new Vector3((dormitoryDecorationDataRow.xSize >= dormitoryDecorationDataRow.ySize) ? 1 : (-1), 1f, 1f);
			furniture.grid = gridObject;
			furniture.SetGridActive(_isEditMode);
			furniture.Unplaced();
			_selectedFurniture = furniture;
			OnEndDrag();
			LinkedListNode<Furniture> value = _furnitures.AddLast(furniture);
			_furnitureMap.Add(furniture.gameObject.GetInstanceID(), value);
		}

		private void OnPress(bool pressed)
		{
			if (pressed)
			{
				OnBeginDrag(delegate(bool isHold)
				{
					dragging = isHold;
				});
			}
			else
			{
				dragging = false;
				OnEndDrag();
			}
		}

		public void OnMessage(GameObject sender)
		{
			switch (sender.name)
			{
			case "Comfirm":
				OnPlaceFurniture(_selectedFurniture);
				_sorter.SortAll();
				editBtnGroup.SetActive(value: false);
				break;
			case "Rotate":
			{
				RotateItem(out var _);
				break;
			}
			case "Remove":
				OnRemoveFurniture(_selectedFurniture);
				_sorter.SortAll();
				editBtnGroup.SetActive(value: false);
				break;
			case "Cancel":
				OnUndo(_selectedFurniture);
				_sorter.SortAll();
				editBtnGroup.SetActive(value: false);
				break;
			}
			SoundManager.PlaySFX("BTN_Positive_001");
		}

		private void Update()
		{
			if (_isEditMode && dragging)
			{
				OnDrag();
			}
		}

		private void OnBeginDrag(Action<bool> isHold)
		{
			if (_selectedFurniture == null)
			{
				GameObject gameObject = Select((GameObject child) => child.transform.parent.GetComponent<Furniture>() != null);
				if (gameObject != null)
				{
					_selectedFurniture = gameObject.transform.parent.GetComponent<Furniture>();
					_selectedFurniture.Unplaced();
				}
				isHold(gameObject != null);
			}
			else
			{
				GameObject gameObject2 = Select((GameObject child) => child.transform.parent.GetComponent<Furniture>() != null);
				isHold(gameObject2 != null && gameObject2.transform.parent.GetComponent<Furniture>() == _selectedFurniture);
			}
		}

		private void OnDrag()
		{
			if (!(_selectedFurniture == null))
			{
				GameObject gameObject = Select((GameObject obj) => obj.GetComponent<Tile>() != null);
				if (gameObject != null)
				{
					editBtnGroup.SetActive(value: false);
					_selectedFurniture.Move(gameObject.GetComponent<Tile>());
					OnInvalid(_selectedFurniture, out var _);
				}
			}
		}

		private void OnEndDrag()
		{
			if (!(_selectedFurniture == null))
			{
				Vector3 position = Camera.main.WorldToScreenPoint(_selectedFurniture.transform.position);
				position = UICamera.mainCamera.ScreenToWorldPoint(position);
				position.z = 0f;
				editBtnGroup.transform.position = position;
				editBtnGroup.SetActive(value: true);
				placeBtn.isEnabled = !OnInvalid(_selectedFurniture, out var _);
				undoBtn.gameObject.SetActive(_selectedFurniture.previous != null);
			}
		}

		private void RotateItem(out List<Tile> area)
		{
			area = new List<Tile>();
			if (_selectedFurniture != null)
			{
				_selectedFurniture.Rotate();
				placeBtn.isEnabled = !OnInvalid(_selectedFurniture, out area);
			}
		}

		private GameObject Select(Predicate<GameObject> condition)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, LayerMask.GetMask("Default"));
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (condition(raycastHit.transform.gameObject))
				{
					return raycastHit.transform.gameObject;
				}
			}
			return null;
		}

		private void OnPlaceFurniture(Furniture furniture)
		{
			if (!(furniture == null) && !OnInvalid(furniture, out var area))
			{
				if (furniture.previous == null)
				{
					RemoteObjectManager.instance.RequestArrangeDormitoryDecoration(SingletonMonoBehaviour<DormitoryData>.Instance.room.fno, furniture.id, furniture.origin.x, furniture.origin.y, (int)furniture.direction);
				}
				else
				{
					RemoteObjectManager.instance.RequestEditDormitoryDecoration(SingletonMonoBehaviour<DormitoryData>.Instance.room.fno, furniture.previous.tile.x, furniture.previous.tile.y, furniture.origin.x, furniture.origin.y, (int)furniture.direction);
				}
				furniture.Place(area);
				furniture.SetColor(Color.white);
				furniture.SetGridColor(Color.green);
				_selectedFurniture = null;
			}
		}

		private void OnRemoveFurniture(Furniture furniture)
		{
			int instanceID = _selectedFurniture.gameObject.GetInstanceID();
			LinkedListNode<Furniture> node = _furnitureMap[instanceID];
			_furnitures.Remove(node);
			_furnitureMap.Remove(instanceID);
			if (furniture.previous != null)
			{
				RemoteObjectManager.instance.RequestRemoveDormitoryDecoration(SingletonMonoBehaviour<DormitoryData>.Instance.room.fno, furniture.previous.tile.x, furniture.previous.tile.y);
			}
			_selectedFurniture = null;
			UnityEngine.Object.DestroyImmediate(furniture.gameObject);
			Resources.UnloadUnusedAssets();
		}

		private bool OnInvalid(Furniture furniture, out List<Tile> area)
		{
			area = new List<Tile>();
			for (int i = 0; i < furniture.width; i++)
			{
				for (int j = 0; j < furniture.length; j++)
				{
					Tile tileByCoordinate = _tiles.GetTileByCoordinate(furniture.origin.x + i, furniture.origin.y + j);
					if (tileByCoordinate == null || tileByCoordinate.isBlock)
					{
						furniture.SetColor(Color.red);
						furniture.SetGridColor(Color.red);
						return true;
					}
					area.Add(tileByCoordinate);
				}
			}
			furniture.SetColor(Color.green);
			furniture.SetGridColor(Color.green);
			return false;
		}

		private void OnUndo(Furniture furniture)
		{
			if (furniture.previous != null)
			{
				furniture.Move(furniture.previous.tile);
				furniture.Rotate(furniture.previous.direction);
				OnPlaceFurniture(furniture);
			}
		}
	}
}
