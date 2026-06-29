using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Core.Units
{
    /// <summary>武將五維（戰鬥力計算用；優先 <c>*Perform</c>，否則用基礎值）。</summary>
    public readonly struct OfficerCombatAbilities
    {
        /// <summary>武力（戰鬥力相關）。</summary>
        public short Attack { get; }

        /// <summary>智力（戰鬥力相關）。</summary>
        public short Intelligence { get; }

        /// <summary>統率（戰鬥力相關）。</summary>
        public short Leadership { get; }

        /// <summary>政治（納入結構，戰鬥力公式目前未用）。</summary>
        public short Policy { get; }

        /// <summary>魅力（納入結構，戰鬥力公式目前未用）。</summary>
        public short Charisma { get; }

        /// <summary>以五維數值建立武將能力快照。</summary>
        public OfficerCombatAbilities(short attack, short intelligence, short leadership, short policy, short charisma)
        {
            Attack = attack;
            Intelligence = intelligence;
            Leadership = leadership;
            Policy = policy;
            Charisma = charisma;
        }

        /// <summary>空武將能力的預設值。</summary>
        public static OfficerCombatAbilities None => default;

        /// <summary>從武將實例擷取戰鬥用五維（優先 Perform 欄位）。</summary>
        public static OfficerCombatAbilities FromOfficer(Officer officer)
        {
            if (officer == null)
                return None;

            return new OfficerCombatAbilities(
                PickPerform(officer.AttackPerform, officer.Attack),
                PickPerform(officer.IntelligencePerform, officer.Intelligence),
                PickPerform(officer.LeadershipPerform, officer.Leadership),
                PickPerform(officer.PolicyPerform, officer.Policy),
                PickPerform(officer.CharismaPerform, officer.Charisma));
        }

        /// <summary>主將權重 2、副將權重 1 的合算（僅副將時回傳副將能力）。</summary>
        public static OfficerCombatAbilities BlendCommanderAndVice(Officer commander, Officer vice)
        {
            OfficerCombatAbilities cmd = FromOfficer(commander);
            OfficerCombatAbilities v = FromOfficer(vice);

            if (commander == null)
                return v;
            if (vice == null)
                return cmd;

            return new OfficerCombatAbilities(
                AverageWeighted(cmd.Attack, v.Attack),
                AverageWeighted(cmd.Intelligence, v.Intelligence),
                AverageWeighted(cmd.Leadership, v.Leadership),
                AverageWeighted(cmd.Policy, v.Policy),
                AverageWeighted(cmd.Charisma, v.Charisma));
        }

        /// <summary>戰鬥力公式使用的三維能力總和（武／智／統）。</summary>
        public int SumCombatRelevant() => Attack + Intelligence + Leadership;

        static short PickPerform(short perform, short baseStat) =>
            perform > 0 ? perform : baseStat;

        static short AverageWeighted(short commander, short vice) =>
            (short)((commander * 2 + vice) / 3);
    }
}
