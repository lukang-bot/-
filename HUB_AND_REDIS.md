# Hub / Redis / HTTP

方法名保持稳定，降低现场切换成本。  
**Core 必须尝试；Optional 失败则跳过并打日志。**

## 客户端

Unity 现网常见 **BestHTTP SignalR Core + LitJson**，不要写成只能用 `Microsoft.AspNetCore.SignalR.Client`。

WebGL：

- `config.json` / Redis JSON **禁止 UTF-8 BOM**
- LitJson 需注册数字 → 字符串 importer（后端常把编号打成 JSON number）
- 浏览器会拒绝设置 `Connection` 头，可忽略

路径：`{equipDataURL}`，示例 `http://{HOST}:44332/DT`（端口按现场）。

## Core（立库通用）

### 初始化 Invoke

| Method | 返回 | 用途 |
|---|---|---|
| `StorageStatusInfo` | `List<StorageStatusDto>` | 货位全量 |
| `RealTimeInventory` | `InventorySummaryDto`（或适配后的摘要） | KPI |
| `CraneDataInfo` | 堆垛机列表 | 无堆垛机的现场可跳过 |
| `ConveyorDataInfo` | 输送点位 | 无线体的现场可跳过 |

### 推送 On

| Method | 用途 |
|---|---|
| `RealTimeInventorySend` | KPI |
| `CraneData` | 堆垛机 |
| `ConveyorData` | 输送点位 |
| `WcsMainTaskCreateEvent` 及 Running/Finish/Error/Cancel/Pause | 任务动画 |
| `WcsLogicTask*` | 子任务（有则订） |

### 查询

| Method | 用途 |
|---|---|
| `WmsStockDtoByLocation` | 双击货位详情 |

## Optional（按 pack.hub.optional 订阅）

| Method | 用途 |
|---|---|
| `UnitData` / `UnitDataInfo` | 产线单元 |
| `PCConveyorData` / `PCConveyorDataInfo` | 板链组 |
| `CodeDiskData` / `CodeDiskDataInfo` | 码盘供料 |
| `AlarmData` | 设备报警 |
| `AGVStatusSend` / `AGVTaskSend` | AGV 位姿与上下货 |
| `ShuttleData` / `ShuttleDataInfo` | RGV/穿梭车 |
| `DayInAndOut*` / `HourInAndOut*` / `StockTaking*` | WMS 报表 |
| `RealTimeWorkOrder*` | MES 工单 |

## 非 Hub：HTTP 反控

反向控制**不是** SignalR。Runtime 使用独立基址：

`POST {equipmentControlURL}/api/app/equipment/buffer-start`

Body 由 pack 声明字段名（现场曾用 `uintId` 而非 `unitId`，以对端为准）。

## Redis（服务端，Core）

| Key | 类型 | 说明 |
|---|---|---|
| `WmsStorageStatus` | Hash，field=Code | 货位 JSON |
| `WmsStocks` | 业务库存 | 可重建货位 Hash |
| `RealTimeInventory` | String JSON | KPI，形状应对齐 `InventorySummaryDto` |

Hash field 名 = 货位 Code = `layout.storageSlots[].code`（若 `sceneObject` 不同则只保证 Code 与后端一致）。

写入只约定：**有货/无货都能被 pack 的 occupancy 解出来**；不要在产品内核里规定必须写 `Status=exist`。

Optional Redis 示例：AGV 任务/状态 Hash（key 名由现场定，写入 contracts 扩展即可）。

## KPI 兼容

旧现场若仍推 `CrystalBarCount` 等固定字段：在 **该 pack 的适配器** 转成 `CountsByType`，不要把固定字段升格为标准 DTO。
