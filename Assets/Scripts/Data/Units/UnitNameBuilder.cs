using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Units
{
    public static class UnitNameBuilder
    {
        public static string Resolve(UnitDef def, UnitKind kind)
        {
            if (def == null) return UnitConfigUtil.FallbackUnitName;

            if (!string.IsNullOrWhiteSpace(def.CustomUnitName))
                return def.CustomUnitName.Trim();

            string commanderName = GetCommanderDisplayName(def.CommanderOfficerId);
            string suffix = UnitNamingSettings.GetSuffix(kind);

            if (string.IsNullOrEmpty(commanderName))
                return UnitConfigUtil.FallbackUnitName + suffix;

            return commanderName + suffix;
        }

        static string GetCommanderDisplayName(int commanderDefId)
        {
            if (commanderDefId <= 0) return "";
            Officer shared = OfficerPool.GetShared(commanderDefId);
            if (shared == null)
                return $"{UnitConfigUtil.FallbackOfficerPrefix}{commanderDefId}";
            return shared.FullName;
        }
    }
}
