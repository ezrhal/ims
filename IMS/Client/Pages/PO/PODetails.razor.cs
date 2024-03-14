using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using IMS.Client.Shared;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components;


namespace IMS.Client.Pages.PO
{
    public partial class PODetails
    {
        POModel po;
        RadzenDataGrid<POItemModel> poItemsGrid;
        [Parameter] public string poid {get;set;} = "";
        [Parameter] public string prid { get; set; }
        [Parameter] public bool projectviewpo { get; set; } = false;

        [Parameter] public EventCallback<POViewModel> OnDetailsViewPO { get; set; }

        [CascadingParameter(Name = "POViewKey")] 
        private POViewModel poview {get;set;} = null;

        protected override async Task OnInitializedAsync()
        {
            if (poid == "" )
                poid =  navigationManager.Uri.Split("?")[1].Split("=")[1];
            
            po = await httpClient.GetFromJsonAsync<POModel>("purchaseorder/getpo?poid=" + poid);
            po.amount = (double)po.items.Sum(q => q.quantity * q.price);
        }

        public async Task AddItems()
        {
            string itemcount = (po.items == null ? "0" : "1");   
            var result = await DialogService.OpenAsync<AddPOItems>("Add PR items ",
                   new Dictionary<string, object>() { { "po", po }, {"poItemsGrid", poItemsGrid}},
                   new DialogOptions() { Width = "800px", Resizable = false, Draggable = true });
            
            po = await httpClient.GetFromJsonAsync<POModel>("purchaseorder/getpo?poid=" + poid);

            po.amount = (double)po.items.Sum(q => q.quantity * q.price);

        }

        public async Task DeletePOItem(string id)
        {
             var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this item?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                List<string> paramList = new();
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.prid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(poid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(id));

                await httpClient.PostAsJsonAsync("purchaseorder/deletepoitem", paramList);

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PO Item has been removed",
                        Duration = 3000
                    }
                );

                po.items.Remove(po.items.Find(q => q.Id.Equals(id)));
                poItemsGrid.Reload();
                po.amount = (double)po.items.Sum(q => q.quantity * q.price);
            }
           
        }

        public async Task EditQuantity(string id)
        {
            POItemModel poItem = po.items.First(q => q.Id.Equals(id));

            string itemcount = (po.items == null ? "0" : "1");   
            var result = await DialogService.OpenAsync<EditQuantity>("Edit Quantity ",
                   new Dictionary<string, object>() {{"poItemGrid", poItemsGrid}, {"poItem", poItem}, {"po", po}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
            
            if (result != null)
            {
                po.amount = (double)po.items.Sum(q => q.quantity * q.price);
            }

        }

        async Task SubmitPO(string id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Submit",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to submit this PO?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                List<string> paramList = new();
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.prid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po.Id));

                await httpClient.PostAsJsonAsync("purchaseorder/submitpo", paramList);
                po.submitted = 1;

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PO has been submitted",
                        Duration = 3000
                    }
                );
            }
           
        }

        async Task LoadData(LoadDataArgs args)
        {
            StateHasChanged();
        }

        async void ButtonBackClick()
        {
            poview.openpo = false;
            po.projectid = "";
            await OnDetailsViewPO.InvokeAsync(poview);
        }
    }
}