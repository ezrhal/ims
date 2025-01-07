using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Maintenance.Menu;

public partial class AddMenu
{
    private List<MenuModel> parentMenus;
    
    [Parameter] public MenuModel menu { get; set; }
    [Parameter] public List<MenuModel> menus { get; set; }
    [Parameter] public RadzenDataGrid<MenuModel> grid { get; set; }
    [Parameter] public int isedit { get; set; } = 0;

    protected override async Task OnInitializedAsync()
    {
        parentMenus = await httpClient.GetFromJsonAsync<List<MenuModel>>("maintenance/getparentmenus");
    }

    private async Task SaveMenu()
    {

        if (isedit == 0)
        {
            var result = await httpClient.PostAsJsonAsync("maintenance/savemenu", menu);
            string id = await result.Content.ReadAsStringAsync();

            if (id != null)
            {
                menus.Add(menu);
                grid.Reload();
            
                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Menu has been added",
                        Duration = 3000
                    });
                menu = new();
            }
        }
        else
        {
            
        }
        
        
    }
    
    void CloseDialog()
    {
        
    }
}