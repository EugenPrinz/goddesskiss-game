using System.Collections.Generic;
using RoomDecorator.Data;
using Shared.Regulation;
using UnityEngine;
using UnityEngine.AI;

namespace RoomDecorator.Model
{
	public abstract class Furniture : BaseUnit
	{
		public Transform left;

		public Transform right;

		[HideInInspector]
		public GridObject grid;

		[HideInInspector]
		public Direction direction;

		[HideInInspector]
		public bool fixedSkin;

		[HideInInspector]
		public DormitoryDecorationDataRow data;

		private List<GameObject> _blocks;

		protected Vector3 _forwardDirection;

		public HistoricalData previous { get; private set; }

		public abstract void Rotate();

		public abstract void Rotate(Direction dir);

		public abstract void SetColor(Color color);

		public void SetGridActive(bool value)
		{
			grid.SetActive(value);
		}

		public void SetGridColor(Color color)
		{
			grid.SetColor(color);
		}

		public virtual bool CanAttachCharacter(RoCharacter data)
		{
			return false;
		}

		public virtual AttachCharacterPoint GetAttachAblePoint()
		{
			return null;
		}

		public virtual bool AttachCharacter(Character character)
		{
			return false;
		}

		public virtual void AttachCharacter(AttachCharacterPoint point, Character character)
		{
		}

		public virtual void DetachCharacter(Character character)
		{
		}

		protected virtual void DetachAllCharacter()
		{
		}

		protected void OnDestroy()
		{
			AssetBundleManager.DeleteAssetBundle(base.name + ".assetbundle");
		}

		public void Set(DormitoryDecorationDataRow data)
		{
			this.data = data;
			width = data.xSize;
			length = data.ySize;
			fixedSkin = data.fixedSkin == 1;
		}

		public void Move(Tile tile)
		{
			base.transform.position = tile.transform.position;
			base.origin = tile;
		}

		public void Place(List<Tile> tiles)
		{
			base.tiles = tiles;
			base.tiles.ForEach(delegate(Tile tile)
			{
				tile.isBlock = true;
				tile.aboveObject = this;
			});
			sortingLayerName = "Default";
			previous = new HistoricalData(base.origin, direction);
			Block(tiles);
			Vector3 lhs = right.position - left.position;
			Vector3 rhs = new Vector3(0f, 0f, base.transform.localScale.x);
			_forwardDirection = Vector3.Cross(lhs, rhs);
			grid.sortingOrder = 2;
		}

		public void Unplaced()
		{
			tiles.ForEach(delegate(Tile tile)
			{
				tile.isBlock = false;
				tile.aboveObject = null;
			});
			tiles = new List<Tile>();
			sortingLayerName = "Preview";
			UnBlock();
			DetachAllCharacter();
			grid.sortingOrder = 3;
		}

		private void Block(List<Tile> tiles)
		{
			_blocks = new List<GameObject>();
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.transform.SetParent(SingletonMonoBehaviour<AIManager>.Instance.rootNavMesh);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3((float)width - 0.25f, 1f, (float)length - 0.25f);
			gameObject.transform.localPosition = new Vector3(tiles[0].x, 0.5f, tiles[0].y);
			gameObject.transform.localPosition += new Vector3(0.5f * (float)(width - 1), 0f, 0.5f * (float)(length - 1));
			gameObject.AddComponent<NavMeshObstacle>().carving = true;
			gameObject.GetComponent<Renderer>().enabled = false;
			_blocks.Add(gameObject);
		}

		private void UnBlock()
		{
			if (_blocks != null)
			{
				_blocks.ForEach(delegate(GameObject block)
				{
					Object.DestroyImmediate(block);
				});
				_blocks = null;
			}
		}

		public override bool OrderCompare(Furniture target)
		{
			return Sorter.OrderCompare(this, target);
		}

		public override bool OrderCompare(Character target)
		{
			if (left == null || right == null)
			{
				return false;
			}
			if (target.origin.aboveObject == this)
			{
				return Vector3.Dot(_forwardDirection, target.transform.position - left.position) < 0f;
			}
			return Sorter.OrderCompare(this, target);
		}
	}
}
