# Feature: agvRealtime

可选模块。无 AGV 的立库保持 `features.agvRealtime=false`，不订阅、不加载驱动。

## 做什么

把 Hub 推送的平面坐标做成车上实时跟踪：平移、朝向、载货显隐。  
**不**做路径规划、避障、站点寻路——那些是 WCS 的事。

```
AGVStatusSend (X,Y)  →  AgvMotion.PoseToLocalMeters  →  原点 TransformPoint  →  Lerp 位置 / Slerp 朝向
AGVTaskSend          →  AgvLoad.IsLoaded（或 ErrorCode 位）→  CargoChild.SetActive
```

## 数据

| Hub | DTO | 用途 |
|---|---|---|
| `AGVStatusSend` | `AgvPoseDto` | 坐标、报警字 |
| `AGVTaskSend` | `AgvTaskDto` | 按 AGVId 挂到车上；可选载货状态 |

场景：`layout.equips[].id`（字符串）= `AGVId.ToString()`。物体名可以是 `AGV5`，**注册 ID 必须是 `5`**。

## Layout（现场）

```json
{
  "id": "5",
  "type": "Agv",
  "sceneObject": "AGV5",
  "originRef": "AgvCadOrigin",
  "motion": {
    "unitsToMeters": 0.001,
    "cadYToUnityZ": true,
    "moveSpeed": 2,
    "rotateSpeed": 5,
    "cargoChild": "Box"
  }
}
```

- 协议单位默认毫米 → 米
- CAD 平面 Y 对 Unity Z；车体高度用场景当前 Y，不跟协议走
- `originRef`：场地 CAD 原点；缺省则局部米制坐标当世界 XZ

## Pack（行业）

载货从哪读由 pack 决定，驱动不写死 bit0：

```json
"agv": {
  "loadFrom": "taskStatus",
  "loadedTaskStatuses": [1],
  "unloadedTaskStatuses": [2]
}
```

`loadFrom=errorCodeBit0` 时用 `ErrorCode` 指定位。任务 Status 的含义（上货结束/下货结束等）只写在 pack 注释里。

## Runtime 驱动要点

1. `features.agvRealtime` 为 false：不 On `AGVStatusSend` / `AGVTaskSend`
2. 收到位姿：换算 → 目标世界坐标（保 Y）→ `Lerp` 位移、位移足够大时 `LookRotation` + `Slerp`
3. 收到任务：按 `AGVId` 查找已注册 Agv；找不到则丢弃并打日志
4. 载货：`AgvLoad.IsLoaded` 返回 null 时不改车上货物显隐
5. 报警色：ErrorCode 如何映射 EquipmentStatus 由 pack 扩展，内核只认 0–4

参考实现：`contracts/AgvRealtime.cs`（无 Unity 依赖，可直接编进共享程序集）。
