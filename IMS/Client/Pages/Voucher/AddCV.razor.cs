using System.Net.Http.Json;
using System.Text.Json;
using IMS.Client.Pages.Maintenance;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.Voucher;

public partial class AddCV
{
    [Parameter] public List<POModel> CVs { get; set; }
    [Parameter] public string projectid { get; set; }
    [Parameter] public RadzenDataGrid<POModel> cvGrid { get; set; }

    POModel cv = new();
    List<POModel> pos;

    string poid = "";

    protected override async Task OnInitializedAsync()
    {
        pos = await httpClient.GetFromJsonAsync<List<POModel>>("voucher/getposdetails?projectid=" + projectid);
    }

    public async Task SaveCV(POModel args)
    {
        string data = Newtonsoft.Json.JsonConvert.SerializeObject(cv);
       
        var response = await httpClient.PostAsJsonAsync("voucher/savecv", data);
        string result = await response.Content.ReadAsStringAsync();

        Console.WriteLine(result);

        if (result.Length > 0)
        {
           NotificationService.Notify(
           new NotificationMessage
           {
               Severity = NotificationSeverity.Success,
               Summary = "Success",
               Detail = "CV has been save",
               Duration = 3000
           });

            cv.cvno = result;
            CVs.Add(cv);
            cvGrid.Reload();
            DialogService.Close();
        }

       
    }

    void CloseDialog()
    {
        DialogService.Close();
    }

    public async Task SelectPO()
    {
        string checkno = cv.checkno;
        cv = pos.First(q => q.Id.Equals(poid));

        if (cv.supplier != "")
            cv.payee = cv.supplier;

        if (cv.supplieraddress != "")
            cv.payeeaddress = cv.supplieraddress;

        cv.checkno = checkno;
       
    }
}
