using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Units
{
    /// <summary>部隊名後綴；優先讀 <see cref="UnitConfigUtil"/>（chinese/unit.properties）。</summary>
    public static class UnitNamingSettings
    {
        public static string Locale
        {
            get => UnitConfigUtil.IsLoaded ? UnitConfigUtil.Locale : "zh";
            set { /* 以 properties 為準，保留 API 相容 */ }
        }

        public static string GetSuffix(UnitKind kind, string locale = null)
        {
            if (UnitConfigUtil.IsLoaded)
                return UnitConfigUtil.GetSuffix(kind);

            if (locale != null && locale.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            {
                return kind switch
                {
                    UnitKind.Legion => " Legion",
                    UnitKind.Transport => " Transport",
                    _ => " Company"
                };
            }

            return kind switch
            {
                UnitKind.Legion => "兵團",
                UnitKind.Transport => "運輸隊",
                UnitKind.Garrison => "駐軍",
                _ => "隊"
            };
        }
    }
}
