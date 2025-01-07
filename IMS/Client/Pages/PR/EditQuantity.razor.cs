using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Net.Http.Json;

namespace IMS.Client.Pages.PR
{
    public partial class EditQuantity
    {
        [Parameter] public PRItemModel prItem { get; set; }
        [Parameter] public string charges { get; set; }
        [Parameter] public string id { get; set; }
        [Parameter] public string projectid { get; set; }
        [Parameter] public string workitemid { get; set; }
        [Parameter] public RadzenDataGrid<PRItemModel> prItemGrid { get; set; }
        TotalProcuredModel totalprocured = new();
        
        string remainingtext = "Remaining Quantity";
        double remainingquantity = 0;

        protected override async Task OnInitializedAsync()
        {
            if (charges != "Admin")
            {
                totalprocured = await httpClient.GetFromJsonAsync<TotalProcuredModel>("purchaserequest/gettotalprocured?projectid=" + projectid + "&itemid=" + prItem.itemid);
                List<MaterialsModel> items = await httpClient.GetFromJsonAsync<List<MaterialsModel>>("purchaserequest/getmaterialsquantity?projectid=" + projectid + "&itemid=" + prItem.itemid + 
                    "&workitemid=" + workitemid);
            
                totalprocured.totalquantity = (double)items.First(q => q.itemid.Equals(prItem.itemid)).quantity;
                remainingquantity = totalprocured.totalquantity - totalprocured.totalprocured;
            }
            else
            {
                
            }
            
        }

        public async Task SaveQuantity(PRItemModel args)
        {
            List<string> paramList = new();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(prItem.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(prItem.quantity));

            await httpClient.PostAsJsonAsync("purchaserequest/updatepritem", paramList);

            prItemGrid.Reload();
            
            NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Quantity has been updated",
                       Duration = 3000
                   });

            dialog.Close(true);

        }

        void CloseDialog()
		{
            dialog.Close();
        }

    }
}