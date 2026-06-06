# 本機單元測試（不用開 Unity）

只編譯 **遊戲邏輯**（`Assets/Scripts` 裡 Core + Data），**排除** `Unity/` 與依賴 `UnityEngine` 的檔案，避免整包 Unity 編譯錯誤。

## 需求

- [.NET SDK](https://dotnet.microsoft.com/download)（本機已測 **10.x**；專案目標 `net10.0`）
- **NuGet 來源**：`local-tests/nuget.config` 已指向 nuget.org（若曾出現 NU1100，多半是沒設套件來源）

## 常見錯誤 NU1100

表示 **NuGet 沒有套件來源**。請在 `local-tests` 目錄執行（會讀同目錄的 `nuget.config`）：

```powershell
cd local-tests
dotnet nuget list source    # 應看到 nuget.org
dotnet restore
dotnet test
```

本機若只有 **.NET 10 SDK**，專案已改為 `net10.0`（不要用 `net8.0` 除非已安裝 8.x SDK）。

## 執行

在專案根目錄（`ThreeKindoms`）或 `local-tests` 下：

```powershell
cd local-tests
.\run-tests.ps1
```

或直接：

```powershell
dotnet run --project ThreeKindoms.TestConsole
```

每一個兵種會 **`Console.WriteLine`** 逐行印出，例如 `OK  [knight.hubao] 攻135 防125 兵5000`，最後 `ALL PASSED`。

（`dotnet test` 仍可用，但預設只顯示 4 條摘要；要看逐行輸出請用上面的 console。）

## 結構

```text
local-tests/
  ThreeKindoms.Local/          ← 連結編譯 Assets/Scripts（無 Unity）
  ThreeKindoms.Local.Tests/    ← Runner + xUnit
  ThreeKindoms.TestConsole/    ← 直接印 console（run-tests.ps1）
  ThreeKindoms.Local.sln
```

## 測試內容

| 測試類 | 內容 |
|--------|------|
| `CombatTroopKindTests` | 37 兵種 × Combat 5000 兵、六圍 |
| `UnitPropertiesTests` | unit.properties 鍵齊全 |
| `TroopKindTreeTests` | 升級樹、水軍鏈 |
| `TroopKindRegistryTests` | 37 種、別名、白马 1.2x |

`unit.properties` 會複製到測試輸出目錄；也可從 repo 向上搜尋 `Assets/StreamingAssets/chinese/unit.properties`。

## 與 `Assets/Tests` 的關係

| 位置 | 用途 |
|------|------|
| **local-tests/** | 本機 `dotnet test`，日常開發用 |
| **Assets/Tests/** | 可選：Unity 選單 / Test Runner（需開編輯器） |

新增邏輯測試請優先寫在 `local-tests/ThreeKindoms.Local.Tests/`。

## 新增測試

1. 在 `ThreeKindoms.Local.Tests/` 加 `*Tests.cs`
2. `using Xunit;` + `[Fact]`
3. 需要設定檔時用 `TestPaths.UnitPropertiesPath`

若新程式碼放在 `Assets/Scripts` 且引用 `UnityEngine`，請把該檔加入 `ThreeKindoms.Local.csproj` 的 `<Compile Remove="..."/>` 列表。
