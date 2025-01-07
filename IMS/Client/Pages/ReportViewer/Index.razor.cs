//using Telerik.ReportServer.HttpClient;
//using Telerik.ReportServer.Services.Models;
//using System.Net.Http;
//using Microsoft.JSInterop;
//using Radzen.Blazor;

//namespace IMS.Client.Pages.ReportViewer
//{
//    public partial class Index
//    {

//        List<ReportInfo> reports = new();
//        RadzenDataGrid<ReportInfo> reportGrid;

//        protected override async Task OnInitializedAsync()
//        {
            
//        }

//        protected override async Task OnAfterRenderAsync(bool firstRender)
//        {
            
            
//        }

//        public async Task Getreport()
//        {
//            await JSRuntime.InvokeVoidAsync("GetToken", "reportserver", "pimo@123");

//            reports = await JSRuntime.InvokeAsync<List<ReportInfo>>("GetReports");

//            foreach(ReportInfo r in reports)
//            {
//                Console.WriteLine(r.Name);
//            }

//            await JSRuntime.InvokeVoidAsync("GetData", "users");
//            // var settings = new Settings()
//            // {
//            //     BaseAddress = "http://172.16.153.90:83/"
//            // };


//            // using (rsClient = new ReportServerClient(settings))
//            // {
//            //     try
//            //     {
                    
//            //         rsClient.Login("reportserver", "pimo@123");
                   
//            //     }
//            //     catch
//            //     {

//            //     }
                
//            // }
            
//            // IEnumerable<ReportInfo> reportInfos = rsClient.GetReportInfos();
//            // foreach(ReportInfo r in reportInfos)
//            // {
//            //     Console.WriteLine(r.Id + " " + r.Name);
//            // }
//        }

//        async Task LoadData()
//        {
//            StateHasChanged();
//        }

//        public async Task DeleteReport(string id)
//        {

//        }

//        public async Task OpenReport(string reportname)
//        {
//            await JSRuntime.InvokeVoidAsync("open", "http://172.16.153.90:83/Report/Share/Samples/" + reportname, "_blank");
//        }


//    }
//}