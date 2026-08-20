# Feature: simulation

在**不接真 Hub** 的情况下跑通布局：货位显隐、设备走动、简易任务/AGV 跟踪。  
用于售前演示、布局验证、培训；不是 WCS 仿真替代品。

## 架构

```
IDataFeed
├── RealHubFeed      → 现网 SignalR（现场）
└── MockHubFeed      → ScenarioPlayer + SimParams（仿真）
         ↓
    同一套 Runtime（SlotRegistry / EquipBus / Occupancy / AgvDriver…）
```

驱动与 Binder **禁止** `if (isSim)` 散落；只依赖 Feed 推送的契约 DTO。

## 开哪些 features

仿真推荐默认：

| feature | 值 |
|---|---|
| storageCargoVisual | true |
| taskAnimation | true（简化点到点即可） |
| inventoryKpi | true（Mock 摘要） |
| equipAlarm | 可选 |
| agvRealtime | 有 AGV 则 true |
| reverseControl | false |
| mesWorkOrder | false |
| wmsInOut | false |

## SimParams

见 `config/sim.params.schema.json` / `sim.params.example.json`。

要点：

- `timeScale`：倍速  
- `seed`：可复现  
- `occupancy.ratio` + 从 pack.`cargoTypes` 抽类型  
- `agv`：路径点列（layout 局部米制或协议单位，与 `motion.unitsToMeters` 一致）  
- `craneTasks`：间隔、从 `storageSlots` 抽样起终点  

## Scenario

`scenarios/*.json`：按仿真时间轴发事件（设货位、推 AGV 位姿、造一条任务）。  
`MockHubFeed` 只回放；复杂逻辑进剧本，不进驱动。

## 能力边界

| 做 | 不做 |
|---|---|
| 几何布局可视化 | 真实调度/路径优化 |
| 参数化占用与车流 | 库存账务、权限 |
| 简易堆垛机/AGV 动画 | PLC 反控闭环 |
| 可复现演示 | 与现场数据对账 |

## 验收

1. 同一 layout + generic pack：Mock 下货位会按占用率亮灭  
2. 打开 agvRealtime：Mock 推送后车移动且不报缺 Hub  
3. 切换 `RealHubFeed` 后 Mock 代码路径不再向设备写数据  
