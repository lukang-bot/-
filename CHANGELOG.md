# Changelog

## 0.2.0

- 内核与行业规则分离：占用判定、货种、KPI 键进入 Industry Pack。
- 契约去掉合晶专用字段作为标准：`InventorySummaryDto.CountsByType` 为 KPI 唯一标准形状。
- Hub 文档拆成 Core / Optional；补充非 Hub 的 HTTP 反控通道。
- Layout schema 支持 `codePattern`、`groups`、可选设备与 feature 开关。
- 新增 `packs/generic`；合晶内容下沉到 `packs/semiconductor-ingot`。

## 0.1.0

- 从合晶半成品项目抽出首版草案。
