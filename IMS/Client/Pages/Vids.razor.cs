using System.Net.Http.Json;
using Blazored.Video;
using Blazored.Video.Support;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using BlazorVideoPlayer;
using Radzen.Blazor;
using Vidazor;

namespace IMS.Client.Pages;

public partial class Vids
{Dictionary<VideoEvents, VideoStateOptions> options = new Dictionary<VideoEvents, VideoStateOptions>();
    
    private BlazoredVideo video;
    private string videosource = "https://cloud.pgas.ph/index.php/s/bTjLZPEEmEE2mSY/download";
    
    private Timer timer1;
    private int elapsedTime1;
    private RadzenTextBox myfocus;
    private int x = 0;

    private List<VideoModel> videos = new();
    
    [Parameter] public EventCallback<int> y { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        videos = await httpClient.GetFromJsonAsync<List<VideoModel>>("https://taskbucket.azurewebsites.net/api/GetVideos?code=GnHDbkhbseQ-yDNCPIk-ztKSEtjFJ8hN7BvISWP5S9AMAzFuBbZ8SQ==");

        if (videos.Count > 0)
        {
            videosource = videos.First().src;
        }
        
        timer1 = new Timer(TimerCallback1, null, 0, 1000);
    }

    private async void TimerCallback1(object state)
    {
        elapsedTime1++;
        if (elapsedTime1 == 5)
        {
            video.StartPlayback();
            timer1.Dispose();
        }
    }

    void VideoEnded(VideoState videoState)
    {
        x++;
        
        if (x > videos.Count - 1)
        {
            x = 0;
            y.InvokeAsync(1);
        }
        
        videosource = videos[x].src;
        video.ReloadControl();
        video.StartPlayback();
       
        // else
        // {
        //     y.InvokeAsync(1);
        //     x = 0;
        // }

        // if (x == videos.Count)
        // {
        //     videosource = "https://cloud.pgas.ph/index.php/s/bTjLZPEEmEE2mSY/download";
        //     x = 0;
        //     video.ReloadControl();
        //     video.StartPlayback();
        // }
        
    }

    void playvideo(VideoState videoState)
    {
        Console.WriteLine("aaaa");
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
           
        }
    }

   
}
