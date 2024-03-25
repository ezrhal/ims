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

        protected override async Task OnInitializedAsync()
        {
            CVs = await httpClient.GetFromJsonAsync<List<POModel>>("voucher/getcvs?projectid=" + projectid);
            
            filteredCVSubmitted = CVs.Where(q => q.cvsubmitted.Equals(1)).ToList();
            CVs.RemoveAll(q => q.cvsubmitted.Equals(1));
            filteredCV = CVs;
            cvview = new();
        }

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
                   new Dictionary<string, object>() { { "CVs", CVs }, {"cvGrid", cvGrid }, {"projectid", projectid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        
        }

        public async Task DeleteCV(string Id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this CV?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                POModel cv = CVs.First(q => q.Id.Equals(Id));

                string data = Newtonsoft.Json.JsonConvert.SerializeObject(cv);

                await httpClient.PostAsJsonAsync("voucher/deletecv", data);

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "CV Item has been removed",
                        Duration = 3000
                    }
                );

                CVs.Remove(cv);
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
                
            }

        }

        async Task ReturnCV(string id)
        {
            Console.WriteLine(id);
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Return",
                   new Dictionary<string, object>() { { "message", "Are you sure you want to return this CV?" } },
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {
                POModel cv = filteredCVSubmitted.First(q => q.Id.Equals(id));

                List<string> paramList = new();
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(cv.prid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(cv.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(0));

                await httpClient.PostAsJsonAsync("voucher/submitcv", paramList);
                cv.cvsubmitted = 0;
                filteredCVSubmitted.Remove(cv);
                filteredCV.Add(cv);

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "CV has been returned",
                        Duration = 3000
                    }
                );
                cvSubmittedGrid.Reload();
                cvGrid.Reload();
            }

        }
    }
}