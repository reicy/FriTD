namespace TD.Core
{
    public interface IDisplayableObject
    {
        int X { get; set; }
        int Y { get; set; }
        int Id { get; set; }
        int SeqId { get; set; }
        double Perc { get; set; }
        
        int WX { get; set; }
        int WY { get; set; }
    }
}