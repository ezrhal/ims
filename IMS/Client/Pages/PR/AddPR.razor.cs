using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IMS.Client.Pages.PR
{
    public partial class AddPR
    {
        [Parameter] public List<PRModel> PRs { get; set; }
        [Parameter] public bool projectview { get; set; }
        [Parameter] public string projectid { get; set; }

        PRModel pr = new();
        List<ProjectModel> projects;
        List<WorkItemInfoModel> workitems;
        private List<string> charges = new();
        private bool chargesProject = false;

        protected override async Task OnInitializedAsync()
        {
            projects = await httpClient.GetFromJsonAsync<List<ProjectModel>>("purchaserequest/getprojectname");
            charges.Add("Project");
            charges.Add("Admin");
            charges.Add("Motor Pool");
            
            if (projectview)
            {
                workitems = await httpClient.GetFromJsonAsync<List<WorkItemInfoModel>>("purchaserequest/getworkitemsinfo?projectid=" + projectid);
            }
            
        }

        public async Task SavePR(PRModel args)
        {
            Console.WriteLine(projectview);
            if (projectview)
            {
                if (projectview)
                    args.projectid = projectid;
                
                args.charges = "Project";
                args.projectname = projects.First(q => q.Id.Equals(args.projectid)).projectname;
                args.workitem = workitems.First(q => q.workitemid.Equals(args.workitemid)).workitem;
            }

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

        public void SelectCharges()
        {
            if (pr.charges == "Project")
            {
                chargesProject = true;
            }
            else
            {
                chargesProject = false;
            }
            
        }
        
        public async Task SelectProject()
        {
            workitems = await httpClient.GetFromJsonAsync<List<WorkItemInfoModel>>("purchaserequest/getworkitems?projectid=" + pr.projectid);
        }

        void CloseDialog()
		{
            DialogService.Close();
        }
    }
}