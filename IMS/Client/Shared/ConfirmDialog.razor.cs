using System;
using Microsoft.AspNetCore.Components;

namespace IMS.Client.Shared
{
	partial class ConfirmDialog
	{
		[Parameter] public string message { get; set; }

		public async Task ConfirmSubmit()
		{
			dialog.Close(true);
		}

	}
}

