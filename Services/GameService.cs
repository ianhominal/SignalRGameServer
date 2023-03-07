using SignalRTest.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace SignalRTest.Services
{
    public class GameService
    {
        internal class GameCards
        {
            public List<Card> playerCards;
            public List<Card> player2Cards;
            public List<Card> playerLifeCards;
            public List<Card> player2LifeCards;
            public List<Card> availablePits;
            public List<Card> deckCards;
        }

        public static GameModel NewGame(PlayerModel player)
        {
            GameModel gameRound = new GameModel()
            {
                Player1 = new PlayerModel(player.PlayerId, player.PlayerName, player.PlayerPhoto, player.PlayerHubId, player.PlayerMail),
                Player2 = new PlayerModel(),
            };

            gameRound.NewRound(true);

            return gameRound;
        }

        public static GameModel JoinGame(GameModel gameToJoin, PlayerModel player2)
        {
            gameToJoin.SetPlayer2(player2);

            gameToJoin.SetRandomPlayerTurn();

            gameToJoin.InitialPlayerTurnId = gameToJoin.PlayerTurnId;

            return gameToJoin;
        }

        public static GameModel UpdateGameState(GameModel gameState)
        {
            gameState.CheckIfPlayerWon();


            if (gameState.GameRoundEnd)
            {
                gameState.NewRound(false);
            }
            return gameState;
        }

      

        public static GameModel EndTurn(GameModel gameState)
        {
            gameState.SetNextPlayerTurn();
            //controla que el mazo tenga cartas y sino mezcla los pozos (si es posible, sino pone GameRoundEnd = true)
            if (!gameState.CheckIfDeckHasCards())
            {
                //ve si el player puede jugar (y no quedan mas cartas del mazo), sino gana el otro
                gameState.CheckIfPlayerCanPlay();
            }

            if (gameState.GameRoundEnd)
            {
                gameState.NewRound(false);
            }

            return gameState;
        }









    }
}
