using System.Collections.Generic;

namespace ThreeKindoms.Data.Skill
{
    /// <summary>
    /// 技能模板池。部隊用 <see cref="CopyForUnit"/> 取得複本；查詢定義用 <see cref="TryGetTemplate"/>。
    /// </summary>
    public static class SkillPool
    {
        static readonly Dictionary<int, Skill> templates = new();

        public static void Register(Skill template)
        {
            if (template.SkillId <= 0) return;
            templates[template.SkillId] = template;
        }

        public static void Register(int skillId, byte level = 1, bool enabled = true) =>
            Register(new Skill { SkillId = skillId, Level = level, Enabled = enabled });

        public static bool TryGetTemplate(int skillId, out Skill skill) =>
            templates.TryGetValue(skillId, out skill);

        /// <summary>部隊攜帶用：複製模板（無模板則建立預設 Lv.1）。</summary>
        public static Skill CopyForUnit(int skillId)
        {
            if (skillId <= 0)
                return default;
            if (TryGetTemplate(skillId, out Skill template))
                return template;
            return new Skill { SkillId = skillId, Level = 1, Enabled = true };
        }
    }
}
