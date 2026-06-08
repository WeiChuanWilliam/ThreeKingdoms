using System.Collections.Generic;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>從表 id 將道具寫入武將（Set 語意：去重、上限見 officer.properties）。</summary>
    public static class OfficerItemLoader
    {
        public static void ApplyFromIds(Officer officer, int[] itemIds)
        {
            if (officer == null || itemIds == null || itemIds.Length == 0)
                return;

            int max = OfficerConfigUtil.GetMaxItems();
            var seen = new HashSet<int>();
            int added = 0;

            foreach (int id in itemIds)
            {
                if (id <= 0 || !seen.Add(id))
                    continue;
                if (added >= max)
                    break;
                officer.AddItemId(id);
                added++;
            }
        }
    }
}
