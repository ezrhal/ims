using Radzen;
using Radzen.Blazor;
using IMS.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using IMS.Client.Shared;

namespace IMS.Client.Pages.PO
{
    public partial class POList
    {
        [Parameter] public bool projectviewpo { get; set; } = false;
        [Parameter] public string projectid { get; set; } = "";
        [Parameter] public string projectname { get; set; }

        [Parameter] public EventCallback<POViewModel> OnDetailsViewPO { get; set; }

        [CascadingParameter(Name = "POViewKey")]
        private POViewModel poview {get; set;}
        RadzenDataGrid<POModel> poGrid;
        List<POModel> POs;
        List<POModel> POSubmitted;
        List<POModel> filteredPO;
        List<POModel> filteredPOSubmitted;
        IList<POModel> selectedPO;



        protected override async Task OnInitializedAsync()
        {
            POs = await httpClient.GetFromJsonAsync<List<POModel>>("purchaseorder/getpos?projectid=" + projectid);
            POSubmitted = POs.Where(q => q.submitted.Equals(1)).ToList();
            POs.RemoveAll(q => q.submitted.Equals(1));

            poview = new();

            foreach(POModel po in POs)
            {
                if (po.items != null)
                {
                    po.amount = po.items.Sum(q => q.price * (q.quantity ?? 0));
                }
                else
                {
                    po.amount = 0;
                }
                
            }

            if (POs != null)
                filteredPO = POs;
            
            if (POSubmitted != null)
                filteredPOSubmitted = POSubmitted;
            
        }

        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredPO = POs.Where(q => q.pono.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredPO = POs;
            }
        }

        public async Task AddPO()
        {
            var result = await DialogService.OpenAsync<AddPO>("Add new PR",
                   new Dictionary<string, object>() { { "POs", POs }, {"projectid", projectid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        
        }

        public async Task DeletePR(string Id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this PR?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                await httpClient.PostAsJsonAsync("purchaserequest/deletepr", Id);

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PR Item has been removed",
                        Duration = 3000
                    }
                );

                POs.Remove(POs.First(q => q.Id.Equals(Id)));
                poGrid.Reload();
                //filteredPR = PRs;
            }
        }

        public async Task OpenPODetails(string Id)
        {
            if (!projectviewpo)
            {
                navigationManager.NavigateTo("/purchaseorder/details?poid=" + Id);
            }
            else
            {
                poview.openpo = true;
                poview.poid = Id;
                await OnDetailsViewPO.InvokeAsync(poview);
                Console.WriteLine(Id);
            }

            
        }
    }
}