namespace TracePlayer.Contracts.Fungun
{
    public class FungunApiResponse
    {
        public Dictionary<string, List<FungunPlayerResult>> Data { get; set; } = [];
        public bool Success { get; set; }
    }
}
