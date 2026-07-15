using System;
using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>基礎六圍 → 發揮值（<c>*Perform</c>）：傷勢、體力、道具。</summary>
    public static class OfficerPerformanceRules
    {
        /// <summary>六圍基礎值下限。</summary>
        public const byte StatMin = 1;

        /// <summary>六圍基礎值與發揮值上限。</summary>
        public const byte StatMax = 125;

        /// <summary>體力下限。</summary>
        public const byte StaminaMin = 0;

        /// <summary>體力上限。</summary>
        public const byte StaminaMax = 100;

        /// <summary>由基礎值、傷勢、體力與道具計算單項發揮值。</summary>
        public static byte ComputePerform(
            byte baseStat,
            OfficerInjuryState injury,
            bool isAlive,
            byte stamina,
            bool isNoFear,
            IReadOnlyCollection<int> itemIds)
        {
            if (!isAlive || baseStat == 0)
                return 0;

            float value = baseStat;
            value *= GetInjuryMultiplier(injury);
            value *= GetStaminaMultiplier(stamina, isNoFear);
            value *= GetItemMultiplier(itemIds);
            return ClampPerform(value);
        }

        /// <summary>依傷勢等級回傳發揮值乘數。</summary>
        public static float GetInjuryMultiplier(OfficerInjuryState injury) => injury switch
        {
            OfficerInjuryState.Normal => 1f,
            OfficerInjuryState.Light => 0.9f,
            OfficerInjuryState.Medium => 0.6f,
            OfficerInjuryState.Severe => 0.3f,
            _ => 1f
        };

        /// <summary>體力 0→50% 係數，100→100% 係數（線性）。</summary>
        public static float GetStaminaMultiplier(byte stamina, bool isNoFear)
        {
            if (isNoFear)
                return 1f;

            byte clamped = NumericUtil.ClampToTarget(stamina, StaminaMin, StaminaMax);
            float t = clamped / (float)StaminaMax;
            float multiplier = 0.25f + (t + 0.25f) * 0.75f;
            return NumericUtil.ClampToTarget(multiplier, 0.25f, 1f);
        }

        /// <summary>道具加成乘數；表未接時為 1。</summary>
        public static float GetItemMultiplier(IReadOnlyCollection<int> itemIds)
        {
            if (itemIds == null || itemIds.Count == 0)
                return 1f;

            // TODO: ItemCatalog 依 itemIds 加總各能力加成％
            return 1f;
        }

        /// <summary>將整數基礎值限制在合法六圍區間。</summary>
        public static byte ClampBaseStat(int value) =>
            (byte)NumericUtil.ClampToTarget(value, StatMin, StatMax);

        /// <summary>將浮點發揮值四捨五入並限制在 0～StatMax。</summary>
        public static byte ClampPerform(float value) =>
            (byte)NumericUtil.ClampToTarget((int)MathF.Round(value), 0, StatMax);

        /// <summary>將整數體力限制在合法區間。</summary>
        public static byte ClampStamina(int value) =>
            (byte)NumericUtil.ClampToTarget(value, StaminaMin, StaminaMax);
    }
}
