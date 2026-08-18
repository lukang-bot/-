# 模块树（建议物理目录）

按功能切，而不是按「合晶半成品/成品」切。现有 `Assets/Scripts/Core` 可逐步迁入。

```
Runtime/                          # 产品内核（跨项目）
├── Bootstrap/                    # Init、配置加载、BOM 剥离
├── Net/
│   ├── HubClient.cs              # SignalR 连接、Invoke/On
│   └── HubMethodNames.cs         # ← contracts/DtHubMethods
├── Interaction/
│   ├── MouseHelper.cs
│   ├── RayUtil.cs
│   ├── DoubleClick.cs            # UIHelp.IsDoubleClick
│   └── BlankClickHideTips.cs
├── Storage/
│   ├── StorageOccupancy.cs       # ← contracts/StorageOccupancy
│   ├── SlotRegistry.cs           # Code → 场景物体
│   └── CargoVisualBinder.cs      # Status → Prefab 实例
├── Equip/
│   ├── EquipBase.cs              # BoxCollider only
│   ├── CraneDriver.cs
│   └── ConveyorDriver.cs
├── Tips/
│   ├── ConveyorIdTip.cs
│   └── GateDirectionLabels.cs
└── Ui/
    ├── MainUiShell.cs
    └── TrayInfoUi.cs

Layout/                           # 每现场一份（StreamingAssets 或 Addressables）
├── layout.json
└── runtime.config.json

IndustryPacks/
├── semiconductor-ingot/          # 合晶半成品
│   ├── pack.json                 # cargoTypes / KPI keys
│   └── prefabs/                  # 晶棒、AS-CUT…
├── semiconductor-finished/       # 合晶成品
└── generic-asrs/                 # 默认空包

ProjectOverlay/                   # 仅本项目特有（尽量少）
├── Scenes/
├── Art/
└── Scripts.Project/              # 禁止堆通用逻辑
```

## 模块职责（一句话）

| 模块 | 做什么 | 不做什么 |
|---|---|---|
| HubClient | 拉全量、收推送、解 JSON | 不解业务规则 |
| StorageOccupancy | 有货/无货判定 | 不决定 Prefab |
| CargoVisualBinder | Type→Prefab、挂载到货位 | 不写死晶棒字符串（读 pack） |
| SlotRegistry | layout.storageSlots + 场景扫描 | 不调 WMS |
| ConveyorIdTip | 单击显示编号、合并 1004&1005 | 不做任务动画 |
| GateDirectionLabels | 入口/出口 billboard | 不绑业务状态 |
| EquipBase | 可点、高亮、报警色 | 不用 MeshCollider |

## Feature 开关（layout.features）

| key | 关闭时 |
|---|---|
| storageCargoVisual | 只显示空货位网格 |
| taskAnimation | 任务只打日志/列表 |
| inventoryKpi | 主界面无库存图 |
| equipAlarm | 设备不闪报警色 |
| conveyorIdTip | 单击线体无编号 |
| gateLabels | 无出入口字 |

新项目默认全开；演示机可关 taskAnimation 减负。
