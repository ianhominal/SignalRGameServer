using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRTest.Hubs
{
    public class GameHub : Hub
    {
        [AllowAnonymous] // Agregar esta línea
        public async Task EndTurn(string gameGuid)
        {
            // Aquí puedes actualizar la base de datos y luego notificar al otro jugador
            await Clients.Others.SendAsync("UpdateGameState", gameGuid);
        }
    }
}
