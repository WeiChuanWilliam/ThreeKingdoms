using System;

using ThreeKindoms.Data.Units;



namespace ThreeKindoms.Data.Officers

{

    /// <summary>武將對五大兵科（步兵／騎兵／弓兵／器械／水軍）的適性。0＝C … 3＝S。</summary>

    [Serializable]

    public struct OfficerTroopAptitude

    {

        public TroopAptitudeGrade Infantry;

        public TroopAptitudeGrade Cavalry;

        public TroopAptitudeGrade Archer;

        public TroopAptitudeGrade Siege;

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



        public static OfficerTroopAptitude Normalize(OfficerTroopAptitude raw) => new()

        {

            Infantry = ClampGrade(raw.Infantry),

            Cavalry = ClampGrade(raw.Cavalry),

            Archer = ClampGrade(raw.Archer),

            Siege = ClampGrade(raw.Siege),

            Navy = ClampGrade(raw.Navy)

        };



        static TroopAptitudeGrade ClampGrade(TroopAptitudeGrade grade) =>

            grade > TroopAptitudeGrade.S ? TroopAptitudeGrade.S : grade;

    }

}

