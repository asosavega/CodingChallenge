using Dog2BoneBackend.Models;
using Dog2BoneBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dog2BoneBackend.Controllers
{
    public class Dog2BoneController : Controller
    {
        private readonly ILogger _logger;
        private IDog2Bone _dog2BoneService;
        public Dog2BoneController(ILogger<Dog2BoneController> logger, IDog2Bone dog2BoneService)
        {
            _logger = logger;
            _dog2BoneService = dog2BoneService;
        }

        [HttpGet("StartNewGame")]
        public Guid StartNewGame(string gameSettingsFileLocation)
        {
            return _dog2BoneService.StartNewGame(gameSettingsFileLocation);
        }

        [HttpPost("RestartGame/{gameID}")]
        public IActionResult RestartGame(Guid gameID)
        {
            return (_dog2BoneService.RestartGame(gameID)) ? Ok() : BadRequest("Unable to start a new game");
        }

        [HttpPost("FinishGame/{gameID}")]
        public IActionResult FinishGame(Guid gameID)
        {
            return (_dog2BoneService.FinishGame(gameID)) ? Ok() : BadRequest("Unable to finish the game properly.");
        }

        [HttpGet("PerformNextDogAction/{gameID}")]
        public GameGridStatus PerformNextDogAction(Guid gameID)
        {
            return _dog2BoneService.PerformNextStep(gameID);
        }

        [HttpGet("PerformRemainingDogActions/{gameID}")]
        public GameGridStatus PerformRemainingDogActions(Guid gameID)
        {
            return _dog2BoneService.PerformStepSeries(gameID);
        }

        [HttpGet("GetCurrentGridStatus/{gameID}")]
        public GameGridStatus GetCurrentGridStatus(Guid gameID)
        {
            return _dog2BoneService.GetCurrentGameStatus(gameID);
        }

        [HttpPost("FeedDogActions/{gameID}")]
        public IActionResult FeedDogActions(Guid gameID, [FromBody] string actionsFilePath)
        {
            return (_dog2BoneService.FeedDogActions(gameID, actionsFilePath)) ? Ok() : BadRequest("Unable to feed dog actions to game");
        }

        
    }
}
