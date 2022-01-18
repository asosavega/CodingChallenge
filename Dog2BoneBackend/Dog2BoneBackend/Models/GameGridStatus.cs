namespace Dog2BoneBackend.Models
{
    public class GameGridStatus
    {
        public bool IsGameOver { get; set; }
        public string CurrentStateMessage { get; set; } = "";
        public string RemainingDogActions { get; set; } = "";
        public int[] GridSize { get; set; } = new int[2];
        public string DogFacing { get; set; } = "";
        public int[] DogLocation { get; set; } = new int[2];
        public int[] BoneLocation { get; set; } = new int[2];
        public List<int[]> CatLocations { get; set; } = new List<int[]>();

    }
}
