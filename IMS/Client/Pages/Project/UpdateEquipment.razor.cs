using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Project
{
    public partial class UpdateEquipment
    {
        [Parameter] public ProjectModel project { get; set; }
        [Parameter] public string workitemid { get; set; }
        [Parameter] public EquipmentModel equipment { get; set; }
        [Parameter] public RadzenDataGrid<EquipmentModel> equipmentGrid { get; set; }
        [Parameter] public RadzenDataGrid<WorkItemModel> workitemGrid { get; set; }

        double amount;
        protected override async Task OnInitializedAsync()
        {
            amount = equipment.amount;
        }

        public async Task SaveQuantity(EquipmentModel args)
        {
            List<string> paramList = new List<string>();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(workitemid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(equipment.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject("equipment"));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(equipment.quantity));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(equipment.hours));

             var res = await httpClient.PostAsJsonAsync("project/updateequipment", paramList);

            dialog.Close();

             NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Quantity/Hours has been updated",
                       Duration = 3000
                   });

            equipment.amount = (double)(equipment.unitcost * equipment.quantity * equipment.hours);

            WorkItemModel workitem = project.workitems.First(q => q.Id.Equals(workitemid));
            workitem.totalequipment += equipment.amount - amount;
            workitem.totalamount += equipment.amount - amount;

            equipmentGrid.Reload();
            workitemGrid.Reload();
        }

        void CloseDialog()
		{
            dialog.Close();
        }

    }
}