using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRTest.Models;
using SignalRTest.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRTest.Hubs
{
    public class GameHub : Hub
    {
        public List<GameModel> ActualGames;
        public GameHub() { ActualGames = new List<GameModel>(); }


        [AllowAnonymous] // Agregar esta línea
        public async Task EndTurn(string gameGuid)
        {
            // Aquí puedes actualizar la base de datos y luego notificar al otro jugador

            await Clients.Others.SendAsync("UpdateGameState", gameGuid);
        }


        [AllowAnonymous] // Agregar esta línea
        public async Task CreateNewGame(PlayerModel playerInfo)
        {
            var newRound = GameService.NewGameRound(playerInfo);
            ActualGames.Add(newRound);

            await Clients.Others.SendAsync("UpdateOnlineGames", playerInfo.PlayerId, ActualGames);
        }

    }
}
