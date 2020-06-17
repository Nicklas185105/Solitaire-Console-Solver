/*
===============================
 AUTHOR: Nicklas Beyer Lydersen (S185105)
 CREATE DATE: 01/06/2020
 PURPOSE: This class is the solver of a solitaire game.
 SPECIAL NOTES: 
===============================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Solitaire_Console
{
    class Solver
    {
        // https://www.chessandpoker.com/solitaire_strategy.html

        List<string> moves = new List<string>();
        List<string> first = new List<string>();
        List<string> second = new List<string>();
        List<int> score = new List<int>();
        List<string> downcard = new List<string>();
        List<string> deckCard = new List<string>();

        /// <summary>
        /// The constructor, that makes sure we are going through everything in the correct order.
        /// </summary>
        /// <param name="solitaire">This is only used for getting the debugMes to display moves</param>
        /// <param name="deck">This is for getting the card from the deck</param>
        /// <param name="colorStacks">This is for getting the cards in the colorStacks. There are 4 lists in this list.</param>
        /// <param name="stacks">This is for getting the card there is in the main stacks. There are 7 lists in this list.</param>
        public Solver(Solitaire solitaire, Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            // Always move aces and deuces no matter what - Calling that logic before anything else.
            MovingAcesAndDeuces(deck, colorStacks, stacks);
            // If there was no moves from before, go through the other logics.
            // If there was moves from before, give the moves a score of zero.
            if (!moves.Any())
            {
                // Start out with finding every possible move
                //MovingLogik(deck, colorStacks, stacks);
                // Adding a score of 0 to all moves found
                //foreach (string move in moves) score.Add(0);
                // Going through the different logics and adding score
                //DowncardOrSmooth(deck, stacks);
                //KingMovement(deck, stacks);

                BuildAceStack(deck, colorStacks, stacks); // Should probably run as the last check
            }
            else foreach (string move in moves) score.Add(0);
            // Resseting the text field.
            solitaire.debugMes = "";
            // If there was no moves found at all, take the next card in the deck.
            if (!moves.Any())
            {
                solitaire.debugMes = "n";
            }
            else
            {
                // If there are moves, then go through each move and look for the one with the highest score and give them the "Best Move" tag.
                int i = 0;
                foreach(string move in moves)
                {
                    solitaire.debugMes += move + (score[i].Equals(score.Max()) && score[i] >= 0 ? " Best Move" : "") + "\n";
                    i++;
                }
            }

            // The class is getting reset automatic because when we are calling the class we are calling it with "new" tag.
        }

        /// <summary>
        /// This is method is getting used for the aces and deuces.
        /// </summary>
        /// <param name="card">The value of the card we are on.</param>
        /// <param name="equal">The card we are looking for.</param>
        /// <param name="colorStacks">The color stacks.</param>
        /// <param name="suit">The suit of the card we are on.</param>
        /// <param name="command">This is the number of the main stacks the card is coming from.</param>
        void Switch(string card, string equal, List<Card>[] colorStacks, string suit, string command)
        {
            // It only goes through the method if the value of the card is equal to the card we are looking for.
            if (card.Equals(equal))
            {
                // Here we do a special check for if the card is a deuce, we check if the color stack for that suit is empty.
                // If it is empty we return out of the method, else we continue.
                if (card.Equals("2"))
                {
                    switch (suit)
                    {
                        case "H":
                            if (!colorStacks[0].Any()) return;
                            break;
                        case "D":
                            if (!colorStacks[1].Any()) return;
                            break;
                        case "C":
                            if (!colorStacks[2].Any()) return;
                            break;
                        case "S":
                            if (!colorStacks[3].Any()) return;
                            break;
                    }
                }
                // The correct move will be added to the moves list.
                switch (suit)
                {
                    case "H":
                        moves.Add(command + " r");
                        break;
                    case "D":
                        moves.Add(command + " m");
                        break;
                    case "C":
                        moves.Add(command + " b");
                        break;
                    case "S":
                        moves.Add(command + " c");
                        break;
                }
            }
        }

        /// <summary>
        /// Done (Rule 1)
        /// 
        /// Always play an Ace or Deuce wherever you can immediately.
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="colorStacks">The color stacks list.</param>
        /// <param name="stacks">The main stacks list.</param>
        void MovingAcesAndDeuces(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            try{
                #region Aces
                // From the stack
                for (int s = 0; s < stacks.Length; s++)
                {
                    if (stacks[s].Count != 0)
                    {
                        if (stacks[s].Last().Value.Equals("A"))
                        {
                            Switch(stacks[s].Last().Value, "A", colorStacks, stacks[s].Last().Suit, s.ToString());
                            return;
                        }
                    }
                }
                // From the deck
                if (deck.Value.Equals("A"))
                {
                    Switch(deck.Value, "A", colorStacks, deck.Suit, "p");
                    return;
                }
                #endregion
                #region Deuces
                // From the stack
                for (int s = 0; s < stacks.Length; s++)
                {
                    if (stacks[s].Count != 0)
                    {
                        if (stacks[s].Last().Value.Equals("2"))
                        {
                            Switch(stacks[s].Last().Value, "2", colorStacks, stacks[s].Last().Suit, s.ToString());
                            return;
                        }
                    }
                }
                // From the deck
                if (deck.Value.Equals("2"))
                {
                    Switch(deck.Value, "2", colorStacks, deck.Suit, "p");
                    return;
                }
                #endregion
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// Done
        /// 
        /// This method is for finding all possible moves and placing them inside the moves list.
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="colorStacks">The color stacks list.</param>
        /// <param name="stacks">The main stacks list.</param>
        void MovingLogik(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            try
            {
                #region Deck to stack
                // The reason for the "P" value, is if the deck is empty, the card of the deck will have a value "P" and a suit "P", so the logik know that it is empty
                if (!deck.Value.Equals("P") && !deck.Suit.Equals("P"))
                {
                    // We are going through the main stacks list backwards, to check the cards with higher start covered cards.
                    for (int s = stacks.Length - 1; s >= 0; s--)
                    {
                        // We are checking if the current list in the stacks list, is not empty
                        if (stacks[s].Count != 0)
                        {
                            // We are checking if the cards can stack with the colors and with the numbers
                            if (stacks[s].Last().CanColorStack(deck) && stacks[s].Last().CanNumberStack(deck))
                            {
                                /*solitaire.Move("p", s.ToString());
                                return;*/
                                moves.Add("p" + s.ToString() + "0");
                            }
                        } // if it is empty and the value of the deck card is a king, then there is a possible move anyways.
                        else if (deck.Value.Equals("K"))
                        {
                            moves.Add("p" + s.ToString() + "0");
                        }
                    }
                }
                #endregion
                #region Stack to stack
                // We are going through the main stacks list backwards, to check the cards with higher start covered cards.
                for (int s = stacks.Length - 1; s >= 0; s--)
                {
                    // We are checking if the current list in the stacks list, is not empty. 
                    if (stacks[s].Count != 0)
                    {
                        // We are setting a new int to the last card in the list.
                        int n = stacks[s].Count - 1;
                        if (n > 0)
                        {
                            // Here we do a check to see if the card under the current card is uncovered, if it is, go to that card and do the same check again.
                            // In short, this is for getting the correct uncovered card.
                            while (stacks[s][n - 1].Uncovered)
                            {
                                n--;
                                if (n == 0) break;
                            }
                        }
                        // When we have the correct uncovered card in the current list, we can check if it can be moved to any other list.
                        for (int i = stacks.Length - 1; i >= 0; i--)
                        {
                            if (stacks[i].Count != 0)
                            {
                                if (stacks[i].Last().CanColorStack(stacks[s][n]) && stacks[i].Last().CanNumberStack(stacks[s][n]))
                                {
                                    /*solitaire.Move(s.ToString(), i.ToString());
                                    return;*/
                                    moves.Add(s.ToString() + i.ToString() + n.ToString());
                                }
                            }
                            else if (stacks[s][n].Value.Equals("K") && n != 0)
                            {
                                /*solitaire.Move(s.ToString(), i.ToString());
                                return;*/
                                moves.Add(s.ToString() + i.ToString() + n.ToString());
                            }
                        }
                    }
                }
                #endregion

                // This logic is commented out
                #region moving logic to color stacks (can maybe be used for rule 7)
                /*
                #region Stack to color stack
                for (int s = stacks.Length - 1; s >= 0; s--)
                {
                    if (stacks[s].Count != 0)
                    {
                        for (int i = colorStacks.Length - 1; i >= 0; i--)
                        {
                            if (colorStacks[i].Count != 0)
                            {
                                if (colorStacks[i].Last().Suit.Equals(stacks[s].Last().Suit) && colorStacks[i].Last().CanNumberColorStack(stacks[s].Last()))
                                {
                                    Switch(stacks[s].Last().Value, stacks[s].Last().Value, colorStacks, stacks[s].Last().Suit, s.ToString());
                                }
                            }
                        }
                    }
                }
                #endregion
                #region deck to color stack
                if (!deck.Value.Equals("P") && !deck.Suit.Equals("P"))
                {
                    for (int s = colorStacks.Length - 1; s >= 0; s--)
                    {
                        if (colorStacks[s].Count != 0)
                        {
                            if (colorStacks[s].Last().Suit.Equals(deck.Suit) && colorStacks[s].Last().CanNumberColorStack(deck))
                            {
                                Switch(deck.Value, deck.Value, colorStacks, deck.Suit, "p");
                            }
                        }
                    }
                }
                #endregion
                */
                #endregion
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// Done (Rule 2, 3 and 4)
        /// 
        /// Always make the play or transfer that frees (or allows a play that frees) a downcard, regardless of any other considerations. (Done)
        /// When faced with a choice, always make the play or transfer that frees (or allows a play that frees) the downcard from the biggest pile of downcards. (Done)
        /// Transfer cards from column to column only to allow a downcard to be freed or to make the columns smoother.
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="stacks">The main stacks list.</param>
        void DowncardOrSmooth(Card deck, List<Card>[] stacks)
        {
            try
            {
                foreach (string move in moves)
                {
                    // Here we are splitting up the string we placed inside the moves list.
                    string[] args = move.Select(i => i.ToString()).Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                    // We are adding the two first variables inside their own lists.
                    first.Add(args[0]);
                    second.Add(args[1]);
                    // If the first variable is from the deck, then add the second variable inside a seperate list. Else add an empty string to that list.
                    if (args[0].Equals("p")) deckCard.Add(args[1]);
                    else deckCard.Add("");
                    // For the last part we are placing the number for how many downcards that move has inside it's own list.
                    downcard.Add(args[2]);
                }
                // After going through all the moves in the moves list, we clear out the list.
                moves.Clear();

                // Make it look for if you move down the card from the deck, if it's possible to move another card on top that has a high number of covered cards.
                // The move this function will find is only if the there isen't a card on the table with the same number and color, that has atleast one covered card.
                // Example where the move of this function will be invalid: https://gyazo.com/3e8b50197c4f09ccb96d6969fb09f4ad
                if (deckCard.Any() || deck.Value.Equals("K"))
                {
                    int i = 0;
                    bool deckCardMatch = false;
                    foreach (List<Card> stack in stacks)
                    {
                        int n = stack.Count - 1;
                        if (n > 0)
                        {
                            while (stack[n - 1].Uncovered)
                            {
                                n--;
                                if (n == 0) break;
                            }
                        }
                        if (stack.Count != 0)
                        {
                            if (deck.Value.Equals(stack[n].Value) && !deck.CanColorStack(stack[n]) && n > 0) deckCardMatch = true;
                            if (deck.CanColorStack(stack[n]) && deck.CanNumberStack(stack[n]))
                            {
                                first.Add(i.ToString());
                                second.Add("p");
                                downcard.Add(n.ToString());
                            }
                        }
                        i++;
                    }
                    if (deckCardMatch)
                    {
                        first.Remove(first.Last());
                        second.Remove(second.Last());
                    }
                }

                // Here we add the moves from temp back to the moves list and add the downcard value as score to the score list.
                for (int i = 0; i < downcard.Count; i++)
                {
                    moves.Add(first[i] + " " + second[i]);
                    //score[i] += (downcard[i].Equals(downcard.Max()) ? 1 : 0);
                    score[i] += int.Parse(downcard[i]);
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// Done (Rule 5 and 6)
        /// 
        /// Don't clear a spot unless there's a King IMMEDIATELY waiting to occupy it. (Done)
        /// Only play a King that will benefit the column(s) with the biggest pile of downcards, unless the play of another King will at least allow a transfer that frees a downcard. (Done)
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="stacks">The main stacks list.</param>
        void KingMovement(Card deck, List<Card>[] stacks)
        {
            try
            {
                /* Do not run this code if there is no longer any covered cards on the table */

                /* Only run the code if there is only one move left.
                 * Look for if the move has 0 over it, if true, then look for a king.
                 * If there is no king then remove the move.
                 * If the move is from the deck then don't do the check. */

                /* Check if there is a king in the deck
                 * Check if there is a queen that can be placed on the king from the deck.
                 * If there is not a queen that can be moved to the king from the deck, then remove the move*/

                // We check if there is only one move and that move is not from the deck to the stacks and the move has 0 downcards under it.
                if (moves.Count == 1 && !deckCard.Any() && downcard[0].Equals("0"))
                {
                    // Make a variable for the number of covered cards
                    int coveredCards = 0;
                    
                    bool kingOnTable = false;
                    bool kingInDeck = false;

                    // Check if there is a king in the deck
                    if (deck.Value.Equals("K"))
                    {
                        kingInDeck = true;
                    }

                    // check if there are any kings on the table
                    foreach (List<Card> stack in stacks)
                    {
                        int n = stack.Count - 1;
                        if (n > 0)
                        {
                            while (stack[n - 1].Uncovered)
                            {
                                n--;
                                if (n == 0) break;
                            }
                        }
                        coveredCards += n;
                        // Here we check if the number of cards in the stack is higher than 1, because if the number of cards is 1 and it is a king, then the move would not make any sense.
                        if (stack.Count > 1)
                        {
                            if (stack[n].Value.Equals("K")) kingOnTable = true;
                        }


                        if (stack.Count != 0 && kingInDeck)
                        {
                            if (deck.CanColorStack(stack[n]) && deck.CanNumberStack(stack[n])) kingInDeck = true;
                            else kingInDeck = false;
                        }
                    }

                    // If there was not any kings on the table or in the deck and there is still covered cards on the table, we remove the move.
                    if (!kingOnTable && !kingInDeck && coveredCards > 0)
                    {
                        moves.Clear();
                        first.Clear();
                        second.Clear();
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// (Rule 7)
        /// 
        /// Only build your Ace stacks (with anything other than an Ace or Deuce) when the play will:
        /// * Not interfere with your Next Card Protection
        /// * Allow a play or transfer that frees (or allows a play that frees) a downcard
        /// * Open up a space for a same-color card pile transfer that allows a downcard to be freed
        /// * Clear a spot for an IMMEDIATE waiting King (it cannot be to simply clear a spot)
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="colorStacks">The color stacks list.</param>
        /// <param name="stacks">The main stacks list.</param>
        void BuildAceStack(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            try
            {
                // Only do this check if there is currently no moves
                if (moves.Any()) return;

                foreach(List<Card> stack in stacks)
                {
                    int n = stack.Count - 1;
                    if (n > 0)
                    {
                        while (stack[n - 1].Uncovered)
                        {
                            n--;
                            if (n == 0) break;
                        }
                    }

                    int stackCardSuit = stack[n].Suit.Equals("C") ? 0 : stack[n].Suit.Equals("D") ? 1 : stack[n].Suit.Equals("H") ? 2 : stack[n].Suit.Equals("S") ? 3 : 4;

                    if(stackCardSuit == 4)
                    {
                        moves.Add("Suit Error");
                        score.Add(-10);
                        return;
                    }

                    if (colorStacks[stackCardSuit].Last().CanNumberStack(stack[n]))
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// (Rule 8)
        /// 
        /// Don't play or transfer a 5, 6, 7 or 8 anywhere unless at least one of these situations will apply after the play:
        /// * It is smooth with it's next highest even/odd partner in the column
        /// * It will allow a play or transfer that will IMMEDIATELY free a downcard
        /// * There have not been any other cards already played to the column
        /// * You have ABSOLUTELY no other choice to continue playing (this is not a good sign)
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="colorStacks">The color stacks list.</param>
        /// <param name="stacks">The main stacks list.</param>
        void DontMove5678Unless(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// (Rule 9)
        /// 
        /// When you get to a point that you think all of your necessary cards are covered and you just can't get to them, 
        /// IMMEDIATELY play any cards you can to their appropriate Ace stacks. You may have to rearrange existing piles to allow 
        /// blocked cards freedom to be able to go to their Ace stack. Hopefully this will clear an existing pile up to the point that you 
        /// can use an existing pile upcard to substitute for the necessary covered card.
        /// </summary>
        /// <param name="deck">The deck card.</param>
        /// <param name="colorStacks">The color stacks list.</param>
        /// <param name="stacks">The main stacks list.</param>
        void NoOtherSolutions(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }
    }
}
