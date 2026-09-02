namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊負面狀態（計略／異常）抽象基類。
    /// 子類尚未實作天數與結算；預設 <see cref="IsActive"/> = false。
    /// </summary>
    public abstract class UnitNegativeStatus
    {
        /// <summary>是否處於此負面狀態。</summary>
        public virtual bool IsActive => false;
    }

    /// <summary>混亂（status.on_mess）。</summary>
    public sealed class UnitMessStatus : UnitNegativeStatus { }

    /// <summary>偽報（status.on_deceive）。</summary>
    public sealed class UnitDeceiveStatus : UnitNegativeStatus { }

    /// <summary>止步（status.on_trap；部隊狀態，非格子陷阱）。</summary>
    public sealed class UnitStopStatus : UnitNegativeStatus { }

    /// <summary>斷糧。</summary>
    public sealed class UnitCutSupplyStatus : UnitNegativeStatus { }
}
