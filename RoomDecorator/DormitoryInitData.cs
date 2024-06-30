using RoomDecorator.Data;

namespace RoomDecorator
{
	public class DormitoryInitData
	{
		private static DormitoryInitData _instance;

		public RoDormitory.User user;

		public bool favorState;

		public RoDormitoryRoom room;

		public static DormitoryInitData Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DormitoryInitData();
				}
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		public void Set(RoDormitory.User userData)
		{
			user = userData;
		}

		public void Set(Protocols.Dormitory.FloorDetailInfo data)
		{
			favorState = false;
			room = new RoDormitoryRoom();
			room.Set(data, user.isMaster);
		}

		public void Set(Protocols.Dormitory.GetUserFloorDetailInfoResponse data)
		{
			user.uno = data.uno;
			favorState = data.favorState;
			room = new RoDormitoryRoom();
			room.Set(data, user.isMaster);
		}
	}
}
