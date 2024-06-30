using UnityEngine;

namespace RoomDecorator.Model
{
	public class Tile : MonoBehaviour
	{
		[SerializeField]
		public bool isBlock;

		public BaseUnit aboveObject;

		public int x { get; private set; }

		public int y { get; private set; }

		public void Set(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}
}
