using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Battle;
using ThreeKindoms.Data.Skill;

namespace ThreeKindoms.Core.Units
{
    /// <summary>從主將／副將彙整技能至部隊 SET（去重）；分類寫入四槽留待擴充。</summary>
    public static class CombatSkillAssembler
    {
        /// <summary>
        /// 從主將與副將的 <see cref="OfficerBattleSkills"/> 收集技能 id，
        /// 併入 <paramref name="target"/>（HashSet 自動去重，同 id 只算一次）。
        /// </summary>
        public static void CollectMergedOfficerSkillIds(Combat combat, HashSet<int> target)
        {
            if (combat == null || target == null)
                return;

            CollectFromOfficer(combat.Commander, target);
            CollectFromOfficer(combat.ViceOfficer, target);
        }

        /// <summary>更新 <see cref="Combat.OfficerSkillIds"/>，並呼叫分類寫入（目前為 stub）。</summary>
        public static void RefreshEquippedSkills(Combat combat)
        {
            if (combat == null)
                return;

            combat.ClearOfficerSkillIds();
            CollectMergedOfficerSkillIds(combat, combat.OfficerSkillIdSet);

            combat.ClearEquippedSkillSets();
            PopulateSkillSetsFromMergedSkills(combat, combat.OfficerSkillIds);
        }

        /// <summary>
        /// TODO：依技能表 <see cref="SkillCategory"/> 將合併後的技能 id 寫入
        /// battle / strategy / mobility / defence 四個 SET。
        /// </summary>
        public static void PopulateSkillSetsFromMergedSkills(Combat combat, IReadOnlyCollection<int> mergedSkillIds)
        {
            if (combat == null || mergedSkillIds == null)
                return;

            // TODO: SkillPool.TryGetCategory(skillId) → combat.Add*Skill(skillId)
        }

        static void CollectFromOfficer(Officer officer, HashSet<int> target)
        {
            if (officer == null)
                return;

            ref OfficerBattleSkills skills = ref officer.BattleSkills;
            TryAddSkillId(skills.ShieldForce.SkillId, target);
            TryAddSkillId(skills.CavalryForce.SkillId, target);
            TryAddSkillId(skills.SpearForce.SkillId, target);
            TryAddSkillId(skills.ArcheryForce.SkillId, target);
            TryAddSkillId(skills.MarineForce.SkillId, target);
            TryAddSkillId(skills.EquipmentForce.SkillId, target);
        }

        static void TryAddSkillId(int skillId, HashSet<int> target)
        {
            if (skillId > 0)
                target.Add(skillId);
        }
    }
}
