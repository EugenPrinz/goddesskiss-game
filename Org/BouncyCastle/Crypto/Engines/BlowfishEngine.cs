using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	public sealed class BlowfishEngine : IBlockCipher
	{
		private static readonly uint[] KP = new uint[18]
		{
			608135816u, 2242054355u, 320440878u, 57701188u, 2752067618u, 698298832u, 137296536u, 3964562569u, 1160258022u, 953160567u,
			3193202383u, 887688300u, 3232508343u, 3380367581u, 1065670069u, 3041331479u, 2450970073u, 2306472731u
		};

		private static readonly uint[] KS0 = new uint[256]
		{
			3509652390u, 2564797868u, 805139163u, 3491422135u, 3101798381u, 1780907670u, 3128725573u, 4046225305u, 614570311u, 3012652279u,
			134345442u, 2240740374u, 1667834072u, 1901547113u, 2757295779u, 4103290238u, 227898511u, 1921955416u, 1904987480u, 2182433518u,
			2069144605u, 3260701109u, 2620446009u, 720527379u, 3318853667u, 677414384u, 3393288472u, 3101374703u, 2390351024u, 1614419982u,
			1822297739u, 2954791486u, 3608508353u, 3174124327u, 2024746970u, 1432378464u, 3864339955u, 2857741204u, 1464375394u, 1676153920u,
			1439316330u, 715854006u, 3033291828u, 289532110u, 2706671279u, 2087905683u, 3018724369u, 1668267050u, 732546397u, 1947742710u,
			3462151702u, 2609353502u, 2950085171u, 1814351708u, 2050118529u, 680887927u, 999245976u, 1800124847u, 3300911131u, 1713906067u,
			1641548236u, 4213287313u, 1216130144u, 1575780402u, 4018429277u, 3917837745u, 3693486850u, 3949271944u, 596196993u, 3549867205u,
			258830323u, 2213823033u, 772490370u, 2760122372u, 1774776394u, 2652871518u, 566650946u, 4142492826u, 1728879713u, 2882767088u,
			1783734482u, 3629395816u, 2517608232u, 2874225571u, 1861159788u, 326777828u, 3124490320u, 2130389656u, 2716951837u, 967770486u,
			1724537150u, 2185432712u, 2364442137u, 1164943284u, 2105845187u, 998989502u, 3765401048u, 2244026483u, 1075463327u, 1455516326u,
			1322494562u, 910128902u, 469688178u, 1117454909u, 936433444u, 3490320968u, 3675253459u, 1240580251u, 122909385u, 2157517691u,
			634681816u, 4142456567u, 3825094682u, 3061402683u, 2540495037u, 79693498u, 3249098678u, 1084186820u, 1583128258u, 426386531u,
			1761308591u, 1047286709u, 322548459u, 995290223u, 1845252383u, 2603652396u, 3431023940u, 2942221577u, 3202600964u, 3727903485u,
			1712269319u, 422464435u, 3234572375u, 1170764815u, 3523960633u, 3117677531u, 1434042557u, 442511882u, 3600875718u, 1076654713u,
			1738483198u, 4213154764u, 2393238008u, 3677496056u, 1014306527u, 4251020053u, 793779912u, 2902807211u, 842905082u, 4246964064u,
			1395751752u, 1040244610u, 2656851899u, 3396308128u, 445077038u, 3742853595u, 3577915638u, 679411651u, 2892444358u, 2354009459u,
			1767581616u, 3150600392u, 3791627101u, 3102740896u, 284835224u, 4246832056u, 1258075500u, 768725851u, 2589189241u, 3069724005u,
			3532540348u, 1274779536u, 3789419226u, 2764799539u, 1660621633u, 3471099624u, 4011903706u, 913787905u, 3497959166u, 737222580u,
			2514213453u, 2928710040u, 3937242737u, 1804850592u, 3499020752u, 2949064160u, 2386320175u, 2390070455u, 2415321851u, 4061277028u,
			2290661394u, 2416832540u, 1336762016u, 1754252060u, 3520065937u, 3014181293u, 791618072u, 3188594551u, 3933548030u, 2332172193u,
			3852520463u, 3043980520u, 413987798u, 3465142937u, 3030929376u, 4245938359u, 2093235073u, 3534596313u, 375366246u, 2157278981u,
			2479649556u, 555357303u, 3870105701u, 2008414854u, 3344188149u, 4221384143u, 3956125452u, 2067696032u, 3594591187u, 2921233993u,
			2428461u, 544322398u, 577241275u, 1471733935u, 610547355u, 4027169054u, 1432588573u, 1507829418u, 2025931657u, 3646575487u,
			545086370u, 48609733u, 2200306550u, 1653985193u, 298326376u, 1316178497u, 3007786442u, 2064951626u, 458293330u, 2589141269u,
			3591329599u, 3164325604u, 727753846u, 2179363840u, 146436021u, 1461446943u, 4069977195u, 705550613u, 3059967265u, 3887724982u,
			4281599278u, 3313849956u, 1404054877u, 2845806497u, 146425753u, 1854211946u
		};

		private static readonly uint[] KS1 = new uint[256]
		{
			1266315497u, 3048417604u, 3681880366u, 3289982499u, 2909710000u, 1235738493u, 2632868024u, 2414719590u, 3970600049u, 1771706367u,
			1449415276u, 3266420449u, 422970021u, 1963543593u, 2690192192u, 3826793022u, 1062508698u, 1531092325u, 1804592342u, 2583117782u,
			2714934279u, 4024971509u, 1294809318u, 4028980673u, 1289560198u, 2221992742u, 1669523910u, 35572830u, 157838143u, 1052438473u,
			1016535060u, 1802137761u, 1753167236u, 1386275462u, 3080475397u, 2857371447u, 1040679964u, 2145300060u, 2390574316u, 1461121720u,
			2956646967u, 4031777805u, 4028374788u, 33600511u, 2920084762u, 1018524850u, 629373528u, 3691585981u, 3515945977u, 2091462646u,
			2486323059u, 586499841u, 988145025u, 935516892u, 3367335476u, 2599673255u, 2839830854u, 265290510u, 3972581182u, 2759138881u,
			3795373465u, 1005194799u, 847297441u, 406762289u, 1314163512u, 1332590856u, 1866599683u, 4127851711u, 750260880u, 613907577u,
			1450815602u, 3165620655u, 3734664991u, 3650291728u, 3012275730u, 3704569646u, 1427272223u, 778793252u, 1343938022u, 2676280711u,
			2052605720u, 1946737175u, 3164576444u, 3914038668u, 3967478842u, 3682934266u, 1661551462u, 3294938066u, 4011595847u, 840292616u,
			3712170807u, 616741398u, 312560963u, 711312465u, 1351876610u, 322626781u, 1910503582u, 271666773u, 2175563734u, 1594956187u,
			70604529u, 3617834859u, 1007753275u, 1495573769u, 4069517037u, 2549218298u, 2663038764u, 504708206u, 2263041392u, 3941167025u,
			2249088522u, 1514023603u, 1998579484u, 1312622330u, 694541497u, 2582060303u, 2151582166u, 1382467621u, 776784248u, 2618340202u,
			3323268794u, 2497899128u, 2784771155u, 503983604u, 4076293799u, 907881277u, 423175695u, 432175456u, 1378068232u, 4145222326u,
			3954048622u, 3938656102u, 3820766613u, 2793130115u, 2977904593u, 26017576u, 3274890735u, 3194772133u, 1700274565u, 1756076034u,
			4006520079u, 3677328699u, 720338349u, 1533947780u, 354530856u, 688349552u, 3973924725u, 1637815568u, 332179504u, 3949051286u,
			53804574u, 2852348879u, 3044236432u, 1282449977u, 3583942155u, 3416972820u, 4006381244u, 1617046695u, 2628476075u, 3002303598u,
			1686838959u, 431878346u, 2686675385u, 1700445008u, 1080580658u, 1009431731u, 832498133u, 3223435511u, 2605976345u, 2271191193u,
			2516031870u, 1648197032u, 4164389018u, 2548247927u, 300782431u, 375919233u, 238389289u, 3353747414u, 2531188641u, 2019080857u,
			1475708069u, 455242339u, 2609103871u, 448939670u, 3451063019u, 1395535956u, 2413381860u, 1841049896u, 1491858159u, 885456874u,
			4264095073u, 4001119347u, 1565136089u, 3898914787u, 1108368660u, 540939232u, 1173283510u, 2745871338u, 3681308437u, 4207628240u,
			3343053890u, 4016749493u, 1699691293u, 1103962373u, 3625875870u, 2256883143u, 3830138730u, 1031889488u, 3479347698u, 1535977030u,
			4236805024u, 3251091107u, 2132092099u, 1774941330u, 1199868427u, 1452454533u, 157007616u, 2904115357u, 342012276u, 595725824u,
			1480756522u, 206960106u, 497939518u, 591360097u, 863170706u, 2375253569u, 3596610801u, 1814182875u, 2094937945u, 3421402208u,
			1082520231u, 3463918190u, 2785509508u, 435703966u, 3908032597u, 1641649973u, 2842273706u, 3305899714u, 1510255612u, 2148256476u,
			2655287854u, 3276092548u, 4258621189u, 236887753u, 3681803219u, 274041037u, 1734335097u, 3815195456u, 3317970021u, 1899903192u,
			1026095262u, 4050517792u, 356393447u, 2410691914u, 3873677099u, 3682840055u
		};

		private static readonly uint[] KS2 = new uint[256]
		{
			3913112168u, 2491498743u, 4132185628u, 2489919796u, 1091903735u, 1979897079u, 3170134830u, 3567386728u, 3557303409u, 857797738u,
			1136121015u, 1342202287u, 507115054u, 2535736646u, 337727348u, 3213592640u, 1301675037u, 2528481711u, 1895095763u, 1721773893u,
			3216771564u, 62756741u, 2142006736u, 835421444u, 2531993523u, 1442658625u, 3659876326u, 2882144922u, 676362277u, 1392781812u,
			170690266u, 3921047035u, 1759253602u, 3611846912u, 1745797284u, 664899054u, 1329594018u, 3901205900u, 3045908486u, 2062866102u,
			2865634940u, 3543621612u, 3464012697u, 1080764994u, 553557557u, 3656615353u, 3996768171u, 991055499u, 499776247u, 1265440854u,
			648242737u, 3940784050u, 980351604u, 3713745714u, 1749149687u, 3396870395u, 4211799374u, 3640570775u, 1161844396u, 3125318951u,
			1431517754u, 545492359u, 4268468663u, 3499529547u, 1437099964u, 2702547544u, 3433638243u, 2581715763u, 2787789398u, 1060185593u,
			1593081372u, 2418618748u, 4260947970u, 69676912u, 2159744348u, 86519011u, 2512459080u, 3838209314u, 1220612927u, 3339683548u,
			133810670u, 1090789135u, 1078426020u, 1569222167u, 845107691u, 3583754449u, 4072456591u, 1091646820u, 628848692u, 1613405280u,
			3757631651u, 526609435u, 236106946u, 48312990u, 2942717905u, 3402727701u, 1797494240u, 859738849u, 992217954u, 4005476642u,
			2243076622u, 3870952857u, 3732016268u, 765654824u, 3490871365u, 2511836413u, 1685915746u, 3888969200u, 1414112111u, 2273134842u,
			3281911079u, 4080962846u, 172450625u, 2569994100u, 980381355u, 4109958455u, 2819808352u, 2716589560u, 2568741196u, 3681446669u,
			3329971472u, 1835478071u, 660984891u, 3704678404u, 4045999559u, 3422617507u, 3040415634u, 1762651403u, 1719377915u, 3470491036u,
			2693910283u, 3642056355u, 3138596744u, 1364962596u, 2073328063u, 1983633131u, 926494387u, 3423689081u, 2150032023u, 4096667949u,
			1749200295u, 3328846651u, 309677260u, 2016342300u, 1779581495u, 3079819751u, 111262694u, 1274766160u, 443224088u, 298511866u,
			1025883608u, 3806446537u, 1145181785u, 168956806u, 3641502830u, 3584813610u, 1689216846u, 3666258015u, 3200248200u, 1692713982u,
			2646376535u, 4042768518u, 1618508792u, 1610833997u, 3523052358u, 4130873264u, 2001055236u, 3610705100u, 2202168115u, 4028541809u,
			2961195399u, 1006657119u, 2006996926u, 3186142756u, 1430667929u, 3210227297u, 1314452623u, 4074634658u, 4101304120u, 2273951170u,
			1399257539u, 3367210612u, 3027628629u, 1190975929u, 2062231137u, 2333990788u, 2221543033u, 2438960610u, 1181637006u, 548689776u,
			2362791313u, 3372408396u, 3104550113u, 3145860560u, 296247880u, 1970579870u, 3078560182u, 3769228297u, 1714227617u, 3291629107u,
			3898220290u, 166772364u, 1251581989u, 493813264u, 448347421u, 195405023u, 2709975567u, 677966185u, 3703036547u, 1463355134u,
			2715995803u, 1338867538u, 1343315457u, 2802222074u, 2684532164u, 233230375u, 2599980071u, 2000651841u, 3277868038u, 1638401717u,
			4028070440u, 3237316320u, 6314154u, 819756386u, 300326615u, 590932579u, 1405279636u, 3267499572u, 3150704214u, 2428286686u,
			3959192993u, 3461946742u, 1862657033u, 1266418056u, 963775037u, 2089974820u, 2263052895u, 1917689273u, 448879540u, 3550394620u,
			3981727096u, 150775221u, 3627908307u, 1303187396u, 508620638u, 2975983352u, 2726630617u, 1817252668u, 1876281319u, 1457606340u,
			908771278u, 3720792119u, 3617206836u, 2455994898u, 1729034894u, 1080033504u
		};

		private static readonly uint[] KS3 = new uint[256]
		{
			976866871u, 3556439503u, 2881648439u, 1522871579u, 1555064734u, 1336096578u, 3548522304u, 2579274686u, 3574697629u, 3205460757u,
			3593280638u, 3338716283u, 3079412587u, 564236357u, 2993598910u, 1781952180u, 1464380207u, 3163844217u, 3332601554u, 1699332808u,
			1393555694u, 1183702653u, 3581086237u, 1288719814u, 691649499u, 2847557200u, 2895455976u, 3193889540u, 2717570544u, 1781354906u,
			1676643554u, 2592534050u, 3230253752u, 1126444790u, 2770207658u, 2633158820u, 2210423226u, 2615765581u, 2414155088u, 3127139286u,
			673620729u, 2805611233u, 1269405062u, 4015350505u, 3341807571u, 4149409754u, 1057255273u, 2012875353u, 2162469141u, 2276492801u,
			2601117357u, 993977747u, 3918593370u, 2654263191u, 753973209u, 36408145u, 2530585658u, 25011837u, 3520020182u, 2088578344u,
			530523599u, 2918365339u, 1524020338u, 1518925132u, 3760827505u, 3759777254u, 1202760957u, 3985898139u, 3906192525u, 674977740u,
			4174734889u, 2031300136u, 2019492241u, 3983892565u, 4153806404u, 3822280332u, 352677332u, 2297720250u, 60907813u, 90501309u,
			3286998549u, 1016092578u, 2535922412u, 2839152426u, 457141659u, 509813237u, 4120667899u, 652014361u, 1966332200u, 2975202805u,
			55981186u, 2327461051u, 676427537u, 3255491064u, 2882294119u, 3433927263u, 1307055953u, 942726286u, 933058658u, 2468411793u,
			3933900994u, 4215176142u, 1361170020u, 2001714738u, 2830558078u, 3274259782u, 1222529897u, 1679025792u, 2729314320u, 3714953764u,
			1770335741u, 151462246u, 3013232138u, 1682292957u, 1483529935u, 471910574u, 1539241949u, 458788160u, 3436315007u, 1807016891u,
			3718408830u, 978976581u, 1043663428u, 3165965781u, 1927990952u, 4200891579u, 2372276910u, 3208408903u, 3533431907u, 1412390302u,
			2931980059u, 4132332400u, 1947078029u, 3881505623u, 4168226417u, 2941484381u, 1077988104u, 1320477388u, 886195818u, 18198404u,
			3786409000u, 2509781533u, 112762804u, 3463356488u, 1866414978u, 891333506u, 18488651u, 661792760u, 1628790961u, 3885187036u,
			3141171499u, 876946877u, 2693282273u, 1372485963u, 791857591u, 2686433993u, 3759982718u, 3167212022u, 3472953795u, 2716379847u,
			445679433u, 3561995674u, 3504004811u, 3574258232u, 54117162u, 3331405415u, 2381918588u, 3769707343u, 4154350007u, 1140177722u,
			4074052095u, 668550556u, 3214352940u, 367459370u, 261225585u, 2610173221u, 4209349473u, 3468074219u, 3265815641u, 314222801u,
			3066103646u, 3808782860u, 282218597u, 3406013506u, 3773591054u, 379116347u, 1285071038u, 846784868u, 2669647154u, 3771962079u,
			3550491691u, 2305946142u, 453669953u, 1268987020u, 3317592352u, 3279303384u, 3744833421u, 2610507566u, 3859509063u, 266596637u,
			3847019092u, 517658769u, 3462560207u, 3443424879u, 370717030u, 4247526661u, 2224018117u, 4143653529u, 4112773975u, 2788324899u,
			2477274417u, 1456262402u, 2901442914u, 1517677493u, 1846949527u, 2295493580u, 3734397586u, 2176403920u, 1280348187u, 1908823572u,
			3871786941u, 846861322u, 1172426758u, 3287448474u, 3383383037u, 1655181056u, 3139813346u, 901632758u, 1897031941u, 2986607138u,
			3066810236u, 3447102507u, 1393639104u, 373351379u, 950779232u, 625454576u, 3124240540u, 4148612726u, 2007998917u, 544563296u,
			2244738638u, 2330496472u, 2058025392u, 1291430526u, 424198748u, 50039436u, 29584100u, 3605783033u, 2429876329u, 2791104160u,
			1057563949u, 3255363231u, 3075367218u, 3463963227u, 1469046755u, 985887462u
		};

		private static readonly int ROUNDS = 16;

		private const int BLOCK_SIZE = 8;

		private static readonly int SBOX_SK = 256;

		private static readonly int P_SZ = ROUNDS + 2;

		private readonly uint[] S0;

		private readonly uint[] S1;

		private readonly uint[] S2;

		private readonly uint[] S3;

		private readonly uint[] P;

		private bool encrypting;

		private byte[] workingKey;

		public string AlgorithmName => "Blowfish";

		public bool IsPartialBlockOkay => false;

		public BlowfishEngine()
		{
			S0 = new uint[SBOX_SK];
			S1 = new uint[SBOX_SK];
			S2 = new uint[SBOX_SK];
			S3 = new uint[SBOX_SK];
			P = new uint[P_SZ];
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to Blowfish init - " + parameters.GetType().ToString());
			}
			encrypting = forEncryption;
			workingKey = ((KeyParameter)parameters).GetKey();
			SetKey(workingKey);
		}

		public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (workingKey == null)
			{
				throw new InvalidOperationException("Blowfish not initialised");
			}
			Check.DataLength(input, inOff, 8, "input buffer too short");
			Check.OutputLength(output, outOff, 8, "output buffer too short");
			if (encrypting)
			{
				EncryptBlock(input, inOff, output, outOff);
			}
			else
			{
				DecryptBlock(input, inOff, output, outOff);
			}
			return 8;
		}

		public void Reset()
		{
		}

		public int GetBlockSize()
		{
			return 8;
		}

		private uint F(uint x)
		{
			return ((S0[x >> 24] + S1[(x >> 16) & 0xFF]) ^ S2[(x >> 8) & 0xFF]) + S3[x & 0xFF];
		}

		private void ProcessTable(uint xl, uint xr, uint[] table)
		{
			int num = table.Length;
			for (int i = 0; i < num; i += 2)
			{
				xl ^= P[0];
				for (int j = 1; j < ROUNDS; j += 2)
				{
					xr ^= F(xl) ^ P[j];
					xl ^= F(xr) ^ P[j + 1];
				}
				xr ^= P[ROUNDS + 1];
				table[i] = xr;
				table[i + 1] = xl;
				xr = xl;
				xl = table[i];
			}
		}

		private void SetKey(byte[] key)
		{
			Array.Copy(KS0, 0, S0, 0, SBOX_SK);
			Array.Copy(KS1, 0, S1, 0, SBOX_SK);
			Array.Copy(KS2, 0, S2, 0, SBOX_SK);
			Array.Copy(KS3, 0, S3, 0, SBOX_SK);
			Array.Copy(KP, 0, P, 0, P_SZ);
			int num = key.Length;
			int num2 = 0;
			for (int i = 0; i < P_SZ; i++)
			{
				uint num3 = 0u;
				for (int j = 0; j < 4; j++)
				{
					num3 = (num3 << 8) | key[num2++];
					if (num2 >= num)
					{
						num2 = 0;
					}
				}
				P[i] ^= num3;
			}
			ProcessTable(0u, 0u, P);
			ProcessTable(P[P_SZ - 2], P[P_SZ - 1], S0);
			ProcessTable(S0[SBOX_SK - 2], S0[SBOX_SK - 1], S1);
			ProcessTable(S1[SBOX_SK - 2], S1[SBOX_SK - 1], S2);
			ProcessTable(S2[SBOX_SK - 2], S2[SBOX_SK - 1], S3);
		}

		private void EncryptBlock(byte[] src, int srcIndex, byte[] dst, int dstIndex)
		{
			uint num = Pack.BE_To_UInt32(src, srcIndex);
			uint num2 = Pack.BE_To_UInt32(src, srcIndex + 4);
			num ^= P[0];
			for (int i = 1; i < ROUNDS; i += 2)
			{
				num2 ^= F(num) ^ P[i];
				num ^= F(num2) ^ P[i + 1];
			}
			num2 ^= P[ROUNDS + 1];
			Pack.UInt32_To_BE(num2, dst, dstIndex);
			Pack.UInt32_To_BE(num, dst, dstIndex + 4);
		}

		private void DecryptBlock(byte[] src, int srcIndex, byte[] dst, int dstIndex)
		{
			uint num = Pack.BE_To_UInt32(src, srcIndex);
			uint num2 = Pack.BE_To_UInt32(src, srcIndex + 4);
			num ^= P[ROUNDS + 1];
			for (int num3 = ROUNDS; num3 > 0; num3 -= 2)
			{
				num2 ^= F(num) ^ P[num3];
				num ^= F(num2) ^ P[num3 - 1];
			}
			num2 ^= P[0];
			Pack.UInt32_To_BE(num2, dst, dstIndex);
			Pack.UInt32_To_BE(num, dst, dstIndex + 4);
		}
	}
}
