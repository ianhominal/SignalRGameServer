using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Linq;
using static SignalRTest.Services.GameService;
using System.IO.Pipes;
using System.Numerics;
using Newtonsoft.Json;

namespace SignalRTest.Models
{
    public class GameModel
    {
        public GameModel()
        {
            GameId = Guid.NewGuid();

            CardPits = new Dictionary<int, List<Card>>();
            //var pitsSplit = cardPits.Split('|');
            //var pitCount = 0;

            //foreach (var pit in pitsSplit)
            //{
            //    CardPits[pitCount] = Card.GetCards(pit);
            //    pitCount++;
            //}
        }


        public Guid GameId { get; set; }
        public PlayerModel Player1 { get; set; }
        public PlayerModel Player2 { get; set; }

        public string PlayerTurnId { get; set; }

        public string InitialPlayerTurnId { get; set; }

        public List<Card> AvailableCards { get; set; }
        public Dictionary<int, List<Card>> CardPits { get; set; }

        public bool PlayerWon { get; set; }
        public string PlayerClosedGame { get; set; }

        public bool GameRoundEnd { get; set; }

        public void NewRound(bool newGame)
        {
            if(!newGame)
            {
                Player1.CalculateGamePoints();
                Player2.CalculateGamePoints();
            }

            var gameCards = GetNewGameCards();

            AvailableCards = gameCards.deckCards;
            CardPits = new Dictionary<int, List<Card>> { { 0, gameCards.availablePits } };

            Player1.PlayerCards = gameCards.playerCards;
            Player1.PlayerLifeCards = gameCards.playerLifeCards;
            Player2.PlayerCards = gameCards.player2Cards;
            Player2.PlayerLifeCards = gameCards.player2LifeCards;

            GameRoundEnd = false;

            SetNextPlayerStartTurn();
        }

        public bool CheckIfDeckHasCards()
        {
            //si son menos de 2 mezclo las que queden disponibles
            if(AvailableCards.Count <= 2)
            {
                List<Card> newAvailableCards = new List<Card>();
                foreach(var pit in CardPits.Values)
                {
                    newAvailableCards.AddRange(pit.GetRange(0, pit.Count - 1));
                    pit.RemoveRange(0, pit.Count - 1);
                }

                AvailableCards = RandomizeList(newAvailableCards);
            }

            //si despues de agregarle las disponibles siguen siendo menos de 2 terminaria el gameround
            if(AvailableCards.Count <=2 )
            {
                GameRoundEnd = true;
                return false;
            }

            return true;
        }


        public bool CheckIfPlayerCanPlay()
        {
            var actualPlayer = Player1.PlayerId == PlayerTurnId ? Player1 : Player2;

            if (actualPlayer.PlayerLifeCards.Count > 0) return true;

            foreach (var card in actualPlayer.PlayerCards)
            {
                if(CardCanBePlayed(card.Number))
                {
                    PlayerWon = true;
                    return true;
                }
            }
            
            //si no retorno true todavia, significa que no puede jugar
            if (Player1.PlayerId == PlayerTurnId)
            {
                Player2.PlayerCards.Clear();
            }
            else
            {
                Player1.PlayerCards.Clear();
            }

            GameRoundEnd = true;
            return false;

        }


        public bool CheckIfPlayerWon()
        {
            var actualPlayer = Player1.PlayerId == PlayerTurnId ? Player1 : Player2;

            var playerWon = actualPlayer.PlayerCards.Count() == 0;
            GameRoundEnd = playerWon;
            return playerWon;

        }


        private bool CardCanBePlayed(int cardNumber) //todo si hay mas de uno deberías poder elegir en cual jugarlo
        {
            int nextCard = cardNumber + 1;
            int previousCard = cardNumber - 1;

            if (nextCard > 12) { nextCard = 0; }
            if (previousCard < 0) { previousCard = 12; }

            var pitWhereCardCanBePlayed = CardPits.Where(p => p.Value.Last().Number == nextCard || p.Value.Last().Number == previousCard).ToList();
            if (pitWhereCardCanBePlayed.Count > 0)
            {
                return true;
            }

            return false;
        }

        public void SetPlayer2(PlayerModel player2)
        {   
            Player2.PlayerId = player2.PlayerId;
            Player2.PlayerName = player2.PlayerName;
            Player2.PlayerPhoto = player2.PlayerPhoto;
            Player2.PlayerHubId = player2.PlayerHubId;
            Player2.PlayerMail = player2.PlayerMail;
        }

        public void SetRandomPlayerTurn()
        {
            var playerTurnId = new Random().Next(1, 2) == 1 ? Player1.PlayerId : Player2.PlayerId;

            PlayerTurnId = playerTurnId;
        }

        public void SetNextPlayerStartTurn()
        {
            var newTurn = (InitialPlayerTurnId == Player1.PlayerId) ? Player2.PlayerId : Player1.PlayerId;

            PlayerTurnId = newTurn;
            InitialPlayerTurnId = newTurn;
        }

        public void SetNextPlayerTurn()
        {
            PlayerTurnId = (PlayerTurnId == Player1.PlayerId) ? Player2.PlayerId : Player1.PlayerId;
        }

        private static GameCards GetNewGameCards()
        {
            GameCards gameCards = new GameCards();
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

            var playerCards = string.Join(",", deckStr.Take(Card.STARTHANDCARDSCOUNT).ToList());
            foreach (var card in playerCards.Split(","))
            {
                deckStr.Remove(card);
            }


            var player2Cards = string.Join(",", deckStr.Take(Card.STARTHANDCARDSCOUNT).ToList());
            foreach (var card in player2Cards.Split(","))
            {
                deckStr.Remove(card);
            }

            var playerLifeCards = string.Join(",", deckStr.Take(Card.STARTLIFECARDSCOUNT).ToList());
            foreach (var card in playerLifeCards.Split(","))
            {
                deckStr.Remove(card);
            }

            var player2LifeCards = string.Join(",", deckStr.Take(Card.STARTLIFECARDSCOUNT).ToList());
            foreach (var card in player2LifeCards.Split(","))
            {
                deckStr.Remove(card);
            }

            var availablePits = deckStr.Take(1).First();
            deckStr.Remove(availablePits);



            gameCards.playerCards = Card.GetCards(playerCards);
            gameCards.player2Cards = Card.GetCards(player2Cards);
            gameCards.playerLifeCards = Card.GetCards(playerLifeCards);
            gameCards.player2LifeCards = Card.GetCards(player2LifeCards);
            gameCards.availablePits = Card.GetCards(availablePits);
            gameCards.deckCards = Card.GetCards(string.Join(",", deckStr.ToList()));


            return gameCards;
        }

        private static List<T> RandomizeList<T>(List<T> cardList)
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
