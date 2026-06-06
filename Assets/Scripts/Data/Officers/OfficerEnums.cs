namespace ThreeKindoms.Data.Officers
{
    /// <summary>0 健康 … 3 重病（對應 C++ health : 2）</summary>
    public enum HealthLevel : byte
    {
        Fine = 0,
        Slight = 1,
        Medium = 2,
        Serious = 3
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
}
