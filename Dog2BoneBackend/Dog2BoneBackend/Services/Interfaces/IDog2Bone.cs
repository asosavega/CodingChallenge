using Dog2BoneBackend.GameClasses;
using Dog2BoneBackend.Models;

namespace Dog2BoneBackend.Services.Interfaces
{
    public interface IDog2Bone
    {
        public Guid StartNewGame(string gameSettingsFile);
        public bool RestartGame(Guid gameID);
        public bool FinishGame(Guid gameID);
        public GameGridStatus GetCurrentGameStatus (Guid gameID);
        public GameGridStatus PerformNextStep (Guid gameID);
        public GameGridStatus PerformStepSeries (Guid gameID);
        public bool FeedDogActions (Guid gameID, string actionsFile);

    }
}
