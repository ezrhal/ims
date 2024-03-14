using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Project
{
    public partial class UpdateQuantity
    {
        [Parameter] public ProjectModel project { get; set; }
        [Parameter] public string workitemid { get; set; }
        [Parameter] public MaterialsModel material { get; set; }
        [Parameter] public RadzenDataGrid<MaterialsModel> materialsGrid { get; set; }
        [Parameter] public RadzenDataGrid<WorkItemModel> workitemGrid { get; set; }

        double amount;
        protected override async Task OnInitializedAsync()
        {
            amount = material.amount;
        }

        public async Task SaveQuantity(MaterialsModel args)
        {
            List<string> paramList = new List<string>();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(workitemid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(material.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject("materials"));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(material.quantity));

             var res = await httpClient.PostAsJsonAsync("project/updatequantity", paramList);

            dialog.Close();

             NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Quantity has been updated",
                       Duration = 3000
                   });

            material.amount = (double)(material.unitcost * material.quantity);

            WorkItemModel workitem = project.workitems.First(q => q.Id.Equals(workitemid));
            workitem.totalmaterials += material.amount - amount;
            workitem.totalamount += material.amount - amount;

            materialsGrid.Reload();
            workitemGrid.Reload();
        }

        void CloseDialog()
		{
            dialog.Close();
        }
    }
}