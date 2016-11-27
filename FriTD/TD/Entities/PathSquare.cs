namespace TD.Entities
{
    public class PathSquare
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PathSquare Next { get; set; }
        public PathSquare Previous { get; set; }
    }
}
