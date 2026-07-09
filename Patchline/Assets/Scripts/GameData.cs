namespace Assets.Scripts
{
    public class GameData
    {
        public MemoryState Memory { get; set; }
        public int StdRow { get; set; }
        public int PlayerRow { get; set; }
        public bool IsEnded { get; set; }
        public GoalResult[] Goals { get; set; }
        public int StepCount { get; set; }
    }
}