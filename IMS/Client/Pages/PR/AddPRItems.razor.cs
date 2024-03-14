using System.Net.Http.Json;
using IMS.Shared;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using Radzen;
using Radzen.Blazor;


namespace IMS.Client.Pages.PR
{
    public partial class AddPRItems
    {

        [Parameter] public PRModel pr { get; set; }
        [Parameter] public int isempty { get; set; }
        [Parameter] public RadzenDataGrid<PRItemModel> prItemsGrid { get; set; }
        
        TotalProcuredModel totalprocured = new();
        PRItemModel prItems = new();
        string remainingtext = "Remaining Quantity";
        double remainingquantity = 0;

        List<MaterialsModel> items;
        List<BalanceMaterialModel> balancematerials;
        protected override async Task OnInitializedAsync()
        {
            items = await httpClient.GetFromJsonAsync<List<MaterialsModel>>("purchaserequest/getmaterialsquantity?projectid=" + pr.projectid + "&itemid=");
            balancematerials = await httpClient.GetFromJsonAsync<List<BalanceMaterialModel>>("purchaserequest/getbalancematerials?projectid=" + pr.projectid);

        }

        public async Task SavePRItem(PRItemModel args)
        {

            PRItemModel pritem = pr.items.Find(q => q.itemid.Equals(prItems.itemid));

            if (pritem == null)
            {
                MaterialsModel item = items.First(q => q.itemid.Equals(prItems.itemid));
                prItems.item = item.item;
                prItems.description = item.description;
                prItems.unit = item.unit;
                prItems.unitcost = item.unitcost;

                isempty = pr.items == null ? 0 : pr.items.Count;

                List<string> paramList = new List<string>();

                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(pr.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(prItems));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(isempty));
                var res = await httpClient.PostAsJsonAsync("purchaserequest/savepritem", paramList);
                 pr.items.Add(prItems);
            }
            else
            {

                double qty = (double)pr.items.Find(q => q.itemid.Equals(prItems.itemid)).quantity;
                qty += (double)prItems.quantity;
                pr.items.Find(q => q.itemid.Equals(prItems.itemid)).quantity = qty;

                List<string> paramList = new();

                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(pr.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(pritem.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(qty));

                await httpClient.PostAsJsonAsync("purchaserequest/updatepritem", paramList);
            }

           
            prItemsGrid.Reload();

             NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Item has been added",
                       Duration = 3000
                   });

            prItems = new();

        }

        public async Task SelectItem()
        {

            List<string> paramList = new();
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(pr.projectid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(prItems.itemid));

            try
            {
                totalprocured = await httpClient.GetFromJsonAsync<TotalProcuredModel>("purchaserequest/gettotalprocured?projectid=" + pr.projectid + "&itemid=" + prItems.itemid);
            }
            catch{}
            
            totalprocured.totalquantity = (double)items.First(q => q.itemid.Equals(prItems.itemid)).quantity;
            remainingquantity = totalprocured.totalquantity - totalprocured.totalprocured;


        }

        async Task LoadData(LoadDataArgs args)
        {
            StateHasChanged();
        }

        void CloseDialog()
		{
            dialog.Close();
        }
    }
}