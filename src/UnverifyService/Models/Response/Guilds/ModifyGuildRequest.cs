using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Models.Response.Guilds;

public class ModifyGuildRequest
{
    [DiscordId]
    [StringLength(32)]
    public string? MuteRoleId { get; set; }
}
