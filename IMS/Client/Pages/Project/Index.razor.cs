﻿
using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using Radzen.Blazor;
using IMS.Client.Shared;
using Microsoft.AspNetCore.Components;
using IMS.Client.Pages.Maintenance;
using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;


namespace IMS.Client.Pages.Project
{
    
    [Authorize]
    partial class Index
    {
        //[CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        
        List<ProjectModel> projects;
        List<ProjectModel> filteredprojects;

        RadzenDataGrid<ProjectModel> grid;

        IList<ProjectModel> selectedItems;
        

        protected override async Task OnInitializedAsync()
        {
            //var authState = await authenticationStateTask;
            //var user = authState.User;

            //if (user.Identity.IsAuthenticated)
            //{
            //    Console.WriteLine($"User {user.Identity.Name} is authenticated.");
            //}
            //else
            //{
            //    Console.WriteLine("User is NOT authenticated.");
            //}

            string token = await _localStorage.GetItemAsync<string>("authToken");

            Console.Write(token);
            
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            //var projects = await httpClient.GetFromJsonAsync<List<ProjectModel>>("project/getprojects");
            projects = await httpClient.GetFromJsonAsync<List<ProjectModel>>("project/getprojects");
            //projects = await httpClient.GetFromJsonAsync<List<ProjectModel>>("https://imsgetprojects.azurewebsites.net/api/getprojects?code=NJIDeaOp9zYZsf2XUq2lBGbzb2me69IjV2T26JRozL9AAzFuflQhyA==");
            
            foreach(ProjectModel proj in projects)
            {
                if (proj.workitems != null)
                {
                    foreach(WorkItemModel work in proj.workitems)
                    {
                        Console.WriteLine(work.totalamount);
                        proj.projectcost += work.totalamount;
                    }
                }
            }            

            filteredprojects = projects;

        }

        public async Task UpdateProject(string id)
        {
            var result = await DialogService.OpenAsync<NewProject>("Add new Project",
                   new Dictionary<string, object>() { { "project", projects.Find(q => q.Id.Equals(id)) }},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        }

        public async Task RemoveProject(string Id)
        {
             var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this project?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });
            
            if (result)
            {
               Console.WriteLine(Id);
                var res = await httpClient.PostAsJsonAsync("project/removeproject", Id );
                //add res verification

                projects.Find(q => q.Id.Equals(Id)).isactive = 0;

                projects.RemoveAll(q => q.isactive.Equals(0));
                filteredprojects = projects;
                grid.Reload();


                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Project has been removed",
                        Duration = 3000
                    });

            }
        }
        
        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredprojects = projects.Where(q => q.projectname.ToLower().Contains(Value.ToLower()) || q.location.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredprojects = projects;
            }
        }


        public async Task AddProject()
        {
            ProjectModel project = new ProjectModel();
            var result = await DialogService.OpenAsync<NewProject>("Add new Project",
                   new Dictionary<string, object>() { { "project", project }},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        }

        public async Task EditMaterial(string id)
        {
            
        }

        public void ProjectDetails(string id)
        {
            ProjectModel project = projects.Find(q => q.Id.Equals(id));
            _navigationManager.NavigateTo("/project/details/" + id);
        }

       

    }
}

