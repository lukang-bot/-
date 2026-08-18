# Industry Pack: semiconductor-ingot

合晶半成品库使用的行业包。成品库可复制为 `semiconductor-finished` 并改 cargoTypes。

## cargoTypes（与 layout.example 一致）

| id | Prefab 名 | 别名 |
|---|---|---|
| 空载具 | 空载具 | 空托盘、空托、保丽龙载具 |
| 保利龙 | 保利龙 | 保丽龙、保利龙载具 |
| 晶棒 | 晶棒 | 晶棒托盘、晶棒载具（默认回退） |
| AS-CUT | AS-CUT | S-CUT、Ascut、ASCUT |

## KPI

现网 Redis / Hub 仍用 `CrystalBarCount` + `AsCutCount`。  
标准产品读 `CountsByType["晶棒"]` / `["AS-CUT"]`，适配层做字段映射。

## UI 文案

- 入口 / 出口（GateDirectionLabels）
- 托盘信息面板字段：货位、类型、托盘号

## 场景约定

- 货位物体名 = WMS location = Redis Code
- 模板物体挂在 `cargoTemplateRootPath` 下，运行时 Instantiate 到货位
- WebGL：货架/设备点击用 BoxCollider，不要 MeshCollider（非 Readable mesh 会炸）
