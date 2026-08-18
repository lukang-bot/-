// 立库数字孪生标准契约（草案）
// 对齐现有 HeJin1/HeJin2：StorageStatusDto / TotalStorageDto / CraneDataDto / Conveyor / Task
// 命名空间可按项目替换；字段名保持稳定，供 Redis / SignalR / Unity 共用。

using System;
using System.Collections.Generic;

namespace Asrs.DigitalTwin.Contracts
{
    /// <summary>货位占用状态。对应 Redis Hash: WmsStorageStatus，Hub: StorageStatusInfo</summary>
    public class StorageStatusDto
    {
        /// <summary>货位编号，必须与场景物体名一致，如 01-011-001</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// empty | exist | import | export
        /// 现场有货时 Status 可能为 null：客户端须结合 Type/Pallet 判定，不可仅看空白当空储位。
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>托盘/载具号（后端常用 Pallet，客户端可映射 Tray）</summary>
        public string Pallet { get; set; } = string.Empty;

        /// <summary>货物类型：晶棒 / AS-CUT / 保利龙 / 空载具 / 空料箱…（由 Industry Pack 解释）</summary>
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>实时库存聚合。对应 Redis String: RealTimeInventory，Hub: RealTimeInventory</summary>
    public class InventorySummaryDto
    {
        /// <summary>按类型计数，标准产品用字典，避免写死晶棒/AS-CUT 字段</summary>
        public Dictionary<string, int> CountsByType { get; set; } = new();

        /// <summary>空储位数</summary>
        public int EmptyCount { get; set; }

        /// <summary>总储位数（可选）</summary>
        public int TotalCount { get; set; }
    }

    /// <summary>兼容合晶旧字段的库存摘要适配</summary>
    public class TotalStorageDto
    {
        public int CurrentAvailableCount { get; set; }
        public int CrystalBarCount { get; set; }
        public int AsCutCount { get; set; }

        public InventorySummaryDto ToSummary() => new InventorySummaryDto
        {
            EmptyCount = CurrentAvailableCount,
            CountsByType = new Dictionary<string, int>
            {
                ["晶棒"] = CrystalBarCount,
                ["AS-CUT"] = AsCutCount
            }
        };
    }

    /// <summary>设备运行状态。Hub 推送: CraneData / ConveyorData</summary>
    public class EquipStatusDto
    {
        public string EquipId { get; set; } = string.Empty;
        /// <summary>Crane | Conveyor | Shuttle | Scanner | Other</summary>
        public string EquipType { get; set; } = string.Empty;
        /// <summary>Idle | Running | Alarm | Offline | Manual</summary>
        public string Status { get; set; } = string.Empty;
        public string AlarmCode { get; set; } = string.Empty;
        public string AlarmMessage { get; set; } = string.Empty;
        /// <summary>扩展位：层/列/排、当前点位等</summary>
        public Dictionary<string, string> Ext { get; set; } = new();
    }

    /// <summary>输送线点位。对应 ConveyorLocationInfoDataDto</summary>
    public class ConveyorPointDto
    {
        public string PointId { get; set; } = string.Empty;
        public bool LoadPhotocell { get; set; }
        public string TrayId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TaskCode { get; set; } = string.Empty;
    }

    /// <summary>物流任务事件。对应 WcsMainTask / ChildTask 推送</summary>
    public class TaskEventDto
    {
        public string TaskCode { get; set; } = string.Empty;
        public string MotherTray { get; set; } = string.Empty;
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        /// <summary>0 Created / 1 Running / 5 Completed … 与现网枚举对齐时可扩展</summary>
        public int Status { get; set; }
        /// <summary>Conveyor | Crane | Shuttle</summary>
        public string TaskKind { get; set; } = string.Empty;
        public string CargoType { get; set; } = string.Empty;
    }

    /// <summary>标准 Hub 方法名（Unity Invoke / On）</summary>
    public static class DtHubMethods
    {
        // 初始化拉取
        public const string StorageStatusInfo = "StorageStatusInfo";
        public const string RealTimeInventory = "RealTimeInventory";
        public const string CraneDataInfo = "CraneDataInfo";
        public const string ConveyorDataInfo = "ConveyorDataInfo";

        // 实时推送
        public const string RealTimeInventorySend = "RealTimeInventorySend";
        public const string CraneData = "CraneData";
        public const string ConveyorData = "ConveyorData";
        public const string WcsMainTaskCreateEvent = "WcsMainTaskCreateEvent";

        // 查询
        public const string WmsStockDtoByLocation = "WmsStockDtoByLocation";
    }

    /// <summary>货位是否有货的标准判定（与半成品 BoxSystem 修复一致）</summary>
    public static class StorageOccupancy
    {
        public static bool IsEmpty(StorageStatusDto dto)
        {
            if (dto == null) return true;

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                var s = dto.Status.Trim();
                if (EqualsAny(s, "exist", "import", "1", "true")) return false;
                if (EqualsAny(s, "empty", "export", "0", "false")) return true;
            }

            if (!string.IsNullOrWhiteSpace(dto.Type)) return false;
            if (!string.IsNullOrWhiteSpace(dto.Pallet)) return false;
            return true;
        }

        static bool EqualsAny(string value, params string[] candidates)
        {
            foreach (var c in candidates)
                if (string.Equals(value, c, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}
