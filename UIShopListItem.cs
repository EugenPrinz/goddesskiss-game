using Shared.Regulation;
using UnityEngine;

public class UIShopListItem : UIDefaultItem
{
	public UILabel targetCount;

	public UILabel bounsDesctiption;

	public GameObject selectSprite;

	public GameObject btnRoot;

	public UISprite costIcon;

	public UISprite goodsIcon;

	public UISprite commanderIcon;

	public GameObject partIcon;

	public UISprite partBG;

	public UISprite part;

	public UISprite partGrade;

	public UISprite partMark;

	public GameObject medalIcon;

	public UISprite medal;

	public UISprite medalMark;

	public GameObject disableRoot;

	public UILabel buttonLabel;

	private const string GoldIconName = "store_gold_";

	private const string GoldCostTypeName = "Goods-gold";

	private const string CashCostTypeName = "Goods-cash";

	private const string ChallengeCoinCostTypeName = "Goods-acon";

	private const string RaidCoinCostTypeName = "Goods-rcon";

	private const string GuildCoinCostTypeName = "Goods-gcon";

	private const string AnnihilationCoinCostTypeName = "Goods-ncon";

	private const string waveDuelCoinCostTypeName = "Goods-wbc";

	private const string worldDuelCoinCostTypeName = "ps-server-silver";

	public void Set(Protocols.SecretShop.ShopData data)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, data.type == ERewardType.Goods);
		UISetter.SetActive(commanderIcon, data.type == ERewardType.Commander);
		UISetter.SetActive(medalIcon, data.type == ERewardType.Medal);
		UISetter.SetActive(partIcon, data.type == ERewardType.UnitMaterial);
		if (data.costType == EPriceType.Cash)
		{
			UISetter.SetSprite(costIcon, "Goods-cash");
			UISetter.SetGameObjectName(btnRoot, $"cash_{data.id}");
		}
		else if (data.costType == EPriceType.Gold)
		{
			UISetter.SetSprite(costIcon, "Goods-gold");
			UISetter.SetGameObjectName(btnRoot, $"gold_{data.id}");
		}
		else if (data.costType == EPriceType.ChallengeCoin)
		{
			UISetter.SetSprite(costIcon, "Goods-acon");
			UISetter.SetGameObjectName(btnRoot, $"challengeCoin_{data.id}");
		}
		else if (data.costType == EPriceType.RaidCoin)
		{
			UISetter.SetSprite(costIcon, "Goods-rcon");
			UISetter.SetGameObjectName(btnRoot, $"raidCoin_{data.id}");
		}
		else if (data.costType == EPriceType.GuildCoin)
		{
			UISetter.SetSprite(costIcon, "Goods-gcon");
			UISetter.SetGameObjectName(btnRoot, $"guildCoin_{data.id}");
		}
		else if (data.costType == EPriceType.AnnihilationCoin)
		{
			UISetter.SetSprite(costIcon, "Goods-ncon");
			UISetter.SetGameObjectName(btnRoot, $"annihilationCoin_{data.id}");
		}
		else if (data.costType == EPriceType.WaveDuelCoin)
		{
			UISetter.SetSprite(costIcon, "Goods-wbc");
			UISetter.SetGameObjectName(btnRoot, $"waveDuelCoin_{data.id}");
		}
		else if (data.costType == EPriceType.WorldDuelCoin)
		{
			UISetter.SetSprite(costIcon, "ps-server-silver");
			UISetter.SetGameObjectName(btnRoot, $"worldDuelCoin_{data.id}");
		}
		UISetter.SetLabel(cost, data.cost);
		UISetter.SetLabel(count, data.count);
		UISetter.SetActive(disableRoot, data.sold == 1);
		UISetter.SetButtonEnable(btnRoot, data.sold != 1);
		UISetter.SetLabelWithLocalization(buttonLabel, (data.sold == 1) ? "1995" : "1002");
		if (data.type == ERewardType.Goods)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[data.idx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetLabel(description, Localization.Get(goodsDataRow.description));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
		}
		else if (data.type == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[data.idx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(commanderDataRow.S_Idx));
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			UISetter.SetLabel(description, Localization.Get(commanderDataRow.medalExplanation));
		}
		else if (data.type == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[data.idx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(commanderDataRow2.S_Idx));
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(description, Localization.Get(commanderDataRow2.explanation));
		}
		else if (data.type == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[data.idx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetLabel(description, Localization.Get(partDataRow.description));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
		}
	}
}
