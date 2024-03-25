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
            var result = await DialogService.OpenAsync<AddPO>("Add new PO",
                   new Dictionary<string, object>() { { "POs", POs }, {"projectid", projectid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        
        }

        public async Task DeletePO(string Id, string prid)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this PO?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                List<string> paramList = new();

                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(prid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(Id));

                await httpClient.PostAsJsonAsync("purchaseorder/deletepo", paramList);

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PO Item has been removed",
                        Duration = 3000
                    }
                );

                POs.Remove(POs.First(q => q.Id.Equals(Id)));
                poGrid.Reload();
                //filteredPR = PRs;
            }
        }
        public async Task EditPO(string id)
        {
            POModel po = POs.First(q => q.Id.Equals(id));

            var result = await DialogService.OpenAsync<EditPO>("Edit PO",
                  new Dictionary<string, object>() { { "po", po }, { "poGrid", poGrid } },
                  new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        }

        public async Task OpenPODetails(string id)
        {
            if (!projectviewpo)
            {
                navigationManager.NavigateTo("/purchaseorder/details?poid=" + id);
            }
            else
            {
                poview.openpo = true;
                poview.poid = id;
                await OnDetailsViewPO.InvokeAsync(poview);
                Console.WriteLine(id);
            }

            
        }

    }
}