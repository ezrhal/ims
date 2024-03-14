using System;
using System.Net.Http;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen;
using MongoDB.Bson;
using Radzen.Blazor;

namespace IMS.Client.Pages.Project
{
    partial class AddWorkItem
    {
        [Parameter] public ProjectModel project {get;set;}
        [Parameter] public RadzenDataGrid<WorkItemModel> workitemGrid {get; set;}
        [Parameter] public string isempty { get; set; }
    
        List<WorkItemModel> workitems;
        WorkItemModel workitem = new();
        
        string workitemid;
        //string projectid = "653f3f559a70aab51f68bdba";

         protected override async Task OnInitializedAsync()
        {
            workitems = await httpClient.GetFromJsonAsync<List<WorkItemModel>>("maintenance/getworkitems");
        }

        public async Task SaveWorkItem()
        {
            
            workitem = workitems.Find(q => q.Id.Equals(workitemid));
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(workitem));
            workitem.Id = BsonObjectId.GenerateNewId().ToString();
            workitem.workitemid = workitemid;
            workitem.materials = new();
            workitem.equipment = new();
            workitem.labor = new();

            List<string> paramList = new List<string>();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(workitem));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(isempty));


            var res = await httpClient.PostAsJsonAsync("project/saveworkitemproject", paramList);
            
            if (project.workitems == null)
            {
                project.workitems = new()
                {
                    workitem
                };
            }
            else
            {
                project.workitems.Add(workitem);
            }

            
            workitemGrid.Reload();
            
        }

        void CloseDialog()
		{
            dialog.Close();
        }
    }
}