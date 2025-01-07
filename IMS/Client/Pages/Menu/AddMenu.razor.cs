using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Menu;

public partial class AddMenu
{
    [Parameter] public List<MenuModel> mainmenu { get; set; }
    [Parameter] public RadzenDataGrid<MenuModel> grid { get; set; }
    private MenuModel menu = new();
    
    
    async Task SaveMenu()
    {
        if (menu.parentid == "")
            menu.submenu = new();
            
        var result = await httpClient.PostAsJsonAsync("maintenance/savemenu", menu);
        
        string id = await result.Content.ReadAsStringAsync();

        NotificationService.Notify(
            new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = "Menu has been saved",
                Duration = 3000
            });

        menu.Id = id;
        mainmenu.Add(menu);
        grid.Reload();
        menu = new();

    }
    
    void CloseDialog()
    {
        DialogService.Close();
    }
}