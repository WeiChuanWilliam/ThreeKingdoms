namespace ThreeKindoms.Data.Officers
{
    /// <summary>傷病：0 正常、1 輕傷、2 重傷、3 死亡。</summary>
    public enum OfficerInjuryState : byte
    {
        Normal = 0,
        Light = 1,
        Severe = 2,
        Dead = 3
    }

    /// <summary>武將顯示/歸屬狀態（對應 C++ show : 2）</summary>
    public enum OfficerShowState : byte
    {
        NoShow = 0,
        HiddenShow = 1,
        OpenShow = 2,
        Belonged = 3
    }

    public enum OfficerGender : byte
    {
        Male = 0,
        Female = 1
    }

    /// <summary>兵科適性（僅 S／A／B／C；對應步兵／騎兵／弓兵／器械／水軍五類）。</summary>
    public enum TroopAptitudeGrade : byte
    {
        S = 0,
        A = 1,
        B = 2,
        C = 3
    }
}
