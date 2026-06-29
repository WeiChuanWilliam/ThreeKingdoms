using System;

using System.Collections.Generic;

using ThreeKindoms.Data.Officers;



namespace ThreeKindoms.Core.Officers

{

    /// <summary>基礎六圍 → 發揮值（<c>*Perform</c>）：傷勢、體力、道具。</summary>

    public static class OfficerPerformanceRules

    {

        /// <summary>六圍基礎值下限。</summary>
        public const byte StatMin = 1;

        /// <summary>六圍基礎值與發揮值上限。</summary>
        public const byte StatMax = 100;

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

            IReadOnlyCollection<int> itemIds)

        {

            if (!isAlive || baseStat == 0)

                return 0;



            float value = baseStat;

            value *= GetInjuryMultiplier(injury);

            value *= GetStaminaMultiplier(stamina);

            value *= GetItemMultiplier(itemIds);

            return ClampPerform(value);

        }



        /// <summary>依傷勢等級回傳發揮值乘數。</summary>
        public static float GetInjuryMultiplier(OfficerInjuryState injury) => injury switch

        {

            OfficerInjuryState.Normal => 1f,

            OfficerInjuryState.Light => 0.9f,

            OfficerInjuryState.Medium => 0.75f,

            OfficerInjuryState.Severe => 0.55f,

            _ => 1f

        };



        /// <summary>體力 0→50% 係數，100→100% 係數（線性）。</summary>

        public static float GetStaminaMultiplier(byte stamina)

        {

            float t = Math.Clamp(stamina, StaminaMin, StaminaMax) / (float)StaminaMax;

            return 0.5f + t * 0.5f;

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

            (byte)Math.Clamp(value, StatMin, StatMax);



        /// <summary>將浮點發揮值四捨五入並限制在 0～100。</summary>
        public static byte ClampPerform(float value)

        {

            int rounded = (int)MathF.Round(value);

            if (rounded < 0) return 0;

            if (rounded > StatMax) return StatMax;

            return (byte)rounded;

        }



        /// <summary>將整數體力限制在 0～100。</summary>
        public static byte ClampStamina(int value) =>

            (byte)Math.Clamp(value, StaminaMin, StaminaMax);

    }

}

