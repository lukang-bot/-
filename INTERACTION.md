# 设备 / 货物交互标准（Unity）

> **已定产品行为**：需要查看详情的对象，统一 **双击 → 弹出交互 UI → 经数据接口填充**。  
> 悬停仅高亮；单击可用于轻量 Tip（如线体编号），不作为详情主路径。

本文属于 Runtime 内核约定，不含货种词。现场差异只进 `layout.interaction` 与 Pack。

---

## 1. 产品口径

| 操作 | 行为 |
|------|------|
| 悬停 | 可选高亮；不打开/不关闭详情面板 |
| 单击 | 默认无详情；可选 Tip（`features.conveyorIdTip`） |
| **双击** | **打开对应详情 UI**（主交互） |
| 单击空白 | 关闭 Tip / 取消选中；不强制关详情面板 |
| 指针在 UI 上 | 忽略 3D 点击（`EventSystem.IsPointerOverGameObject`） |

流程：

```text
双击命中 → InteractableKind + Identity → OpenPanel → Hub/HTTP 查询 → 绑定 UI
```

---

## 2. InteractableKind（可交互种类）

| Kind | 对象 | Identity | 详情 Panel（逻辑名） | 数据入口（示例） |
|------|------|----------|----------------------|------------------|
| `StorageCargo` | 货架货位货物 | 货位 Code | `CargoInfoPanel` | `WmsStockDtoByLocation` |
| `LineCargo` | CV / 环穿上的货物 | TrayId / PointId | `CargoInfoPanel` | 点位或托盘查询 |
| `Conveyor` | 输送线 / CV 段 | EquipId（或 groups 合并号） | `ConveyorInfoPanel` | `ConveyorDataInfo` |
| `Crane` | 堆垛机 | EquipId | `EquipInfoPanel` | `CraneDataInfo` |
| `Rgv` | 轨道 RGV | EquipId | `EquipInfoPanel` | 设备状态 |
| `Agv` | AGV | EquipId | `EquipInfoPanel` | 设备状态（可与 agvRealtime 并存） |
| `Shuttle` | 穿梭车 | EquipId | `EquipInfoPanel` | 设备状态 |
| `Unit` / `Other` | 其它可点设备 | EquipId | `EquipInfoPanel` | 设备状态 |

Pack 可扩展 Kind，须在 `layout.interaction.bindings` 注册 `panel` + `query`。

---

## 3. Unity 实现（标准三步）

### 3.1 碰撞体

对每个可交互根节点：

1. 保证有且仅需 **一个点击用 Collider**。
2. **强制 `BoxCollider`**，用 Mesh `bounds` 拟合 center/size。
3. **禁止用 `MeshCollider` 做点击**（WebGL 不可读网格会 `CollisionMeshData` / cooking 失败，拖垮初始化）。
4. 启动时剥离无效 `MeshCollider` 再挂 Box。
5. Layer 纳入 `interaction.raycastMask`。

建议组件：

```text
InteractableTarget
  kind: InteractableKind
  identity: string
  panelOverride?: string
+ BoxCollider
+ HighlightEffect?   // 悬停
```

合晶等旧工程中的 `EquipBase.EnsureClickCollider`、货位 `TrayComponent*` 应按此收敛。

### 3.2 射线 + 双击

1. 指针在 UI 上 → return  
2. `Camera.ScreenPointToRay` → `Physics.Raycast(..., raycastMask)`  
3. 命中后向上找 `InteractableTarget`  
4. 同一目标在 **`doubleClickSeconds`（默认 0.3）** 内两次有效点击 → 激活  
5. 发出 `InteractableActivated(kind, identity)`

### 3.3 面板 + 数据

1. 查 `layout.interaction.bindings[kind]`  
2. `OpenPanel(panel, { kind, identity })`  
3. Panel：先 Loading → 调 query → 绑定；失败显示错误态，不自动关  
4. 仅用户关闭或业务强制关；**悬停离开不关详情**

---

## 4. 命中优先级

同一射线建议：

1. `StorageCargo` / `LineCargo`  
2. `Agv` / `Rgv` / `Shuttle` / `Crane` / `Unit`  
3. `Conveyor`

可用更紧的货物 Box、Layer 或 Kind 排序实现。

`layout.groups`（如 `1004&1005`）：命中合并物体时 Identity 用 `displayName` 或主 Id，详情内列出 `linkedIds`。

---

## 5. 单击 Tip（可选）

当 `features.conveyorIdTip == true` 且 `interaction.conveyorIdTrigger == "singleClick"`：

- 单击 Conveyor → 跟随编号 Tip  
- **不是**详情 UI；详情仍走双击 `ConveyorInfoPanel`

---

## 6. 面板数据最低集

**CargoInfoPanel**：Location/Point、Tray/Pallet、Type、Status、TaskCode（可选）  
**EquipInfoPanel**：EquipId、EquipType、Status、AlarmCode、AlarmMessage、Ext  
**ConveyorInfoPanel**：段 Id、点位列表摘要、光电/托盘占用

面板上下文：

```csharp
public class InteractablePanelData
{
    public string Kind;
    public string Identity;
    public string DisplayName;
}
```

契约见 `contracts/AsrsContracts.cs`（`InteractableKind` / `InteractionDefaults`）。

---

## 7. layout 示例片段

```json
"interaction": {
  "primaryTrigger": "doubleClick",
  "doubleClickSeconds": 0.3,
  "blankClickHidesTips": true,
  "ignoreWhenPointerOverUi": true,
  "raycastMaxDistance": 200,
  "hoverHighlight": true,
  "conveyorIdTrigger": "singleClick",
  "bindings": {
    "StorageCargo": { "panel": "CargoInfoPanel", "query": "WmsStockDtoByLocation" },
    "LineCargo":    { "panel": "CargoInfoPanel", "query": "WmsStockDtoByTray" },
    "Conveyor":     { "panel": "ConveyorInfoPanel", "query": "ConveyorDataInfo" },
    "Crane":        { "panel": "EquipInfoPanel", "query": "CraneDataInfo" },
    "Rgv":          { "panel": "EquipInfoPanel", "query": "EquipStatusInfo" },
    "Agv":          { "panel": "EquipInfoPanel", "query": "EquipStatusInfo" },
    "Shuttle":      { "panel": "EquipInfoPanel", "query": "EquipStatusInfo" }
  }
},
"features": {
  "doubleClickInfoUi": true,
  "conveyorIdTip": true,
  "hoverHighlight": true
}
```

---

## 8. Runtime 模块

```
Runtime/Interaction/
├── InteractableKind.cs
├── InteractableTarget.cs
├── ClickColliderUtil.cs       # BoxCollider only
├── InteractionRaycaster.cs
├── DoubleClickGate.cs         # 默认 0.3s
├── InteractionRouter.cs       # Kind → Panel + Query
└── HoverHighlightBinder.cs
```

Pack / 现场脚本不得各自实现第二套点击；只提供 Prefab、文案、额外字段。

---

## 9. 验收

- [ ] 货位货物、线体货物双击出 Cargo 面板且数据正确  
- [ ] 堆垛机 / RGV / AGV / 穿梭车双击出设备面板  
- [ ] CV 双击出线体详情（启用时）；单击仅 Tip（若开启）  
- [ ] WebGL 无 MeshCollider cooking 报错，点击可用  
- [ ] 悬停离开不关详情；点在 UI 上不穿透  
- [ ] 接口 / config UTF-8 无 BOM  

---

## 10. 与旧现场

| 旧行为 | 标准 |
|--------|------|
| 货物双击开信息 | 保持 |
| 线体单击仅编号 Tip | Tip 可保留；详情改双击 |
| 设备单击 / 仅报警可点 | 统一双击开 EquipInfoPanel |
| MeshCollider 点击 | 改为 BoxCollider |
