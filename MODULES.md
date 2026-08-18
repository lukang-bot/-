# 模块树

按能力切，不按客户名切。现有项目的 `Assets/Scripts/Core` 可逐步迁入 Runtime。

```
Runtime/                          # 产品内核（跨项目，无货种词）
├── Bootstrap/                    # Init、配置加载、UTF-8 BOM 剥离
├── Net/
│   ├── HubClient.cs              # 连接、Invoke/On；可选方法失败则跳过
│   └── HubMethodNames.cs         # ← contracts/DtHubMethods
├── Interaction/                  # 射线、单击/双击、点空白关 Tip
├── Storage/
│   ├── StorageOccupancy.cs       # ← contracts，rule 来自 pack
│   ├── SlotRegistry.cs           # Code → 场景物体（读 layout）
│   └── CargoVisualBinder.cs      # 占用 + cargoTypes → Prefab
├── Equip/
│   ├── EquipBase.cs              # 可点、状态色；WebGL 用 BoxCollider
│   ├── EquipBus.cs               # 按 layout.equips 注册驱动
│   ├── CraneDriver.cs            # 可选
│   ├── ConveyorDriver.cs         # 可选
│   └── AgvDriver.cs              # 可选
├── Control/
│   └── ReverseControlClient.cs   # 可选；HTTP 不是 Hub
├── Task/
│   └── TaskPlayer.cs             # 点到点动画壳
└── Ui/
    ├── MainUiShell.cs
    └── InfoPanels.cs             # 货位/设备/报警；反控按钮由 feature 决定

Layout/                           # 每现场一份
├── layout.json
└── runtime.config.json           # 对应 StreamingAssets/config.json

IndustryPacks/
├── generic/
├── semiconductor-ingot/
└── {your-pack}/

ProjectOverlay/                   # 仅本现场：场景、美术、极少脚本
```

## 职责

| 模块 | 做什么 | 不做什么 |
|---|---|---|
| HubClient | 拉全量、收推送 | 不解占用、不写死方法是否必须成功 |
| StorageOccupancy | 按 pack.rule 判断空/有货 | 不决定 Prefab、不写死 token |
| CargoVisualBinder | match → Prefab，挂到货位 | 不写死货物名 |
| SlotRegistry | layout.storageSlots + 场景 | 不调 WMS |
| EquipBus | 按类型加载驱动 | 现场没有的设备不实例化 |
| ReverseControlClient | POST pack 声明的路径 | 不进 SignalR |
| TaskPlayer | 从 layout 点位做动画 | 不解析行业 KPI |

## Feature 开关（layout.features）

| key | 关闭时 |
|---|---|
| storageCargoVisual | 只显示空货位 |
| taskAnimation | 任务只进列表 |
| inventoryKpi | 无库存图 |
| equipAlarm | 设备不闪报警色 |
| conveyorIdTip | 单击线体无编号 |
| gateLabels | 无出入口字 |
| agvRealtime | 不订阅 AGV |
| reverseControl | 报警页无反控按钮 |
| mesWorkOrder | 无工单面板 |
| wmsInOut | 无出入库/盘点面板 |

新产品默认：货位可视化 / 报警 / KPI 开；AGV / 反控 / MES 关，现场有再开。
