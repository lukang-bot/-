# 立库数字孪生 — 行业产品骨架

面向**整个立库行业**的可复用边界，不绑定某一货种、某一客户、某一套 WMS 编码。

目标：新现场只交付 **Runtime（不变）+ Layout（本现场）+ Industry Pack（货种/规则）**，而不是复制整仓脚本。

## 三层分工

| 层 | 内容 | 换客户时 |
|---|---|---|
| **Runtime** | 相机、点击、SignalR 壳、货位渲染、堆垛机/输送/可选 AGV 动画壳 | 不改 |
| **Layout** | `layout.json`：货位 Code、场景路径、设备清单、组合设备、功能开关 | 每现场一份 |
| **Industry Pack** | 占用判定、货物类型、Prefab、KPI 键、可选 Hub 扩展 | 按货种换包 |

Runtime **不得**出现业务词（晶棒、klp、某设备 ID）。这些只属于 Pack。

## 目录

```
├── README.md
├── MODULES.md
├── HUB_AND_REDIS.md
├── CHANGELOG.md
├── contracts/                 ← 内核契约（无行业词）
├── config/                    ← layout / runtime 示例与 schema
└── packs/
    ├── _template.md           ← 新建 pack 检查清单
    ├── generic/               ← 默认包（可配占用规则，无货种假设）
    └── semiconductor-ingot/   ← 合晶半成品示例包（不是内核）
```

## 任意货种

占用与 Prefab 一律由 pack 驱动：

- `statusEnum`：Status 为 exist/empty 等枚举（部分 WMS）
- `tokenContains`：Status 包含 token 即有货（部分离散制造）
- 空 Status 时可选回退：有 Type 或 Pallet 视为有货

内核只调用 `StorageOccupancy.IsEmpty(dto, rule)`。

## 现场最少要满足

1. 场景物体名 = 后端货位/设备 Code（或 layout 里显式映射 `sceneObject`）
2. Pack 能判定有货，并能解析到 Prefab
3. Hub 能拉到货位列表（`StorageStatusInfo`）；没有的可选接口跳过即可

## 不是什么

- 不是完整 Unity 工程，不含场景/Prefab/动画实现
- 不是某一客户的现场仓库备份
- 合晶、温控阀产线等都只是 Pack + Layout 的实例
