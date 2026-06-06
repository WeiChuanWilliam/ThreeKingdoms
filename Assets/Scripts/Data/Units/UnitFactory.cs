using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Units
{
    /// <summary>等同直接 {@code new Combat(def)}；保留給習慣用 Factory 的呼叫端。</summary>
    public static class UnitFactory
    {
        public static Unit Create(UnitDef def) => def switch
        {
            CombatUnitDef combat => new Combat(combat),
            LegionUnitDef legion => new Legion(legion),
            TransportUnitDef transport => new Transport(transport),
            _ => throw new System.ArgumentException($"Unknown UnitDef: {def?.GetType().Name}")
        };
    }
}
