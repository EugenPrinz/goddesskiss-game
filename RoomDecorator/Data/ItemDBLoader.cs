using System;
using Shared.Regulation;

namespace RoomDecorator.Data
{
	public static class ItemDBLoader
	{
		private static Func<string, DataRow>[] _func = new Func<string, DataRow>[7] { null, LoadDecoration, LoadDecoration, LoadWallpaper, LoadTheme, LoadCostumeBody, LoadCostumeHead };

		private static DataRow LoadCostumeHead(string id)
		{
			return RemoteObjectManager.instance.regulation.dormitoryHeadCostumeDtbl[id];
		}

		private static DataRow LoadCostumeBody(string id)
		{
			return RemoteObjectManager.instance.regulation.dormitoryBodyCostumeDtbl[id];
		}

		private static DataRow LoadDecoration(string id)
		{
			return RemoteObjectManager.instance.regulation.dormitoryDecorationDtbl[id];
		}

		private static DataRow LoadWallpaper(string id)
		{
			return RemoteObjectManager.instance.regulation.dormitoryWallPaperDtbl[id];
		}

		private static DataRow LoadTheme(string id)
		{
			return RemoteObjectManager.instance.regulation.dormitoryThemeMap[id][0];
		}

		public static DataRow Load(EDormitoryItemType type, string id)
		{
			return _func[(int)type](id);
		}
	}
}
