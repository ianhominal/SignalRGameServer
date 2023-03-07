using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using SignalRTest.Models;
using SignalRTest.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SignalRTest.Hubs
{
    public class GameHub : Hub
    {
        public async Task RegisterGameGroup(string gameStateJson)
        {
            var gameState = JsonConvert.DeserializeObject<GameModel>(gameStateJson);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameState.GameId.ToString());

            await Clients.Group(gameState.GameId.ToString()).SendAsync("UpdateGameState", gameStateJson);
        }

        public async Task UpdateGameState(string gameStateJson)
        {
            var gameState = JsonConvert.DeserializeObject<GameModel>(gameStateJson);

            gameState = GameService.UpdateGameState(gameState);

            gameStateJson = JsonConvert.SerializeObject(gameState);

            await Clients.Group(gameState.GameId.ToString()).SendAsync("UpdateGameState", gameStateJson);
        }

        public async Task PlayerDrawCardsFromDeck(string gameStateJson)
        {
            var gameState = JsonConvert.DeserializeObject<GameModel>(gameStateJson);

            if (!gameState.CheckIfPlayerCanPlay())
            {
                gameState = GameService.EndTurn(gameState);

                gameStateJson = JsonConvert.SerializeObject(gameState);


                await Clients.Caller.SendAsync("ForceEndTurn", true);

                await Clients.Group(gameState.GameId.ToString()).SendAsync("UpdateGameState", gameStateJson);
            }
        }

        public async Task EndTurn(string gameStateJson)
        {
            var gameState = JsonConvert.DeserializeObject<GameModel>(gameStateJson);
            gameState = GameService.EndTurn(gameState);

            gameStateJson = JsonConvert.SerializeObject(gameState);

            if (gameState.PlayerWon)
            {
                await Clients.Caller.SendAsync("ForceEndTurn", true);
            }

            await Clients.Group(gameState.GameId.ToString()).SendAsync("UpdateGameState", gameStateJson);
        }

        public async Task CloseGame(string gameStateJson)
        {
            var gameState = JsonConvert.DeserializeObject<GameModel>(gameStateJson);

            string playerName = gameState.Player1.PlayerHubId == Context.ConnectionId ? gameState.Player1.PlayerName : gameState.Player2.PlayerName;

            await Clients.Group(gameState.GameId.ToString()).SendAsync("CloseGame", playerName);
        }
    }
}
