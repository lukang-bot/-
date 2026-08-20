# Feature: scene-builder

Unity 可视化搭建仓。产出与现场共用的 **layout.json**，不实现业务调度。

## 做什么

- 调色板拖拽：货位、巷道区、堆垛机、输送、门、可选 AGV / 工作站
- 网格吸附；`code` 按 `codePattern` 生成或手改
- 多选合并 → `groups[]`
- 属性面板改 `equips[].motion` / `originRef` 等
- 保存前用 `config/layout.schema.json` 校验

## 不做什么

- 不写货种/占用逻辑（属 Pack）
- 不接真 Hub、不做库存账
- 不替代美术建模（只摆标准 Prefab 实例）

## 与产品分层

| 层 | Builder 关系 |
|---|---|
| Layout | **写入** `layout.json`（唯一交付形状） |
| Pack | 只选择 `industryPack` id，不编辑占用规则 UI（高级可只读预览） |
| Runtime | Builder 不运行完整 Runtime；进入 Play 前切 Simulation 或 Live |

## Unity 落点（建议）

```
Editor/SceneBuilder/
  PaletteWindow.cs
  LayoutSerializer.cs      # Scene ↔ layout.json
  LayoutValidator.cs       # schema + 业务规则（重复 Code）
  SnapGrid.cs
```

原则：编辑器是 Layout 的可视化编辑器，不是第二套业务系统。

## 验收

1. 空场景拖 10 个货位 + 1 台堆垛机，导出 layout 能通过 schema  
2. 再导入同一 layout，场景物体与 `code`/`sceneObject` 一致  
3. 换 Pack id 不改 layout 几何  
