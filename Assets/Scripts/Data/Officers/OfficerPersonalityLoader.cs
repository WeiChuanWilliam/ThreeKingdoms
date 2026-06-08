using System.Collections.Generic;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>從表 id 將個性寫入武將（Set；金／藍／紅／紫上限見 officer.properties）。</summary>
    public static class OfficerPersonalityLoader
    {
        public static void ApplyFromIds(Officer officer, int[] personalityIds, PersonalityDatabase database)
        {
            if (officer == null || personalityIds == null || personalityIds.Length == 0)
                return;

            int totalMax = OfficerConfigUtil.GetPersonalityTotalMax();
            int goldMax = OfficerConfigUtil.GetPersonalityGoldMax();
            int blueMax = OfficerConfigUtil.GetPersonalityBlueMax();
            int redMax = OfficerConfigUtil.GetPersonalityRedMax();
            int purplePerCategory = OfficerConfigUtil.GetPersonalityPurpleMaxPerCategory();

            var seen = new HashSet<int>();
            var purpleByCategory = new Dictionary<string, int>();
            int gold = 0, blue = 0, red = 0, total = 0;

            foreach (int id in personalityIds)
            {
                if (id <= 0 || !seen.Add(id) || total >= totalMax)
                    continue;

                PersonalityTraitDef trait = database?.GetById(id);
                PersonalityTier tier = ParseTier(trait?.tier);
                string category = trait?.category ?? "";

                if (!CanAddTier(tier, category, ref gold, ref blue, ref red, purpleByCategory,
                        goldMax, blueMax, redMax, purplePerCategory))
                    continue;

                if (trait != null)
                {
                    officer.AddPersonality(trait.id, trait.key, trait.name);
                    officer.AddPersonalityId(trait.id);
                }
                else
                {
                    officer.AddPersonality(id, $"trait_{id}", $"trait_{id}");
                    officer.AddPersonalityId(id);
                }

                total++;
            }
        }

        static bool CanAddTier(
            PersonalityTier tier,
            string category,
            ref int gold,
            ref int blue,
            ref int red,
            Dictionary<string, int> purpleByCategory,
            int goldMax,
            int blueMax,
            int redMax,
            int purplePerCategory)
        {
            switch (tier)
            {
                case PersonalityTier.Gold:
                    if (gold >= goldMax) return false;
                    gold++;
                    return true;
                case PersonalityTier.Blue:
                    if (blue >= blueMax) return false;
                    blue++;
                    return true;
                case PersonalityTier.Red:
                    if (red >= redMax) return false;
                    red++;
                    return true;
                case PersonalityTier.Purple:
                    string cat = string.IsNullOrEmpty(category) ? "_" : category;
                    purpleByCategory.TryGetValue(cat, out int count);
                    if (count >= purplePerCategory) return false;
                    purpleByCategory[cat] = count + 1;
                    return true;
                default:
                    return true;
            }
        }

        static PersonalityTier ParseTier(string tier)
        {
            if (string.IsNullOrEmpty(tier))
                return PersonalityTier.Red;
            return tier switch
            {
                "紫" => PersonalityTier.Purple,
                "金" => PersonalityTier.Gold,
                "藍" or "蓝" => PersonalityTier.Blue,
                "紅" or "红" => PersonalityTier.Red,
                _ => PersonalityTier.Red
            };
        }
    }
}
