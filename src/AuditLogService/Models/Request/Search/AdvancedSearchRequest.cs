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
}
