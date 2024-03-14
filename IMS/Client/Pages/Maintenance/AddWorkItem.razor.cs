using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen;

namespace IMS.Client.Pages.Maintenance
{
    partial class AddWorkItem
    {
        [Parameter] public WorkItemModel workItem {get;set;}
        [Parameter] public int edit {get;set;}
        List<WorkCategoryModel> workCategory;

        protected override async Task OnInitializedAsync()
        {
            workCategory = await httpClient.GetFromJsonAsync<List<WorkCategoryModel>>("lookup/getworkcategories");
            
        }

        public async Task SaveWorkItem(WorkItemModel args)
        {
            string _id = "";

            if (edit == 0)
            {
                var result = await httpClient.PostAsJsonAsync<WorkItemModel>("maintenance/saveworkitem", args);
                _id = await result.Content.ReadAsStringAsync();

            }
            else
            {
                await httpClient.PostAsJsonAsync<WorkItemModel>("maintenance/saveeditworkitem", args);
                _id = args.Id;
            }

            NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Work item has been save",
                       Duration = 3000
                   });

            
            args.Id = _id;
            dialog.Close(args);
            
        }

        void CloseDialog()
        {
             dialog.Close();
        }
    }
}