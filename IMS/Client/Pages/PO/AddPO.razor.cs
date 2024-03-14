using System.Net.Http.Json;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;


namespace IMS.Client.Pages.PO
{
    public partial class AddPO
    {
        [Parameter] public string projectid { get; set; }
        [Parameter] public List<POModel> POs { get; set; }
        List<PRModel> prs = new();
        List<SupplierModel> suppliers;
        string prid = "";

        POModel po = new();

        protected override async Task OnInitializedAsync()
        {
            prs = await httpClient.GetFromJsonAsync<List<PRModel>>("purchaserequest/getprs?projectid=" + projectid);
            suppliers = await httpClient.GetFromJsonAsync<List<SupplierModel>>("purchaseorder/getsuppliers");
            prs.RemoveAll(q => q.submitted.Equals(0));
        }

        public async Task SavePO(POModel args)
        {
            string isempty = prs.Find(q => q.Id.Equals(prid)).PO.Count > 0 ? "1" : "0";
            
            SupplierModel supplier = suppliers.Find(q => q.Id.Equals(args.supplierid));
            args.supplier = supplier.supplier;
            args.supplieraddress = supplier.address;

            List<string> paramList = new List<string>();

            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(args));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(prid));
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(isempty));

            
            var ret = await httpClient.PostAsJsonAsync("purchaseorder/savepo", paramList);
            string result = await ret.Content.ReadAsStringAsync();

            if (result != "")
            {
                args.Id = result;
                POs.Add(args);

                 NotificationService.Notify(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "PO has been save",
                        Duration = 3000
                    });

                //NavigationManager.NavigateTo("/purchaserequest/Details?id=" + result);
                
            }

        }

        void CloseDialog()
		{
            DialogService.Close();
        }
    }
}