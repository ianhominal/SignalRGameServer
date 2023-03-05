using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System;

namespace SignalRTest.Models
{
    public class Card
    {
        public static List<string> Suits = new List<string>() { "D", "P", "T", "C" };
        public static List<string> Ranks = new List<string>() { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        public static readonly int STARTHANDCARDSCOUNT = 7;
        public static readonly int STARTLIFECARDSCOUNT = 3;

        public string Suit { get; set; } //palo
        public int Number { get; set; }

        public static List<Card> GetCards(string cardsStr)
        {
            var cardList = new List<Card>();
            if (string.IsNullOrEmpty(cardsStr)) return cardList;

            var cards = cardsStr.Split(',');

            foreach (var card in cards)
            {
                var cardProps = card.Split('_');

                var suit = "";
                switch (cardProps[0].ToString())
                {
                    case "C":
                        suit = "Hearts";
                        break;
                    case "D":
                        suit = "Diamonds";
                        break;
                    case "T":
                        suit = "Clubs";
                        break;
                    case "P":
                        suit = "Spades";
                        break;
                }

                cardList.Add(new Card()
                {
                    Suit = suit,
                    Number = Ranks.IndexOf(cardProps[1].ToString()),
                });

            }

            return cardList;
        }

    }


}
