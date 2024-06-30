using System.Collections.Generic;
using UnityEngine;

public class GameBillingManager : MonoBehaviour
{
	private static bool _isInited;

	public const string DIA_500 = "gk.dia.100";

	public const string DIA_1000 = "gk.dia.400";

	public const string DIA_3000 = "gk.dia.1200";

	public const string DIA_5000 = "gk.dia.2000";

	public const string DIA_10000 = "gk.dia.4000";

	public const string PACKAGE_MONTH = "gk.package.monthly";

	public const string PACKAGE_STARTER_01 = "gk.package.starter.01";

	public const string PACKAGE_STARTER_02 = "gk.package.starter.02";

	public const string PACKAGE_STARTER_03 = "gk.package.starter.03";

	public const string PACKAGE_STARTER_04 = "gk.package.starter.04";

	public const string PACKAGE_STARTER_05 = "gk.package.starter.05";

	public const string PACKAGE_STARTER_06 = "gk.package.starter.06";

	public const string PACKAGE_STARTER_07 = "gk.package.starter.07";

	public const string PACKAGE_STARTER_08 = "gk.package.starter.08";

	public const string PACKAGE_GOLD_01 = "gk.package.gold.01";

	public const string PACKAGE_INTERLEVEL = "gk.package.interlevel";

	public const string PACKAGE_HIGHLEVEL = "gk.package.highlevel";

	public const string PACKAGE_TICKET_01 = "gk.package.ticket.01";

	public const string PACKAGE_TICKET_02 = "gk.package.ticket.02";

	public const string PACKAGE_GIFT_01 = "gk.package.gift.01";

	public const string PACKAGE_GIFT_02 = "gk.package.gift.02";

	public const string PACKAGE_MONTH_02 = "gk.package.monthly02";

	public const string PACKAGE_MONTH_03 = "gk.package.monthly03";

	public const string PACKAGE_PILOT_01 = "gk.package.pilot01";

	public const string PACKAGE_PILOT_02 = "gk.package.pilot02";

	public const string PACKAGE_PILOT_03 = "gk.package.pilot03";

	public const string PACKAGE_MONTH_GOLD = "gk.package.monthly.gold";

	public const string PACKAGE_GIFT_PREMIUM = "gk.package.gift.premium";

	public const string PACKAGE_GROWTH_GOLD = "gk.package.growth.gold";

	public const string PACKAGE_ENHANCE_RED = "gk.package.enhance.red";

	public const string PACKAGE_WRING_BASIC = "gk.package.wring.basic";

	public const string PACKAGE_WRING_ADV = "gk.package.wring.adv";

	public const string PACKAGE_BALDR_01 = "gk.package.baldr01";

	public const string PACKAGE_BALDR_02 = "gk.package.baldr02";

	public const string PACKAGE_BALDR_03 = "gk.package.baldr03";

	public const string PACKAGE_GROWTH_COMMANDER = "gk.package.growth.commander";

	public const string PACKAGE_CLEAR_STAGE = "gk.package.clear.stage";

	public const string PACKAGE_STARTER_BOOSTER = "gk.package.starter.booster";

	public const string PACKAGE_NEW_PILOT_01 = "gk.package.new.pilot01";

	public const string PACKAGE_NEW_PILOT_02 = "gk.package.new.pilot02";

	public const string PACKAGE_SPECIAL_PILOT = "gk.package.special.pilot";

	public const string PACKAGE_SPECIAL_LIMIT_01 = "gk.package.special.limit01";

	public const string PACKAGE_SPECIAL_LIMIT_02 = "gk.package.special.limit02";

	public const string PACKAGE_PILOT_04 = "gk.package.pilot04";

	public const string PACKAGE_PILOT_05 = "gk.package.pilot05";

	public const string PACKAGE_PILOT_06 = "gk.package.pilot06";

	public const string PACKAGE_PILOT_07 = "gk.package.pilot07";

	public const string PACKAGE_PILOT_08 = "gk.package.pilot08";

	public const string PACKAGE_DIA_200_DIS = "gk.package.dia.200.discount";

	public const string PACKAGE_DIA_400_DIS = "gk.package.dia.400.discount";

	public const string PACKAGE_DIA_1200_DIS = "gk.package.dia.1200.discount";

	public const string PACKAGE_DIA_2000_DIS = "gk.package.dia.2000.discount";

	public const string PACKAGE_DIA_4000_DIS = "gk.package.dia.4000.discount";

	public const string PACKAGE_LEVELUP_BOOST_01 = "gk.package.lvlboost.01";

	public const string PACKAGE_LEVELUP_BOOST_02 = "gk.package.lvlboost.02";

	public const string PACKAGE_LEVELUP_BOOST_03 = "gk.package.lvlboost.03";

	public const string PACKAGE_LEVELUP_BOOST_04 = "gk.package.lvlboost.04";

	public const string PACKAGE_LEVELUP_BOOST_05 = "gk.package.lvlboost.05";

	public const string PACKAGE_DORMITORY_01 = "gk.package.dormitory01";

	public const string PACKAGE_MONTY_DORMITORY = "gk.package.monthly.dorm";

	public const string PACKAGE_LIMIT_BONUS = "gk.package.ltd.bonuspack";

	public const string PACKAGE_CRYSTAL_A = "gk.package.crystal.a";

	public const string PACKAGE_CRYSTAL_B = "gk.package.crystal.b";

	public const string PACKAGE_CRYSTAL_C = "gk.package.crystal.c";

	public const string PACKAGE_CRYSTAL_D = "gk.package.crystal.d";

	public const string PACKAGE_CRYSTAL_E = "gk.package.crystal.e";

	public const string PACKAGE_CRYSTAL_F = "gk.package.crystal.f";

	public const string PACKAGE_CRYSTAL_G = "gk.package.crystal.g";

	public const string PACKAGE_CRYSTAL_H = "gk.package.crystal.h";

	public const string PACKAGE_CRYSTAL_I = "gk.package.crystal.i";

	public const string PACKAGE_DIA_8000 = "gk.package.dia.8000";

	public const string PACKAGE_MONTH_GOLD_02 = "gk.package.monthly.gold02";

	public const string PACKAGE_MONTH_AFFECTION = "gk.package.monthly.affection";

	public const string PACKAGE_MONTH_PREMIUM_AFFECTION = "gk.package.monthly.premium.affection";

	public const string PACKAGE_5500R1 = "gk.package.5500r1";

	public const string PACKAGE_5500R2 = "gk.package.5500r2";

	public const string PACKAGE_11000R1 = "gk.package.11000r1";

	public const string PACKAGE_11000R2 = "gk.package.11000r2";

	public const string PACKAGE_11000R3 = "gk.package.11000r3";

	public const string PACKAGE_11000R4 = "gk.package.11000r4";

	public const string PACKAGE_11000R5 = "gk.package.11000r5";

	public const string PACKAGE_33000R1 = "gk.package.33000r1";

	public const string PACKAGE_33000R2 = "gk.package.33000r2";

	public const string PACKAGE_33000R3 = "gk.package.33000r3";

	public const string PACKAGE_33000R4 = "gk.package.33000r4";

	public const string PACKAGE_33000R5 = "gk.package.33000r5";

	public const string PACKAGE_55000R1 = "gk.package.55000r1";

	public const string PACKAGE_55000R2 = "gk.package.55000r2";

	public const string PACKAGE_55000R3 = "gk.package.55000r3";

	public const string PACKAGE_55000R4 = "gk.package.55000r4";

	public const string PACKAGE_55000R5 = "gk.package.55000r5";

	public const string PACKAGE_99000R1 = "gk.package.99000r1";

	public const string PACKAGE_99000R2 = "gk.package.99000r2";

	public const string PACKAGE_99000R3 = "gk.package.99000r3";

	public const string PACKAGE_99000R4 = "gk.package.99000r4";

	public const string PACKAGE_99000R5 = "gk.package.99000r5";

	public const string PACKAGE_110000R1 = "gk.package.110000r1";

	public const string PACKAGE_110000R2 = "gk.package.110000r2";

	public const string PACKAGE_110000R3 = "gk.package.110000r3";

	public const string PACKAGE_110000R4 = "gk.package.110000r4";

	public const string PACKAGE_110000R5 = "gk.package.110000r5";

	private static bool ListnersAdded;

	private static Dictionary<string, AmazonProductTemplate> availableItems;

	private static List<string> unavailableSkus;

	private static List<string> entitlements;

	private static List<SA_AmazonReceipt> listReceipts;

	public static bool isInited => _isInited;

	private static void OnGetUserDataReceived(AMN_GetUserDataResponse result)
	{
		string requestId = result.RequestId;
		string userId = result.UserId;
		string marketplace = result.Marketplace;
		string status = result.Status;
	}

	private static void OnPurchaseProductReceived(AMN_PurchaseResponse result)
	{
		if (result.isSuccess)
		{
			string requestId = result.RequestId;
			string userId = result.UserId;
			string marketplace = result.Marketplace;
			string receiptId = result.ReceiptId;
			long cancelDate = result.CancelDate;
			long purchaseDatee = result.PurchaseDatee;
			string sku = result.Sku;
			string productType = result.ProductType;
			string status = result.Status;
			RemoteObjectManager.instance.RequestCheckPaymentAmazon(sku, userId, receiptId);
		}
		else
		{
			string requestId2 = result.RequestId;
			string status2 = result.Status;
		}
	}

	private static void OnGetProductDataReceived(AMN_GetProductDataResponse result)
	{
		string requestId = result.RequestId;
		string status = result.Status;
		availableItems = AMN_Singleton<SA_AmazonBillingManager>.Instance.availableItems;
		unavailableSkus = AMN_Singleton<SA_AmazonBillingManager>.Instance.unavailableSkus;
	}

	private static void OnGetPurchaseProductsUpdatesReceived(AMN_GetPurchaseProductsUpdateResponse result)
	{
		string requestId = result.RequestId;
		string userId = result.UserId;
		string marketplace = result.Marketplace;
		string status = result.Status;
		bool hasMore = result.HasMore;
		listReceipts = AMN_Singleton<SA_AmazonBillingManager>.Instance.listReceipts;
		foreach (SA_AmazonReceipt listReceipt in listReceipts)
		{
			string sku = listReceipt.Sku;
			string productType = listReceipt.ProductType;
			string receiptId = listReceipt.ReceiptId;
			long purchaseDate = listReceipt.PurchaseDate;
			long cancelDate = listReceipt.CancelDate;
		}
	}

	public static void Purchase(string sku)
	{
		if (!entitlements.Contains(sku))
		{
			AMN_Singleton<SA_AmazonBillingManager>.Instance.Purchase(sku);
		}
	}

	private void GetUserData()
	{
		AMN_Singleton<SA_AmazonBillingManager>.Instance.GetUserData();
	}

	private void GetProductUpdates()
	{
		AMN_Singleton<SA_AmazonBillingManager>.Instance.GetProductUpdates();
	}

	private void AddEntitlement(string SKU)
	{
		if (!entitlements.Contains(SKU))
		{
			entitlements.Add(SKU);
			AMN_PlayerData.AddNewSKU(SKU);
		}
	}

	public static void init()
	{
		if (!ListnersAdded)
		{
			AndroidInAppPurchaseManager.Client.AddProduct("gk.dia.100");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.dia.400");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.dia.1200");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.dia.2000");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.dia.4000");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.03");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.04");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.05");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.06");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.07");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.08");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.gold.01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.interlevel");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.highlevel");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.ticket.01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.ticket.02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.gift.01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.gift.02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly03");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot03");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly.gold");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.gift.premium");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.growth.gold");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.enhance.red");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.wring.basic");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.wring.adv");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.baldr01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.baldr02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.baldr03");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.growth.commander");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.clear.stage");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.starter.booster");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.new.pilot01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.new.pilot02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.special.pilot");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.special.limit01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.special.limit02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot04");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot05");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot06");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot07");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.pilot08");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dia.200.discount");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dia.400.discount");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dia.1200.discount");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dia.2000.discount");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dia.4000.discount");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.lvlboost.01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.lvlboost.02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.lvlboost.03");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.lvlboost.04");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.lvlboost.05");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dormitory01");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly.dorm");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.ltd.bonuspack");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.a");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.b");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.c");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.d");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.e");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.f");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.g");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.h");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.crystal.i");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.dia.8000");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly.gold02");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly.affection");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.monthly.premium.affection");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.5500r1");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.5500r2");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.11000r1");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.11000r2");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.11000r3");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.11000r4");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.11000r5");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.33000r1");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.33000r2");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.33000r3");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.33000r4");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.33000r5");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.55000r1");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.55000r2");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.55000r3");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.55000r4");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.55000r5");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.99000r1");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.99000r2");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.99000r3");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.99000r4");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.99000r5");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.110000r1");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.110000r2");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.110000r3");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.110000r4");
			AndroidInAppPurchaseManager.Client.AddProduct("gk.package.110000r5");
			AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;
			AndroidInAppPurchaseManager.ActionProductConsumed += OnProductConsumed;
			AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
			AndroidInAppPurchaseManager.Client.Connect();
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.dia.100");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.dia.400");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.dia.1200");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.dia.2000");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.dia.4000");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.03");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.04");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.05");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.06");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.07");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.08");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.gold.01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.interlevel");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.highlevel");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.ticket.01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.ticket.02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.gift.01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.gift.02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly03");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot03");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly.gold");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.gift.premium");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.growth.gold");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.enhance.red");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.wring.basic");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.wring.adv");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.baldr01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.baldr02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.baldr03");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.growth.commander");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.clear.stage");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.starter.booster");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.new.pilot01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.new.pilot02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.special.pilot");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.special.limit01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.special.limit02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot04");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot05");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot06");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot07");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.pilot08");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dia.200.discount");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dia.400.discount");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dia.1200.discount");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dia.2000.discount");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dia.4000.discount");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.lvlboost.01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.lvlboost.02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.lvlboost.03");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.lvlboost.04");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.lvlboost.05");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dormitory01");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly.dorm");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.ltd.bonuspack");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.a");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.b");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.c");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.d");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.e");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.f");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.g");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.h");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.crystal.i");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.dia.8000");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly.gold02");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly.affection");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.monthly.premium.affection");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.5500r1");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.5500r2");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.11000r1");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.11000r2");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.11000r3");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.11000r4");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.11000r5");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.33000r1");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.33000r2");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.33000r3");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.33000r4");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.33000r5");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.55000r1");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.55000r2");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.55000r3");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.55000r4");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.55000r5");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.99000r1");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.99000r2");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.99000r3");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.99000r4");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.99000r5");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.110000r1");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.110000r2");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.110000r3");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.110000r4");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("gk.package.110000r5");
			IOSInAppPurchaseManager.OnVerificationComplete += HandleOnVerificationComplete;
			IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;
			IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
			IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.LoadStore();
			ListnersAdded = true;
		}
	}

	public static void purchase(string SKU, string payload)
	{
		AndroidInAppPurchaseManager.Client.Purchase(SKU, payload);
	}

	public static void consume(string SKU)
	{
		AndroidInAppPurchaseManager.Client.Consume(SKU);
	}

	private static void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase)
	{
		switch (purchase.SKU)
		{
		case "gk.dia.100":
		case "gk.dia.400":
		case "gk.dia.1200":
		case "gk.dia.2000":
		case "gk.dia.4000":
		case "gk.package.monthly":
		case "gk.package.starter.01":
		case "gk.package.starter.02":
		case "gk.package.starter.03":
		case "gk.package.starter.04":
		case "gk.package.starter.05":
		case "gk.package.starter.06":
		case "gk.package.starter.07":
		case "gk.package.starter.08":
		case "gk.package.gold.01":
		case "gk.package.interlevel":
		case "gk.package.highlevel":
		case "gk.package.ticket.01":
		case "gk.package.ticket.02":
		case "gk.package.gift.01":
		case "gk.package.gift.02":
		case "gk.package.monthly02":
		case "gk.package.monthly03":
		case "gk.package.pilot01":
		case "gk.package.pilot02":
		case "gk.package.pilot03":
		case "gk.package.monthly.gold":
		case "gk.package.gift.premium":
		case "gk.package.growth.gold":
		case "gk.package.enhance.red":
		case "gk.package.wring.basic":
		case "gk.package.wring.adv":
		case "gk.package.baldr01":
		case "gk.package.baldr02":
		case "gk.package.baldr03":
		case "gk.package.growth.commander":
		case "gk.package.clear.stage":
		case "gk.package.starter.booster":
		case "gk.package.new.pilot01":
		case "gk.package.new.pilot02":
		case "gk.package.special.pilot":
		case "gk.package.special.limit01":
		case "gk.package.special.limit02":
		case "gk.package.pilot04":
		case "gk.package.pilot05":
		case "gk.package.pilot06":
		case "gk.package.pilot07":
		case "gk.package.pilot08":
		case "gk.package.dia.200.discount":
		case "gk.package.dia.400.discount":
		case "gk.package.dia.1200.discount":
		case "gk.package.dia.2000.discount":
		case "gk.package.dia.4000.discount":
		case "gk.package.lvlboost.01":
		case "gk.package.lvlboost.02":
		case "gk.package.lvlboost.03":
		case "gk.package.lvlboost.04":
		case "gk.package.lvlboost.05":
		case "gk.package.dormitory01":
		case "gk.package.monthly.dorm":
		case "gk.package.ltd.bonuspack":
		case "gk.package.crystal.a":
		case "gk.package.crystal.b":
		case "gk.package.crystal.c":
		case "gk.package.crystal.d":
		case "gk.package.crystal.e":
		case "gk.package.crystal.f":
		case "gk.package.crystal.g":
		case "gk.package.crystal.h":
		case "gk.package.crystal.i":
		case "gk.package.dia.8000":
		case "gk.package.monthly.gold02":
		case "gk.package.monthly.affection":
		case "gk.package.monthly.premium.affection":
		case "gk.package.5500r1":
		case "gk.package.5500r2":
		case "gk.package.11000r1":
		case "gk.package.11000r2":
		case "gk.package.11000r3":
		case "gk.package.11000r4":
		case "gk.package.11000r5":
		case "gk.package.33000r1":
		case "gk.package.33000r2":
		case "gk.package.33000r3":
		case "gk.package.33000r4":
		case "gk.package.33000r5":
		case "gk.package.55000r1":
		case "gk.package.55000r2":
		case "gk.package.55000r3":
		case "gk.package.55000r4":
		case "gk.package.55000r5":
		case "gk.package.99000r1":
		case "gk.package.99000r2":
		case "gk.package.99000r3":
		case "gk.package.99000r4":
		case "gk.package.99000r5":
		case "gk.package.110000r1":
		case "gk.package.110000r2":
		case "gk.package.110000r3":
		case "gk.package.110000r4":
		case "gk.package.110000r5":
			consume(purchase.SKU);
			break;
		}
	}

	private static void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase)
	{
		switch (purchase.SKU)
		{
		case "gk.dia.100":
		case "gk.dia.400":
		case "gk.dia.1200":
		case "gk.dia.2000":
		case "gk.dia.4000":
		case "gk.package.monthly":
		case "gk.package.starter.01":
		case "gk.package.starter.02":
		case "gk.package.starter.03":
		case "gk.package.starter.04":
		case "gk.package.starter.05":
		case "gk.package.starter.06":
		case "gk.package.starter.07":
		case "gk.package.starter.08":
		case "gk.package.gold.01":
		case "gk.package.interlevel":
		case "gk.package.highlevel":
		case "gk.package.ticket.01":
		case "gk.package.ticket.02":
		case "gk.package.gift.01":
		case "gk.package.gift.02":
		case "gk.package.monthly02":
		case "gk.package.monthly03":
		case "gk.package.pilot01":
		case "gk.package.pilot02":
		case "gk.package.pilot03":
		case "gk.package.monthly.gold":
		case "gk.package.gift.premium":
		case "gk.package.growth.gold":
		case "gk.package.enhance.red":
		case "gk.package.wring.basic":
		case "gk.package.wring.adv":
		case "gk.package.baldr01":
		case "gk.package.baldr02":
		case "gk.package.baldr03":
		case "gk.package.growth.commander":
		case "gk.package.clear.stage":
		case "gk.package.starter.booster":
		case "gk.package.new.pilot01":
		case "gk.package.new.pilot02":
		case "gk.package.special.pilot":
		case "gk.package.special.limit01":
		case "gk.package.special.limit02":
		case "gk.package.pilot04":
		case "gk.package.pilot05":
		case "gk.package.pilot06":
		case "gk.package.pilot07":
		case "gk.package.pilot08":
		case "gk.package.dia.200.discount":
		case "gk.package.dia.400.discount":
		case "gk.package.dia.1200.discount":
		case "gk.package.dia.2000.discount":
		case "gk.package.dia.4000.discount":
		case "gk.package.lvlboost.01":
		case "gk.package.lvlboost.02":
		case "gk.package.lvlboost.03":
		case "gk.package.lvlboost.04":
		case "gk.package.lvlboost.05":
		case "gk.package.dormitory01":
		case "gk.package.monthly.dorm":
		case "gk.package.ltd.bonuspack":
		case "gk.package.crystal.a":
		case "gk.package.crystal.b":
		case "gk.package.crystal.c":
		case "gk.package.crystal.d":
		case "gk.package.crystal.e":
		case "gk.package.crystal.f":
		case "gk.package.crystal.g":
		case "gk.package.crystal.h":
		case "gk.package.crystal.i":
		case "gk.package.dia.8000":
		case "gk.package.monthly.gold02":
		case "gk.package.monthly.affection":
		case "gk.package.monthly.premium.affection":
		case "gk.package.5500r1":
		case "gk.package.5500r2":
		case "gk.package.11000r1":
		case "gk.package.11000r2":
		case "gk.package.11000r3":
		case "gk.package.11000r4":
		case "gk.package.11000r5":
		case "gk.package.33000r1":
		case "gk.package.33000r2":
		case "gk.package.33000r3":
		case "gk.package.33000r4":
		case "gk.package.33000r5":
		case "gk.package.55000r1":
		case "gk.package.55000r2":
		case "gk.package.55000r3":
		case "gk.package.55000r4":
		case "gk.package.55000r5":
		case "gk.package.99000r1":
		case "gk.package.99000r2":
		case "gk.package.99000r3":
		case "gk.package.99000r4":
		case "gk.package.99000r5":
		case "gk.package.110000r1":
		case "gk.package.110000r2":
		case "gk.package.110000r3":
		case "gk.package.110000r4":
		case "gk.package.110000r5":
			RemoteObjectManager.instance.RequestCheckPayment(purchase.packageName, purchase.SKU, purchase.time, (int)purchase.state, purchase.developerPayload, purchase.token, purchase.orderId);
			break;
		}
	}

	private static void OnProductPurchased(BillingResult result)
	{
		if (result.isSuccess)
		{
			OnProcessingPurchasedProduct(result.purchase);
		}
	}

	private static void OnProductConsumed(BillingResult result)
	{
		if (result.isSuccess)
		{
			OnProcessingConsumeProduct(result.purchase);
		}
	}

	private static void OnBillingConnected(BillingResult result)
	{
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
		if (result.isSuccess)
		{
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
		}
	}

	private static void OnRetrieveProductsFinised(BillingResult result)
	{
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
		if (result.isSuccess)
		{
			UpdateStoreData();
			_isInited = true;
		}
	}

	private static void UpdateStoreData()
	{
		foreach (GoogleProductTemplate product in AndroidInAppPurchaseManager.Client.Inventory.Products)
		{
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.dia.100"))
		{
			consume("gk.dia.100");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.dia.400"))
		{
			consume("gk.dia.400");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.dia.1200"))
		{
			consume("gk.dia.1200");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.dia.2000"))
		{
			consume("gk.dia.2000");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.dia.4000"))
		{
			consume("gk.dia.4000");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly"))
		{
			consume("gk.package.monthly");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.01"))
		{
			consume("gk.package.starter.01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.02"))
		{
			consume("gk.package.starter.02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.03"))
		{
			consume("gk.package.starter.03");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.04"))
		{
			consume("gk.package.starter.04");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.05"))
		{
			consume("gk.package.starter.05");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.06"))
		{
			consume("gk.package.starter.06");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.07"))
		{
			consume("gk.package.starter.07");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.08"))
		{
			consume("gk.package.starter.08");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.gold.01"))
		{
			consume("gk.package.gold.01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.interlevel"))
		{
			consume("gk.package.interlevel");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.highlevel"))
		{
			consume("gk.package.highlevel");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.ticket.01"))
		{
			consume("gk.package.ticket.01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.ticket.02"))
		{
			consume("gk.package.ticket.02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.gift.01"))
		{
			consume("gk.package.gift.01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.gift.02"))
		{
			consume("gk.package.gift.02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly02"))
		{
			consume("gk.package.monthly02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly03"))
		{
			consume("gk.package.monthly03");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot01"))
		{
			consume("gk.package.pilot01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot02"))
		{
			consume("gk.package.pilot02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot03"))
		{
			consume("gk.package.pilot03");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly.gold"))
		{
			consume("gk.package.monthly.gold");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.gift.premium"))
		{
			consume("gk.package.gift.premium");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.growth.gold"))
		{
			consume("gk.package.growth.gold");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.enhance.red"))
		{
			consume("gk.package.enhance.red");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.wring.basic"))
		{
			consume("gk.package.wring.basic");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.wring.adv"))
		{
			consume("gk.package.wring.adv");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.baldr01"))
		{
			consume("gk.package.baldr01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.baldr02"))
		{
			consume("gk.package.baldr02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.baldr03"))
		{
			consume("gk.package.baldr03");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.growth.commander"))
		{
			consume("gk.package.growth.commander");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.clear.stage"))
		{
			consume("gk.package.clear.stage");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.starter.booster"))
		{
			consume("gk.package.starter.booster");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.new.pilot01"))
		{
			consume("gk.package.new.pilot01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.new.pilot02"))
		{
			consume("gk.package.new.pilot02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.special.pilot"))
		{
			consume("gk.package.special.pilot");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.special.limit01"))
		{
			consume("gk.package.special.limit01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.special.limit02"))
		{
			consume("gk.package.special.limit02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot04"))
		{
			consume("gk.package.pilot04");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot05"))
		{
			consume("gk.package.pilot05");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot06"))
		{
			consume("gk.package.pilot06");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot07"))
		{
			consume("gk.package.pilot07");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.pilot08"))
		{
			consume("gk.package.pilot08");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dia.200.discount"))
		{
			consume("gk.package.dia.200.discount");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dia.400.discount"))
		{
			consume("gk.package.dia.400.discount");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dia.1200.discount"))
		{
			consume("gk.package.dia.1200.discount");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dia.2000.discount"))
		{
			consume("gk.package.dia.2000.discount");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dia.4000.discount"))
		{
			consume("gk.package.dia.4000.discount");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.lvlboost.01"))
		{
			consume("gk.package.lvlboost.01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.lvlboost.02"))
		{
			consume("gk.package.lvlboost.02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.lvlboost.03"))
		{
			consume("gk.package.lvlboost.03");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.lvlboost.04"))
		{
			consume("gk.package.lvlboost.04");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.lvlboost.05"))
		{
			consume("gk.package.lvlboost.05");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dormitory01"))
		{
			consume("gk.package.dormitory01");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly.dorm"))
		{
			consume("gk.package.monthly.dorm");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.ltd.bonuspack"))
		{
			consume("gk.package.ltd.bonuspack");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.a"))
		{
			consume("gk.package.crystal.a");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.b"))
		{
			consume("gk.package.crystal.b");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.c"))
		{
			consume("gk.package.crystal.c");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.d"))
		{
			consume("gk.package.crystal.d");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.e"))
		{
			consume("gk.package.crystal.e");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.f"))
		{
			consume("gk.package.crystal.f");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.g"))
		{
			consume("gk.package.crystal.g");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.h"))
		{
			consume("gk.package.crystal.h");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.crystal.i"))
		{
			consume("gk.package.crystal.i");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.dia.8000"))
		{
			consume("gk.package.dia.8000");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly.gold02"))
		{
			consume("gk.package.monthly.gold02");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly.affection"))
		{
			consume("gk.package.monthly.affection");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.monthly.premium.affection"))
		{
			consume("gk.package.monthly.premium.affection");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.5500r1"))
		{
			consume("gk.package.5500r1");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.5500r2"))
		{
			consume("gk.package.5500r2");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.11000r1"))
		{
			consume("gk.package.11000r1");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.11000r2"))
		{
			consume("gk.package.11000r2");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.11000r3"))
		{
			consume("gk.package.11000r3");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.11000r4"))
		{
			consume("gk.package.11000r4");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.11000r5"))
		{
			consume("gk.package.11000r5");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.33000r1"))
		{
			consume("gk.package.33000r1");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.33000r2"))
		{
			consume("gk.package.33000r2");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.33000r3"))
		{
			consume("gk.package.33000r3");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.33000r4"))
		{
			consume("gk.package.33000r4");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.33000r5"))
		{
			consume("gk.package.33000r5");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.55000r1"))
		{
			consume("gk.package.55000r1");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.55000r2"))
		{
			consume("gk.package.55000r2");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.55000r3"))
		{
			consume("gk.package.55000r3");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.55000r4"))
		{
			consume("gk.package.55000r4");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.55000r5"))
		{
			consume("gk.package.55000r5");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.99000r1"))
		{
			consume("gk.package.99000r1");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.99000r2"))
		{
			consume("gk.package.99000r2");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.99000r3"))
		{
			consume("gk.package.99000r3");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.99000r4"))
		{
			consume("gk.package.99000r4");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.99000r5"))
		{
			consume("gk.package.99000r5");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.110000r1"))
		{
			consume("gk.package.110000r1");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.110000r2"))
		{
			consume("gk.package.110000r2");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.110000r3"))
		{
			consume("gk.package.110000r3");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.110000r4"))
		{
			consume("gk.package.110000r4");
		}
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("gk.package.110000r5"))
		{
			consume("gk.package.110000r5");
		}
	}

	public static void buyItem(string productId)
	{
		ISN_Singleton<IOSInAppPurchaseManager>.Instance.BuyProduct(productId);
	}

	private static void UnlockProducts(string productIdentifier, string receipt)
	{
		switch (productIdentifier)
		{
		case "gk.dia.100":
		case "gk.dia.400":
		case "gk.dia.1200":
		case "gk.dia.2000":
		case "gk.dia.4000":
		case "gk.package.monthly":
		case "gk.package.starter.01":
		case "gk.package.starter.02":
		case "gk.package.starter.03":
		case "gk.package.starter.04":
		case "gk.package.starter.05":
		case "gk.package.starter.06":
		case "gk.package.starter.07":
		case "gk.package.starter.08":
		case "gk.package.gold.01":
		case "gk.package.interlevel":
		case "gk.package.highlevel":
		case "gk.package.ticket.01":
		case "gk.package.ticket.02":
		case "gk.package.gift.01":
		case "gk.package.gift.02":
		case "gk.package.monthly02":
		case "gk.package.monthly03":
		case "gk.package.pilot01":
		case "gk.package.pilot02":
		case "gk.package.pilot03":
		case "gk.package.monthly.gold":
		case "gk.package.gift.premium":
		case "gk.package.growth.gold":
		case "gk.package.enhance.red":
		case "gk.package.wring.basic":
		case "gk.package.wring.adv":
		case "gk.package.baldr01":
		case "gk.package.baldr02":
		case "gk.package.baldr03":
		case "gk.package.growth.commander":
		case "gk.package.clear.stage":
		case "gk.package.starter.booster":
		case "gk.package.new.pilot01":
		case "gk.package.new.pilot02":
		case "gk.package.special.pilot":
		case "gk.package.special.limit01":
		case "gk.package.special.limit02":
		case "gk.package.pilot04":
		case "gk.package.pilot05":
		case "gk.package.pilot06":
		case "gk.package.pilot07":
		case "gk.package.pilot08":
		case "gk.package.dia.200.discount":
		case "gk.package.dia.400.discount":
		case "gk.package.dia.1200.discount":
		case "gk.package.dia.2000.discount":
		case "gk.package.dia.4000.discount":
		case "gk.package.lvlboost.01":
		case "gk.package.lvlboost.02":
		case "gk.package.lvlboost.03":
		case "gk.package.lvlboost.04":
		case "gk.package.lvlboost.05":
		case "gk.package.dormitory01":
		case "gk.package.monthly.dorm":
		case "gk.package.ltd.bonuspack":
		case "gk.package.crystal.a":
		case "gk.package.crystal.b":
		case "gk.package.crystal.c":
		case "gk.package.crystal.d":
		case "gk.package.crystal.e":
		case "gk.package.crystal.f":
		case "gk.package.crystal.g":
		case "gk.package.crystal.h":
		case "gk.package.crystal.i":
		case "gk.package.dia.8000":
		case "gk.package.monthly.gold02":
		case "gk.package.monthly.affection":
		case "gk.package.monthly.premium.affection":
		case "gk.package.5500r1":
		case "gk.package.5500r2":
		case "gk.package.11000r1":
		case "gk.package.11000r2":
		case "gk.package.11000r3":
		case "gk.package.11000r4":
		case "gk.package.11000r5":
		case "gk.package.33000r1":
		case "gk.package.33000r2":
		case "gk.package.33000r3":
		case "gk.package.33000r4":
		case "gk.package.33000r5":
		case "gk.package.55000r1":
		case "gk.package.55000r2":
		case "gk.package.55000r3":
		case "gk.package.55000r4":
		case "gk.package.55000r5":
		case "gk.package.99000r1":
		case "gk.package.99000r2":
		case "gk.package.99000r3":
		case "gk.package.99000r4":
		case "gk.package.99000r5":
		case "gk.package.110000r1":
		case "gk.package.110000r2":
		case "gk.package.110000r3":
		case "gk.package.110000r4":
		case "gk.package.110000r5":
			RemoteObjectManager.instance.RequestCheckPaymentIOS(productIdentifier, receipt);
			break;
		}
	}

	private static void OnTransactionComplete(IOSStoreKitResult result)
	{
		ISN_Logger.Log("OnTransactionComplete: " + result.ProductIdentifier);
		ISN_Logger.Log("OnTransactionComplete: state: " + result.State);
		switch (result.State)
		{
		case InAppPurchaseState.Purchased:
		case InAppPurchaseState.Restored:
			UnlockProducts(result.ProductIdentifier, result.Receipt);
			break;
		case InAppPurchaseState.Failed:
			ISN_Logger.Log("Transaction failed with error, code: " + result.Error.Code);
			ISN_Logger.Log("Transaction failed with error, description: " + result.Error.Description);
			break;
		}
		if (result.State == InAppPurchaseState.Failed)
		{
			IOSNativePopUpManager.showMessage("Transaction Failed", "Error code: " + result.Error.Code + "\nError description:" + result.Error.Description);
		}
	}

	private static void OnRestoreComplete(IOSStoreKitRestoreResult res)
	{
		if (!res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Error: " + res.Error.Code, res.Error.Description);
		}
	}

	private static void HandleOnVerificationComplete(IOSStoreKitVerificationResponse response)
	{
		if (response.status == 0)
		{
		}
		ISN_Logger.Log("ORIGINAL JSON: " + response.originalJSON);
	}

	private static void OnStoreKitInitComplete(ISN_Result result)
	{
		if (result.IsSucceeded)
		{
			int num = 0;
			foreach (IOSProductTemplate product in ISN_Singleton<IOSInAppPurchaseManager>.instance.Products)
			{
				if (product.IsAvaliable)
				{
					num++;
				}
			}
			ISN_Logger.Log("StoreKit Init Succeeded Available products count: " + num);
		}
		else
		{
			IOSNativePopUpManager.showMessage("StoreKit Init Failed", "Error code: " + result.Error.Code + "\nError description:" + result.Error.Description);
		}
	}
}
