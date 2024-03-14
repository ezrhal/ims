﻿namespace IMS.Shared.Models;

public class TotalProcuredModel
{
    public string projectid { get; set; }
    public string itemid { get; set; }
    public double totalprocured { get; set; }
    public double totalquantity { get; set; }

    public TotalProcuredModel()
    {
        projectid = "";
        itemid = "";
        totalprocured = 0;
        totalquantity = 0;
    }

}