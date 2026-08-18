# Pack: semiconductor-ingot

合晶半成品的**示例包**，仅作行业实例。晶棒 / AS-CUT 不得进入 `contracts/` 内核。

成品库：复制本目录为 `semiconductor-finished`，改 `cargoTypes` 与 `kpiKeys`。

## 占用

`statusEnum`。现场有货时 Status 可能为空，因此 `fallbackToTypeOrPallet=true`。

旧 KPI 字段 `CrystalBarCount` / `AsCutCount` 经 `legacyKpiMap` 转入 `CountsByType`。

## 场景

货位物体名建议 = WMS location = Redis Code（如 `01-011-001`）。  
模板物体在 `cargoTemplateRootPath`，运行时 Instantiate 到货位。  
WebGL：点击用 BoxCollider，不要 MeshCollider。
