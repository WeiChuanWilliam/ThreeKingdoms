# 5 分鐘啟動（第一次）

## 1. 安裝 Unity

- [Unity Hub](https://unity.com/download) → 安裝 **2022.3 LTS**
- Hub → **Add** → 選本資料夾 `ThreeKindoms`

若版本號與 `ProjectSettings/ProjectVersion.txt` 不同，Hub 會提示升級/降級，選 **2022.3 LTS** 即可。

## 2. 建立測試場景（僅第一次）

1. 在 Unity 中：**File → New Scene → 2D**
2. **File → Save As** → `Assets/Scenes/HexSpike.unity`
3. Hierarchy 右鍵 → **Create Empty** → 命名 `Game`
4. 選中 `Game` → **Add Component** → 搜尋 `HexSpikeBootstrap` → 加入
5. **File → Build Settings** → Add Open Scenes（把 HexSpike 加進 build）

## 3. Play

- 按 **Play**
- 應看到綠色六角格、紅色單位
- **左鍵** 格子移動；**Shift+左鍵** 預覽路徑；**R** 下一天

## 4. 建置 exe

**File → Build Settings → Windows → Build**

輸出資料夾內會有 `.exe` 與 `_Data`（內含 `StreamingAssets/hex_map_sample.json`）。

## 疑難排解

| 問題 | 處理 |
|------|------|
| 點格子沒反應 | 確認 Main Camera 存在（Bootstrap 會自動建） |
| 全灰沒格子 | 看 Console 是否有 JSON 錯誤 |
| 找不到腳本 | 等 Unity 編譯完成（右下角轉圈） |

詳見 [README.md](README.md)。
