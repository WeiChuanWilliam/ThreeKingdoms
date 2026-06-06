# 5 分鐘啟動（第一次）

## 1. 安裝 Unity

**本專案版本：Unity `6000.4.10f1`（Unity 6）**

- [Unity Hub](https://unity.com/download) → **Installs** → **Install Editor** → 選 **6000.4.10f1**
- Hub → **Add** → 選本資料夾 `ThreeKindoms`
- **Windows 與 Mac 必須裝同一版本**（6000.4.10f1）

### Mac Hub 要勾哪些 Platform？（Install Editor → Modules）

| 模組 | 要不要勾 | 說明 |
|------|:--------:|------|
| **Editor application** | ✅ 必選 | 主程式本身 |
| **Documentation** | 可選 | 離線文件；不勾也能用 |
| **Mac Build Support** | ✅ 建議 | 在 Mac 上 **Build Settings → Mac** 打包 `.app`；只在編輯器 Play 可不勾，但建議勾 |
| **Mac Dedicated Server Build Support** | ❌ 不要 | 無頭伺服器建置；本專案用不到 |
| **Windows Build Support** | 可選 | 只在 **Mac 上建 Windows .exe** 才要 |
| **Android / iOS / WebGL …** | ❌ 先不要 | 之後要做再裝 |

**只做 Mac 開發 + Play 測試**：裝 **6000.4.10f1 + Mac Build Support** 就夠。  
**Dedicated Server** 不要勾。

### Windows 建議模組

| 模組 | 建議 |
|------|------|
| **Windows Build Support** | ✅（本機建 exe） |
| **Mac Build Support** | 可選（在 Windows 上建 Mac 版才要） |

---

## 2. 第一次用 Unity 6 開專案（Win / Mac 各做一次）

1. **刪除本機 `Library/`**（若從 2022.3 升級過；不要 commit 此資料夾）
2. Hub 用 **6000.4.10f1** 開啟 `ThreeKindoms`
3. 若跳出 **Upgrade / Migrate**，選 **Continue** / **Upgrade**
4. 等 **Library 重建**、Console 無紅字
5. Unity 可能改寫 `ProjectSettings/`、`Packages/packages-lock.json` → **commit 這些變更**（不要 commit `Library/`）

`ProjectVersion.txt` 已指向 `6000.4.10f1`；其餘 ProjectSettings 以 **Editor 第一次開啟後** 產生的為準。

---

## 3. 建立測試場景（僅第一次）

1. 在 Unity 中：**File → New Scene → 2D**
2. **File → Save As** → `Assets/Scenes/HexSpike.unity`
3. Hierarchy 右鍵 → **Create Empty** → 命名 `Game`
4. 選中 `Game` → **Add Component** → 搜尋 `HexSpikeBootstrap` → 加入
5. **File → Build Settings** → Add Open Scenes（把 HexSpike 加進 build）

---

## 4. Play

- 按 **Play**
- 應看到綠色六角格、紅色單位
- **左鍵** 格子移動；**Shift+左鍵** 預覽路徑；**R** 下一天

---

## 5. 建置

| 平台 | 選項 |
|------|------|
| Windows | **File → Build Settings → Windows → Build** |
| Mac | **File → Build Settings → Mac → Build**（需 Mac Build Support） |

輸出資料夾內含 `_Data`（內有 `StreamingAssets/`）。

---

## 6. 離線測試（不需 Unity）

```bash
cd local-tests
dotnet test ThreeKindoms.Local.Tests/ThreeKindoms.Local.Tests.csproj
```

---

## 疑難排解

| 問題 | 處理 |
|------|------|
| Hub 版本不對 | 確認 Installs 為 **6000.4.10f1**，不要混用 2022.3 |
| 點格子沒反應 | 確認 Main Camera 存在（Bootstrap 會自動建） |
| 全灰沒格子 | 看 Console 是否有 JSON 錯誤 |
| 找不到腳本 | 等 Unity 編譯完成（右下角轉圈） |
| 升級後怪錯 | 刪 `Library/` 後用 6000.4.10f1 重開 |
