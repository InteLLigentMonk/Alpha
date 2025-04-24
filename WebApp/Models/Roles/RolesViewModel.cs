using Data.Entities;
using Domain.Models;

namespace WebApp.Models.Roles
{
    public class RolesViewModel
    {
        public IEnumerable<AppRole> Roles { get; set; } = [];
        public IEnumerable<SimpleUser> Users { get; set; } = [];
        public AddRoleFormViewModel AddRoleForm { get; set; } = new();
        public AssignRoleToUserFormViewModel AssignRoleToUserForm { get; set; } = new();
    }
}
