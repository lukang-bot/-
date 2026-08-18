using System;
using System.Collections.Generic;

namespace Asrs.DigitalTwin.Contracts
{
    /// <summary>货位占用。Hub: StorageStatusInfo；Redis Hash field = Code。</summary>
    public class StorageStatusDto
    {
        public string Code { get; set; } = string.Empty;

        /// <summary>原始状态字。如何解释由 Industry Pack 的 occupancy 决定，内核不得写死枚举。</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>托盘/载具号（后端常用 Pallet）。</summary>
        public string Pallet { get; set; } = string.Empty;

        /// <summary>兼容旧字段 Tray；优先 Pallet。</summary>
        public string Tray { get; set; } = string.Empty;

        /// <summary>货物类型原始值，由 pack.cargoTypes.match 解释。</summary>
        public string Type { get; set; } = string.Empty;

        public string GetTrayId()
        {
            if (!string.IsNullOrEmpty(Pallet))
                return Pallet;
            return Tray ?? string.Empty;
        }
    }

    /// <summary>实时库存。标准形状只有字典，禁止在内核增加「晶棒」等固定属性。</summary>
    public class InventorySummaryDto
    {
        public Dictionary<string, int> CountsByType { get; set; } = new Dictionary<string, int>();
        public int EmptyCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class EquipStatusDto
    {
        public string EquipId { get; set; } = string.Empty;

        /// <summary>Crane / Conveyor / Shuttle / Agv / Unit / Other</summary>
        public string EquipType { get; set; } = string.Empty;

        /// <summary>
        /// 与常见后端 EquipmentStatus 数值对齐：0 未初始化 1 初始化 2 手动 3 自动 4 故障。
        /// 字符串展示由 UI 映射，不要用 Idle/Running 当协议值。
        /// </summary>
        public int Status { get; set; }

        public string AlarmCode { get; set; } = string.Empty;
        public string AlarmMessage { get; set; } = string.Empty;
        public Dictionary<string, string> Ext { get; set; } = new Dictionary<string, string>();
    }

    public class ConveyorPointDto
    {
        public string PointId { get; set; } = string.Empty;
        public bool LoadPhotocell { get; set; }
        public string TrayId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TaskCode { get; set; } = string.Empty;
    }

    public class TaskEventDto
    {
        public string TaskCode { get; set; } = string.Empty;
        public string MotherTray { get; set; } = string.Empty;
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public int Status { get; set; }
        public string TaskKind { get; set; } = string.Empty;
        public string CargoType { get; set; } = string.Empty;
    }

    /// <summary>场景中多个后端 ID 对应一个可点物体（板链组、双单元、合并线体号）。</summary>
    public class EquipGroupDto
    {
        public string SceneObject { get; set; } = string.Empty;
        public string[] LinkedIds { get; set; } = Array.Empty<string>();
        public string DisplayName { get; set; } = string.Empty;
    }

    public static class DtHubMethods
    {
        public const string StorageStatusInfo = "StorageStatusInfo";
        public const string RealTimeInventory = "RealTimeInventory";
        public const string CraneDataInfo = "CraneDataInfo";
        public const string ConveyorDataInfo = "ConveyorDataInfo";
        public const string RealTimeInventorySend = "RealTimeInventorySend";
        public const string CraneData = "CraneData";
        public const string ConveyorData = "ConveyorData";
        public const string WcsMainTaskCreateEvent = "WcsMainTaskCreateEvent";
        public const string WmsStockDtoByLocation = "WmsStockDtoByLocation";
    }

    /// <summary>按 pack.hub.optional 订阅；Invoke 失败必须跳过。</summary>
    public static class OptionalHubMethods
    {
        public const string UnitData = "UnitData";
        public const string UnitDataInfo = "UnitDataInfo";
        public const string PCConveyorData = "PCConveyorData";
        public const string PCConveyorDataInfo = "PCConveyorDataInfo";
        public const string CodeDiskData = "CodeDiskData";
        public const string CodeDiskDataInfo = "CodeDiskDataInfo";
        public const string AlarmData = "AlarmData";
        public const string AGVStatusSend = "AGVStatusSend";
        public const string AGVTaskSend = "AGVTaskSend";
        public const string ShuttleData = "ShuttleData";
        public const string DayInAndOut = "DayInAndOut";
        public const string StockTaking = "StockTaking";
        public const string RealTimeWorkOrder = "RealTimeWorkOrder";
        public const string RealTimeWorkOrderSend = "RealTimeWorkOrderSend";
    }
}
