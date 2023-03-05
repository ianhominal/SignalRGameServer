using System.Collections.Generic;

namespace SignalRTest.Models
{
    public class PlayerModel
    {

        public PlayerModel(string playerCards, string playerLifeCards, string playerId, string playerName, string playerPhotoURL)
        {
            this.PlayerId = playerId;
            this.PlayerLifeCardsObj = Card.GetCards(playerLifeCards);
            this.PlayerCardsObj = Card.GetCards(playerCards); ;

            this.PlayerName = playerName;
            this.PlayerPhoto = playerPhotoURL;

            PlayerPoints = 0;
            PlayerRoundsWins = 0;
        }

        public string PlayerId { get; set; }
        public List<Card> PlayerCardsObj { get; set; }
        public List<Card> PlayerLifeCardsObj { get; set; }
        public int PlayerPoints { get; set; }
        public int PlayerRoundsWins { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPhoto { get; set; }
    }
}
