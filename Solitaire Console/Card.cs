using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire_Console
{
    class Card
    {
        // The value of the card (F.eks. 5)
        public string Value;
        // The suit of the card (F.eks. Diamonds)
        public string Suit;
        // Is the card covered or not
        public bool Uncovered;

        public Card(string value, string suit)
        {
            Value = value;
            Suit = suit;
            Uncovered = false;
        }

        public void Uncover()
        {
            Uncovered = true;
        }

        public void Print()
        {
            // Saving the consoles foreground color
            ConsoleColor o = Console.ForegroundColor;
            ConsoleColor color = Console.ForegroundColor;

            // Giving the card a color which depends on the suit of the card
            switch (Suit)
            {
                // Clubs is Blue
                case "C":
                    color = ConsoleColor.Blue;
                    break;
                // Diamonds is Magenta
                case "D":
                    color = ConsoleColor.Magenta;
                    break;
                // Hearths is Red
                case "H":
                    color = ConsoleColor.Red;
                    break;
                // Spades is Cyan
                case "S":
                    color = ConsoleColor.Cyan;
                    break;
            }

            // If the card is uncovered take the color from the suit else make it white
            Console.ForegroundColor = Uncovered ? color : ConsoleColor.White;
            // If the card is uncovered display the correct value else display "#"
            Console.Write(Uncovered ? Value : "#");
            // Set the console foreground color back to its original
            Console.ForegroundColor = o;
        }

        // Making sure that only the correct colors can stack depedning on the suit of the card
        public bool CanColorStack(Card onto) => onto switch
        {
            _ when
                (onto.Suit == "H" || onto.Suit == "D") && (Suit == "H" || Suit == "D") => false,
            _ when
                (onto.Suit == "H" || onto.Suit == "D") && (Suit == "C" || Suit == "S") => true,
            _ when
                (onto.Suit == "C" || onto.Suit == "S") && (Suit == "H" || Suit == "D") => true,
            _ when
                (onto.Suit == "C" || onto.Suit == "S") && (Suit == "C" || Suit == "S") => false,
            _ => throw new Exception("The hell")
        };

        // Making sure that you can only place a value thats lower than the value you are trying to place on
        public bool CanNumberStack(Card onto) =>
            Array.IndexOf(Solitaire.values, onto.Value) == Array.IndexOf(Solitaire.values, Value) - 1;

        // Using this for checking if the number can stack when looking for moves into stack
        public bool CanNumberColorStack(Card onto) =>
            Array.IndexOf(Solitaire.values, onto.Value) - 1 == Array.IndexOf(Solitaire.values, Value);

        // Making sure that you can only stack the same suit at the top
        public bool IsSameColor(Card onto) => onto.Suit == Suit;
    }
}
