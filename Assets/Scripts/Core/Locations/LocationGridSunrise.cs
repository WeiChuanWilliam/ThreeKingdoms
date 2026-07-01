using System;
using System.Collections.Generic;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>
    /// 日出時對 <see cref="LocationGrid"/> 的火勢與部隊結算。
    /// <para>SKELETON：僅保留方法簽名；實作待與火計、行動力規則一併定稿。</para>
    /// </summary>
    public static class LocationGridSunrise
    {
        /// <summary>日出：遍歷著火格，續燃與蔓延。</summary>
        public static void TickFireAtSunrise(this LocationGrid grid, Random rng) { }

        /// <summary>日出：著火格反查部隊／建築，套用灼燒結算。</summary>
        public static void ApplyBurnDamageAtSunrise(this LocationGrid grid) { }

        /// <summary>日出：同步部隊腳下格狀態並補行動力。</summary>
        public static void TickAllUnitsAtSunrise(this LocationGrid grid) { }

        [Obsolete("改用 TickAllUnitsAtSunrise 與 ApplyBurnDamageAtSunrise。")]
        public static void TickAllUnitsFireAtSunrise(this LocationGrid grid) { }
    }
}
