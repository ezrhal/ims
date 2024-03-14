using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;

namespace IMS.Client.Pages.PO
{
    public partial class EditQuantity
    {
        [Parameter] public POItemModel poitem { get; set; }
        [Parameter] public RadzenDataGrid<POItemModel> poItemGrid { get; set; }
        [Parameter] public POModel po { get; set; }

        void CloseDialog()
        {
            dialog.Close(false);
        }

        public async Task SaveQuantity(POItemModel args)
        {
            List<string> paramList = new();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.prid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(poitem));

            var result = await httpClient.PostAsJsonAsync("purchaseorder/editquantity", paramList);

            NotificationService.Notify(
                new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Item has been updated",
                    Duration = 3000
                }
            );


            poItemGrid.Reload();
        }
    }
}