using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using IMS.Client.Shared;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace IMS.Client.Pages.Voucher
{
	public partial class CVDetails
	{
        POModel cv;
        [Parameter] public string poid { get; set; } = "";
        [Parameter] public string prid { get; set; }
        
        [Parameter] public bool projectviewcv { get; set; } = false;

        [Parameter] public EventCallback<CVViewModel> OnDetailsViewCV { get; set; }

        [CascadingParameter(Name = "CVViewKey")]
        private CVViewModel cvview { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            if (poid == "")
                poid = navigationManager.Uri.Split("?")[1].Split("=")[1];

            cv = await httpClient.GetFromJsonAsync<POModel>("voucher/getcv?poid=" + poid);
            //cv.amount = (double)cv.items.Sum(q => q.quantity * q.price);
        }

        

        async Task SubmitCV(string id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Submit",
                   new Dictionary<string, object>() { { "message", "Are you sure you want to submit this CV?" } },
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

            if (result)
            {

                List<string> paramList = new();
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(cv.prid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(cv.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(1));

                await httpClient.PostAsJsonAsync("voucher/submitcv", paramList);
                cv.cvsubmitted = 1;

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "CV has been submitted",
                        Duration = 3000
                    }
                );
            }

        }

        async Task LoadData(LoadDataArgs args)
        {
            StateHasChanged();
        }
        
        public async Task PrintCV(string id)
        {
            await JSRuntime.InvokeVoidAsync("open", "api/reports/printcv?poid=" + id, "_blank"); 
        }

        async void ButtonBackClick()
        {
            cvview.opencv = false;
            cv.projectid = "";
            await OnDetailsViewCV.InvokeAsync(cvview);
        }
    }
}

