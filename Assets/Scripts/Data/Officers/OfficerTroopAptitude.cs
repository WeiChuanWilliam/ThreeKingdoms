using System;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>武將對五大兵科（步兵／騎兵／弓兵／器械／水軍）的適性。</summary>
    [Serializable]
    public struct OfficerTroopAptitude
    {
        /// <summary>步兵（含刀兵、槍系、甲系等）。</summary>
        public TroopAptitudeGrade Infantry;

        /// <summary>騎兵。</summary>
        public TroopAptitudeGrade Cavalry;

        /// <summary>弓兵。</summary>
        public TroopAptitudeGrade Archer;

        /// <summary>器械。</summary>
        public TroopAptitudeGrade Siege;

        /// <summary>水軍。</summary>
        public TroopAptitudeGrade Navy;

        public static OfficerTroopAptitude DefaultC => new()
        {
            Infantry = TroopAptitudeGrade.C,
            Cavalry = TroopAptitudeGrade.C,
            Archer = TroopAptitudeGrade.C,
            Siege = TroopAptitudeGrade.C,
            Navy = TroopAptitudeGrade.C
        };

        public TroopAptitudeGrade Get(TroopType troop) => troop switch
        {
            TroopType.Cavalry => Cavalry,
            TroopType.Archer => Archer,
            TroopType.Siege => Siege,
            TroopType.Navy => Navy,
            _ => Infantry
        };

        public static OfficerTroopAptitude Normalize(OfficerTroopAptitude raw)
        {
            return new OfficerTroopAptitude
            {
                Infantry = ClampGrade(raw.Infantry),
                Cavalry = ClampGrade(raw.Cavalry),
                Archer = ClampGrade(raw.Archer),
                Siege = ClampGrade(raw.Siege),
                Navy = ClampGrade(raw.Navy)
            };
        }

        static TroopAptitudeGrade ClampGrade(TroopAptitudeGrade grade) =>
            grade > TroopAptitudeGrade.C ? TroopAptitudeGrade.C : grade;
    }
}
