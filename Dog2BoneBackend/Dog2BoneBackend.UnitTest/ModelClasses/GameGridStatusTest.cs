using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dog2BoneBackend.UnitTest.ModelClasses
{
    public class GameGridStatusTest
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
