using RoomDecorator.Data;
using RoomDecorator.Model;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UICharacter : MonoBehaviour
	{
		public CharacterView characterView;

		public UIEffectRender effectRender;

		public void Set(RoCharacter data)
		{
			if (data != null)
			{
				DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = (DormitoryHeadCostumeDataRow)data.head.data;
				characterView.InitHead(dormitoryHeadCostumeDataRow.resource);
				characterView.InitAccessory((!(dormitoryHeadCostumeDataRow.accessory != "0")) ? null : dormitoryHeadCostumeDataRow.accessory);
				DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = (DormitoryBodyCostumeDataRow)data.body.data;
				characterView.InitBody(dormitoryBodyCostumeDataRow.resource);
				characterView.HeadAnimation("idle", loop: true);
				characterView.BodyAnimation("idle", loop: true);
				effectRender.RebuildMaterial();
				Resources.UnloadUnusedAssets();
			}
		}
	}
}
