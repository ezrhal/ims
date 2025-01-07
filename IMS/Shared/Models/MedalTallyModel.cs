using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class MedalTallyModel
{
    public string Province { get; set; }
    public string Gold { get; set; }
    public string Silver { get; set; }
    public string Bronze { get; set; }
    public double totalscore { get; set; }
    public string rank { get; set; }
    public string rankstyle { get; set; }
    public string Province_id { get; set; }
    public int totalmedal { get; set; }
    public int rankint { get; set; }
    public string sport { get; set; }
    public string asof { get; set; }

    public MedalTallyModel()
    {
        Province = "";
        Gold = "0";
        Silver = "0";
        Bronze = "0";
        totalscore = 0;
        rank = "";
        rankstyle = "";
        Province_id = "";
        totalmedal = 0;
        rankint = 1;
        sport = "";
        asof = "";
    }
}