using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;


namespace IMS.Client.Pages.PO
{
    public partial class AddPOItems
    {
        [Parameter] public POModel po { get; set; }
        [Parameter] public RadzenDataGrid<POItemModel> poItemsGrid { get; set; }
        RadzenDataGrid<PRItemModel> grid;
        IList<PRItemModel> selectedItems;
        List<PRItemModel> pritems;
        List<PRItemModel> filteredItems = new List<PRItemModel>();

        private bool itemadded = false;

        protected override async Task OnInitializedAsync()
        {
            pritems = await httpClient.GetFromJsonAsync<List<PRItemModel>>("purchaseorder/getpritems?prid=" + po.prid);
            filteredItems = pritems;
        }

        async Task LoadData(LoadDataArgs args)
        {
            StateHasChanged();
        }

        void CloseDialog()
		{
            dialog.Close(itemadded);
        }

        public async Task SavePOItems()
        {
            List<string> paramList = new();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.prid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(selectedItems));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.items));

            var ret = await httpClient.PostAsJsonAsync("purchaseorder/savepoitems", paramList);

            string result = await ret.Content.ReadAsStringAsync();

            if (result != "")
            {
                
                 NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PO item has been save",
                        Duration = 3000
                    });

                foreach(PRItemModel pritem in selectedItems)
                {
                    pritems.Remove(pritem);
                }

                filteredItems = pritems;

                grid.Reload();
                selectedItems = null;
                itemadded = true;

            }

        }

    }
}