using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Account;

public partial class AddUser
{
    [Parameter] public List<UserModel> users { get; set; }
    [Parameter] public RadzenDataGrid<UserModel> grid { get; set; }
    private UserModel user = new();
    private List<RoleModel> roles;

    protected override async Task OnInitializedAsync()
    {
        roles = await httpClient.GetFromJsonAsync<List<RoleModel>>("lookup/getroles");
    }
    
    public async Task SaveUser()
    {

        user.role = roles.Find(q => q.Id.Equals(user.roleid)).role;
        
        var result = await httpClient.PostAsJsonAsync("api/account/saveuser", user);
        string id = await result.Content.ReadAsStringAsync();

        if (id != null)
        {
            users.Add(user);
            grid.Reload();
            
            NotificationService.Notify(
                new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "user has been added",
                    Duration = 3000
                });
            user = new();
        }
        
    }

    void CloseDialog()
    {
        
    }
    
}