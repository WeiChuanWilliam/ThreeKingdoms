using System;

namespace ThreeKindoms.Data.Skill
{
    /// <summary>兵科/裝備系統戰法（對應 C++ BattleSkill 成員）。</summary>
    [Serializable]
    public struct Skill
    {
        public int SkillId;
        public byte Level;
        public bool Enabled;
    }
}
