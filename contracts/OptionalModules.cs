namespace Asrs.DigitalTwin.Contracts
{
    /// <summary>可选：HTTP 反控。路径与字段名以现场为准。</summary>
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
