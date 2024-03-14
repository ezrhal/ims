using System;
using IMS.Client.Shared;

namespace IMS.Client.Pages.Maintenance
{
	partial class Index
	{
        public MainLayout layout { get; set; }

        protected override void OnInitialized()
        {

            layout.txt = "IMS";
        }
    }
}

