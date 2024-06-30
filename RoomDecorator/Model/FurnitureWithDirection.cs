using UnityEngine;

namespace RoomDecorator.Model
{
	public class FurnitureWithDirection : Furniture
	{
		public GameObject[] ListDirectionItem = new GameObject[4];

		public override void Rotate()
		{
			direction = (Direction)((int)(direction + 1) % 4);
			GameObject[] listDirectionItem = ListDirectionItem;
			foreach (GameObject gameObject in listDirectionItem)
			{
				gameObject.SetActive(value: false);
			}
			ListDirectionItem[(int)direction].SetActive(value: true);
			int num = width;
			width = length;
			length = num;
		}

		public override void Rotate(Direction dir)
		{
			if (Mathf.Abs(dir - direction) % 2 == 1)
			{
				int num = width;
				width = length;
				length = num;
			}
			direction = dir;
			GameObject[] listDirectionItem = ListDirectionItem;
			foreach (GameObject gameObject in listDirectionItem)
			{
				gameObject.SetActive(value: false);
			}
			ListDirectionItem[(int)dir].SetActive(value: true);
		}

		public override void SetColor(Color color)
		{
			GameObject[] listDirectionItem = ListDirectionItem;
			foreach (GameObject gameObject in listDirectionItem)
			{
				gameObject.GetComponent<SpriteRenderer>().color = color;
			}
		}
	}
}
