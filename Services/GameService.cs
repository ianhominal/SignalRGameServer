using SignalRTest.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace SignalRTest.Services
{
    public class GameService
    {


        public static GameModel NewGameRound(PlayerModel player)
        {
            //creo un string con el mazo entero
            List<string> deckStr = new List<string>();
            foreach (var cardSuit in Card.Suits)
            {
                foreach (var rank in Card.Ranks)
                {
                    deckStr.Add($"{cardSuit}_{rank}");
                }
            }

            deckStr = RandomizeList(deckStr);

            string playerCards = string.Join(",", deckStr.Take(Card.STARTHANDCARDSCOUNT).ToList());
            foreach (var card in playerCards.Split(","))
            {
                deckStr.Remove(card);
            }

            string player2Cards = string.Join(",", deckStr.Take(Card.STARTHANDCARDSCOUNT).ToList());
            foreach (var card in player2Cards.Split(","))
            {
                deckStr.Remove(card);
            }

            string playerLifeCards = string.Join(",", deckStr.Take(Card.STARTLIFECARDSCOUNT).ToList());
            foreach (var card in playerLifeCards.Split(","))
            {
                deckStr.Remove(card);
            }

            string player2LifeCards = string.Join(",", deckStr.Take(Card.STARTLIFECARDSCOUNT).ToList());
            foreach (var card in player2LifeCards.Split(","))
            {
                deckStr.Remove(card);
            }

            string pit = deckStr.Take(1).First();
            deckStr.Remove(pit);

            GameModel gameRound = new GameModel(pit)
            {
                Player1 = new PlayerModel(playerCards, playerLifeCards, player.PlayerId, player.PlayerName, player.PlayerPhoto),
                Player2 = new PlayerModel(player2Cards, player2LifeCards, null, null, null),
                AvailableCards = Card.GetCards(string.Join(",", deckStr.ToList())),
            };

            return gameRound;
        }

        private static List<string> RandomizeList(List<string> cardList)
        {
            Random _rand = new Random();
            for (int i = cardList.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = cardList[k];
                cardList[k] = cardList[i];
                cardList[i] = value;
            }
            return cardList;
        }
    }
}
