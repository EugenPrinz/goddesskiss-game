using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;

namespace Shared.Battle
{
	[JsonObject(MemberSerialization.OptIn)]
	public class FirePoint
	{
		[JsonProperty]
		internal int _subIdx;

		[JsonProperty]
		internal int _projectileDri;

		[JsonProperty]
		internal List<int> _statusEffectDris;

		[JsonProperty]
		internal List<Projectile> _projectiles;

		[JsonProperty]
		internal string _firePattern;

		public int subIdx => _subIdx;

		public int projectileDri => _projectileDri;

		public string firePattern => _firePattern;

		public IList<int> statusEffectDris => _statusEffectDris.AsReadOnly();

		public IList<Projectile> projectiles => _projectiles.AsReadOnly();

		private FirePoint()
		{
		}

		internal static FirePoint _Create(Shared.Regulation.Regulation rg, SkillDataRow skillDr, int index, int subIdx)
		{
			string text = ((subIdx != 0) ? skillDr.fireSubProjectileDrks[index] : skillDr.projectileDrks[index]);
			int num = rg.projectileDtbl.FindIndex(text);
			if (num < 0)
			{
				throw new ArgumentException("projectileDtbl drk doesn't exist. : " + text);
			}
			FirePoint firePoint = new FirePoint();
			firePoint._subIdx = subIdx;
			firePoint._projectiles = new List<Projectile>();
			firePoint._statusEffectDris = new List<int>();
			firePoint._projectileDri = num;
			firePoint._firePattern = ((subIdx != 0) ? skillDr.fireSubPatterns[index] : skillDr.firePatterns[index]);
			ProjectileDataRow projectileDataRow = rg.projectileDtbl[firePoint._projectileDri];
			foreach (string statusEffectDrk in projectileDataRow.statusEffectDrks)
			{
				if (!string.IsNullOrEmpty(statusEffectDrk) && statusEffectDrk != "0")
				{
					int num2 = rg.statusEffectDtbl.FindIndex(statusEffectDrk);
					if (num2 < 0)
					{
						throw new ArgumentException("statusEffectDtbl drk doesn't exist. : " + statusEffectDrk);
					}
					firePoint._statusEffectDris.Add(num2);
				}
			}
			return firePoint;
		}

		internal static FirePoint _Copy(FirePoint src)
		{
			FirePoint firePoint = new FirePoint();
			firePoint._subIdx = src._subIdx;
			firePoint._projectileDri = src._projectileDri;
			firePoint._firePattern = src._firePattern;
			firePoint._statusEffectDris = new List<int>(src._statusEffectDris);
			firePoint._projectiles = new List<Projectile>(src._projectiles.Count);
			for (int i = 0; i < src._projectiles.Count; i++)
			{
				firePoint._projectiles.Add(Projectile._Copy(src._projectiles[i]));
			}
			return firePoint;
		}
	}
}
