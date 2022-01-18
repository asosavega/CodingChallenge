namespace Dog2BoneFrontEnd.Data
{
    public class GameGridSettings
    {
        public int[] BoardSize { get; set; } = new int[2];
        public int[] StartingPosition { get; set; } = new int[2];
        public int[] BonePosition { get; set; } = new int[2];
        public List<int[]> CatLocations { get; set; }
    }
}
