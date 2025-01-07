namespace IMS.Client.Pages;

public partial class Index
{
    private Timer timer1;
    private int elapsedTime1;
    private int x = 0;
    
    protected override async Task OnInitializedAsync()
    {
        //timer1 = new Timer(TimerCallback1, null, 0, 1000);
    }

    private async void TimerCallback1(object state)
    {

        if (x != 0)
        {
            elapsedTime1++;

            if (elapsedTime1 == 60)
            {
                x = 0;
                elapsedTime1 = 0;
                StateHasChanged();
            }
            
        }
        
        // if (elapsedTime1 == 60)
        // {
        //     x = 1;
        //     StateHasChanged();
        // }
        // else if (elapsedTime1 == 80)
        // {
        //     x = 0;
        //     elapsedTime1 = 0;
        //     StateHasChanged();
        // }
        
    }

    void UpdateXfromVids(int y)
    {
        x = y;
    }

    void UpdateXfromImg(int y)
    {
        x = y;
    }
    
    void UpdateXfromMedal(int y)
    {
        x = y;
    }
}