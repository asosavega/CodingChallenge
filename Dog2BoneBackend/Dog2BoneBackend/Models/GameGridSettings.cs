using System.Text.Json.Serialization;

namespace Dog2BoneBackend.Models
{
    public class GameGridSettings
    {
        public int[] BoardSize { get; set; } = new int[2];
        public int[] StartingPosition { get; set; } = new int[2];
        public int[] BonePosition { get; set; } = new int[2];
        public int[]? CatLocations { get; set; }
    }
}
