# 立库数字孪生 — 标准化产品骨架（落地草案）

本目录是从合晶半成品/成品项目抽出来的**可复用产品边界**，不是完整引擎重构。
目标：下一个项目按「Runtime + Layout + Industry Pack」交付，而不是复制整仓代码。

## 目录

```
ProductStandard/
├── README.md                          ← 本文件
├── MODULES.md                         ← 模块树与职责
├── HUB_AND_REDIS.md                   ← SignalR / Redis 约定
├── contracts/
│   └── AsrsContracts.cs               ← 标准 DTO + 货位判定 + Hub 方法名
├── config/
│   ├── layout.schema.json             ← layout 校验
│   ├── layout.example.json            ← 半成品示例（货位/线体/门/货物类型）
│   └── runtime.config.example.json    ← 对应 StreamingAssets/config.json
└── packs/
    └── semiconductor-ingot.md         ← 半导体晶棒行业包说明
```

## 三层分工

| 层 | 内容 | 项目间是否变化 |
|---|---|---|
| **Runtime** | 相机、点击、SignalR、货位渲染、堆垛机/输送动画壳 | 尽量不变 |
| **Layout** | `layout.json`：货位 Code、场景路径、线体合并、门标签、功能开关 | 每个现场一份 |
| **Industry Pack** | 货物类型别名、Prefab、KPI 字段、UI 文案 | 按行业换包 |

## 与现网代码的对应

| 标准能力 | 半成品现状 |
|---|---|
| StorageStatus + IsEmpty | `StorageStatusDto` + `BoxSystem.IsStorageEmpty` |
| Inventory KPI | `TotalStorageDto` / Redis `RealTimeInventory` |
| 货位双击信息 | `TrayComponentBox` + `UIHelp.IsDoubleClick` |
| 线体编号 Tip | `ConveyorIdTip` |
| 出入口标签 | `GateDirectionLabels` |
| WebGL 点击 | `EquipBase` BoxCollider（禁 MeshCollider） |

## 下一步（可直接排期）

1. 把 `AsrsContracts.StorageOccupancy` 抽到共享程序集，半成品/成品共用。
2. Runtime 启动时读 `layout.json`，用 `cargoTypes` 替代 `BoxSystem` 硬编码别名。
3. 新项目：只改 layout + pack + 场景，不 fork 业务脚本。
