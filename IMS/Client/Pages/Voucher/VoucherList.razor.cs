using Radzen;
using Radzen.Blazor;
using IMS.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using IMS.Client.Shared;


namespace IMS.Client.Pages.Voucher
{
    public partial class VoucherList
    {
        [Parameter] public bool projectviewcv { get; set; } = false;
        [Parameter] public string projectid { get; set; } = "";

        [Parameter] public EventCallback<CVViewModel> OnDetailsViewCV { get; set; }

        [CascadingParameter(Name = "CVViewKey")]
        private CVViewModel cvview {get; set;}
        RadzenDataGrid<POModel> cvGrid;
         RadzenDataGrid<POModel> cvSubmittedGrid;
        List<POModel> CVs;
        List<POModel> CVSubmitted;
        List<POModel> filteredCV;
        List<POModel> filteredCVSubmitted;
        IList<POModel> selectedCV;

        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredCV = CVs.Where(q => q.pono.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredCV = CVs;
            }
        }

        public async Task AddCV()
        {
            var result = await DialogService.OpenAsync<AddCV>("Add new Voucher",
                   new Dictionary<string, object>() { { "CVs", CVs }, {"projectid", projectid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        
        }

        public async Task DeleteCV(string Id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this CV?"}},
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

                CVs.Remove(CVs.First(q => q.Id.Equals(Id)));
                cvGrid.Reload();
                //filteredPR = PRs;
            }
        }

        public async Task OpenCVDetails(string Id)
        {
            if (!projectviewcv)
            {
                navigationManager.NavigateTo("/purchaseorder/details?poid=" + Id);
            }
            else
            {
                cvview.opencv = true;
                cvview.cvid = Id;
                await OnDetailsViewCV.InvokeAsync(cvview);
                Console.WriteLine(Id);
            }

            
        }
    }
}