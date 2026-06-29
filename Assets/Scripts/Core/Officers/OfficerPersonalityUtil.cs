using System;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>武將個性查詢輔助。</summary>
    public static class OfficerPersonalityUtil
    {
        /// <summary>判斷武將是否持有指定名稱或鍵值的個性。</summary>
        public static bool HasTrait(AbstractOfficer officer, string traitName)
        {
            if (officer == null || string.IsNullOrEmpty(traitName))
                return false;
            foreach (PersonalityDef p in officer.Personalities)
            {
                if (string.Equals(p.DisplayName, traitName, StringComparison.Ordinal))
                    return true;
                if (string.Equals(p.Key, traitName, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }
    }
}
