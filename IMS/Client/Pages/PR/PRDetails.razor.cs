using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using IMS.Client.Shared;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components;


namespace IMS.Client.Pages.PR
{
    public partial class PRDetails
    {
        PRModel pr;
        RadzenDataGrid<PRItemModel> prItemsGrid;
        [Parameter] public string prid {get;set;} = "";
        [Parameter] public bool projectview { get; set; } = false;

        [Parameter] public EventCallback<PRViewModel> OnDetailsView { get; set; }

        [CascadingParameter(Name = "PRViewKey")] 
        private PRViewModel prview {get;set;} = null;
        protected override async Task OnInitializedAsync()
        {
            if (prid == "" )
                prid =  navigationManager.Uri.Split("?")[1].Split("=")[1];
            
            pr = await httpClient.GetFromJsonAsync<PRModel>("purchaserequest/getpr?id=" + prid);
            pr.amount = (double)pr.items.Sum(q => q.quantity * q.unitcost);
        }

        public async Task AddItems()
        {
            string itemcount = (pr.items == null ? "0" : "1");   
            var result = await DialogService.OpenAsync<AddPRItems>("Add work item ",
                   new Dictionary<string, object>() { { "pr", pr }, {"prItemsGrid", prItemsGrid}},
                   new DialogOptions() { Width = "700px", Resizable = false, Draggable = true });
            
           pr.amount = (double)pr.items.Sum(q => q.quantity * q.unitcost);

        }

        public async Task DeletePRItem(string id)
        {
             var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this item?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                List<string> paramList = new();
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(pr.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(id));

                await httpClient.PostAsJsonAsync("purchaserequest/removepritem", paramList);

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PR Item has been removed",
                        Duration = 3000
                    }
                );

                pr.items.Remove(pr.items.Find(q => q.Id.Equals(id)));
                prItemsGrid.Reload();
                pr.amount = (double)pr.items.Sum(q => q.quantity * q.unitcost);
            }
           
        }

        public async Task EditQuantity(string id)
        {
            PRItemModel prItem = pr.items.First(q => q.Id.Equals(id));

            string itemcount = (pr.items == null ? "0" : "1");   
            var result = await DialogService.OpenAsync<EditQuantity>("Edit Quantity ",
                   new Dictionary<string, object>() { { "id", pr.Id }, {"prItemGrid", prItemsGrid}, {"prItem", prItem}, {"projectid", pr.projectid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
            
            if (result != null)
            {
                pr.amount = (double)pr.items.Sum(q => q.quantity * q.unitcost);
            }

        }

        async Task SubmitPR(string id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Submit",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to submit this PR?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                await httpClient.PostAsJsonAsync("purchaserequest/submitpr", id);
                pr.submitted = 1;

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PR has been submitted",
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
            prview.openpr = false;
            pr.projectid = "";
            await OnDetailsView.InvokeAsync(prview);
        }
    }
}