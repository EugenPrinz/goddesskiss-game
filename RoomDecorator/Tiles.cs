using System;
using RoomDecorator.Model;
using UnityEngine;

namespace RoomDecorator
{
	public class Tiles : MonoBehaviour
	{
		public int width = 10;

		public int length = 10;

		private Tile[,] tiles;

		private int autoIncrement = 1;

		private void Awake()
		{
			tiles = new Tile[width, length];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < length; j++)
				{
					tiles[i, j] = GenerateTile(i, j);
				}
			}
		}

		private Tile GenerateTile(int x, int y)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[4]
			{
				new Vector3(-2f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(2f, 0f, 0f),
				new Vector3(0f, -1f, 0f)
			};
			mesh.triangles = new int[6] { 1, 2, 3, 0, 1, 3 };
			GameObject gameObject = new GameObject("Tile" + autoIncrement++);
			gameObject.AddComponent<MeshFilter>().mesh = mesh;
			gameObject.AddComponent<MeshCollider>();
			gameObject.transform.SetParent(base.gameObject.transform);
			gameObject.transform.localPosition = new Vector3((x - y) * 2, x + y, 0f);
			Tile tile = gameObject.AddComponent<Tile>();
			tile.Set(x, y);
			return tile;
		}

		public Tile GetTileByCoordinate(int x, int y)
		{
			if (x < 0 || y < 0)
			{
				return null;
			}
			if (x >= width || y >= length)
			{
				return null;
			}
			return tiles[x, y];
		}

		public Tile GetTileByPoint(Vector2 point)
		{
			int num = (int)Math.Round(point.y / 2f + point.x / 4f);
			int num2 = (int)Math.Round(point.y / 2f - point.x / 4f);
			if (num < 0 || num >= width)
			{
				return null;
			}
			if (num2 < 0 || num2 >= length)
			{
				return null;
			}
			return tiles[num, num2];
		}

		public Tile GetRandomTile()
		{
			return GetTileByCoordinate(UnityEngine.Random.Range(0, width - 1), UnityEngine.Random.Range(0, length - 1));
		}
	}
}
