using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace IMS.Client.Pages;

public partial class ImageDisplay
{
    private string imgsource = "https://cloud.pgas.ph/index.php/s/nbo5EzBckn8dk9J/download";
    private List<ImageSourceModel> images;
    
    private Timer timer1;
    
    private int elapsedTime1;
    private int x = 0;
    
    [Parameter] public EventCallback<int> y { get; set; }
    

    protected override async Task OnInitializedAsync()
    {
        x = 0;
        elapsedTime1 = 0;
        images = await httpClient.GetFromJsonAsync<List<ImageSourceModel>>("https://taskbucket.azurewebsites.net/api/GetImages?code=GnHDbkhbseQ-yDNCPIk-ztKSEtjFJ8hN7BvISWP5S9AMAzFuBbZ8SQ==");
        
        timer1 = new Timer(TimerCallback1, null, 0, 1000);
    }

    private async void TimerCallback1(object state)
    {
        elapsedTime1++;

        if (elapsedTime1 == 60)
        {
            elapsedTime1 = 0;
            
            x++;
        
            if (x > images.Count - 1)
            {
                x = 0;
                y.InvokeAsync(2);
            }
            
            imgsource = images[x].src;
            StateHasChanged();
        }
        
    }
    



}