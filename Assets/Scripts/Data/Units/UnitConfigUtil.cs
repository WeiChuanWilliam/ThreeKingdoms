using System;
using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Scenario;

namespace ThreeKindoms.Data.Units
{
    /// <summary>
    /// 讀取 <c>chinese/unit.properties</c>，存成 static。
    /// 兵種細項用 <see cref="GetKindDisplayName"/>（troop.kind.*）；
    /// 六圍用 <see cref="TryGetKindBaseStats"/>（troop.kind.*.attack 等）。
    /// </summary>
    public static class UnitConfigUtil
    {
        const string DefaultRelativePath = "chinese/unit.properties";

        static Dictionary<string, string> entries = new();
        static bool loaded;

        public static bool IsLoaded => loaded;
        public static string Locale { get; private set; } = "zh";

        /// <summary>戰鬥部隊後綴（properties: suffix.company）。</summary>
        public static string SuffixCompany => Get("suffix.company", Get("suffix.combat", "隊"));

        public static string SuffixLegion => Get("suffix.legion", "軍");
        public static string SuffixTransport => Get("suffix.transport", "運輸隊");
        public static string SuffixEscort => Get("suffix.escort", "護衛隊");
        public static string SuffixGarrison => Get("suffix.garrison", "駐軍");

        /// <summary>相容舊程式碼。</summary>
        public static string SuffixCombat => SuffixCompany;

        public static string FallbackUnitName => Get("name.fallback_unit", "部隊");
        public static string FallbackOfficerPrefix => Get("name.fallback_officer_prefix", "武將");

        public static int DailyMovementPoints => GetInt("movement.daily_points", 15);
        public static int MinSoldiers => GetInt("manpower.min_soldiers", 10);

        public static string StatusOnFire => Get("status.on_fire", "著火");

        /// <summary>每日續燃：n×step%（step 預設 25）；n=0 不續燃。</summary>
        public static int GetFireBurnContinuationStepPercent() =>
            System.Math.Clamp(GetInt("unit.fire.burn_continuation_step", 25), 0, 100);
        public static string StatusOnTrap => Get("status.on_trap", "止步");
        public static string StatusOnMess => Get("status.on_mess", "混亂");
        public static string StatusOnDeceive => Get("status.on_deceive", "偽報");
        public static string StatusOnCrash => Get("status.on_crash", "潰散");

        public static IReadOnlyDictionary<string, string> Entries
        {
            get
            {
                lock (entries)
                    return new Dictionary<string, string>(entries);
            }
        }

        public static bool LoadDefault(string streamingAssetsRoot)
        {
            if (string.IsNullOrEmpty(streamingAssetsRoot))
                return LoadFromRelativePath(DefaultRelativePath);
            return Load(Path.Combine(streamingAssetsRoot, DefaultRelativePath));
        }

        public static bool LoadFromRelativePath(string relativePath)
        {
            string baseDir = Directory.GetCurrentDirectory();
            return Load(Path.Combine(baseDir, relativePath));
        }

        public static bool Load(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                loaded = false;
                return false;
            }

            var parsed = PropertiesFile.LoadFromFile(absolutePath);
            lock (entries)
            {
                entries = parsed;
                loaded = true;
            }

            Locale = Get("locale", "zh");
            return true;
        }

        public static string Get(string key, string defaultValue = "")
        {
            lock (entries)
                return entries.TryGetValue(key, out string v) ? v : defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0) =>
            int.TryParse(Get(key), out int n) ? n : defaultValue;

        public static float GetFloat(string key, float defaultValue = 0f) =>
            float.TryParse(Get(key), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float f)
                ? f
                : defaultValue;

        /// <summary>战斗部队专用：与兵种 mobility 相乘的倍率（unit.march_speed.combat）。</summary>
        public static float GetCombatMarchSpeedFactor() =>
            GetFloat("unit.march_speed.combat", 1f);

        /// <summary>军团／运输队：参数档绝对行军 mobility（非倍率）。</summary>
        public static float GetUnitMarchMobility(UnitKind kind) => kind switch
        {
            UnitKind.Legion => GetFloat("unit.mobility.legion", 70f),
            UnitKind.Transport => GetFloat("unit.mobility.transport", 85f),
            _ => 0f
        };

        /// <summary>耗粮倍率；见 unit.food_factor.*。</summary>
        public static float GetFoodConsumptionFactor(UnitKind kind) => kind switch
        {
            UnitKind.Legion => GetFloat("unit.food_factor.legion", 0.5f),
            UnitKind.Transport => GetFloat("unit.food_factor.transport", 0.7f),
            UnitKind.Garrison => 0f,
            _ => GetFloat("unit.food_factor.combat", 1f)
        };

        static string NormalizeKindKey(string kindKey)
        {
            if (string.IsNullOrWhiteSpace(kindKey)) return "";
            if (kindKey.StartsWith("troop.kind.", StringComparison.Ordinal))
                return kindKey.Substring("troop.kind.".Length);
            return kindKey.Trim();
        }

        static string KindStatKey(string kindKey, string statSuffix) =>
            "troop.kind." + NormalizeKindKey(kindKey) + "." + statSuffix;

        /// <summary>讀單項數值；<paramref name="statSuffix"/> 如 attack、defense、stamina。</summary>
        public static short GetKindStatShort(string kindKey, string statSuffix, short defaultValue = 0)
        {
            int n = GetInt(KindStatKey(kindKey, statSuffix), defaultValue);
            if (n < short.MinValue) return short.MinValue;
            if (n > short.MaxValue) return short.MaxValue;
            return (short)n;
        }

        public static bool HasKindStat(string kindKey, string statSuffix)
        {
            lock (entries)
                return entries.ContainsKey(KindStatKey(kindKey, statSuffix));
        }

        /// <summary>攻擊距離（格）；優先 attack_range，相容舊鍵 range、siege_range。</summary>
        public static bool TryGetKindAttackRange(string kindKey, out short range)
        {
            range = 0;
            string key = NormalizeKindKey(kindKey);
            if (string.IsNullOrEmpty(key))
                return false;

            foreach (string suffix in new[] { "attack_range", "range", "siege_range" })
            {
                if (!HasKindStat(key, suffix))
                    continue;
                range = GetKindStatShort(key, suffix, 1);
                return true;
            }
            return false;
        }

        /// <summary>六圍：attack、defense（或 defence）、mobility、jipo、gongcheng、stamina。</summary>
        public static bool TryGetKindBaseStats(string kindKey, out TroopKindBaseStats stats)
        {
            stats = default;
            string key = NormalizeKindKey(kindKey);
            if (string.IsNullOrEmpty(key))
                return false;

            if (!HasKindStat(key, "attack"))
                return false;

            short defense = GetKindStatShort(key, "defense", -1);
            if (defense < 0)
                defense = GetKindStatShort(key, "defence", 0);

            stats = new TroopKindBaseStats(
                GetKindStatShort(key, "attack"),
                defense,
                GetKindStatShort(key, "mobility"),
                GetKindStatShort(key, "jipo"),
                GetKindStatShort(key, "gongcheng"),
                GetKindStatShort(key, "stamina"));
            return true;
        }

        public static string GetSuffix(UnitKind kind) => kind switch
        {
            UnitKind.Legion => SuffixLegion,
            UnitKind.Transport => SuffixTransport,
            UnitKind.Garrison => SuffixGarrison,
            _ => SuffixCompany
        };

        /// <summary>大類顯示名（troop.type.*）。</summary>
        public static string GetTroopTypeDisplayName(TroopType troop) => troop switch
        {
            TroopType.Cavalry => Get("troop.type.cavalry", Get("troop.cavalry", "騎兵")),
            TroopType.Archer => Get("troop.type.archer", Get("troop.archer", "弓兵")),
            TroopType.Siege => Get("troop.type.siege", Get("troop.siege", "器械")),
            TroopType.Navy => Get("troop.type.navy", Get("troop.navy", "水軍")),
            _ => Get("troop.type.infantry", Get("troop.infantry", "步兵"))
        };

        /// <summary>具體兵種線（troop.kind.spear.daji → 傳 "spear.daji" 或完整鍵）。</summary>
        public static string GetKindDisplayName(string kindKey)
        {
            if (string.IsNullOrWhiteSpace(kindKey)) return "";
            string key = kindKey.StartsWith("troop.kind.") ? kindKey : "troop.kind." + kindKey;
            if (entries.ContainsKey(key))
                return Get(key);
            string legacy = kindKey switch
            {
                "spear.chinzhou" => "troop.kind.spear.qingzhou",
                "armor.baihau" => "troop.kind.armor.baimao",
                _ => null
            };
            if (legacy != null && entries.ContainsKey(legacy))
                return Get(legacy);
            return Get(key, kindKey);
        }

        /// <summary>相容舊 API。</summary>
        public static string GetTroopDisplayName(TroopType troop) => GetTroopTypeDisplayName(troop);

        public static string GetHazardDisplayName(HazardDamageLevel level) => level switch
        {
            HazardDamageLevel.Slight => Get("hazard.slight", "輕微"),
            HazardDamageLevel.Medium => Get("hazard.medium", "中等"),
            HazardDamageLevel.Serious => Get("hazard.serious", "嚴重"),
            _ => Get("hazard.none", "無")
        };
    }
}
