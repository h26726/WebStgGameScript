# STG Project

簡介
- 這是一個 2D 彈幕射擊（STG）遊戲專案，使用 Unity 引擎與 C# 開發。  
- 專案以模組化、事件驅動與物件池為主要設計思路，包含遊戲流程控制、關卡資料序列化、物件池管理、回放系統等子系統。

目的
- 提供可擴充的關卡與角色設定系統（VersionData / StageData / PlayerData）。
- 在效能允許下，維持每幀低 GC 與穩定的主迴圈邏輯。
- 支援儲存/載入遊戲資料（JSON），並可以快速切換測試版本。

檔案與資料夾重點
- Assets/Script/LoadCtrl.cs — 啟動與主迴圈、場景切換、全域系統重置與初始化。
- Assets/Script/Save/SaveJsonData.cs — 儲存/載入 JSON、JsonHelper wrapper。
- Assets/Script/VersionCreate/VersionData.cs — 版本相關資料與取用方法（player / stage / dialog / practice）。
- Assets/Script/Unit/… — 單位控制、AI、行為控制器（ActCtrl）。
- Assets/Script/Game/… — GameMainCtrl、GameDebut、GameObjCtrl、GameProgressStageCtrl、GameReplay 等遊戲核心子系統。
- .github/copilot-instructions.md — 專案開發規範樣板（命名、程式風格等）。

專案概覽
- 啟動層（LoadCtrl）
  - 負責流程：初始化設定、載入儲存資料、初始化物件池、準備 UI/相機、切換頁面（Title ↔ Game）。
  - 主迴圈 Update：驅動 UI 選單的 UpdateHandler 與 GameMainCtrl 的 UpdateHandler。
- 子系統（單例或靜態）
  - GameMainCtrl：遊戲主邏輯，每幀更新遊戲狀態與單位管理。
  - GameObjCtrl：場內物件之建立/回收（配合 ObjectPoolCtrl）。
  - GameDebut：單位出場與出場隊列管理。
  - GameProgressStageCtrl：關卡進度控制、流程觸發。
  - GameReplay：重放錄製/回放邏輯。
  - ObjectPoolCtrl：物件池的建立、借出、回收與統計（LogNum）。
- 資料層
  - VersionData / StageData / PlayerData / PowerData：關卡與版本資料結構。
  - SaveJsonData：提供 Save/Load 接口與 JsonHelper（wrapper for JsonUtility）。

核心流程
1. 啟動（Application 啟動時）
   - [`LoadCtrl.Start()`](Assets/Script/LoadCtrl.cs) 被呼叫，啟動 CoroutineRun()。
   - CoroutineRun() 流程：
     - 播放 loading show 動畫（TryPlayLoadingShowAni）。
     - 初始化 ObjectPool（pool.Init()）。
     - 切換至 Title 頁面（SwitchTitlePageCoroutine）。
   - 同步/非同步讀取儲存資料（[`SaveJsonData.Load...`](Assets/Script/Save/SaveJsonData.cs)），初始化音量、畫面設定等。
   - 建立或清理單位紀錄目錄（CreateUnitLogDirectory / ClearUnitLogDirectoryFile）。

2. Title -> Game 切換
   - 使用者在 Title 選單觸發 Start Game。
   - [`LoadCtrl.SwitchPage(PageIndex)`](Assets/Script/LoadCtrl.cs) → [`SwitchGamePageCoroutine()`](Assets/Script/LoadCtrl.cs)：
     - 顯示 loading；呼叫 [`GameSelect.InitPlayerAndGameCtrlDatas()`](Assets/Script/Game/GameMainCtrl.cs) 建立玩家與關卡資料。
     - 切換相機（[`LoadCtrl.SwitchGameCamera()`](Assets/Script/LoadCtrl.cs)），啟動 [`GameMainCtrl.GameStartSet()`](Assets/Script/Game/GameMainCtrl.cs)。
     - 設定 gameState = Run。

3. 每幀更新（主迴圈）
   - [`LoadCtrl.Update()`](Assets/Script/LoadCtrl.cs)：
     - 迭代 selectList，呼叫每個 Select.UpdateHandler()（處理 UI／選單輸入）。
     - 呼叫 [`GameMainCtrl.Instance.UpdateHandler()`](Assets/Script/Game/GameMainCtrl.cs)（主遊戲邏輯：單位、碰撞、子彈、事件、重放等）。
   - 避免高 GC：Update 路徑內請勿 new 大量物件、避免 LINQ/匿名 delegate/頻繁字串操作。

4. 清理與返回 Title
   - [`LoadCtrl.ClearGameScene()`](Assets/Script/LoadCtrl.cs)：在 gameState 從 Run 轉 Stop 時被呼叫。
     - Reset 或 Clear 各子系統並記錄物件池狀態（ObjectPoolCtrl.LogNum）。

5. 輸入處理與設定映射
   - 輸入先由 UI / 選單層處理（`selectList`），遊戲中由 [`GameMainCtrl`、`GamePlayer` 等接收）](/Assets/Script/Game/GameMainCtrl.cs)。
   - 鍵位設定由 [`SaveJsonData.LoadKeyBoardSaveDatas()`](Assets/Script/Save/SaveJsonData.cs) 載入並應用（KeyBoardSelect）。
   - 支援重綁定、預設回復與輸入防抖處理，確保 replay 系統錄製一致性。

6. 生成與產生排程（Spawn）
   - 關卡腳本（StageData）提供產生時間表與生成參數；由 [`GameProgressStageCtrl`](/Assets/Script/Game/GameProgressStageCtrl.cs) 讀取並觸發。
   - 生成實際使用 ObjectPool（[`ObjectPoolCtrl` ](/Assets/Script/ObjectPoolCtrl.cs)）借出物件以降低 Instantiate/Destroy 的成本。
   - 支援生成延遲、群組與隨機化參數以豐富關卡。

7. 子彈 / 碰撞 / 效能策略
   - 子彈使用輕量資料結構與池化物件，碰撞採用簡化的格子或層級檢測以降低每幀檢查次數。
   - 碰撞回報將只在必要時產生 GC（避免在迴圈內 new string、LINQ 等）。
   - 建議在高密度情況下使用分批更新或固定時間步進控制負載。

8. 得分、UI 與即時顯示更新
   - 得分、生命、道具數量等由遊戲邏輯計算後回傳給 UI（避免每幀重建 UI 元件）。
   - 文字格式化請使用緩存格式或限制更新頻率以免大量 ToString() 造成 GC。

9. 道具、強化與資源管理
   - Power 由關卡或單位掉落觸發，使用物件池管理其生命周期。
   

10. Boss 階段與狀態機
    - Boss 採用階段狀態機（phase），每個 phase 有獨立生成與行為規則，由 [`GameBoss`](/Assets/Script/Game/GameBoss.cs) 管理。
    - 階段切換會觸發特效、BGM 與鏡頭切換（確保平滑過場，不阻塞主迴圈）。

11. 回放（Replay）錄製與回放流程
    - 由 [`GameReplay`](Assets/Script/Game/GameReplay.cs) 記錄關鍵輸入（節省頻寬使用壓縮/時間桶）。
    - 儲存時由 [`SaveJsonData.SaveReplayData()`](Assets/Script/Save/SaveJsonData.cs) 序列化到 JSON；讀取時重建 replayKeyDict 與索引以供回放。
    - 回放模式下遊戲邏輯以回放資料驅動，並關閉不可重現的隨機化或即時輸入。


12. 音效與 BGM 管理
    - BGM、SE 由 ObjectPoolCtrl 或 Audio 管理器統一播放（可動態調整 volume，參考 `LoadCtrl` 內的設定邏輯）。
    - 場景/事件切換時確保音效平滑淡入/淡出以提升體驗。