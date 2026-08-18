# Hub / Redis 约定

与半成品现网对齐；标准产品**方法名不改**，降低现场切换成本。

## SignalR

- 路径：`{equipDataURL}`，现网示例 `http://host:44329/DT`
- 客户端：Unity `Microsoft.AspNetCore.SignalR.Client`

### 初始化 Invoke（进场景后）

| Method | 返回 | 用途 |
|---|---|---|
| `StorageStatusInfo` | `List<StorageStatusDto>` | 货位全量，驱动 3D 货物 |
| `RealTimeInventory` | `TotalStorageDto` 或 `InventorySummaryDto` | KPI 图 |
| `CraneDataInfo` | 堆垛机状态列表 | 设备着色/姿态 |
| `ConveyorDataInfo` | 输送线点位 | 光电/托盘 |

### 推送 On

| Method | 载荷 | 用途 |
|---|---|---|
| `RealTimeInventorySend` | 库存摘要 | KPI 刷新 |
| `CraneData` | `EquipStatusDto` / 现网 CraneData | 堆垛机 |
| `ConveyorData` | 点位变化 | 输送 |
| `WcsMainTaskCreateEvent` | `TaskEventDto` | 任务动画入口 |

### 查询

| Method | 参数 | 用途 |
|---|---|---|
| `WmsStockDtoByLocation` | location code | 双击货位详情 |

## Redis（服务端）

| Key | 类型 | 说明 |
|---|---|---|
| `WmsStorageStatus` | Hash，field=Code | 货位状态 JSON |
| `WmsStocks` | 业务库存 | 可重建 WmsStorageStatus |
| `RealTimeInventory` | String JSON | KPI |

### 写入规则（必须）

1. 有货时：`Type` 或 `Pallet` 至少填一个；建议同时写 `Status=exist`。
2. 空货位：`Status=empty`，清空 Type/Pallet。
3. **禁止 UTF-8 BOM**；客户端 `config.json` 同理。
4. Hash field 名 = 货位 Code = 场景物体名 = `layout.storageSlots[].code`。

### 客户端判定（标准）

```csharp
// 见 contracts/AsrsContracts.cs → StorageOccupancy.IsEmpty
// Status 空白时：有 Type 或 Pallet → 有货
```

## 兼容旧 TotalStorageDto

现网字段：`CrystalBarCount` / `AsCutCount` / `CurrentAvailableCount`。  
标准侧用 `InventorySummaryDto.CountsByType`；适配器用 `ToSummary()`。
