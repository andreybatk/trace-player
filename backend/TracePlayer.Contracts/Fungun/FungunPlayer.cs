namespace TracePlayer.Contracts.Fungun
{
    public class FungunPlayer
    {
        public FungunPlayerResult? LastSuccess { get; set; }
        public FungunPlayerResult? LastWarning { get; set; }
        public FungunPlayerResult? LastDanger { get; set; }
    }
}
