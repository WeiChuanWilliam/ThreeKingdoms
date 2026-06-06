using System;

namespace ThreeKindoms.Data.Battle
{
    /// <summary>武將六系統戰法槽位（shield / cavalry / spear / archery / marine / equipment）。</summary>
    [Serializable]
    public struct BattleSkill
    {
        public int SkillId;
    }

    /// <summary>武將六系統戰法（對應 C++ BattleSkill 成員集合）。</summary>
    [Serializable]
    public struct OfficerBattleSkills
    {
        public BattleSkill ShieldForce;
        public BattleSkill CavalryForce;
        public BattleSkill SpearForce;
        public BattleSkill ArcheryForce;
        public BattleSkill MarineForce;
        public BattleSkill EquipmentForce;
    }
}
