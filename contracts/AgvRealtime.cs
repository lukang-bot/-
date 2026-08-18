using System;

namespace Asrs.DigitalTwin.Contracts
{
    /// <summary>AGV 实时位姿。Hub: AGVStatusSend。</summary>
    public class AgvPoseDto
    {
        public int AGVId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int ErrorCode { get; set; }
    }

    /// <summary>AGV 任务。Hub: AGVTaskSend。</summary>
    public class AgvTaskDto
    {
        public string TaskId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StationNo { get; set; } = string.Empty;
        public int AGVId { get; set; }
    }

    /// <summary>layout.equips[] 中 type=Agv 的运动参数。</summary>
    public class AgvMotionConfig
    {
        /// <summary>坐标单位换算到米。毫米现场为 0.001。</summary>
        public float UnitsToMeters { get; set; } = 0.001f;

        /// <summary>CAD Y → Unity Z（右手平面投影）。</summary>
        public bool CadYToUnityZ { get; set; } = true;

        public float MoveSpeed { get; set; } = 2f;
        public float RotateSpeed { get; set; } = 5f;

        /// <summary>场景中 CAD 原点物体名；空则把换算后的 XZ 当世界坐标。</summary>
        public string OriginRef { get; set; } = string.Empty;

        /// <summary>车上货物子物体名，常见 Box。</summary>
        public string CargoChild { get; set; } = "Box";
    }

    /// <summary>pack.agv.loadFrom：errorCodeBit0 | taskStatus | poseFlag（扩展）。</summary>
    public class AgvLoadRule
    {
        public string LoadFrom { get; set; } = "taskStatus";
        public int ErrorCodeLoadBit { get; set; } = 0;
        public int[] LoadedTaskStatuses { get; set; } = new[] { 1 };
        public int[] UnloadedTaskStatuses { get; set; } = new[] { 2 };
    }

    public static class AgvMotion
    {
        /// <summary>协议平面坐标 → 地面局部 X/Z（米）。Y 高度由场景保持。</summary>
        public static void PoseToLocalMeters(AgvPoseDto pose, AgvMotionConfig cfg, out float localX, out float localZ)
        {
            if (cfg == null)
                cfg = new AgvMotionConfig();
            float scale = cfg.UnitsToMeters <= 0 ? 0.001f : cfg.UnitsToMeters;
            double x = pose == null ? 0 : pose.X;
            double y = pose == null ? 0 : pose.Y;
            localX = (float)(x * scale);
            localZ = (float)(y * scale);
        }

        public static bool ShouldRotate(float dx, float dz, float epsilon = 0.01f)
        {
            return dx * dx + dz * dz > epsilon * epsilon;
        }
    }

    public static class AgvLoad
    {
        public static bool? IsLoaded(AgvPoseDto pose, AgvTaskDto task, AgvLoadRule rule)
        {
            if (rule == null)
                rule = new AgvLoadRule();

            if (string.Equals(rule.LoadFrom, "errorCodeBit0", StringComparison.OrdinalIgnoreCase)
                || string.Equals(rule.LoadFrom, "errorCodeBit", StringComparison.OrdinalIgnoreCase))
            {
                if (pose == null)
                    return null;
                int bit = rule.ErrorCodeLoadBit;
                return (pose.ErrorCode & (1 << bit)) != 0;
            }

            if (task == null)
                return null;
            if (Contains(rule.LoadedTaskStatuses, task.Status))
                return true;
            if (Contains(rule.UnloadedTaskStatuses, task.Status))
                return false;
            return null;
        }

        static bool Contains(int[] values, int status)
        {
            if (values == null)
                return false;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == status)
                    return true;
            }
            return false;
        }
    }
}
