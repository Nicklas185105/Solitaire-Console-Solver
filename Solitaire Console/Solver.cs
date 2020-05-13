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
        List<string> temp = new List<string>();
        List<int> score = new List<int>();
        List<string> downcard = new List<string>();
        List<string> deckCard = new List<string>();

        public Solver(Solitaire solitaire, Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            // Always move aces and deuces no matter what
            MovingAcesAndDeuces(deck, colorStacks, stacks);
            if (!moves.Any())
            {
                MovingLogik(deck, colorStacks, stacks);
                foreach (string move in moves) score.Add(0);
                DowncardOrSmooth(deck, stacks);
            }
            else foreach (string move in moves) score.Add(0);
            solitaire.debugMes = "";
            if (!moves.Any())
            {
                solitaire.debugMes = "n";
            }
            else
            {
                int i = 0;
                foreach(string move in moves)
                {
                    solitaire.debugMes += move + (score[i].Equals(score.Max()) ? " Best Move" : "") + "\n";
                    i++;
                }
            }
        }

        void Switch(string card, string equal, List<Card>[] colorStacks, string suit, string command)
        {
            if (card.Equals(equal))
            {
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

        // Done (Rule 1)
        void MovingAcesAndDeuces(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            // Always play an Ace or Deuce wherever you can immediately.
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

        // Done
        void MovingLogik(Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            try
            {
                #region Deck to stack
                if (!deck.Value.Equals("P") && !deck.Suit.Equals("P"))
                {
                    for (int s = stacks.Length - 1; s >= 0; s--)
                    {
                        if (stacks[s].Count != 0)
                        {
                            if (stacks[s].Last().CanColorStack(deck) && stacks[s].Last().CanNumberStack(deck))
                            {
                                /*solitaire.Move("p", s.ToString());
                                return;*/
                                moves.Add("p" + s.ToString() + "0");
                            }
                        }
                        else if (deck.Value.Equals("K"))
                        {
                            moves.Add("p" + s.ToString() + "0");
                        }
                    }
                }
                #endregion
                #region Stack to stack
                for (int s = stacks.Length - 1; s >= 0; s--)
                {
                    if (stacks[s].Count != 0)
                    {
                        int n = stacks[s].Count - 1;
                        if (n > 0)
                        {
                            while (stacks[s][n - 1].Uncovered)
                            {
                                n--;
                                if (n == 0) break;
                            }
                        }
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
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        // Done (Rule 2, 3 and 4)
        void DowncardOrSmooth(Card deck, List<Card>[] stacks)
        {
            // Always make the play or transfer that frees (or allows a play that frees) a downcard, regardless of any other considerations. (Done)
            // When faced with a choice, always make the play or transfer that frees (or allows a play that frees) the downcard from the biggest pile of downcards. (Done)
            // Transfer cards from column to column only to allow a downcard to be freed or to make the columns smoother.
            try
            {
                foreach (string move in moves)
                {
                    string[] args = move.Select(i => i.ToString()).Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                    temp.Add(args[0] + " " + args[1]);
                    if (args[0].Equals("p")) deckCard.Add(args[1]);
                    else deckCard.Add("");
                    downcard.Add(args[2]);
                }
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
                                temp.Add(i.ToString() + " p");
                                downcard.Add(n.ToString());
                            }
                        }
                        i++;
                    }
                    if (deckCardMatch) temp.Remove(temp.Last());
                }

                for (int i = 0; i < downcard.Count; i++)
                {
                    moves.Add(temp[i]);
                    score[i] += (downcard[i].Equals(downcard.Max()) ? 1 : 0);
                }
                temp.Clear();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        // (Rule 5 and 6)
        void KingMovement(Solitaire solitaire, Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            // Don't clear a spot unless there's a King IMMEDIATELY waiting to occupy it.
            // Only play a King that will benefit the column(s) with the biggest pile of downcards, 
            // unless the play of another King will at least allow a transfer that frees a downcard.
            try
            {
                /* Look for if a move has 0 over it, if true, then look for a king and give give 0 points.
                 * If false give -1 point.
                 * If from deck then don't do the check */

                int n = 0;
                foreach (string downcard in downcard)
                {
                    if (downcard.Equals("0") && deckCard[n].Equals(""))
                    {

                    }
                        
                    n++;
                }
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        // (Rule 7)
        void BuildAceStack(Solitaire solitaire, Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            // Only build your Ace stacks (with anything other than an Ace or Deuce) when the play will:
            // * Not interfere with your Next Card Protection
            // * Allow a play or transfer that frees (or allows a play that frees) a downcard
            // * Open up a space for a same-color card pile transfer that allows a downcard to be freed
            // * Clear a spot for an IMMEDIATE waiting King (it cannot be to simply clear a spot)
            try
            {

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        // (Rule 8)
        void DontMove5678Unless(Solitaire solitaire, Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            // Don't play or transfer a 5, 6, 7 or 8 anywhere unless at least one of these situations will apply after the play:
            // * It is smooth with it's next highest even/odd partner in the column
            // * It will allow a play or transfer that will IMMEDIATELY free a downcard
            // * There have not been any other cards already played to the column
            // * You have ABSOLUTELY no other choice to continue playing (this is not a good sign)
            try
            {

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        // (Rule 9)
        void NoOtherSolutions(Solitaire solitaire, Card deck, List<Card>[] colorStacks, List<Card>[] stacks)
        {
            /* When you get to a point that you think all of your necessary cards are covered and you just can't get to them, 
             * IMMEDIATELY play any cards you can to their appropriate Ace stacks. You may have to rearrange existing piles to allow 
             * blocked cards freedom to be able to go to their Ace stack. Hopefully this will clear an existing pile up to the point that you 
             * can use an existing pile upcard to substitute for the necessary covered card. */
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
