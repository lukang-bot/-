# 模块树

按能力切，不按客户名切。现有项目的 `Assets/Scripts/Core` 可逐步迁入 Runtime。

```
Runtime/                          # 产品内核（跨项目，无货种词）
├── Bootstrap/
├── Net/
│   ├── HubClient.cs              # 连接、Invoke/On；可选方法失败则跳过
│   ├── IDataFeed.cs              # RealHubFeed | MockHubFeed
│   └── HubMethodNames.cs         # ← contracts/DtHubMethods
├── Interaction/                  # 详见 INTERACTION.md
│   ├── InteractableTarget.cs     # kind + identity
│   ├── ClickColliderUtil.cs      # 仅 BoxCollider
│   ├── InteractionRaycaster.cs
│   ├── DoubleClickGate.cs        # 默认 0.3s
│   ├── InteractionRouter.cs      # Kind → Panel + Query
│   └── HoverHighlightBinder.cs
├── Storage/
├── Equip/
│   ├── EquipBase.cs
│   ├── EquipBus.cs
│   ├── CraneDriver.cs
│   ├── ConveyorDriver.cs
│   └── AgvDriver.cs              # features/agv-realtime.md
├── Control/
├── Task/
└── Ui/

Editor/SceneBuilder/              # 仅编辑器；features/scene-builder.md
├── PaletteWindow.cs
├── LayoutSerializer.cs
└── LayoutValidator.cs

Simulation/                       # features/simulation.md
├── MockHubFeed.cs
├── ScenarioPlayer.cs
├── SimClock.cs
└── SimHud.cs

Layout/                           # 每现场一份
├── layout.json
├── runtime.config.json
└── sim.params.json               # 仅仿真

IndustryPacks/
scenarios/                        # 仿真剧本（可随标准库或项目）
ProjectOverlay/
```

## 职责

| 模块 | 做什么 | 不做什么 |
|---|---|---|
| InteractionRouter | 双击后按 Kind 开 Panel 并拉数 | 不解析网格 |
| ClickColliderUtil | 挂/修 BoxCollider | 不用 MeshCollider 点击 |
| HubClient / IDataFeed | 拉全量、收推送；仿真走 Mock | 不解占用、驱动内不写 isSim |
| StorageOccupancy | 按 pack.rule 判断空/有货 | 不决定 Prefab、不写死 token |
| CargoVisualBinder | match → Prefab，挂到货位 | 不写死货物名 |
| SlotRegistry | layout.storageSlots + 场景 | 不调 WMS |
| EquipBus | 按类型加载驱动 | 现场没有的设备不实例化 |
| AgvDriver | 位姿跟踪、朝向、载货显隐 | 不规划路径 |
| SceneBuilder | 拖拽写 layout.json | 不做调度与库存账 |
| MockHubFeed | 按 SimParams/Scenario 推契约 DTO | 不替代真 WCS |
| ReverseControlClient | POST pack 声明的路径 | 不进 SignalR |
| TaskPlayer | 从 layout 点位做动画 | 不解析行业 KPI |

## Feature 开关（layout.features）

| key | 关闭时 |
|---|---|
| storageCargoVisual | 只显示空货位 |
| taskAnimation | 任务只进列表 |
| inventoryKpi | 无库存图 |
| equipAlarm | 设备不闪报警色 |
| doubleClickInfoUi | 双击不打开详情 UI |
| conveyorIdTip | 单击线体无编号 |
| hoverHighlight | 悬停无高亮 |
| gateLabels | 无出入口字 |
| agvRealtime | 不订阅 AGV、不跑车上跟踪 |
| reverseControl | 报警页无反控按钮 |
| mesWorkOrder | 无工单面板 |
| wmsInOut | 无出入库/盘点面板 |

新产品默认：货位可视化 / 报警 / KPI 开；AGV / 反控 / MES 关。  
仿真推荐再开 `taskAnimation` +（有车时）`agvRealtime`，关反控/MES/出入库。
