# ThreeKindoms 測試（Unity 可選）

**日常本機單元測試請用：`local-tests/` → `dotnet test`（不需開 Unity、不編整包專案）。**  
見 [local-tests/README.md](../../local-tests/README.md)。

以下為 **Unity 編輯器內** 可選方式（會載入整個 Assembly-CSharp）。

## 怎麼跑（三選一）

### 1. 選單（推薦，等同「Main」）

Unity 頂部選單：

```text
ThreeKindoms → Tests → Run All (Main)
```

- 一次跑完下面 4 類測試  
- 結果在 **Console**；`GameTestSuite.LastCombinedReport` 有完整文字  
- 退出碼概念：`LastExitCode` 0=全過、1=有失敗  

單項測試：

| 選單 | 內容 |
|------|------|
| Combat / 全兵種 5000 | 每種兵種 `Combat` 5000 兵、無技能、六圍對表 |
| Unit Properties / 鍵與六圍 | `unit.properties` 顯示名 + 六圍齊全 |
| Troop Tree / 升級樹 | registry 與 `TroopKindTree` 一致、水軍鏈 |
| Troop Registry / 電話簿 | 37 兵種、拼音別名、白马對弓 1.2x |

**注意**：Play 模式中選單會灰掉，請在編輯模式執行。

### 2. Test Runner（自動化）

1. 安裝後打開 **Window → General → Test Runner**  
2. 切到 **EditMode**  
3. 跑 `GameEditModeTests`  

（需 Package：`com.unity.test-framework`，已寫入 `Packages/manifest.json`）

### 3. 場景 Play（可選）

場景物件掛 `CombatTroopKindTestBootstrap`（`Assets/Scripts/Unity/`），勾 **Run On Start** 或右鍵執行。  
現在內部應改呼叫 `GameTestSuite.RunAll`（若仍只跑 combat，以選單為準）。

## 目錄結構

```text
Assets/Tests/
  README.md                 ← 本說明
  GameTestSuite.cs          ← 總入口（RunAll =「Main」）
  GameTestResult.cs
  Runners/
    CombatTroopKindTestRunner.cs
    UnitPropertiesTestRunner.cs
    TroopKindTreeTestRunner.cs
    TroopKindRegistryTestRunner.cs
  Editor/
    GameTestMenu.cs         ← ThreeKindoms/Tests 選單
  EditMode/
    GameEditModeTests.cs    ← NUnit 自動測試
```

## 為什麼沒有 Main？

- 遊戲邏輯編在 **Assembly-CSharp**，由 Unity 載入，不是控制台專案。  
- `GameTestSuite.RunAll(path)` 就是程式化入口；Editor 選單與 NUnit 都呼叫它。  

若日後要 **命令列**（CI），可用：

```bash
Unity.exe -batchmode -quit -projectPath "..." -runTests -testPlatform editmode
```

## 依賴

- `Assets/StreamingAssets/chinese/unit.properties` 必須存在  
- 不需武將池、不需進 Play 場景（EditMode 即可）
