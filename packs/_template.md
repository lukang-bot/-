# 新建 Industry Pack 检查清单

复制 `packs/generic`，改 `pack.json` 与 README。内核 C# 不要为新货种加 `if`。

## 必须

- [ ] `id` 与 layout.`industryPack` 一致
- [ ] `occupancy.mode` 已选：`statusEnum` 或 `tokenContains`
- [ ] `cargoTypes[].id` / `match` / `prefab` 覆盖现场实际类型
- [ ] 明确「无匹配货种」时的回退（`defaultFallback` 至多一个）
- [ ] KPI 只使用 `CountsByType` 的 key，与 `cargoTypes.id` 对齐

## 按现场可选

- [ ] `features.agvRealtime`：有 AGV 则配 `equips[].motion` 与 `pack.agv.loadFrom`
- [ ] `groups`：合并显示的线体/单元（layout 也可配）
- [ ] 反控 HTTP：是否需要 `equipmentControlURL`
- [ ] 仿真：是否提供 `sim.params.json` / scenario（见 features/simulation.md）
- [ ] 搭仓：layout 是否由 Scene Builder 导出并通过 schema

## 禁止

- 在 Runtime 或 `contracts/` 核心文件里写死货种名、客户设备 ID、某一套 Status 枚举
- 把 Microsoft SignalR.Client 写成 Unity 唯一客户端（现网常见 BestHTTP + LitJson）
