﻿namespace AuditLogService.Models.Response.Search;

public class InteractionCommandPreview
{
    public string Name { get; set; } = null!;
    public bool HasResponded { get; set; }
    public bool IsSuccess { get; set; }
    public int? CommandError { get; set; }
}
