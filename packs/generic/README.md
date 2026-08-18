# Pack: generic

默认行业包。不假设货种名称。

- 占用：`statusEnum`（exist / empty 等），Status 空白时看 Type/Pallet
- 货物：单一 `cargo` + 默认 Prefab `CargoDefault`
- Hub 扩展：无

现场应复制本目录，改 `id`、`cargoTypes`、`occupancy`。  
若 Status 是拼接关键字而不是枚举，改用同目录 `pack.token-contains.example.json`。
