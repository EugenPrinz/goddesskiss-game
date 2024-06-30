using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator
{
	public class RoomView : MonoBehaviour
	{
		public GameObject grid;

		public SpriteRenderer wall;

		public SpriteRenderer ground;

		public List<Tiles> tiles;

		private DormitoryData _data;

		private string _resourceName;

		private void Awake()
		{
			_data = SingletonMonoBehaviour<DormitoryData>.Instance;
		}

		private void OnDestroy()
		{
			if (!string.IsNullOrEmpty(_resourceName))
			{
				AssetBundleManager.DeleteAssetBundle(_resourceName + ".assetbundle");
			}
		}

		private void OnEnable()
		{
			Message.AddListener("Room.Update.Mode", InvalidMode);
			Message.AddListener("Room.Update.WallPaper", InvalidWallPaper);
		}

		private void OnDisable()
		{
			Message.RemoveListener("Room.Update.Mode", InvalidMode);
			Message.RemoveListener("Room.Update.WallPaper", InvalidWallPaper);
		}

		private void InvalidMode()
		{
			grid.SetActive(SingletonMonoBehaviour<DormitoryData>.Instance.isEditMode);
		}

		private void InvalidWallPaper()
		{
			if (!string.IsNullOrEmpty(_resourceName))
			{
				ground.sprite = null;
				AssetBundleManager.DeleteAssetBundle(_resourceName + ".assetbundle");
				Resources.UnloadUnusedAssets();
			}
			DormitoryWallpaperDataRow dormitoryWallpaperDataRow = (DormitoryWallpaperDataRow)ItemDBLoader.Load(EDormitoryItemType.Wallpaper, _data.room.wallpaper);
			_resourceName = dormitoryWallpaperDataRow.resource;
			Sprite sprite = null;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(_resourceName + ".assetbundle", ECacheType.Texture));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(_resourceName + ".assetbundle");
			if (assetBundle != null)
			{
				sprite = assetBundle.LoadAsset<Sprite>(_resourceName + ".jpg");
			}
			if (sprite == null)
			{
				sprite = (Sprite)Resources.Load("Prefabs/Cache/Dormitory/Rooms/" + _resourceName, typeof(Sprite));
			}
			ground.sprite = sprite;
		}

		public void Init()
		{
			InvalidWallPaper();
			InvalidMode();
		}
	}
}
