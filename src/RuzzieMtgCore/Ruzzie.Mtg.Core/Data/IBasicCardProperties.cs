namespace Ruzzie.Mtg.Core.Data
{
    public interface IBasicCardProperties
    {
        string Name { get; set; }
        double Price { get; set; }      
        int Cmc { get; set; }
        int ColorIdentity { get; set; }
        int BasicType { get; set; }
        string Types { get; set; }
        double? Rating { get; set; }
        int Legality { get; set; }
    }
}