using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire_Console
{
    class Solitaire
    {
        public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "X", "J", "Q", "K" };
        public static string[] suits = new string[] { "H", "D", "C", "S" };

        int ColorStackIndex(string suit) => suit switch
        {
            "H" => 0,
            "D" => 1,
            "C" => 2,
            "S" => 3,
            _ => throw new Exception("Somethings Wrong")
        };

        public List<Card> Deck;
        List<Card>[] ColorStacks;
        List<Card>[] Stacks;

        int CurrentDeckIndex = 0;

        public Solitaire(bool nothing)
        {

        }

        public Solitaire()
        {
            // Give the console a black background color
            Console.BackgroundColor = ConsoleColor.Black;

            // Creating 4 lists of card in ColorStacks
            ColorStacks = new Object[4].Select(i => new List<Card>()).ToArray();
            // Creating 7 lists of card in Stacks
            Stacks = new Object[7].Select(i => new List<Card>()).ToArray();

            // Creating a list to hold all the cards
            List<Card> all = new List<Card>();

            // Placing all the cards inside the all list
            foreach (string suit in suits)
            {
                all.AddRange(values.Select(i => new Card(i, suit)));
            }

            Random random = new Random();

            // Placing the solitaire order inside the Stacks list
            for (int i = 0; i < Stacks.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    int sel = random.Next(0, all.Count);
                    Stacks[i].Add(all[sel]);
                    all.RemoveAt(sel);
                }
            }

            Deck = new List<Card>();

            // Placing the rest of the cards inside the deck, and making sure that they are marked as uncovered
            while (all.Any())
            {
                int sel = random.Next(0, all.Count);
                Deck.Add(all[sel]);
                all[sel].Uncover();
                all.RemoveAt(sel);
            }

            // Making sure the game runs until closed
            
            Loop();

            Console.WriteLine();
        }

        bool Done = false;

        void Loop()
        {
            while (!Done)
            {
                Write();
                Prompt();
            }
        }

        public string debugMes = "";
        bool finishMode = false;
        int currfinstack = 0;
        string[] ColorStackPlacement = new string[] { "r", "m", "b", "c" };
        string input;
        string[] args;
        public void Prompt(string overrideInput = "")
        {
            if (Stacks.All(i => i.All(j => j.Uncovered)))
                debugMes = "You Win!\n" + (string.IsNullOrWhiteSpace(debugMes) ? "" : (": " + debugMes));

            if (!string.IsNullOrEmpty(debugMes))
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 3 - debugMes.Split('\n').Length);
                Console.Write(debugMes);
            }

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.Write(": ");

            debugMes = "";
            input = "";

            if (overrideInput != string.Empty)
                input = overrideInput;

            if (!finishMode)
                if (overrideInput == string.Empty)
                    input = Console.ReadLine();
                else
                {
                    if (Stacks.All(i => !i.Any()) && !Deck.Any())
                    {
                        finishMode = false;
                        return;
                    }

                    currfinstack = (currfinstack + 1) % 9;
                    if (currfinstack < 7)
                    {
                        if (!Stacks[currfinstack].Any())
                            return;

                        input = "m";
                        input += currfinstack.ToString();
                        input += ColorStackPlacement[ColorStackIndex(Stacks[currfinstack].Last().Suit)];
                    }
                    else if (currfinstack == 7)
                        input = "n";
                    else if (currfinstack == 8)
                    {
                        if (!Deck.Any())
                            return;

                        input = "mp";
                        input += ColorStackPlacement[ColorStackIndex(Deck[CurrentDeckIndex].Suit)];
                    }
                }

            args = input.Select(i => i.ToString()).Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            if (args.Length == 0)
            {
                debugMes = "Invalid Command";
                return;
            }

            switch (args[0])
            {
                case "q":
                    Q();
                    break;
                case "r":
                    R();
                    break;
                case "h":
                    H();
                    break;
                case "f":
                    F();
                    break;
                case "m":
                    M();
                    break;
                case "n":
                    N();
                    break;
                case "s":
                    S();
                    break;
                case "t":
                    Deck.Clear();
                    break;
                default:
                    debugMes = "Invalid Command";
                    break;
            }
        }

        public void Q()
        {
            Done = true;
        }
        public void R()
        {
            Done = true;
            Program.Restart = true;
        }
        public void H()
        {
            debugMes =
                        "Help               = h\n" +
                        "Quit               = q\n" +
                        "Next Card(p)       = n\n" +
                        "Move A to B        = m [p(deck), r(red), m(magenta), b(blue), c(cyan), (0-6)(Table index)] [p(, r, m, b, c, (0-6)]\n" +
                        "Reset (New Game)   = r\n" +
                        "Finish Game        = f\n" +
                        "Solve the Game     = s";
        }
        public void F()
        {
            if (!Stacks.All(i => i.All(j => j.Uncovered)))
            {
                debugMes = "Game not yet won, cannot finish";
                return;
            }
            debugMes = "test";
            finishMode = true;
        }

        public void M() { M(null); }

        public void M(string[] argsoverride)
        {
            if (argsoverride != null) args = argsoverride;


            if (args.Length < 3)
            {
                debugMes = "Invalid Command";
                return;
            }
            string loc1 = args[1];
            string loc2 = args[2];
            string[] areas = new string[] { "0", "1", "2", "3", "4", "5", "6", "p", "r", "m", "b", "c" };
            if (!areas.Contains(loc1) || !areas.Contains(loc2))
            {
                debugMes = $"Invalid Move Locations {loc1}, {loc2}";
                return;
            }

            var loc1p = getLoc(loc1);
            var loc2p = getLoc(loc2);

            Card c1 = loc1p switch
            {
                ("Stacks", _) => Stacks[loc1p.Item2].Any() ? Stacks[loc1p.Item2].Last() : null,
                ("Deck", _) => Deck.Count > CurrentDeckIndex ? Deck[CurrentDeckIndex] : null,
                ("ColorStacks", _) => ColorStacks[loc1p.Item2].Any() ? ColorStacks[loc1p.Item2].Last() : null,
                (_, _) => throw new Exception("damn")
            };

            Card c2 = loc2p switch
            {
                ("Stacks", _) => Stacks[loc2p.Item2].Any() ? Stacks[loc2p.Item2].Last() : null,
                ("Deck", _) => Deck.Count > CurrentDeckIndex ? Deck[CurrentDeckIndex] : null,
                ("ColorStacks", _) => ColorStacks[loc2p.Item2].Any() ? ColorStacks[loc2p.Item2].Last() : null,
                (_, _) => throw new Exception("damn")
            };

            if (c1 == null)
            {
                debugMes = "Cannot find card";
                return;
            }

            if (c2 == null)
                if (loc2p.Item1 == "ColorStacks")
                {
                    if (c1.Suit == suits[loc2p.Item2] && c1.Value == "A")
                    {
                        ColorStacks[loc2p.Item2].Add(c1);
                        if (loc1p.Item1 == "Stacks")
                            Stacks[loc1p.Item2].Remove(c1);

                        if (loc1p.Item1 == "Deck")
                        {
                            Deck.Remove(c1);
                            CurrentDeckIndex--;
                        }
                        return;
                    }
                    else
                    {
                        debugMes = "Colors don't match, or not A";
                        return;
                    }
                }
                else if (loc2p.Item1 == "Stacks")
                {
                    if (loc1p.Item1 == "Stacks")
                    {
                        if (Stacks[loc1p.Item2].Any(card => card.Value == "K" && card.Uncovered))
                        {
                            Card r = Stacks[loc1p.Item2].First(card => card.Value == "K" && card.Uncovered);
                            int l = Array.IndexOf(Stacks[loc1p.Item2].ToArray(), r);
                            for (int index = l; index < Stacks[loc1p.Item2].Count;)
                            {
                                Stacks[loc2p.Item2].Add(Stacks[loc1p.Item2][index]);
                                Stacks[loc1p.Item2].RemoveAt(index);
                            }
                            return;
                        }
                        else
                        {
                            debugMes = "Not a King";
                            return;
                        }
                    }
                    else if (loc1p.Item1 == "Deck")
                    {
                        if (Deck[CurrentDeckIndex].Value == "K")
                        {
                            Card r = Deck[CurrentDeckIndex];
                            Stacks[loc2p.Item2].Add(r);
                            Deck.Remove(r);
                            CurrentDeckIndex--;
                            return;
                        }
                        else
                        {
                            debugMes = "Not a King";
                            return;
                        }
                    }
                    else if (loc1p.Item1 == "ColorStacks")
                    {
                        if (ColorStacks[loc1p.Item2].Any())
                            if (ColorStacks[loc1p.Item2].Last().Value == "K")
                            {
                                Card r = ColorStacks[loc1p.Item2].Last();
                                Stacks[loc2p.Item2].Add(r);
                                ColorStacks[loc1p.Item2].Remove(r);
                                return;
                            }
                            else
                            {
                                debugMes = "Not a King";
                                return;
                            }
                        else
                        {
                            debugMes = "No item";
                            return;
                        }
                    }
                    else
                    {
                        debugMes = "Items cannot stack";
                        return;
                    }
                }
                else
                {
                    debugMes = "Can't find card 2";
                    return;
                }

            if (loc2p.Item1 == "Stacks")
            {
                if (c1.CanColorStack(c2) && c2.CanNumberStack(c1))
                {
                    Stacks[loc2p.Item2].Add(c1);
                    if (loc1p.Item1 == "Stacks")
                        Stacks[loc1p.Item2].Remove(c1);

                    if (loc1p.Item1 == "Deck")
                    {
                        Deck.Remove(c1);
                        CurrentDeckIndex--;
                    }
                    if (loc1p.Item1 == "ColorStacks")
                        ColorStacks[loc1p.Item2].Remove(c1);

                }
                else if (loc1p.Item1 == "Stacks" && Stacks[loc1p.Item2].Any(card =>
                           card.CanColorStack(c2) &&
                           c2.CanNumberStack(card) &&
                           card.Uncovered))
                {
                    Card r = Stacks[loc1p.Item2].First(card =>
                           card.CanColorStack(c2) &&
                           c2.CanNumberStack(card) &&
                           card.Uncovered);
                    int l = Array.IndexOf(Stacks[loc1p.Item2].ToArray(), r);
                    for (int index = l; index < Stacks[loc1p.Item2].Count;)
                    {
                        Stacks[loc2p.Item2].Add(Stacks[loc1p.Item2][index]);
                        Stacks[loc1p.Item2].RemoveAt(index);
                    }
                }
                else
                {
                    debugMes = "Items cannot stack";
                    return;
                }
            }
            else if (loc2p.Item1 == "Deck")
            {
                debugMes = "Cannot move into deck";
                return;
            }
            else if (loc2p.Item1 == "ColorStacks")
            {
                if (c1.IsSameColor(c2) && c1.CanNumberStack(c2))
                {
                    ColorStacks[loc2p.Item2].Add(c1);
                    if (loc1p.Item1 == "Stacks")
                        Stacks[loc1p.Item2].Remove(c1);

                    if (loc1p.Item1 == "Deck")
                    {
                        Deck.Remove(c1); CurrentDeckIndex--;
                    }
                    if (loc1p.Item1 == "ColorStacks")
                        ColorStacks[loc1p.Item2].Remove(c1);

                }
                else
                {
                    debugMes = "Items cannot stack";
                    return;
                }
            }

            (string, int) getLoc(string find)
            {
                if (int.TryParse(find, out int sid))
                    return ("Stacks", sid);

                if (find == "p")
                    return ("Deck", 0);

                return find switch
                {
                    "r" => ("ColorStacks", 0),
                    "m" => ("ColorStacks", 1),
                    "b" => ("ColorStacks", 2),
                    "c" => ("ColorStacks", 3),
                    _ => throw new Exception("stuff")
                };
            }
        }
        public void Move(string card1, string card2)
        {
            string loc1 = card1;
            string loc2 = card2;
            string[] areas = new string[] { "0", "1", "2", "3", "4", "5", "6", "p", "r", "m", "b", "c" };
            if (!areas.Contains(loc1) || !areas.Contains(loc2))
            {
                debugMes = $"Invalid Move Locations {loc1}, {loc2}";
                return;
            }

            var loc1p = getLoc(loc1);
            var loc2p = getLoc(loc2);

            Card c1 = loc1p switch
            {
                ("Stacks", _) => Stacks[loc1p.Item2].Any() ? Stacks[loc1p.Item2].Last() : null,
                ("Deck", _) => Deck.Count > CurrentDeckIndex ? Deck[CurrentDeckIndex] : null,
                ("ColorStacks", _) => ColorStacks[loc1p.Item2].Any() ? ColorStacks[loc1p.Item2].Last() : null,
                (_, _) => throw new Exception("damn")
            };

            Card c2 = loc2p switch
            {
                ("Stacks", _) => Stacks[loc2p.Item2].Any() ? Stacks[loc2p.Item2].Last() : null,
                ("Deck", _) => Deck.Count > CurrentDeckIndex ? Deck[CurrentDeckIndex] : null,
                ("ColorStacks", _) => ColorStacks[loc2p.Item2].Any() ? ColorStacks[loc2p.Item2].Last() : null,
                (_, _) => throw new Exception("damn")
            };

            if (c1 == null)
            {
                debugMes = "Cannot find card";
                return;
            }

            if (c2 == null)
                if (loc2p.Item1 == "ColorStacks")
                {
                    if (c1.Suit == suits[loc2p.Item2] && c1.Value == "A")
                    {
                        ColorStacks[loc2p.Item2].Add(c1);
                        if (loc1p.Item1 == "Stacks")
                            Stacks[loc1p.Item2].Remove(c1);

                        if (loc1p.Item1 == "Deck")
                        {
                            Deck.Remove(c1);
                            CurrentDeckIndex--;
                        }
                        return;
                    }
                    else
                    {
                        debugMes = "Colors don't match, or not A";
                        return;
                    }
                }
                else if (loc2p.Item1 == "Stacks")
                {
                    if (loc1p.Item1 == "Stacks")
                    {
                        if (Stacks[loc1p.Item2].Any(card => card.Value == "K" && card.Uncovered))
                        {
                            Card r = Stacks[loc1p.Item2].First(card => card.Value == "K" && card.Uncovered);
                            int l = Array.IndexOf(Stacks[loc1p.Item2].ToArray(), r);
                            for (int index = l; index < Stacks[loc1p.Item2].Count;)
                            {
                                Stacks[loc2p.Item2].Add(Stacks[loc1p.Item2][index]);
                                Stacks[loc1p.Item2].RemoveAt(index);
                            }
                            return;
                        }
                        else
                        {
                            debugMes = "Not a King";
                            return;
                        }
                    }
                    else if (loc1p.Item1 == "Deck")
                    {
                        if (Deck[CurrentDeckIndex].Value == "K")
                        {
                            Card r = Deck[CurrentDeckIndex];
                            Stacks[loc2p.Item2].Add(r);
                            Deck.Remove(r);
                            CurrentDeckIndex--;
                            return;
                        }
                        else
                        {
                            debugMes = "Not a King";
                            return;
                        }
                    }
                    else if (loc1p.Item1 == "ColorStacks")
                    {
                        if (ColorStacks[loc1p.Item2].Any())
                            if (ColorStacks[loc1p.Item2].Last().Value == "K")
                            {
                                Card r = ColorStacks[loc1p.Item2].Last();
                                Stacks[loc2p.Item2].Add(r);
                                ColorStacks[loc1p.Item2].Remove(r);
                                return;
                            }
                            else
                            {
                                debugMes = "Not a King";
                                return;
                            }
                        else
                        {
                            debugMes = "No item";
                            return;
                        }
                    }
                    else
                    {
                        debugMes = "Items cannot stack";
                        return;
                    }
                }
                else
                {
                    debugMes = "Can't find card 2";
                    return;
                }

            if (loc2p.Item1 == "Stacks")
            {
                if (c1.CanColorStack(c2) && c2.CanNumberStack(c1))
                {
                    Stacks[loc2p.Item2].Add(c1);
                    if (loc1p.Item1 == "Stacks")
                        Stacks[loc1p.Item2].Remove(c1);

                    if (loc1p.Item1 == "Deck")
                    {
                        Deck.Remove(c1);
                        CurrentDeckIndex--;
                    }
                    if (loc1p.Item1 == "ColorStacks")
                        ColorStacks[loc1p.Item2].Remove(c1);

                }
                else if (loc1p.Item1 == "Stacks" && Stacks[loc1p.Item2].Any(card =>
                           card.CanColorStack(c2) &&
                           c2.CanNumberStack(card) &&
                           card.Uncovered))
                {
                    Card r = Stacks[loc1p.Item2].First(card =>
                           card.CanColorStack(c2) &&
                           c2.CanNumberStack(card) &&
                           card.Uncovered);
                    int l = Array.IndexOf(Stacks[loc1p.Item2].ToArray(), r);
                    for (int index = l; index < Stacks[loc1p.Item2].Count;)
                    {
                        Stacks[loc2p.Item2].Add(Stacks[loc1p.Item2][index]);
                        Stacks[loc1p.Item2].RemoveAt(index);
                    }
                }
                else
                {
                    debugMes = "Items cannot stack";
                    return;
                }
            }
            else if (loc2p.Item1 == "Deck")
            {
                debugMes = "Cannot move into deck";
                return;
            }
            else if (loc2p.Item1 == "ColorStacks")
            {
                if (c1.IsSameColor(c2) && c1.CanNumberStack(c2))
                {
                    ColorStacks[loc2p.Item2].Add(c1);
                    if (loc1p.Item1 == "Stacks")
                        Stacks[loc1p.Item2].Remove(c1);

                    if (loc1p.Item1 == "Deck")
                    {
                        Deck.Remove(c1); CurrentDeckIndex--;
                    }
                    if (loc1p.Item1 == "ColorStacks")
                        ColorStacks[loc1p.Item2].Remove(c1);

                }
                else
                {
                    debugMes = "Items cannot stack";
                    return;
                }
            }

            (string, int) getLoc(string find)
            {
                if (int.TryParse(find, out int sid))
                    return ("Stacks", sid);

                if (find == "p")
                    return ("Deck", 0);

                return find switch
                {
                    "r" => ("ColorStacks", 0),
                    "m" => ("ColorStacks", 1),
                    "b" => ("ColorStacks", 2),
                    "c" => ("ColorStacks", 3),
                    _ => throw new Exception("stuff")
                };
            }
        }
        public void N()
        {
            CurrentDeckIndex++;
            if (CurrentDeckIndex >= Deck.Count)
                CurrentDeckIndex = 0;
        }
        public void S()
        {
            int nValue = 0;

            while (!Stacks.All(i => i.All(j => j.Uncovered)))
            {
                if (CurrentDeckIndex < 0) continue;
                if (nValue > 15)
                {
                    Program.Lost++;
                    R();
                    return;
                }

                debugMes = "Analyzing for best move";
                new Solver(this, Deck.Any() ? Deck[CurrentDeckIndex] : new Card("P", "P"), ColorStacks, Stacks, ref nValue);

            }

            Program.Wins++;
            R();
            return;
            
        }

        // Printing the board
        public void Write()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            // Deck
            // Printing "  " if the deck is empty else print "# " (This is to visualize if there is a card in the deck, and when the deck starts over)
            // Printing the correct card
            if (Deck.Any())
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(CurrentDeckIndex == Deck.Count - 1 ?
                    "  " :
                    "# ");

                if (CurrentDeckIndex >= Deck.Count)
                    CurrentDeckIndex = Deck.Count - 1;

                if (CurrentDeckIndex <= 0)
                    CurrentDeckIndex = 0;

                Deck[CurrentDeckIndex].Print();
            }

            // Color Stacks
            // Print the last(highest) value if there are any
            for (int s = 0; s < ColorStacks.Length; s++)
            {
                Console.SetCursorPosition(7 + (s * 2), 0);
                if (ColorStacks[s].Any())
                    ColorStacks[s].Last()?.Print();
            }

            // Stacks
            // Printing the right value at the right position
            // If the last value in the list is covered then uncover it
            for (int s = 0; s < Stacks.Length; s++)
            {
                Console.SetCursorPosition(s * 3, 2);
                Console.Write(s);
                Console.SetCursorPosition(s * 3, 3);
                Console.Write("---");

                for (int i = 0; i < Stacks[s].Count; i++)
                {
                    Console.SetCursorPosition(s * 3, i + 4);
                    if (i == Stacks[s].Count - 1)
                        Stacks[s][i].Uncover();

                    Stacks[s][i].Print();
                }
            }
        }

    }
}
