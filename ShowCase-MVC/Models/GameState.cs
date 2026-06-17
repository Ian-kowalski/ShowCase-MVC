namespace ShowCase_MVC.Models
{
    public class GameState
    {
        public int Id { get; set; }
        public string? PlayerId { get; set; }
        public string? PlayerBoard { get; set; }
        public string? TrackingBoard { get; set; }
        public string? CurrentTurnPlayerId { get; set; }
    }
}
