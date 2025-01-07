using System.Net.Http.Json;
using IMS.Client.Pages.PO;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Menu;

public partial class Index
{
    private List<MenuModel> mainmenu = new();
    private List<MenuModel> tasksbyme = new();
    
    private List<MenuModel> subtasksbyme = new();
    private List<MenuModel> submenu = new();

    private RadzenDataGrid<MenuModel> grid;
    private bool gridLoading = true;

    protected override async Task OnInitializedAsync()
    {
        mainmenu = await httpClient.GetFromJsonAsync<List<MenuModel>>("maintenance/getmenus");
        submenu = GetAllSubTasks(mainmenu);
        gridLoading = false;
    }

    public async Task NewMenu()
    {
        var result = await DialogService.OpenAsync<AddMenu>("Add new Menu",
            new Dictionary<string, object>() { { "mainmenu", mainmenu }, {"grid", grid}},
            new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
    }

    async Task AddSubMenu(string parentid)
    {
        
    }
    
    async Task LoadData(LoadDataArgs args)
    {
        StateHasChanged();
    }
    
    private async Task LoadChildData(DataGridLoadChildDataEventArgs<MenuModel> args)
    {
        try
        {
            args.Data = submenu.Where(q => q.parentid.Equals(args.Item.Id));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    void RowRender(RowRenderEventArgs<MenuModel> args)
    {
        try
        {
            args.Expandable = submenu.Where(q => q.parentid.Equals(args.Data.Id)).Any();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }  
    
    public static List<MenuModel> GetAllSubTasks(List<MenuModel> _menus)
    {
        List<MenuModel> allSubTasks = new List<MenuModel>();
        foreach (var menu in _menus)
        {
            AddSubTasksToList(menu, allSubTasks);
        }
        return allSubTasks;
    }

    public static void AddSubTasksToList(MenuModel _menus, List<MenuModel> allSubTasks)
    {
        if (_menus.submenu != null && _menus.submenu.Count > 0)
        {
            foreach (var menu in _menus.submenu)
            {
                allSubTasks.Add(menu);
                AddSubTasksToList(menu, allSubTasks); // Recursively add sub-subtasks
            }
        }
    }
    
}