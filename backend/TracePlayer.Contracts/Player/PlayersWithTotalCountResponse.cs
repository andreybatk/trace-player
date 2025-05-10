namespace TracePlayer.Contracts.Player
{
    public class PlayersWithTotalCountResponse
    {
        public List<GetPlayerPaginationResponse> Players { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
