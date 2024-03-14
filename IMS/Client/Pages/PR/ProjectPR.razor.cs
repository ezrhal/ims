using Radzen;
using Radzen.Blazor;
using IMS.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using IMS.Client.Shared;

namespace IMS.Client.Pages.PR
{
    public partial class ProjectPR
    {
        [Parameter] public bool projectview { get; set; } = false;
        [Parameter] public string projectid { get; set; } = "";
        [Parameter] public string projectname { get; set; }

        [Parameter] public EventCallback<PRViewModel> OnDetailsView { get; set; }

        [CascadingParameter(Name = "PRViewKey")]
        private PRViewModel prview {get; set;}
        RadzenDataGrid<PRModel> prGrid;
        List<PRModel> PRs;
        List<PRModel> PRSubmitted;
        List<PRModel> filteredPR;
        List<PRModel> filteredPRSubmitted;
        IList<PRModel> selectedItems;



        protected override async Task OnInitializedAsync()
        {
            PRs = await httpClient.GetFromJsonAsync<List<PRModel>>("purchaserequest/getprs?projectid=" + projectid);
            PRSubmitted = PRs.Where(q => q.submitted.Equals(1)).ToList();
            PRs.RemoveAll(q => q.submitted.Equals(1));

            prview = new();

            foreach(PRModel pr in PRs)
            {
                if (pr.items != null)
                {
                    pr.amount = pr.items.Sum(q => q.unitcost * (q.quantity ?? 0));
                }
                else
                {
                    pr.amount = 0;
                }
                
            }

            if (PRs != null)
                filteredPR = PRs;
            
            if (PRSubmitted != null)
                filteredPRSubmitted = PRSubmitted;
            
        }

        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredPR = PRs.Where(q => q.projectname.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredPR = PRs;
            }
        }

        public async Task AddPR()
        {
            if (projectview)
            {
                PRModel pr = new();
                pr.projectid = projectid;
                pr.projectname = projectname;

                var ret = await httpClient.PostAsJsonAsync("purchaserequest/savepr", pr);
                string result = await ret.Content.ReadAsStringAsync();

                if (result != "")
                {
                    pr.Id = result;
                    PRs.Add(pr);

                    NotificationService.Notify(
                        new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = "PR has been save",
                            Duration = 3000
                        });

                    prview.openpr = true;
                    prview.prid = result;
                    await OnDetailsView.InvokeAsync(prview);

                    
                }
            }
            else
            {
                 var result = await DialogService.OpenAsync<AddPR>("Add new PR",
                   new Dictionary<string, object>() { { "PRs", PRs }},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
            }
        
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

                PRs.Remove(PRs.First(q => q.Id.Equals(Id)));
                prGrid.Reload();
                //filteredPR = PRs;
            }
        }

        public async Task OpenPRDetails(string Id)
        {
            if (!projectview)
            {
                navigationManager.NavigateTo("/purchaserequest/details?prid=" + Id);
            }
            else
            {
                prview.openpr = true;
                prview.prid = Id;
                await OnDetailsView.InvokeAsync(prview);
                Console.WriteLine(Id);
            }

            
        }

    }
}