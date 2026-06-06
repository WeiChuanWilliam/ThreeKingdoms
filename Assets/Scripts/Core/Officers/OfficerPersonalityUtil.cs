using System;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    public static class OfficerPersonalityUtil
    {
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
