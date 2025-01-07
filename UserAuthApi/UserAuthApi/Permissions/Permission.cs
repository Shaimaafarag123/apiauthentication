namespace UserAuthApi.Permissions
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; } 
    }
}
