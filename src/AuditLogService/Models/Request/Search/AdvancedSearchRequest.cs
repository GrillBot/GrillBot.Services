namespace AuditLogService.Models.Request.Search;

public class AdvancedSearchRequest
{
    public TextSearchRequest? Info { get; set; }
    public TextSearchRequest? Warning { get; set; }
    public TextSearchRequest? Error { get; set; }
    public ExecutionSearchRequest? Interaction { get; set; }
    public ExecutionSearchRequest? Job { get; set; }
    public ApiSearchRequest? Api { get; set; }
    public UserIdSearchRequest? OverwriteCreated { get; set; }
    public UserIdSearchRequest? OverwriteDeleted { get; set; }
    public UserIdSearchRequest? OverwriteUpdated { get; set; }
    public UserIdSearchRequest? MemberRolesUpdated { get; set; }
    public UserIdSearchRequest? MemberUpdated { get; set; }
    public MessageDeletedSearchRequest? MessageDeleted { get; set; }
    public UserIdSearchRequest? MemberWarning { get; set; }

    public bool IsAnySet()
    {
        var items = new IAdvancedSearchRequest?[]
        {
            Info, Warning, Error, Interaction, Job, Api,
            OverwriteCreated, OverwriteDeleted, OverwriteUpdated,
            MemberRolesUpdated, MemberUpdated, MessageDeleted,
            MemberWarning
        };

        return Array.Exists(items, o => o?.IsSet() == true);
    }
}
