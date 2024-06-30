using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using Shared.Regulation;

namespace RoomDecorator
{
	public class DormitoryData : SingletonMonoBehaviour<DormitoryData>
	{
		public const string AtlasThemePrifix = "DormitoryTheme_";

		public const string AtlasCostumePrifix = "DormitoryCostume_";

		public const string GridObjectName = "GridObject_{0}x{1}";

		public const string DEFAULT_LAYER = "Default";

		public const string PREVIEW_LAYER = "Preview";

		public const string RES_PATH_GRID_OBJECT = "Prefabs/GridObjects/";

		public const string RES_PATH_ROOM = "Prefabs/Cache/Dormitory/Rooms/";

		public const string RES_PATH_FURNITURE = "Prefabs/Cache/Dormitory/Furnitures/";

		public const string RES_PATH_CHR = "Prefabs/Cache/Dormitory/Characters/";

		public const string RES_PATH_ACC = "Prefabs/Cache/Dormitory/Accessories/";

		public const string BONE_NAME_NECK = "neck";

		public const string BONE_NAME_CAP = "cap";

		public Regulation regulation;

		public RoDormitory.Config config;

		public RoLocalUser localUser;

		public RoDormitory dormitory;

		public RoDormitory.User user;

		public RoDormitoryRoom room;

		public bool favorState;

		private bool _isEditMode;

		public Dictionary<string, RoCharacter> characters => dormitory.characters;

		public RoDormitory.InventoryData inventory => dormitory.invenData;

		public bool isEditMode
		{
			get
			{
				return _isEditMode;
			}
			set
			{
				_isEditMode = value;
				Message.Send("Room.Update.Mode");
			}
		}

		protected override void Awake()
		{
			regulation = RemoteObjectManager.instance.regulation;
			localUser = RemoteObjectManager.instance.localUser;
			dormitory = localUser.dormitory;
			config = localUser.dormitory.config;
			user = DormitoryInitData.Instance.user;
			room = DormitoryInitData.Instance.room;
			favorState = DormitoryInitData.Instance.favorState;
			base.Awake();
		}
	}
}
