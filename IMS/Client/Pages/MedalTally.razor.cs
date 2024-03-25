using System;
namespace IMS.Client.Pages
{
	public partial class MedalTally
	{
        public bool IsSmall = false;

        public async Task OnChange(bool match)
        {
            IsSmall = match;
        }

    }
}

