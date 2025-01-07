using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace IMS.Client.Pages;

public partial class EventMedals
{
    public List<MedalTallyModel> pronviceEventMedals;
    [Parameter] public string province { get; set; }
    
    private string rowstyle = "background-color: rgba(145, 200, 228, 0.2)";
    private string rowstyle1 = "background-color: rgba(70, 130, 169, 0.1)";

    protected override async Task OnInitializedAsync()
    {
        List<MedalTallyModel> eventMedals = await httpClient.GetFromJsonAsync<List<MedalTallyModel>>("/maintenance/geteventmdedals");

        pronviceEventMedals = eventMedals.Where(q => q.Province.Equals(province)).ToList();
        pronviceEventMedals.RemoveAll(q => q.Gold == "" && q.Silver == "" && q.Bronze == "");
        pronviceEventMedals.RemoveAll(q => q.sport == "PENCAK SILATDemo Sports");
        pronviceEventMedals.RemoveAll(q => q.sport == "Special GamesSpecial / Para Games");

        foreach (var eventMedal in pronviceEventMedals)
        {
            if (eventMedal.Gold == "")
                eventMedal.Gold = "0";
            
            if (eventMedal.Silver == "")
                eventMedal.Silver = "0";
            
            if (eventMedal.Bronze == "")
                eventMedal.Bronze = "0";
        }
        
    }
}