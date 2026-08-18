using System;

namespace Asrs.DigitalTwin.Contracts
{
    public enum OccupancyMode
    {
        /// <summary>Status 落在 occupied/empty 列表中（部分 WMS）。</summary>
        StatusEnum = 0,

        /// <summary>Status 包含任一 hasCargo token 即有货（部分离散制造）。</summary>
        TokenContains = 1
    }

    /// <summary>来自 packs/*/pack.json 的 occupancy 段。mode: statusEnum | tokenContains</summary>
    public class OccupancyRule
    {
        public string Mode { get; set; } = "statusEnum";

        public string[] OccupiedStatuses { get; set; } =
            new[] { "exist", "import", "occupied", "1", "true" };

        public string[] EmptyStatuses { get; set; } =
            new[] { "empty", "export", "vacant", "0", "false" };

        public string[] HasCargoTokens { get; set; } = Array.Empty<string>();

        /// <summary>Status 无法判定时：有 Type 或 Pallet 视为有货。</summary>
        public bool FallbackToTypeOrPallet { get; set; } = true;
    }

    public static class StorageOccupancy
    {
        public static bool IsEmpty(StorageStatusDto dto, OccupancyRule rule)
        {
            if (dto == null)
                return true;
            if (rule == null)
                rule = new OccupancyRule();

            if (ParseMode(rule.Mode) == OccupancyMode.TokenContains)
            {
                if (!string.IsNullOrWhiteSpace(dto.Status) && ContainsAny(dto.Status, rule.HasCargoTokens))
                    return false;
                return FallbackEmpty(dto, rule);
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                var s = dto.Status.Trim();
                if (EqualsAny(s, rule.OccupiedStatuses))
                    return false;
                if (EqualsAny(s, rule.EmptyStatuses))
                    return true;
            }

            return FallbackEmpty(dto, rule);
        }

        public static bool IsOccupied(StorageStatusDto dto, OccupancyRule rule)
        {
            return !IsEmpty(dto, rule);
        }

        public static OccupancyMode ParseMode(string mode)
        {
            if (string.Equals(mode, "tokenContains", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mode, "TokenContains", StringComparison.OrdinalIgnoreCase))
                return OccupancyMode.TokenContains;
            return OccupancyMode.StatusEnum;
        }

        static bool FallbackEmpty(StorageStatusDto dto, OccupancyRule rule)
        {
            if (!rule.FallbackToTypeOrPallet)
                return true;
            if (!string.IsNullOrWhiteSpace(dto.Type))
                return false;
            if (!string.IsNullOrWhiteSpace(dto.Pallet) || !string.IsNullOrWhiteSpace(dto.Tray))
                return false;
            return true;
        }

        static bool ContainsAny(string value, string[] tokens)
        {
            if (tokens == null || tokens.Length == 0)
                return false;
            for (int i = 0; i < tokens.Length; i++)
            {
                var t = tokens[i];
                if (string.IsNullOrEmpty(t))
                    continue;
                if (value.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        static bool EqualsAny(string value, string[] candidates)
        {
            if (candidates == null)
                return false;
            for (int i = 0; i < candidates.Length; i++)
            {
                if (string.Equals(value, candidates[i], StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
