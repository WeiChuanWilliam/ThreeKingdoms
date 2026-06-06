using System.Collections.Generic;

namespace ThreeKindoms.Data.Skill
{
    public sealed class SkillByIdComparer : IEqualityComparer<Skill>
    {
        public static readonly SkillByIdComparer Instance = new();

        public bool Equals(Skill x, Skill y) => x.SkillId == y.SkillId;

        public int GetHashCode(Skill obj) => obj.SkillId;
    }
}
