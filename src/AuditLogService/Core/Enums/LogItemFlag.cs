namespace AuditLogService.Core.Enums;

[Flags]
public enum LogItemFlag : long
{
    None = 0,
    ToProcess = 1,
    Deleted = 2
}
