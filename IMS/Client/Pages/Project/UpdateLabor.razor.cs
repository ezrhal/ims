using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;


namespace IMS.Client.Pages.Project
{
    public partial class UpdateLabor
    {
        [Parameter] public ProjectModel project { get; set; }
        [Parameter] public string workitemid { get; set; }
        [Parameter] public LaborModel labor { get; set; }
        [Parameter] public RadzenDataGrid<LaborModel> laborGrid { get; set; }
        [Parameter] public RadzenDataGrid<WorkItemModel> workitemGrid { get; set; }

        double amount;
        protected override async Task OnInitializedAsync()
        {
            amount = labor.amount;
        }

        public async Task SaveQuantity()
        {
            List<string> paramList = new List<string>();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(workitemid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(labor.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject("labor"));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(labor.quantity));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(labor.days));

             var res = await httpClient.PostAsJsonAsync("project/updatequantity", paramList);

            dialog.Close();

             NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Quantity/Days has been updated",
                       Duration = 3000
                   });

            labor.amount = (double)(labor.unitcost * labor.quantity * labor.days);

            WorkItemModel workitem = project.workitems.First(q => q.Id.Equals(workitemid));
            workitem.totallabor += labor.amount - amount;
            workitem.totalamount += labor.amount - amount;
            
            //project.workitems.First(q => q.Id.Equals(workitemid)).totallabor = labor.amount;

            laborGrid.Reload();
            workitemGrid.Reload();
        }

        void CloseDialog()
		{
            dialog.Close();
        }
    }
}