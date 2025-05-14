namespace TracePlayer.Contracts.Player
{
    public class AddPlayersNamesRequest
    {
        public List<AddPlayerNameRequest> Players { get; set; } = [];
        public string Server { get; set; } = string.Empty;
    }
}
