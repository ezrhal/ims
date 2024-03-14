namespace IMS.Client;

public class AppSettings
    {
        public static string GetBiddocDownloadUrl { get { return "https://pgas.ph/epslite/public/generatefile"; } }
        public static string GetPostWinnerQouteUrl { get { return "https://pgas.ph/epslite/public/execwinner"; } }
        public static string GetPostSaveQuoteUrl { get { return "https://pgas.ph/epslite/public/savesupplierquotation"; } }



        //public static string GetBiddocDownloadUrl { get { return "http://10.0.1.2/eps/public/generatefile"; } }
        //public static string GetPostWinnerQouteUrl { get { return "http://10.0.1.2/eps/public/execwinner"; }
        //public static string GetPostSaveQuoteUrl { get { return "http://10.0.0.243/eps/public/savesupplierquotation"; } }

        public static string GetMapInitLoc { get { return "./js/map_picker_1.3.js"; } }

        public static string Version { get { return "v12.6.3"; } }

    }

    public class Links
    {
        public class DGSign
        {
            public static string GetPDFUrl(int docID)
            {
                return $"https://pgas.ph/dgsign/blank/getPDF_digital_only?pdfData=application/pdf&FormID={docID}";
            }

            public static string GetSignUrl(string param)
            {
                return $"https://pgas.ph/dgsign/blank/jklsdhfikujdfhfgiuodfhf?data={param}";
            }
        }
    }