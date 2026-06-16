namespace ThreeKindoms.Data.Officers

{

    /// <summary>傷勢程度（與死亡分離；<see cref="OfficerFlag.IsAlive"/> 表示是否存活）。</summary>

    public enum OfficerInjuryState : byte

    {

        /// <summary>0 — 無傷／正常。</summary>

        Normal = 0,



        /// <summary>1 — 輕傷。</summary>

        Light = 1,



        /// <summary>2 — 中傷。</summary>

        Medium = 2,



        /// <summary>3 — 重傷。</summary>

        Severe = 3

    }



    /// <summary>武將在劇本中的可見／登用狀態（<c>show</c>）。</summary>

    public enum OfficerShowState : byte

    {

        /// <summary>0 — 尚未登場：未滿 15 歲，或劇本尚未讓其出現。</summary>

        NoShow = 0,



        /// <summary>1 — 隱藏：已在劇本登場，需探索／發現人才後才可見。</summary>

        HiddenShow = 1,



        /// <summary>2 — 在野可見：可直接登用。</summary>

        OpenShow = 2,



        /// <summary>3 — 已歸屬勢力。</summary>

        Belonged = 3

    }



    public enum OfficerGender : byte

    {

        Male = 0,

        Female = 1

    }



    /// <summary>兵科適性：0＝C（最差）… 3＝S（最好）。</summary>

    public enum TroopAptitudeGrade : byte

    {

        C = 0,

        B = 1,

        A = 2,

        S = 3

    }

}

