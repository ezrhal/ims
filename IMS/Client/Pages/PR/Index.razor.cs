using Radzen;
using Radzen.Blazor;
using IMS.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace IMS.Client.Pages.PR
{
    [Authorize]
    public partial class Index
    {
        RadzenDataGrid<PRModel> grid;
        List<PRModel> PRs;
        List<PRModel> filteredPR;
         IList<PRModel> selectedItems;

        protected override async Task OnInitializedAsync()
        {
            PRs = await httpClient.GetFromJsonAsync<List<PRModel>>("purchaserequest/getprs");
            if (PRs != null)
            {
                filteredPR = PRs;
            }
        }

        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredPR = PRs.Where(q => q.projectname.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredPR = PRs;
            }
        }

        public async Task AddPR()
        {
            
            var result = await DialogService.OpenAsync<AddPR>("Add new PR",
                   new Dictionary<string, object>() { { "PRs", PRs }},
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });
        
        }

        public async Task RemovePR(string Id)
        {

        }

    }
}