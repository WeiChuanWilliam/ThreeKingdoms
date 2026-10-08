using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Terrain;

namespace ThreeKindoms.Core.Terrain
{
    /// <summary>
    /// 地形類型定義（平原／山地等），非地圖上某一格。每種地形全域只有一個實例，由 <see cref="TerrainRegistry"/> 持有。
    /// 格座標、建築、OnFire／OnTrap 屬 Location，不在此類。
    /// </summary>
    public abstract class AbstractTerrain
    {
        /// <summary>地形類型 id（等於 <see cref="TerrainType"/> 數值）。</summary>
        public int TerrainId { get; }

        /// <summary>地形顯示名稱。</summary>
        public string TerrainName { get; }

        /// <summary>可達、可建、可燃等旗標。</summary>
        public TerrainFlags TerrainFlags { get; }

        /// <summary>離開此格所需行動力。</summary>
        public byte MoveCost { get; }

        /// <summary>火計／續燃相關參數 n。</summary>
        public short FireEffect { get; }

        /// <summary>士氣懲罰（負值＝回復）。</summary>
        public short MoralePenalty { get; }

        /// <summary>體力懲罰（負值＝回復）。</summary>
        public short StaminaPenalty { get; }

        /// <summary>進入所需最低船級：-1 一般部隊、0 走舸以上、1 蒙衝以上、2 樓船以上。</summary>
        public sbyte ShipRequirement { get; }

        protected AbstractTerrain(
            TerrainType type,
            string name,
            byte moveCost,
            TerrainFlags flags,
            short fireEffect = 0,
            short moralePenalty = 0,
            short staminaPenalty = 0,
            sbyte shipRequirement = -1)
        {
            TerrainId = (int)type;
            TerrainName = name;
            MoveCost = moveCost;
            TerrainFlags = flags;
            FireEffect = fireEffect;
            MoralePenalty = moralePenalty;
            StaminaPenalty = staminaPenalty;
            ShipRequirement = shipRequirement;
        }

        /// <summary>移動計算：計算部隊離開此格所需行動力並從部隊扣除；不足時不扣，回傳 false。</summary>
        public abstract bool CalculateMovement(Unit unit);

        /// <summary>以 <see cref="MoveCost"/> 依部隊行軍倍率折算後扣除。</summary>
        protected bool SpendMoveCost(Unit unit)
        {
            float speed = UnitMarchSpeed.GetMarchSpeedMultiplier(unit);
            if (speed <= 0f)
                return false;

            int cost = MoveCost;
            if (speed < 1f && cost > 0)
                cost = System.Math.Max(1, (int)(cost / speed));
            return unit.TrySpendMovementPoints(cost);
        }
    }
}
