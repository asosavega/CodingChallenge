using Dog2BoneBackend.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dog2BoneBackend.GameClasses
{
    public class GameGrid
    {
        private GridToken[,] grid;
        private int gridWidth;
        private int gridHeight;
        private DogToken dog;
        private GameGridStatus gameGridStatus;
        private GameGridSettings? settings;

        public Queue<char> StepSequence { get; private set; } = new Queue<char>();
        

        public GameGrid(string gameSettingsFile)
        {
            using (StreamReader rdr = new StreamReader(gameSettingsFile))
            {
                using (FileStream openStream = File.OpenRead(gameSettingsFile))
                {
                    settings =
                        JsonSerializer.Deserialize<GameGridSettings>(openStream, new JsonSerializerOptions()
                        {
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                        });
                    SetStartupGrid();
                }
            }
        }

        public void RestartGame()
        {
            SetStartupGrid();
        }

        private void SetStartupGrid()
        {
            if (settings == null)
                throw new Exception();


            gridWidth = settings.BoardSize[0];
            gridHeight = settings.BoardSize[1];
            grid = new GridToken[gridWidth, gridHeight]; //Create Grid
            int dogXStartingPosition = settings.StartingPosition[0];
            int dogYStatingPostition = settings.StartingPosition[1];
            grid[dogXStartingPosition, dogYStatingPostition] = this.dog = new DogToken(dogXStartingPosition, dogYStatingPostition); //Setup Dog Starting Position

            int boneXLoc = settings.BonePosition[0];
            int boneYLoc = settings.BonePosition[1];
            grid[boneXLoc, boneYLoc] = new BoneToken(boneXLoc, boneYLoc); //Setup Bone Position

            //Initialize Grid Status
            gameGridStatus = new GameGridStatus()
            {
                IsGameOver = false,
                CurrentStateMessage = "Still Looking",
                GridSize = new[] { gridWidth, gridHeight },
                DogFacing = dog.dogDirection.ToString(),
                DogLocation = new int[] { dogXStartingPosition, dogYStatingPostition },
                BoneLocation = new int[] { boneXLoc, boneYLoc },
                RemainingDogActions = ""
            };

            //Set Cat positions if provided
            if (settings.CatLocations != null)
            {
                foreach (int[] catLocPair in settings.CatLocations)
                {
                    CatToken cat = new CatToken(catLocPair[0], catLocPair[1]);
                    grid[catLocPair[0], catLocPair[1]] = cat;
                    gameGridStatus.CatLocations.Add(new int[] { cat.tXLoc, cat.tYLoc });
                }
            }

            //Ensure queue is Empty in case of restart
            StepSequence.Clear();
        }

        public GameGridStatus PerformNextDogAction()
        {
            if (gameGridStatus.IsGameOver || StepSequence.Count == 0)
                return gameGridStatus;

            dog.PerformDogAction(StepSequence.Dequeue());

            //assume game will be over. Else clause will set it back to false if otherwise
            gameGridStatus.IsGameOver = true;

            //Analize dog new position
            if (dog.newtXLoc < 0 || dog.newtYLoc < 0 
                || dog.newtXLoc >= gridWidth || dog.newtYLoc >= gridHeight) //Out of bounds Scenario
            {
                gameGridStatus.CurrentStateMessage = "Out of bounds";
                gameGridStatus.RemainingDogActions = (StepSequence.Count > 0) ? new string(StepSequence.ToArray()) : "";
                return gameGridStatus;

            }
            else if(grid[dog.newtXLoc, dog.newtYLoc]?.GetType() == typeof(CatToken)) //Has awaken a cat
            {
                gameGridStatus.CurrentStateMessage = "Wake up cat";
            }
            else if(grid[dog.newtXLoc, dog.newtYLoc]?.GetType() == typeof(BoneToken)) //Winner!
            {
                gameGridStatus.CurrentStateMessage = "Success";
                StepSequence.Clear();
            }
            else //Keep Trying
            {
                gameGridStatus.IsGameOver = false;
                gameGridStatus.CurrentStateMessage = "Still Looking";
            }

            //Move Dog to new Location on the Grid
            grid[dog.tXLoc, dog.tYLoc] = null;
            grid[dog.newtXLoc, dog.newtYLoc] = dog;
            


            //Update Grid Status Dog Coordinates
            gameGridStatus.DogLocation[0] = dog.newtXLoc; 
            gameGridStatus.DogLocation[1] = dog.newtYLoc;
            gameGridStatus.DogFacing = dog.dogDirection.ToString();
            gameGridStatus.RemainingDogActions = (StepSequence.Count > 0) ? new string(StepSequence.ToArray()) : "";

            return gameGridStatus;
        }

        public GameGridStatus PerformAllDogActions()
        {
            while(StepSequence.Count > 0)
            {
                if (PerformNextDogAction().IsGameOver)
                    break;
            }

            return gameGridStatus;
        }

        public GameGridStatus GetGridCurrentStatus()
        {
            return gameGridStatus;
        }

        public bool FeedDogActions(string actionsFile)
        {
            string dogActions = File.ReadAllText(actionsFile);
            if (string.IsNullOrEmpty(dogActions))
                return false;

            foreach (char element in dogActions.ToCharArray())
            {
                StepSequence.Enqueue(element);
            }
            gameGridStatus.RemainingDogActions += dogActions;

            return true;
        }
    }
}
