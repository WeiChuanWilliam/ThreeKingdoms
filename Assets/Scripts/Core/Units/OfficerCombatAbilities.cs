using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Core.Units
{
    /// <summary>武將五維（戰鬥力計算用；優先 <c>*Perform</c>，否則用基礎值）。</summary>
    public readonly struct OfficerCombatAbilities
    {
        public short Attack { get; }
        public short Intelligence { get; }
        public short Leadership { get; }
        public short Policy { get; }
        public short Charisma { get; }

        public OfficerCombatAbilities(short attack, short intelligence, short leadership, short policy, short charisma)
        {
            Attack = attack;
            Intelligence = intelligence;
            Leadership = leadership;
            Policy = policy;
            Charisma = charisma;
        }

        public static OfficerCombatAbilities None => default;

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

        public int SumCombatRelevant() => Attack + Intelligence + Leadership;

        static short PickPerform(short perform, short baseStat) =>
            perform > 0 ? perform : baseStat;

        static short AverageWeighted(short commander, short vice) =>
            (short)((commander * 2 + vice) / 3);
    }
}
