using System.Text;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Local.Tests.Runners
{
    /// <summary>將 <see cref="Officer"/> 格式化成一行，供 local 測試輸出。</summary>
    public static class OfficerDisplay
    {
        public static string FormatLine(Officer o)
        {
            if (o == null) return "ERROR (null officer)";

            var sb = new StringBuilder();
            sb.Append($"id={o.RuntimeId} {o.DisplayName}");
            sb.Append($" | 統{o.LeadershipPerform}({o.Leadership})");
            sb.Append($" 武{o.AttackPerform}({o.Attack})");
            sb.Append($" 智{o.IntelligencePerform}({o.Intelligence})");
            sb.Append($" 政{o.PolicyPerform}({o.Policy})");
            sb.Append($" 魅{o.CharismaPerform}({o.Charisma})");
            sb.Append($" | 體{o.Stamina} 相{o.Compatibility}");
            sb.Append($" 生{o.BirthYear} 壽{o.Lifespan}→{o.DeathYear}");
            sb.Append($" | 勢{o.Belong} 忠{o.Loyalty}");
            sb.Append($" | 適{FormatAptitude(o)}");
            sb.Append($" | 個性{o.Personalities.Count} 道具{o.ItemIds.Count}");
            sb.Append($" | 親{o.BelovedOfficerIds.Count} 義{o.SwornBrotherIds.Count}");
            if (!o.IsAlive)
                sb.Append(" [陣亡]");
            else if (o.Injury != OfficerInjuryState.Normal)
                sb.Append($" 傷{o.Injury}");
            return sb.ToString();
        }

        static string FormatAptitude(Officer o)
        {
            OfficerTroopAptitude a = o.TroopAptitude;
            return $"步{Grade(a.Infantry)}騎{Grade(a.Cavalry)}弓{Grade(a.Archer)}器{Grade(a.Siege)}水{Grade(a.Navy)}";
        }

        static char Grade(TroopAptitudeGrade g) => g switch
        {
            TroopAptitudeGrade.S => 'S',
            TroopAptitudeGrade.A => 'A',
            TroopAptitudeGrade.B => 'B',
            _ => 'C'
        };
    }
}
