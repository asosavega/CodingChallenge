using Dog2BoneBackend.UnitTest.ModelClasses;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Dog2BoneBackend.UnitTest
{
    public class Do2gBoneUnitTest
    {
        [Fact]
        public void LoadGameSettingsAndShowGridStatus()
        {
            HttpClient restClient = new HttpClient();
            string testGameFile = @"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\GameSettings1.json";
            FileInfo gameFileInfo = new FileInfo(testGameFile);

            var response = restClient.GetAsync($"https://localhost:5001/StartNewGame?gameSettingsFileLocation={gameFileInfo.FullName}").GetAwaiter().GetResult();
            Assert.False(!response.IsSuccessStatusCode, "Rest Command to start Game didn't executed successfully");

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Guid gameID = JsonSerializer.Deserialize<Guid>(content);

            HttpResponseMessage responseMessage = restClient.GetAsync($"https://localhost:5001/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Game Status wasn't successful");

            string statusString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GameGridStatusTest? gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(statusString,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            Assert.False(gameStatus == null, "Rest Command to obtain Game Status didn't returned a valid result");
        }

        [Fact]
        public void LoadGameSettingsAndDogActions()
        {
            HttpClient restClient = new HttpClient();
            string testGameFile = @"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\GameSettings1.json";
            FileInfo gameFileInfo = new FileInfo(testGameFile);

            var response = restClient.GetAsync($"https://localhost:5001/StartNewGame?gameSettingsFileLocation={gameFileInfo.FullName}").GetAwaiter().GetResult();
            Assert.False(!response.IsSuccessStatusCode, "Rest Command to start Game didn't executed successfully");

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Guid gameID = JsonSerializer.Deserialize<Guid>(content);

            string dogActionFiles = @"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions1.txt";
            gameFileInfo = new FileInfo(dogActionFiles);
            string postJson = JsonSerializer.Serialize(gameFileInfo.FullName);

            bool success = restClient.PostAsync($"https://localhost:5001/FeedDogActions/{gameID}",
            new StringContent(postJson, System.Text.Encoding.UTF8, "application/json"))
            .GetAwaiter().GetResult().IsSuccessStatusCode;
            Assert.False(!success, "Rest Command to obtain send dog actions wasn't successful");

            HttpResponseMessage responseMessage = restClient.GetAsync($"https://localhost:5001/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Game Status wasn't successful");

            string statusString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GameGridStatusTest? gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(statusString,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            Assert.False(string.IsNullOrEmpty(gameStatus?.remainingDogActions), "Rest Command to obtain Game Status didn't returned a valid result");

        }

        [Fact]
        public void SuccessGameScenario()
        {
            HttpClient restClient = new HttpClient();
            string testGameFile = @"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\GameSettings1.json";
            FileInfo gameFileInfo = new FileInfo(testGameFile);

            var response = restClient.GetAsync($"https://localhost:5001/StartNewGame?gameSettingsFileLocation={gameFileInfo.FullName}").GetAwaiter().GetResult();
            Assert.False(!response.IsSuccessStatusCode, "Rest Command to start Game didn't executed successfully");

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Guid gameID = JsonSerializer.Deserialize<Guid>(content);


            FileInfo[] dogActionFileInfo = new FileInfo[]
            {
                new FileInfo(@"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions3.txt"),
                new FileInfo(@"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions4.txt")
            };

            foreach(FileInfo fileInfo in dogActionFileInfo)
            {
                string postJson = JsonSerializer.Serialize(fileInfo.FullName);

                bool success = restClient.PostAsync($"https://localhost:5001/FeedDogActions/{gameID}",
                new StringContent(postJson, System.Text.Encoding.UTF8, "application/json"))
                .GetAwaiter().GetResult().IsSuccessStatusCode;
                Assert.False(!success, "Rest Command to obtain send dog actions wasn't successful");
            }


            HttpResponseMessage responseMessage = restClient.GetAsync($"https://localhost:5001/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Game Status wasn't successful");

            string statusString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GameGridStatusTest? gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(statusString,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            Assert.False(string.IsNullOrEmpty(gameStatus?.remainingDogActions), "Rest Command to obtain Game Status didn't returned a valid result");

            responseMessage = restClient.GetAsync($"https://localhost:5001/PerformRemainingDogActions/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Perform All Dog Actions Failed");

            gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(responseMessage.Content
                .ReadAsStringAsync().GetAwaiter().GetResult());

            Assert.False((gameStatus != null && !gameStatus.isGameOver), "Game is not Over. Incomplete Series of Steps");
            Assert.False((gameStatus != null && gameStatus.currentStateMessage.ToLower() != "success"), "Game is Over but it wasn't a successfull game");
        }

        [Fact]
        public void FailedGameScenario()
        {
            HttpClient restClient = new HttpClient();
            string testGameFile = @"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\GameSettings1.json";
            FileInfo gameFileInfo = new FileInfo(testGameFile);

            var response = restClient.GetAsync($"https://localhost:5001/StartNewGame?gameSettingsFileLocation={gameFileInfo.FullName}").GetAwaiter().GetResult();
            Assert.False(!response.IsSuccessStatusCode, "Rest Command to start Game didn't executed successfully");

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Guid gameID = JsonSerializer.Deserialize<Guid>(content);


            FileInfo[] dogActionFileInfo = new FileInfo[]
            {
                new FileInfo(@"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions4.txt"),
                new FileInfo(@"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions3.txt")
            };

            foreach (FileInfo fileInfo in dogActionFileInfo)
            {
                string postJson = JsonSerializer.Serialize(fileInfo.FullName);

                bool success = restClient.PostAsync($"https://localhost:5001/FeedDogActions/{gameID}",
                new StringContent(postJson, System.Text.Encoding.UTF8, "application/json"))
                .GetAwaiter().GetResult().IsSuccessStatusCode;
                Assert.False(!success, "Rest Command to obtain send dog actions wasn't successful");
            }


            HttpResponseMessage responseMessage = restClient.GetAsync($"https://localhost:5001/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Game Status wasn't successful");

            string statusString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GameGridStatusTest? gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(statusString,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            Assert.False(string.IsNullOrEmpty(gameStatus?.remainingDogActions), "Rest Command to obtain Game Status didn't returned a valid result");

            responseMessage = restClient.GetAsync($"https://localhost:5001/PerformRemainingDogActions/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Perform All Dog Actions Failed");

            gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(responseMessage.Content
                .ReadAsStringAsync().GetAwaiter().GetResult());

            Assert.False((gameStatus != null && !gameStatus.isGameOver), "Game is not Over. Incomplete Series of Steps");
            Assert.False((gameStatus != null && gameStatus.currentStateMessage.ToLower() == "success"), "Game is Over but it wasn't a successfull game");
        }

        [Fact]
        public void GameInProgressScenario()
        {
            HttpClient restClient = new HttpClient();
            string testGameFile = @"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\GameSettings1.json";
            FileInfo gameFileInfo = new FileInfo(testGameFile);

            var response = restClient.GetAsync($"https://localhost:5001/StartNewGame?gameSettingsFileLocation={gameFileInfo.FullName}").GetAwaiter().GetResult();
            Assert.False(!response.IsSuccessStatusCode, "Rest Command to start Game didn't executed successfully");

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Guid gameID = JsonSerializer.Deserialize<Guid>(content);


            FileInfo[] dogActionFileInfo = new FileInfo[]
            {
                new FileInfo(@"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions3.txt"),
                new FileInfo(@"C:\Users\alexa\Desktop\CodingChallenge\GameFiles\dogActions6.txt")
            };

            foreach (FileInfo fileInfo in dogActionFileInfo)
            {
                string postJson = JsonSerializer.Serialize(fileInfo.FullName);

                bool success = restClient.PostAsync($"https://localhost:5001/FeedDogActions/{gameID}",
                new StringContent(postJson, System.Text.Encoding.UTF8, "application/json"))
                .GetAwaiter().GetResult().IsSuccessStatusCode;
                Assert.False(!success, "Rest Command to obtain send dog actions wasn't successful");
            }


            HttpResponseMessage responseMessage = restClient.GetAsync($"https://localhost:5001/GetCurrentGridStatus/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Game Status wasn't successful");

            string statusString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GameGridStatusTest? gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(statusString,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            Assert.False(string.IsNullOrEmpty(gameStatus?.remainingDogActions), "Rest Command to obtain Game Status didn't returned a valid result");

            responseMessage = restClient.GetAsync($"https://localhost:5001/PerformRemainingDogActions/{gameID}").GetAwaiter().GetResult();
            Assert.False(!responseMessage.IsSuccessStatusCode, "Rest Command to obtain Perform All Dog Actions Failed");

            gameStatus = JsonSerializer.Deserialize<GameGridStatusTest>(responseMessage.Content
                .ReadAsStringAsync().GetAwaiter().GetResult());

            Assert.False((gameStatus != null && gameStatus.isGameOver), "Game is not Over. Incomplete Series of Steps");
            Assert.False((gameStatus != null && gameStatus.currentStateMessage.ToLower() != "still looking"), "Game is Over but it wasn't a successfull game");
        }
    }
}