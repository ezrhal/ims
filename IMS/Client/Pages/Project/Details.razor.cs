using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using IMS.Client.Shared;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Authorization;


namespace IMS.Client.Pages.Project
{
    [Authorize]
    partial class Details
    {
        [Parameter] public string projectid {get;set;} = "";
        
        PRViewModel prview = new();
        POViewModel poview = new();
        CVViewModel cvview = new();
        public string prid { get; set; } = "";
        public ProjectModel project = new();
        RadzenDataGrid<WorkItemModel> workitemGrid;
        RadzenDataGrid<MaterialsModel> materialsGrid;
        RadzenDataGrid<EquipmentModel> equipmentGrid;
        RadzenDataGrid<LaborModel> laborGrid;
        

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine(projectid);
            try
            {
                //projectid =  navigationManager.Uri.Split("?")[1].Split("=")[1];
                project = await httpClient.GetFromJsonAsync<ProjectModel>("project/getproject?id=" + projectid);
            }
            catch{}

            if (projectid == "")
            {
                navigationManager.NavigateTo("project/index");
            }

            //project.Id = "653f3f559a70aab51f68bdba";
        } 

        public async Task AddWorkItem()
        {
            Console.WriteLine("aaa" + project.Id);
            string workitemcount = (project.workitems == null ? "0" : "1");   
            var result = DialogService.OpenAsync<AddWorkItem>("Add work item ",
                   new Dictionary<string, object>() { { "project", project }, {"workitemGrid", workitemGrid}, {"isempty", workitemcount}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

        }

        public async Task EditWorkItem(string id)
        {

        }

        public async Task DeleteWorkItem(string id)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this item?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });
            
            if (result)
            {
                List<string> paramList = new List<string>();

                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(id));

                var res = await httpClient.PostAsJsonAsync("project/deleteworkitem", paramList);
                //add res verification

                project.workitems.Find(q => q.Id.Equals(id)).isactive = 0;

                project.workitems.RemoveAll(q => q.isactive.Equals(0));
                workitemGrid.Reload();


                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Workitem has been removed",
                        Duration = 3000
                    });

            }

        }

        public async Task AddMaterial(string id)
        {

            string materialcount = (project.workitems.First(q => q.Id.Equals(id)).materials == null ? "0" : "1");
            bool isadded = false;
            
            var result = DialogService.OpenAsync<AddMaterials>("Add material ",
                   new Dictionary<string, object>() { {"materialsGrid", materialsGrid}, {"project", project}, {"workitemid", id}, {"isempty", materialcount}, 
                   {"isadded", isadded}, {"workitemGrid", workitemGrid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (isadded)
            {
               
            }
        }

        public async Task AddEquipment(string id)
        {
            string equipmentcount = (project.workitems.First(q => q.Id.Equals(id)).equipment == null ? "0" : "1");
            bool isadded = false;
            
            var result = DialogService.OpenAsync<AddEquipment>("Add equipment ",
                   new Dictionary<string, object>() { {"equipmentGrid", equipmentGrid}, {"project", project}, {"workitemid", id}, {"isempty", equipmentcount}, 
                   {"isadded", isadded}, {"workitemGrid", workitemGrid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (isadded)
            {
                
            }
        }

        public async Task AddLabor(string id)
        {
            string materialcount = (project.workitems.First(q => q.Id.Equals(id)).labor == null ? "0" : "1");
            bool isadded = false;
            
            var result = DialogService.OpenAsync<AddLabor>("Add Labor ",
                   new Dictionary<string, object>() { {"laborGrid", laborGrid}, {"project", project}, {"workitemid", id}, {"isempty", materialcount}, 
                   {"isadded", isadded}, {"workitemGrid", workitemGrid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (isadded)
            {
               
            }
        }

        public async Task RemoveItem(string workitemid, string id, string type)
        {
            var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                   new Dictionary<string, object>() {{"message", "Are you sure you want to delete this item?"}},
                   new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });
            
            if (result)
            {
                List<string> paramList = new List<string>();

                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(project.Id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(workitemid));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(id));
                paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(type));

                var res = await httpClient.PostAsJsonAsync("project/removeitem", paramList);


                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Item has been removed",
                        Duration = 3000
                    });

                WorkItemModel workitem = project.workitems.First(q => q.Id.Equals(workitemid));
                

                if (type == "materials")
                {
                    MaterialsModel material = workitem.materials.First(q => q.Id.Equals(id));
                    workitem.totalmaterials -= material.amount;
                    workitem.totalamount -= material.amount;
                    project.workitems.First(q => q.Id.Equals(workitemid)).materials.Remove(material);
                    materialsGrid.Reload();
                }
                else if (type == "equipment")
                {
                    EquipmentModel equipment = workitem.equipment.First(q => q.Id.Equals(id));
                    workitem.totalmaterials -= equipment.amount;
                    workitem.totalamount -= equipment.amount;
                    project.workitems.First(q => q.Id.Equals(workitemid)).equipment.Remove(equipment);
                    equipmentGrid.Reload();
                }
                else
                {
                    LaborModel labor = workitem.labor.First(q => q.Id.Equals(id));
                    workitem.totalmaterials -= labor.amount;
                    workitem.totalamount -= labor.amount;
                    project.workitems.First(q => q.Id.Equals(workitemid)).labor.Remove(labor);
                    laborGrid.Reload();
                }
               
            }
            
        }

        public async Task UpdateQuantity(string workitemid, MaterialsModel data)
        {
             var result = await DialogService.OpenAsync<UpdateQuantity>("Edit quantity ",
                   new Dictionary<string, object>() { {"materialsGrid", materialsGrid}, {"project", project}, {"workitemid", workitemid},
                   {"material", data}, {"workitemGrid", workitemGrid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        }

        public async Task UpdateEquipment(string workitemid, EquipmentModel data)
        {
             var result = await DialogService.OpenAsync<UpdateEquipment>("Edit quantity ",
                   new Dictionary<string, object>() { {"equipmentGrid", equipmentGrid}, {"project", project}, {"workitemid", workitemid},
                   {"equipment", data}, {"workitemGrid", workitemGrid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        }

        public async Task UpdateLabor(string workitemid, LaborModel data)
        {
             var result = await DialogService.OpenAsync<UpdateLabor>("Edit quantity ",
                   new Dictionary<string, object>() { {"laborGrid", laborGrid}, {"project", project}, {"workitemid", workitemid},
                   {"labor", data}, {"workitemGrid", workitemGrid}},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        }

        async Task LoadData(LoadDataArgs args)
        {
            StateHasChanged();
        }

        async void PrintProject()
        {
            //navigationManager.NavigateTo("/Viewer?projectid=" + projectid);
            await JSRuntime.InvokeVoidAsync("open", "api/reports/printproject?projectid=" + projectid , "_blank");  
        }

        async void PrintWorkItemsDetails(string id)
        {
            await JSRuntime.InvokeVoidAsync("open", "api/reports/printworkitem?projectid=" + projectid + "&workitemid=" + id, "_blank");  
        }


        #region SPIRE

        public async Task ReportWorkitem(string id)
        {
            await httpClient.GetFromJsonAsync<string>("api/reports/generatereport");
        }

        #endregion


        void OnDetailsViewFromChild(PRViewModel _prview) => prview = _prview;
        void OnDetailsViewPOFromChild(POViewModel _poview) => poview = _poview;

        void OnDetailsViewCVFromChild(CVViewModel _cvview) => cvview = _cvview;
        
    }
}