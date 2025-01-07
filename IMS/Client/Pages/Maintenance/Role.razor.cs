using IMS.Shared.Models;
using Radzen.Blazor;

namespace IMS.Client.Pages.Maintenance;

public partial class Role
{
    public RoleModel role;
    public List<RoleModel> roles = new();
    private RadzenDataGrid<RoleModel> rolegrid;
    private RadzenDataGrid<MenuModel> menugrid; 

    protected override async Task OnInitializedAsync()
    {
        
    }

    private void NewRole()
    {
        role = new RoleModel();
    }

    private async Task EditRole(string Id)
    {
        
    }

    private async Task DeleteRole(string Id)
    {
        
    }
    
    private async Task AddRoleMenu(string roleid)
    {
        
    }

    private async Task RemoveRoleMenu(string roleid)
    {
        
    }
    
}