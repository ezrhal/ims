using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IMS.Client.Pages.PR
{
    public partial class AddPR
    {
        [Parameter] public List<PRModel> PRs { get; set; }
        PRModel pr = new();
        List<ProjectModel> projects;

        protected override async Task OnInitializedAsync()
        {
            projects = await httpClient.GetFromJsonAsync<List<ProjectModel>>("purchaserequest/getprojectname");
        }

        public async Task SavePR(PRModel args)
        {
            args.projectname = projects.First(q => q.Id.Equals(args.projectid)).projectname;

            var ret = await httpClient.PostAsJsonAsync("purchaserequest/savepr", args);
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
                        Detail = "Project has been save",
                        Duration = 3000
                    });

                NavigationManager.NavigateTo("/purchaserequest/Details?id=" + result);
                
            }

        }

        void CloseDialog()
		{
            DialogService.Close();
        }
    }
}