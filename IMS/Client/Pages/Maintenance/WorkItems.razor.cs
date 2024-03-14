using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using Radzen.Blazor;
using IMS.Client.Shared;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace IMS.Client.Pages.Maintenance
{
    partial class WorkItems
    {
        public List<WorkItemModel> workitems;
        public List<WorkItemModel> filteredworkitems;
         RadzenDataGrid<WorkItemModel> grid;
        protected override async Task OnInitializedAsync()
        {
            workitems = await httpClient.GetFromJsonAsync<List<WorkItemModel>>("maintenance/getworkitems");
            filteredworkitems = workitems;
            

        }

        void OnSearchWorkItem(string value)
        {

        }
        
        public async Task NewWorkItem(int edit = 0, string id = "")
        {
            WorkItemModel workItem = new();

            if (edit == 1)
                workItem = workitems.Find(q => q.Id.Equals(id));

            var result = await DialogService.OpenAsync<AddWorkItem>("Add new Work Item",
                   new Dictionary<string, object>() { { "workItem", workItem },{ "edit", edit } },
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (result != null && edit == 0)
            {
                workitems.Add(result);
                filteredworkitems = workitems;
                grid.Reload();
            }

        }

        public async Task DeleteWorkItem(string id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                        new Dictionary<string, object>() { { "message", "Are you sure you want to delete the selected item?" } },
                        new DialogOptions() { Width = "350px", Resizable = false, Draggable = true });
            
            if (result != null && result)
            {                           
                await httpClient.PostAsJsonAsync<string>("maintenance/deleteworkitem", id);
                workitems.Remove(workitems.Find(q => q.Id.Equals(id)));

                 NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Work item has been deleted",
                       Duration = 3000
                   });

                filteredworkitems = workitems;
                grid.Reload();
            }

            
        
        }


    }

}