namespace ThreeKindoms.Data.Units.TroopKinds
{
    // ========== 各兵科 abstract：不可 new，只給底下具體兵種繼承 ==========

    /// <summary>槍兵線（長槍→戟兵→青州/大戟）。</summary>
    public abstract class AbstractSpearTroopKind : AbstractTroopKind { }

    /// <summary>重步兵線（重步→精銳步→陷陣/白毦）。</summary>
    public abstract class AbstractArmorTroopKind : AbstractTroopKind { }

    /// <summary>重騎線（重騎→精銳騎→虎豹/西涼）。</summary>
    public abstract class AbstractKnightTroopKind : AbstractTroopKind { }

    /// <summary>游騎線（游騎→烏桓→并州/白馬）。</summary>
    public abstract class AbstractHorsemanTroopKind : AbstractTroopKind { }

    /// <summary>弓線（長弓→精銳弓→無當/丹陽）。</summary>
    public abstract class AbstractBowTroopKind : AbstractTroopKind { }

    /// <summary>弩線（弩→重弩→先登/連弩）。</summary>
    public abstract class AbstractCrossbowTroopKind : AbstractTroopKind { }

    /// <summary>衝車線（衝車→木獸/象兵）。</summary>
    public abstract class AbstractSiegeChargerTroopKind : AbstractTroopKind { }

    /// <summary>井欄線（井欄→投石/弩床）。</summary>
    public abstract class AbstractSiegeShooterTroopKind : AbstractTroopKind { }

    /// <summary>水軍兵科（走舸→蒙衝→樓船→鬥艦）。</summary>
    public abstract class AbstractNavyTroopKind : AbstractTroopKind { }
}
