using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RaSetMaker.Services
{
    public class RaClient
    {
        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<bool> TestLogin(string user)
        {
            var response = await _client.GetAsync(GetUserProfileUri(user));
            return response.IsSuccessStatusCode;
        }

        public async Task<RaUserProfile> GetUserProfile(string user)
        {
            var response = await _client.GetAsync(GetUserProfileUri(user));

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var userProfile = await JsonSerializer.DeserializeAsync<RaUserProfile>(stream) ?? new();
                userProfile.UserPic = $"{Constants.RA_BASE_URL}{userProfile.UserPic}";
                return userProfile;
            }

            throw new Exception($"Failed to fetch profile for {user}, server responded {response.StatusCode}");
        }

        public async Task<List<Game>> FetchGames(GameSystem gameSystem)
        {
            if (gameSystem.RaId == 0)
            {
                throw new Exception($"Retroachievement ID for System {gameSystem.Name} is not valid");
            }

            var response = await _client.GetAsync(GetGamesUri(gameSystem.RaId));

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var raGames = await JsonSerializer.DeserializeAsync<List<RaGame>>(stream) ?? [];
                return raGames.Select(g => g.ToGame()).ToList();
            }

            throw new Exception($"Failed to fetch games for {gameSystem.Name}, server responded {response.StatusCode}");
        }

        public async Task<List<Rom>> FetchGameRoms(Game game)
        {
            if (game.RaId == 0)
            {
                throw new Exception($"Retroachievement ID for Game {game.Name} is not valid");
            }

            var response = await _client.GetAsync(GetRomsUri(game.RaId));

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var raRomsResponse = await JsonSerializer.DeserializeAsync<RaRomResponse>(stream);

                return raRomsResponse?.Results.Select(r => new Rom
                {
                    Hash = r.MD5
                }).ToList() ?? [];
            }
            else
            {
                throw new Exception($"Rom fetching error for {game.Name}, served responded {response.StatusCode}");
            }
        }


        private string _apiKey = string.Empty;

        private readonly HttpClient _client = new();


        private string GetUserProfileUri(string user) => $"{Constants.RA_BASE_URL}/API/API_GetUserProfile.php?u={user}&y={_apiKey}";
        private string GetSystemsUri() => $"{Constants.RA_BASE_URL}/API/API_GetConsoleIDs.php?a=1&g=1&y={_apiKey}";

        private string GetGamesUri(int systemId) => $"{Constants.RA_BASE_URL}/API/API_GetGameList.php?i={systemId}&f=1&h=1&y={_apiKey}";
        private string GetRomsUri(int gameId) => $"{Constants.RA_BASE_URL}/API/API_GetGameHashes.php?i={gameId}&y={_apiKey}";
    }

    internal class RaGameSystem
    {
        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool IsGameSystem { get; set; }
    }

    internal class RaGame
    {
        public int ID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImageIcon { get; set; } = string.Empty;

        public int NumAchivements { get; set; }
        public int NumLeaderboards { get; set; }
        public int Points { get; set; }

        public List<string> Hashes { get; set; } = [];

        public Game ToGame()
        {
            var (gameTypes, gameName) = GameTypeAndNameFromString(Title);

            // detect subset
            if (gameTypes.Count == 0 && gameName.Contains("[Subset"))
            {
                gameTypes.Add(GameType.Subset);
            }

            return new Game
            {
                RaId = ID,
                IconUrl = $"{Constants.RA_BASE_URL}{ImageIcon}",
                GameTypes = gameTypes,
                Name = gameName,
                Roms = [.. Hashes.Select(h => new Rom() { Hash = h.ToLower() })]
            };
        }

        private static (List<GameType>, string) GameTypeAndNameFromString(string str)
        {
            var trimmedStr = str;
            List<GameType> types = [];
            while (trimmedStr.StartsWith("~"))
            {
                if (trimmedStr.StartsWith("~Homebrew~"))
                {
                    types.Add(GameType.Homebrew);
                    trimmedStr = trimmedStr.Remove(0, 10).Trim();
                }
                else if (trimmedStr.StartsWith("~Hack~"))
                {
                    types.Add(GameType.Hack);
                    trimmedStr = trimmedStr.Remove(0, 6).Trim();
                }
                else if (trimmedStr.StartsWith("~Unlicensed~"))
                {
                    types.Add(GameType.Unlicensed);
                    trimmedStr = trimmedStr.Remove(0, 12).Trim();
                }
                else if (trimmedStr.StartsWith("~Prototype~"))
                {
                    types.Add(GameType.Prototype);
                    trimmedStr = trimmedStr.Remove(0, 11).Trim();
                }
                else if (trimmedStr.StartsWith("~Demo~"))
                {
                    types.Add(GameType.Demo);
                    trimmedStr = trimmedStr.Remove(0, 6).Trim();
                }
                else if (trimmedStr.StartsWith("~Test Kit~"))
                {
                    types.Add(GameType.TestKit);
                    trimmedStr = trimmedStr.Remove(0, 10).Trim();
                }
                else
                {
                    return (types, trimmedStr);
                }
            }

            return (types, trimmedStr);

        }
    }

    public class RaUserProfile
    {
        public string User { get; set; } = string.Empty;
        public string ULID { get; set; } = string.Empty;
        public string UserPic { get; set; } = string.Empty;
        public string MemberSince { get; set; } = string.Empty;
        public string RichPresenceMsg { get; set; } = string.Empty;
        public int LastGameID { get; set; }
        public int ContribCount { get; set; }
        public int ContribYield { get; set; }
        public int TotalPoints { get; set; }
        public int TotalSoftcorePoints { get; set; }
        public int TotalTruePoints { get; set; }
        public int Permissions { get; set; }
        public int Untracked { get; set; }
        public int ID { get; set; }
        public bool UserWallActive { get; set; }
        public string Motto { get; set; } = string.Empty;
    }

    internal class RaRomResponse
    {
        public List<RaRom> Results { get; set; } = [];
    }

    internal class RaRom
    {
        public string Name { get; set; } = string.Empty;
        public string MD5 { get; set; } = string.Empty;

        public List<string> Labels { get; set; } = [];

        public string? PatchUrl { get; set; }

    }
}
