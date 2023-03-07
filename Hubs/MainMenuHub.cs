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
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SignalRTest.Hubs
{
    public class MainMenuHub : Hub
    {
        public static Dictionary<Guid, GameModel> CreatedGames;
        //public static List<GameModel> PlayingGames;


        private void InitializeCreatedGames()
        {
            if (CreatedGames == null)
            {
                CreatedGames = new Dictionary<Guid, GameModel>();
            }

            //if (PlayingGames == null)
            //{
            //    PlayingGames = new List<GameModel>();
            //}
            
        }

        public async Task Authenticate(string accessToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
                // Validación exitosa, el token es válido y pertenece a un usuario de Google.


                // Crear un objeto ClaimsIdentity para representar al usuario autenticado
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, payload.Email),
                    new Claim(ClaimTypes.Email, payload.Email),
                    new Claim(ClaimTypes.NameIdentifier, payload.Subject),
                    // Aquí puedes agregar más reclamos personalizados según tus necesidades.
                });
                Context.User.AddIdentity(identity);

                InitializeCreatedGames();

                //var gameInitiated = PlayingGames.Where(g => g.Player1.PlayerMail == payload.Email || g.Player2.PlayerMail == payload.Email)?.FirstOrDefault();
                //if (gameInitiated != null)
                //{
                //    var game = gameInitiated;
                //    await Groups.AddToGroupAsync(Context.ConnectionId, game.GameId.ToString());

                //    var gameStateJson = JsonConvert.SerializeObject(game);

                //    if (game.Player2.PlayerId != null)
                //    {
                //        await Clients.Group(game.GameId.ToString()).SendAsync("StartGame", gameStateJson);
                //    }
                //    else
                //    {
                //        await Clients.Group(game.GameId.ToString()).SendAsync("GameCreatedByUser", gameStateJson);
                //    }
                //}
                

                var createdGamesJson = JsonConvert.SerializeObject(CreatedGames.Values.ToList());
                await Clients.All.SendAsync("UpdateOnlineGames", createdGamesJson);
            }
            catch (InvalidJwtException ex)
            {
                // El token no es válido, hacer algo al respecto.
            }
        }


        public async Task CreateNewGame(string playerInfoJson)
        {
            InitializeCreatedGames();
            var playerInfo = JsonConvert.DeserializeObject<PlayerModel>(playerInfoJson);
            playerInfo.PlayerHubId = Context.ConnectionId;

            var newRound = GameService.NewGame(playerInfo);

            await Groups.AddToGroupAsync(Context.ConnectionId, newRound.GameId.ToString());

            CreatedGames.Add(newRound.GameId, newRound);

            var createdGamesJson = JsonConvert.SerializeObject(CreatedGames.Values.ToList());
            await Clients.All.SendAsync("UpdateOnlineGames", createdGamesJson);


            await Clients.Client(playerInfo.PlayerHubId).SendAsync("GameCreatedByUser", JsonConvert.SerializeObject(newRound));


        }
      
        
        public async Task CancelCreateGame(Guid gameGuid)
        {
            CreatedGames.Remove(gameGuid);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameGuid.ToString());

            var createdGamesJson = JsonConvert.SerializeObject(CreatedGames.Values.ToList());
            await Clients.All.SendAsync("UpdateOnlineGames", createdGamesJson);
        }
        

        public async Task JoinGame(string player2InfoJson, string gameToStartJson)
        {
            var player2Info = JsonConvert.DeserializeObject<PlayerModel>(player2InfoJson);
            var gameToStart = JsonConvert.DeserializeObject<GameModel>(gameToStartJson);

            player2Info.PlayerHubId = Context.ConnectionId;

            gameToStart = GameService.JoinGame(gameToStart,player2Info);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameToStart.GameId.ToString());

            //PlayingGames.Add(gameToStart);
            CreatedGames.Remove(gameToStart.GameId);

            var createdGamesJson = JsonConvert.SerializeObject(CreatedGames.Values.ToList());
            await Clients.All.SendAsync("UpdateOnlineGames", createdGamesJson);


            var gameStateJson = JsonConvert.SerializeObject(gameToStart);
            //le envio el StartGame a los users correspondientes


            await Clients.Group(gameToStart.GameId.ToString()).SendAsync("StartGame", gameStateJson);
         //   await Clients.Clients(gameToStart.Player1.PlayerHubId, gameToStart.Player2.PlayerHubId).SendAsync("StartGame", gameStateJson);
        }

    }

}
