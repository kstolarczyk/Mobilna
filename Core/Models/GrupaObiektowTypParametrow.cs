namespace Core.Models
{
    public class GrupaObiektowTypParametrow
    {
        public int GrupaObiektowId { get; set; }
        public GrupaObiektow GrupaObiektow { get; set; }
        public int  TypParametrowId { get; set; }
        public TypParametrow TypParametrow { get; set; }
    }
}