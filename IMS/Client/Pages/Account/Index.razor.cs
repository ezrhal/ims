using System.Net.Http.Json;
using IMS.Shared.Models;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Account;

public partial class Index
{
    private List<UserModel> users;
    private RadzenDataGrid<UserModel> grid;

    protected override async Task OnInitializedAsync()
    {
        users = await httpClient.GetFromJsonAsync<List<UserModel>>("api/account/getusers");
    }
    async Task NewUser()
    {
        var result = await DialogService.OpenAsync<AddUser>("Add new Menu",
            new Dictionary<string, object>() { { "users", users }, {"grid", grid}},
            new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
    }
    
    async Task DeleteUser(string id)
    {
        
    }
}