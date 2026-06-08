using System;
using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>特色兵種解鎖條件（如白馬義從、陷陣營）：武將該兵科大類適性 S ＋ 勢力已研發科技。</summary>
    public readonly struct SignatureTroopRequirement
    {
        public string KindKey { get; }
        public TroopType TroopType { get; }
        public string TechKey { get; }

        public SignatureTroopRequirement(string kindKey, TroopType troopType, string techKey)
        {
            KindKey = kindKey ?? "";
            TroopType = troopType;
            TechKey = techKey ?? "";
        }
    }

    public static class OfficerSignatureTroopRules
    {
        static readonly Dictionary<string, SignatureTroopRequirement> byKindKey =
            new(StringComparer.OrdinalIgnoreCase);

        public static void EnsureBuilt()
        {
            byKindKey.Clear();
            foreach (SignatureTroopRequirement req in OfficerConfigUtil.GetSignatureTroopRequirements())
                byKindKey[req.KindKey] = req;
        }

        public static bool TryGetRequirement(string troopKindKey, out SignatureTroopRequirement requirement)
        {
            EnsureBuilt();
            return byKindKey.TryGetValue(troopKindKey ?? "", out requirement);
        }

        public static bool IsSignatureTroop(string troopKindKey)
        {
            EnsureBuilt();
            return byKindKey.ContainsKey(troopKindKey ?? "");
        }

        /// <summary>非特色兵種一律可編制（之後可再加一般科技門檻）。</summary>
        public static bool CanOfficerLeadTroopKind(
            Officer officer,
            string troopKindKey,
            Func<string, bool> factionHasTech)
        {
            if (officer == null || string.IsNullOrWhiteSpace(troopKindKey))
                return false;

            if (!TryGetRequirement(troopKindKey, out SignatureTroopRequirement req))
                return true;

            if (officer.GetTroopAptitude(req.TroopType) != TroopAptitudeGrade.S)
                return false;

            return factionHasTech != null && factionHasTech(req.TechKey);
        }
    }
}
