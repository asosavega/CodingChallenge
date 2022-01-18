namespace Dog2BoneFrontEnd.Data
{
    public class GameGridStatus
    {
        public bool isGameOver { get; set; }
        public string currentStateMessage { get; set; } = "";
        public string remainingDogActions { get; set; } = "";
        public int[] gridSize { get; set; } = new int[2];
        public string dogFacing { get; set; } = "";
        public int[] dogLocation { get; set; } = new int[2];
        public int[] boneLocation { get; set; } = new int[2];
        public List<int[]> catLocations { get; set; } = new List<int[]>();

    }
}
