using System.Text.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Dog2BoneFrontEnd.Data
{
    public enum PieceStyle
    {
        Dog,
        Cat,
        Bone,
        Blank
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public class GamePiece
    {
        public PieceStyle Style;
        public Direction faceDirection;
        

        public GamePiece()
        {
            Style = PieceStyle.Blank;
            faceDirection = Direction.Up;
        }
    }
    public class GameBoard
    {
        public GamePiece[,] Board { get; set; }
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }
        public bool IsGameOver { get; private set; }
        public string StateMessage { get; private set; } 

        private readonly Guid gameID = new Guid();
        private GameGridStatus? gameStatus;
        private HttpClient httpClient = new HttpClient();

        public GameBoard(Guid gid)
        {
            gameID = gid;
            Setup();
        }

        public void RestartGame()
        {
           Setup();
        }
        public void Setup()
        {
            HttpResponseMessage httpResponse;
            if (Board != null)
            {
                //Send REST API Call to Restart provided gid
                httpResponse = httpClient.PostAsync($"https://localhost:7056/RestartGame/{gameID}", null).GetAwaiter().GetResult();
                if (!httpResponse.IsSuccessStatusCode)
                    throw new Exception($"Error while attempting to restart game {gameID}"); 
            }

            httpResponse = httpClient.GetAsync($"https://localhost:7056/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
            if (!httpResponse.IsSuccessStatusCode)
                throw new Exception("Error");
            string statusString = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            gameStatus = JsonSerializer.Deserialize<GameGridStatus>(statusString,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
            if (gameStatus == null)
                throw new Exception("Error while obtaining game status from backend server");

            BoardWidth = gameStatus.gridSize[0];
            BoardHeight = gameStatus.gridSize[1];
            IsGameOver = gameStatus.isGameOver;
            StateMessage = gameStatus.currentStateMessage;

            Board = new GamePiece[BoardWidth, BoardHeight];

            for(int i = 0; i < BoardWidth; i++)
            {
                for(int j = 0; j < BoardHeight; j++)
                {
                    Board[i, j] = new GamePiece();
                }
            }

            try
            {
                Board[gameStatus.dogLocation[0], gameStatus.dogLocation[1]].Style = PieceStyle.Dog;
                Board[gameStatus.boneLocation[0], gameStatus.boneLocation[1]].Style = PieceStyle.Bone;
                if(gameStatus.catLocations?.Count > 0)
                {
                    foreach(int[] cl in gameStatus.catLocations)
                    { 
                        Board[cl[0], cl[1]].Style = PieceStyle.Cat;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public bool FeedDogActions(string actionsFile)
        {
            string postJson = JsonSerializer.Serialize(actionsFile);
            bool success = httpClient.PostAsync($"https://localhost:7056/FeedDogActions/{gameID}", 
                new StringContent(postJson, System.Text.Encoding.UTF8, "application/json"))
                .GetAwaiter().GetResult().IsSuccessStatusCode;

            if(success)
            {
                HttpResponseMessage httpResponse = httpClient.GetAsync($"https://localhost:7056/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
                if (!httpResponse.IsSuccessStatusCode)
                    throw new Exception("Error");
                string statusString = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                gameStatus = JsonSerializer.Deserialize<GameGridStatus>(statusString,
                    new JsonSerializerOptions()
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });
            }
            return success;
        }

        public bool PerformNextDogAction()
        {
            HttpResponseMessage actionResponse =  httpClient.GetAsync($"https://localhost:7056/PerformNextDogAction/{gameID}").GetAwaiter().GetResult();
            if(!actionResponse.IsSuccessStatusCode)
                return false;

            UpdateDogToken(JsonSerializer.Deserialize<GameGridStatus>(actionResponse.Content
                .ReadAsStringAsync().GetAwaiter().GetResult())
            );
            return true;
            
        }

        public bool PerformRemainingDogActions()
        {
            HttpResponseMessage actionResponse = httpClient.GetAsync($"https://localhost:7056/PerformRemainingDogActions/{gameID}").GetAwaiter().GetResult();
            if (!actionResponse.IsSuccessStatusCode)
                return false;

            UpdateDogToken(JsonSerializer.Deserialize<GameGridStatus>(actionResponse.Content
                .ReadAsStringAsync().GetAwaiter().GetResult())
            );
            return true;

        }

        public bool FinishGame()
        {
            return httpClient.PostAsync($"https://localhost:7056/FinishGame/{gameID}", null).GetAwaiter().GetResult().IsSuccessStatusCode;
        }

        public string GetActionsQueue()
        {
            return (gameStatus != null) ? gameStatus.remainingDogActions : "";
        }

        public string GetStatusMessage()
        {
            return (gameStatus != null) ? gameStatus.currentStateMessage : "";
        }

        private void UpdateDogToken(GameGridStatus? gridStatus)
        {
            if (gridStatus == null)
                throw new Exception("Error with the obtained game status. Unable to process");

            Board[gameStatus.dogLocation[0], gameStatus.dogLocation[1]].Style = PieceStyle.Blank;
            Board[gameStatus.dogLocation[0], gameStatus.dogLocation[1]].faceDirection = Direction.Up;

            gameStatus = gridStatus;
            Board[gameStatus.dogLocation[0], gameStatus.dogLocation[1]].Style = PieceStyle.Dog;
            Board[gameStatus.dogLocation[0], gameStatus.dogLocation[1]].faceDirection = Enum.Parse<Direction>(gameStatus.dogFacing);
            IsGameOver = gameStatus.isGameOver;
            StateMessage = gameStatus.currentStateMessage;
        }
    }
}
