using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace IMS.Client.Pages
{
    public partial class Viewer
    {
        [Parameter] public string report {get;set;}
        [Parameter] public string projectid { get; set; }
        [Parameter] public string workitemid { get; set; }

        protected override async Task OnInitializedAsync()
        {
            report = navigationManager.Uri.Split("?")[1].Split("=")[1].Split("&")[0];

            projectid =  navigationManager.Uri.Split("&")[1].Split("=")[1];
            try{
                workitemid = navigationManager.Uri.Split("&")[2].Split("=")[1];
            }catch{}
            
            
        }
    }
}