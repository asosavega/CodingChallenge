using Dog2BoneBackend.GameClasses;
using Dog2BoneBackend.Models;
using Dog2BoneBackend.Services.Interfaces;

namespace Dog2BoneBackend.Services.Implementations
{
    public class Dog2BoneService : IDog2Bone
    {
        private Dictionary<Guid, GameGrid> dog2boneInstances = new Dictionary<Guid, GameGrid>();

        public Guid StartNewGame(string gameSettingsFile)
        {
            Guid newGameID = Guid.NewGuid();
            dog2boneInstances.Add(newGameID, new GameGrid(gameSettingsFile));
            return newGameID;
        }

        public bool RestartGame(Guid gameID, string gameSettingsFile)
        {
            try
            {
                dog2boneInstances.Add(gameID, new GameGrid(gameSettingsFile));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool FinishGame(Guid gameID)
        {
            return dog2boneInstances.Remove(gameID);
        }

        public GameGridStatus GetCurrentGameStatus(Guid gameID)
        {
            return dog2boneInstances[gameID].GetGridCurrentStatus();
        }

        public GameGridStatus PerformNextStep(Guid gameID)
        {
            return dog2boneInstances[gameID].PerformNextDogAction();
        }

        public GameGridStatus PerformStepSeries(Guid gameID)
        {
            return dog2boneInstances[gameID].PerformAllDogActions();
        }

        public bool FeedDogActions(Guid gameID, string actionsFile)
        {
            return dog2boneInstances[gameID].FeedDogActions(actionsFile);
        }
    }
}
