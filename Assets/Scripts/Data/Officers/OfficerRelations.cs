using System;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>武將人際關係（表資料／執行時共用形狀）。</summary>
    [Serializable]
    public class OfficerRelations
    {
        /// <summary>親愛武將（最多五名）。</summary>
        public int[] BelovedOfficerIds = Array.Empty<int>();

        /// <summary>厭惡武將。</summary>
        public int[] HatedOfficerIds = Array.Empty<int>();

        /// <summary>義兄弟（最多三名）。</summary>
        public int[] SwornBrotherIds = Array.Empty<int>();

        /// <summary>配偶武將 id；男最多三名、女最多一名（見 <see cref="OfficerRelationsRules"/>）。</summary>
        public int[] SpouseOfficerIds = Array.Empty<int>();
    }

    /// <summary>人際欄位上限與驗證。</summary>
    public static class OfficerRelationsRules
    {
        public static int MaxBeloved => OfficerConfigUtil.GetMaxBelovedOfficers();
        public static int MaxSwornBrothers => OfficerConfigUtil.GetMaxSwornBrothers();
        public static int MaxSpouses(OfficerGender gender) =>
            gender == OfficerGender.Female
                ? OfficerConfigUtil.GetMaxSpousesFemale()
                : OfficerConfigUtil.GetMaxSpousesMale();

        public static bool IsWithinSpouseLimit(OfficerGender gender, int count) =>
            count >= 0 && count <= MaxSpouses(gender);
    }
}
