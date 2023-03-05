using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace SignalRTest.Models
{
    public class GameModel
    {
        public GameModel(string cardPits)
        {
            GameRoundId = Guid.NewGuid();


            var pitsSplit = cardPits.Split('|');
            var pitCount = 0;

            CardPits = new Dictionary<int, List<Card>>();
            foreach (var pit in pitsSplit)
            {
                CardPits[pitCount] = Card.GetCards(pit);
                pitCount++;
            }
        }


        public Guid GameRoundId { get; set; }
        public PlayerModel Player1 { get; set; }
        public PlayerModel Player2 { get; set; }

        public string PlayerTurnId { get; set; }

        public List<Card> AvailableCards { get; set; }
        public Dictionary<int, List<Card>> CardPits { get; set; }


    }
}
