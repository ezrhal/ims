using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using Radzen.Blazor;
using IMS.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using IMS.Shared;


namespace IMS.Client.Pages.Project
{

	partial class NewProject
	{
		[Parameter] public ProjectModel project { get; set; }

        IJSObjectReference _module;

        protected override async Task OnInitializedAsync()
        {
            //project.geodata.address = "9246+62 Rosario, Agusan del Sur, Philippines";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            
            try
            {
                // if(_module is null)
                //     _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", AppSettings.GetMapInitLoc);
                
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SaveProject(ProjectModel args)
        {
            //if (edit == 0)
            //{
            //    var result = await httpClient.PostAsJsonAsync<ItemModel>("maintenance/savematerial", args);
            //    //string _id = result.Content.ReadFromJsonAsync<string>().Result;

            //    string _id = await result.Content.ReadAsStringAsync();
            //    args.Id = _id;
            //}
            //else
            //{
            //    await httpClient.PostAsJsonAsync<ItemModel>("maintenance/saveeditmaterial", args);
            //}

            if (project.Id == null || project.Id == "")
            {
                var res = await httpClient.PostAsJsonAsync("project/saveproject", args);
                string id = await res.Content.ReadAsStringAsync();

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Project has been save",
                        Duration = 3000
                    });
                project.Id = id;
                navigationManager.NavigateTo("/project/Details?id=" + id);
            }
            else
            {
                var res = await httpClient.PostAsJsonAsync("project/updateproject", args);
                string ret = await res.Content.ReadAsStringAsync();

                NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Project has been save",
                        Duration = 3000
                    });
            }
           
            dialog.Close(args);
            
        }

        void CloseDialog()
        {
            dialog.Close();
        }

        public async Task MapLoc()
        {
            
            var result = await dialog.OpenSideAsync<MapLocation>("Set Project Location",
                   new Dictionary<string, object>() {{"geoData", project.geodata}},
                   new SideDialogOptions() { Position = DialogPosition.Right, Width = "600px", CloseDialogOnOverlayClick = true});

        }

        public async Task HandleChange (ChangeEventArgs args){
   
            Console.WriteLine(args.Value.ToString());
        }

        private void OnInputEvent(ChangeEventArgs args)
        {
            project.geodata.address = args.Value.ToString();
        }

        private void OnInputPlusCode(ChangeEventArgs args)
        {
            project.geodata.pluscode = args.Value.ToString();
            Console.WriteLine(project.geodata.pluscode);
        }
        private void OnInputLat(ChangeEventArgs args)
        {
            project.geodata.Lat = args.Value.ToString();
            Console.WriteLine(project.geodata.Lat);
        }
        private void OnInputLng(ChangeEventArgs args)
        {
            project.geodata.Lng = args.Value.ToString();
            Console.WriteLine(project.geodata.Lng);
        }


    }
}

