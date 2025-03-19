namespace AuditLogService.Models.Response.Statistics;

public class AuditLogStatistics
{
    /// <summary>
    /// Statistics by type.
    /// Key is type name, value is count of records.
    /// </summary>
    public Dictionary<string, long> ByType { get; set; } = [];

    /// <summary>
    /// Statistics by date.
    /// Key is month and year, value is count of records.
    /// </summary>
    public Dictionary<string, long> ByDate { get; set; } = [];

    /// <summary>
    /// Statistics of stored files in the audit log.
    /// </summary>
    public List<FileExtensionStatistic> FileExtensionStatistics { get; set; } = [];
}
