namespace Asrs.DigitalTwin.Contracts
{
    /// <summary>可选：AGV 位姿。Hub: AGVStatusSend。坐标单位由 layout.equips 声明（常见 mm）。</summary>
    public class AgvPoseDto
    {
        public int AGVId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int ErrorCode { get; set; }
    }

    /// <summary>可选：AGV 任务。Hub: AGVTaskSend。Status 含义由 pack 解释（如 1 上货结束 2 下货结束）。</summary>
    public class AgvTaskDto
    {
        public string TaskId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StationNo { get; set; } = string.Empty;
        public int AGVId { get; set; }
    }

    /// <summary>可选：HTTP 反控。路径与字段名以现场为准，此处仅为常用形状。</summary>
    public class ReverseControlRequest
    {
        public string Type { get; set; } = string.Empty;
        public string UintId { get; set; } = string.Empty;
        public bool Value { get; set; } = true;
    }

    public class ReverseControlResponse
    {
        public bool Success { get; set; }
        public string Data { get; set; } = string.Empty;
    }
}
