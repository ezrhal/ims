using System;
using System.Net;
using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IMS.Client.Pages;

public partial class OverAllRanking
{
    [Parameter] public EventCallback<int> y { get; set; }
    [Parameter] public bool istimer { get; set; } = false;
        public bool IsSmall = false;
    

        private List<List<MedalTallyModel>> medalTallys = new();
        private List<MedalTallyModel> medalDispay = new();
        private List<MedalTallyModel> firsthalf = new();
        private List<MedalTallyModel> secondhalf = new();
        private List<MedalTallyModel> eventMedals = new();
        private List<MedalTallyModel> pronviceEventMedals = new();
        private int x = 1;

        private readonly PeriodicTimer _changeDisplayTimer = new(TimeSpan.FromSeconds(20));
        private readonly PeriodicTimer _updateDataTimer = new(TimeSpan.FromMinutes(5));

        private Timer timer1;
        private Timer timer2;

        private int elapsedTime1;
        private int elapsedTime2;
        private int elapsedTime3;

        private string rowstyle = "background-color: rgba(145, 200, 228, 0.2)";
        private string rowstyle1 = "background-color: rgba(70, 130, 169, 0.1)";

        private string label = "Overall Ranking";
        private int medalindex = 0;
        private string asof = "";
        
        protected override async Task OnInitializedAsync()
        {
            //medalTallys = await httpClient.GetFromJsonAsync<List<MedalTallyModel>>(
            //    "https://taskbucket.azurewebsites.net/api/CRAGMedalTally?code=GnHDbkhbseQ-yDNCPIk-ztKSEtjFJ8hN7BvISWP5S9AMAzFuBbZ8SQ==");

            medalTallys = await httpClient.GetFromJsonAsync<List<List<MedalTallyModel>>>("https://taskbucket.azurewebsites.net/api/GetAllMedals?code=GnHDbkhbseQ-yDNCPIk-ztKSEtjFJ8hN7BvISWP5S9AMAzFuBbZ8SQ==");

            
            
            var SortedList = medalTallys[2].OrderBy(o => o.rankint).ToList();
                   
            var rank = 1;
            var rnkstr = "";

            asof = SortedList[0].asof;
            
            foreach (var rnk in SortedList)
            {
                if (rnk.rankint == 1)
                {
                    rnkstr = "1st";
                    rnk.rankstyle = "color: #ffcf40; font-size: 20px";
                }
                else if (rnk.rankint == 2)
                {
                    rnk.rankstyle = "color: #ced4da;font-size: 20px";
                    rnkstr = "2nd";
                }
                else if (rnk.rankint == 3)
                {
                    rnk.rankstyle = "color: #cb8c47;font-size: 20px";
                    rnkstr = "3rd";
                }
                else
                {
                    rnk.rankstyle = "font-size: 20px";
                    rnkstr = rnk.rankint + "th";
                }
                
                rnk.Province_id = "image/" + rnk.Province.Trim() + ".png";
                rnk.rank = rnkstr;
            }

            for (var i = 0; i < 6; i++)
            {
                firsthalf.Add(SortedList[i]);
            }

            for (var i = 6; i < 12; i++)
            {
                secondhalf.Add(SortedList[i]);
            }
            
            medalDispay = SortedList;
            
            if (istimer)
            {
                timer1 = new Timer(TimerCallback1, null, 0, 1000);
            }
                
        }

        private async void TimerCallback1(object state)
        {
            elapsedTime1++;
            elapsedTime2++;
            elapsedTime3++;

            // if (elapsedTime3 == 40)
            // {
            //     medalindex++;
            //     
            //     if (medalindex == 3)
            //     {
            //         medalindex = 0;
            //     }
            //
            //     if (medalindex == 0)
            //     {
            //         label = "Elementary";
            //     }
            //     else if (medalindex == 1)
            //     {
            //         label = "Secondary";
            //     }
            //     else if (medalindex == 2)
            //     {
            //         label = "Overall Ranking";
            //     }
            //     
            //     var SortedList = medalTallys[medalindex].OrderBy(o => o.rankint).ToList();
            //        
            //     var rank = 1;
            //     var rnkstr = "";
            //     
            //     foreach (var rnk in SortedList)
            //     {
            //         firsthalf = new();
            //         secondhalf = new();
            //         if (rnk.rankint == 1)
            //         {
            //             rnkstr = "1st";
            //             rnk.rankstyle = "color: #ffcf40; font-size: 30px";
            //         }
            //         else if (rnk.rankint == 2)
            //         {
            //             rnk.rankstyle = "color: #ced4da;font-size: 30px";
            //             rnkstr = "2nd";
            //         }
            //         else if (rnk.rankint == 3)
            //         {
            //             rnk.rankstyle = "color: #cb8c47;font-size: 30px";
            //             rnkstr = "3rd";
            //         }
            //         else
            //         {
            //             rnk.rankstyle = "font-size: 30px";
            //             rnkstr = rnk.rankint + "th";
            //         }
            //     
            //         rnk.Province_id = "image/" + rnk.Province.Trim() + ".png";
            //         rnk.rank = rnkstr;
            //     }
            //
            //     for (var i = 0; i < 6; i++)
            //     {
            //         firsthalf.Add(SortedList[i]);
            //     }
            //
            //     for (var i = 6; i < 12; i++)
            //     {
            //         secondhalf.Add(SortedList[i]);
            //     }
            //
            //     elapsedTime3 = 0;
            // }
            //
            // if (elapsedTime1 == 20)
            // {
            //     if (x == 1)
            //     {
            //         medalDispay = secondhalf;
            //         x = 0;
            //     }
            //     else
            //     {
            //         medalDispay = firsthalf;
            //         x = 1;
            //     }   
            //     StateHasChanged();
            //     elapsedTime1 = 0;
            // }



            if (elapsedTime2 == 180)
            {
                y.InvokeAsync(0);
                // firsthalf = new();
                // secondhalf = new();
                // medalTallys = await httpClient.GetFromJsonAsync<List<List<MedalTallyModel>>>(
                //     "https://taskbucket.azurewebsites.net/api/GetAllMedals?code=GnHDbkhbseQ-yDNCPIk-ztKSEtjFJ8hN7BvISWP5S9AMAzFuBbZ8SQ==");
                //
                // var SortedList = medalTallys[0].OrderBy(o => o.rankint).ToList();
                //
                // var rank = 1;
                // var rnkstr = "";
                // label = "Elementary";
                //
                // foreach (var rnk in SortedList)
                // {
                //     if (rnk.rankint == 1)
                //     {
                //         rnkstr = "1st";
                //         rnk.rankstyle = "color: #ffcf40; font-size: 30px";
                //     }
                //     else if (rnk.rankint == 2)
                //     {
                //         rnk.rankstyle = "color: #ced4da;font-size: 30px";
                //         rnkstr = "2nd";
                //     }
                //     else if (rnk.rankint == 3)
                //     {
                //         rnk.rankstyle = "color: #cb8c47;font-size: 30px";
                //         rnkstr = "3rd";
                //     }
                //     else
                //     {
                //         rnk.rankstyle = "font-size: 30px";
                //         rnkstr = rank + "th";
                //     }
                //
                //     rnk.Province_id = "image/" + rnk.Province.Trim() + ".png";
                //     rnk.rank = rnkstr;
                // }
                //
                // for (var i = 0; i < 6; i++)
                // {
                //     firsthalf.Add(SortedList[i]);
                // }
                //
                // for (var i = 6; i < 12; i++)
                // {
                //     secondhalf.Add(SortedList[i]);
                // }
            }

           // medalDispay = firsthalf;
            
            // InvokeAsync(StateHasChanged); // Update UI
        }

        private async Task GetEventMedals(string province)
        {
            
            
            // foreach (var eventMedal in pronviceEventMedals)
            // {
            //     if (eventMedal.Gold == "" && eventMedal.Silver == "" && eventMedal.Bronze == "")
            //     {
            //         pronviceEventMedals.Remove(eventMedal);
            //     }
            // }
            
            var result = await DialogService.OpenAsync<EventMedals>(province + " (Overall Ranking)",
                new Dictionary<string, object>() { { "province", province}},
                new DialogOptions() { Width = "800px", Resizable = false, Draggable = true });
            
        }

        private async void SetDispplay()
        {
            while (await _changeDisplayTimer.WaitForNextTickAsync())
                if (x == 1)
                {
                    medalDispay = secondhalf;
                    x = 0;
                }
                else
                {
                    medalDispay = firsthalf;
                    x = 1;
                }
        }

        // private async void SetRequestData()
        // {
        //     while (await _updateDataTimer.WaitForNextTickAsync())
        //     {
        //         medalTallys = await httpClient.GetFromJsonAsync<List<MedalTallyModel>>(
        //             "https://taskbucket.azurewebsites.net/api/CRAGMedalTally?code=GnHDbkhbseQ-yDNCPIk-ztKSEtjFJ8hN7BvISWP5S9AMAzFuBbZ8SQ==");
        //
        //         for (var i = 0; i < 6; i++) firsthalf.Add(medalTallys[i]);
        //
        //         for (var i = 6; i < 12; i++) secondhalf.Add(medalTallys[i]);
        //     }
        // }


        public async Task OnChange(bool match)
        {
            IsSmall = match;
        }
}