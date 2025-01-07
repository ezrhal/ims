using System.Net.Http.Json;
using IMS.Shared.Models;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Maintenance.Menu;

public partial class Menu
{
    private RadzenDataGrid<MenuModel> menugrid;
    
    private List<MenuModel> menus = new();

    protected override async Task OnInitializedAsync()
    {
        menus = await httpClient.GetFromJsonAsync<List<MenuModel>>("maintenance/getmenus");
    }
    
    private async Task NewMenu()
    {
        var result = await DialogService.OpenAsync<AddMenu>("Add new Menu",
            new Dictionary<string, object>() { { "menus", menus }, {"menu", new Menu()}, {"grid", menugrid}, {"isedit", 0}},
            new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
    }

    private async Task EditMenu(string Id)
    {
        MenuModel menu = menus.Find(q => q.Id == Id);
        var result = await DialogService.OpenAsync<AddMenu>("Add new Menu",
            new Dictionary<string, object>() { { "menus", menus }, {"menu", menu}, {"grid", menugrid}, {"isedit", 1}},
            new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        
        throw new NotImplementedException();
    }

    private Task DeleteMenu(string Id)
    {
        throw new NotImplementedException();
    }
}