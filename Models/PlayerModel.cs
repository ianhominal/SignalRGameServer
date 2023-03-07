using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SignalRTest.Models
{
    public class PlayerModel
    {

        public PlayerModel(string playerId = null, string playerName = null, string playerPhotoURL = null, string playerHubId = null, string playerMail = null)
        {
            this.PlayerId = playerId;
            this.PlayerHubId = playerHubId;

            this.PlayerName = playerName;
            this.PlayerPhoto = playerPhotoURL;
            this.PlayerMail = playerMail;
            PlayerPoints = 0;
            PlayerRoundsWins = 0;
        }

        public string PlayerId { get; set; }
        public string PlayerHubId { get; set; }
        public List<Card> PlayerCards { get; set; }
        public List<Card> PlayerLifeCards { get; set; }
        public int PlayerPoints { get; set; }
        public int PlayerRoundsWins { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPhoto { get; set; }

        public string PlayerMail { get; set; }

        public void CalculateGamePoints()
        {
            //si es el player que ganó la ronda
            if (PlayerCards.Count == 0)
            {
                PlayerPoints += 1;
                PlayerRoundsWins += 1;
            }

            PlayerPoints -= PlayerCards.Count();
            PlayerPoints += PlayerLifeCards.Count();

        }
    }
}
