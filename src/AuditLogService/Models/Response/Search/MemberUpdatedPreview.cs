﻿namespace AuditLogService.Models.Response.Search;

public class MemberUpdatedPreview
{
    public string UserId { get; set; } = null!;

    public bool NicknameChanged { get; set; }
    public bool VoiceMuteChanged { get; set; }
    public bool SelfUnverifyMinimalTimeChange { get; set; }
    public bool FlagsChanged { get; set; }
    public bool PointsDeactivatedChanged { get; set; }
}
