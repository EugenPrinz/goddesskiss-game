using System.Collections.Generic;

namespace RoomDecorator.Event
{
	public class MessageEvent
	{
		public class Room
		{
			public const string Update_RoomList = "Room.Update.List";

			public const string Update_Name = "Room.Update.Name";

			public const string Update_Mode = "Room.Update.Mode";

			public const string Update_WallPaper = "Room.Update.WallPaper";

			public const string Update_BuildState = "Room.Update.Build";

			public const string Update_PointState = "Room.Update.PointState";

			public const string Add_Character = "Room.Add.Character";

			public const string Remove_Character = "Room.Remove.Character";
		}

		public class Shop
		{
			public const string Update = "Shop.Update";

			public const string Open_CashShop = "Shop.Open.CashShop";
		}

		public class User
		{
			public const string Update_FavorState = "User.Update.FavorState";
		}

		public class Favor
		{
			public const string Add = "Favor.Add";

			public const string Remove = "Favor.Remove";
		}

		public class Search
		{
			public class Data
			{
				public EVisitType type;

				public List<Protocols.Dormitory.SearchUserInfo> users;
			}

			public const string Get = "Search.Get.Users";
		}

		public class Inventory
		{
			public const string Update = "Inven.Update";
		}

		public class Character
		{
			public const string Get = "Chr.Get";

			public const string Update_Floor = "Chr.Update.Floor";

			public const string Update_Costume = "Chr.Update.Costume";

			public const string Update_RewardRemain = "Chr.Update.RewardRemain";
		}

		public const string Update_Goods = "Update.Goods";
	}
}
