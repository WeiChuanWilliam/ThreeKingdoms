using System;

namespace ThreeKindoms.Data.Items
{
    /// <summary>道具定義（C++ Item 佔位）。</summary>
    [Serializable]
    public struct ItemDef
    {
        public int Id;
        public string Key;
        public string DisplayName;
    }

    /// <summary>武將持有道具實例。</summary>
    [Serializable]
    public struct ItemInstance
    {
        public int ItemDefId;
        public short Count;
    }
}
