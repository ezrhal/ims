using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Project
{
    public partial class AddMaterials
    {
        [Parameter] public ProjectModel project { get; set; }
        [Parameter] public string workitemid { get; set; }
        [Parameter] public string isempty { get; set; } = "0";
        [Parameter] public bool isadded { get; set; }
        [Parameter] public RadzenDataGrid<MaterialsModel> materialsGrid { get; set; }
        [Parameter] public RadzenDataGrid<WorkItemModel> workitemGrid { get; set; }


        MaterialsModel material = new();
        List<ItemModel> items;
        protected override async Task OnInitializedAsync()
        {
            items = await httpClient.GetFromJsonAsync<List<ItemModel>>("maintenance/getitemsbytype?typeid=1");
        }

        public async Task SaveMaterial(MaterialsModel args)
        {
            
            ItemModel item = items.First(q => q.Id.Equals(args.itemid));
            args.item = item.item;
            args.description = item.description;
            args.unit = item.unit;
            args.unitcost = item.unitcost;

            List<string> paramList = new List<string>();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(args));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(workitemid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(isempty));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject("materials"));

            var res = await httpClient.PostAsJsonAsync("project/addmaterial", paramList);

             NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Maaterial has been added",
                       Duration = 3000
                   });

            material = new();

            args.amount = (double)(item.unitcost * args.quantity);
            WorkItemModel workitem = project.workitems.First(q => q.Id.Equals(workitemid));
            workitem.totalmaterials += args.amount;
            workitem.totalamount += args.amount;

            project.workitems.First(q => q.Id.Equals(workitemid)).materials.Add(args);
            materialsGrid.Reload();
            workitemGrid.Reload();
        }

        
        void CloseDialog()
		{
            dialog.Close();
        }
    }
}