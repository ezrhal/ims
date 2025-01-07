using IMS.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Project
{
    public partial class MapLocation
    {
         
        [Parameter] public IJSObjectReference _module {get;set;}
        [Parameter] public GeoDataModel geoData {get; set;}
         
        protected override async Task OnInitializedAsync()
        {
            
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            
            try
            {
                double Lat = 8.55387619375953;
                double Lng = 125.94693580091435;
                await JSRuntime.InvokeVoidAsync("initMap", "map", Lat, Lng);
            
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseSide()
        {
            mapDialogService.CloseSide();
        }
        
        
    }
}