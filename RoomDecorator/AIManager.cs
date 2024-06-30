using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using RoomDecorator.Model;
using UnityEngine;
using UnityEngine.AI;

namespace RoomDecorator
{
	public class AIManager : SingletonMonoBehaviour<AIManager>
	{
		public Transform rootNavMesh;

		public GameObject sourceAgent;

		public GameObject sourceCharacter;

		private Sorter _sorter;

		private Tiles _tiles;

		private List<Character> _characters = new List<Character>();

		private bool _isEditMode;

		private void Start()
		{
			_sorter = SingletonMonoBehaviour<Sorter>.Instance;
			_tiles = SingletonMonoBehaviour<GridManager>.Instance.GetTiles();
		}

		private void OnEnable()
		{
			Message.AddListener("Room.Update.Mode", InvalidMode);
			Message.AddListener<RoCharacter>("Room.Add.Character", AddCharacter);
			Message.AddListener<string>("Room.Remove.Character", RemoveCharacter);
			Message.AddListener<string>("Chr.Update.Costume", InvalidCharacterCostume);
			Message.AddListener<string>("Chr.Update.RewardRemain", InvalidCharacterRewardPoint);
		}

		private void OnDisable()
		{
			Message.RemoveListener("Room.Update.Mode", InvalidMode);
			Message.RemoveListener<RoCharacter>("Room.Add.Character", AddCharacter);
			Message.RemoveListener<string>("Room.Remove.Character", RemoveCharacter);
			Message.RemoveListener<string>("Chr.Update.Costume", InvalidCharacterCostume);
			Message.RemoveListener<string>("Chr.Update.RewardRemain", InvalidCharacterRewardPoint);
		}

		private void InvalidMode()
		{
			_isEditMode = SingletonMonoBehaviour<DormitoryData>.Instance.isEditMode;
			if (!_isEditMode)
			{
				UICamera uICamera = UICamera.list[1];
				uICamera.eventReceiverMask = (int)uICamera.eventReceiverMask | LayerMask.GetMask("Character");
			}
			else
			{
				UICamera uICamera2 = UICamera.list[1];
				uICamera2.eventReceiverMask = (int)uICamera2.eventReceiverMask ^ ((int)UICamera.list[1].eventReceiverMask & LayerMask.GetMask("Character"));
			}
		}

		private void InvalidCharacterCostume(string cid)
		{
			int index = _characters.FindIndex((Character x) => x.id == cid);
			_characters[index].InvalidCostume();
			Resources.UnloadUnusedAssets();
		}

		private void InvalidCharacterRewardPoint(string cid)
		{
			int index = _characters.FindIndex((Character x) => x.id == cid);
			_characters[index].InvalidRewardPoint();
		}

		public void Init()
		{
			_tiles = SingletonMonoBehaviour<GridManager>.Instance.GetTiles();
			InitCharacter();
		}

		private void InitCharacter()
		{
			ReleaseAll();
			Dictionary<string, RoCharacter>.Enumerator enumerator = SingletonMonoBehaviour<DormitoryData>.Instance.room.characters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AddCharacter(enumerator.Current.Value);
			}
		}

		private void AddCharacter(RoCharacter data)
		{
			_characters.Add(CreateCharacter(data));
		}

		private void RemoveCharacter(string cid)
		{
			int index = _characters.FindIndex((Character x) => x.id == cid);
			NavMeshAgent agent = _characters[index].agent;
			Object.DestroyImmediate(_characters[index].gameObject);
			Object.DestroyImmediate(agent.gameObject);
			_characters.RemoveAt(index);
			_sorter.SortAll();
			Resources.UnloadUnusedAssets();
		}

		private Character CreateCharacter(RoCharacter data)
		{
			Tile randomTile = _tiles.GetRandomTile();
			NavMeshAgent component = Object.Instantiate(sourceAgent).GetComponent<NavMeshAgent>();
			component.transform.parent = rootNavMesh;
			component.transform.position = new Vector3(randomTile.transform.position.x, 0f, randomTile.transform.position.y * 2f);
			component.gameObject.SetActive(value: true);
			Character component2 = Object.Instantiate(sourceCharacter).GetComponent<Character>();
			component2.transform.parent = _sorter.transform;
			component2.transform.position = randomTile.transform.position;
			component2.gameObject.SetActive(value: true);
			component2.Init(component, data);
			component2.UpdateTile(randomTile);
			_sorter.SortUnit(component2);
			return component2;
		}

		private void ReleaseAll()
		{
			for (int i = 0; i < _characters.Count; i++)
			{
				Object.DestroyImmediate(_characters[i].gameObject);
			}
			_characters.Clear();
		}
	}
}
