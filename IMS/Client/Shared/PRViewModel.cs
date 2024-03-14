namespace IMS.Client;

public class PRViewModel
{
    public bool openpr { get; set; } = false;
    public string prid { get; set; } = "";
    public bool projectview { get; set; } = false;
}

public class POViewModel
{
    public bool openpo { get; set; } = false;
    public string poid { get; set; } = "";
    public bool projectviewpo { get; set; } = false;
}

public class DVViewModel
{
    public bool opendv { get; set; } = false;
    public string dvid { get; set; } = "";
    public bool projectviewdv { get; set; } = false;
}
