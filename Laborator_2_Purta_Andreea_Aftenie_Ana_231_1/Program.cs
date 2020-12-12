using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Laborator_2_Purta_Andreea_Aftenie_Ana_231_1
{
    class Program
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        //FirstProduction == I
        public static List<(string, string)> INC(List<(string, string)> production, (string, string) firstProduction, string nonterminals)
        {
            List<(string, string)> M = new List<(string, string)>();
            M.Add(firstProduction);
            for (int i = 0; i < M.Count; i++)
            {
                //caut primul element  de dupa punct din partea dreapta a productiei
                char firstElementAfterDot = M[i].Item2[M[i].Item2.IndexOf('.') + 1];
                //daca e neterminal ii bagam productia in multime
                foreach (var nonterminal in nonterminals)
                {
                    if (firstElementAfterDot == nonterminal)
                    {   //ca sa nu adaug si prima productie adica E+T de exemplu pt ca e bagat la linia 24
                        var productions = production.Where(j => j.Item1.Equals(firstElementAfterDot.ToString())).ToList();
                        foreach (var item in productions)
                        {
                            if (!M.Contains(item))
                            {
                                //gasesc toate productiile pentru elementul de dupa punct
                                M.Add(item);
                            }
                        }
                    }
                }
            }

            return M;
        }

        public static void Salt(List<(string, string)> I, char x)
        {
            for (int i = 0; i < I.Count; i++)
            {
                char firstElementAfterDot = I[i].Item2[I[i].Item2.IndexOf('.') + 1];
                if (firstElementAfterDot == x)
                {
                   I[i].Item2.Replace(("." + x), (x + "."));
                }
            }
            INC(I, I[0], "ETF");
        }

        static void Main(string[] args)
        {

            Stack stack = new Stack();
            string terminals = "";
            string nonterminals = "";
            var production = new List<(string, string)>();
            var incCollection = new List<List<(string, string)>>();
            string[,] TS = new string[100, 100];
            string[,] TA = new string[100, 100];
            string cuvIntrare = "";
            int productionLength;
            //int linesTA = 0, colTA = 0, linesTS = 0, colTS = 0;
            int indexLine = 1;
            int indexCol = 0;
            string value = "";

            //citire fisier
            using (StreamReader fisier = new StreamReader(@"C:\Users\Andreea Purta\Source\Repos\Laborator_2_Purta_Andreea_Aftenie_Ana_231_12\Laborator_2_Purta_Andreea_Aftenie_Ana_231_1\textFile.txt"))
            {
                nonterminals += fisier.ReadLine();
                terminals += fisier.ReadLine();
                Int32.TryParse(fisier.ReadLine(), out productionLength);
                for (int i = 0; i < productionLength; i++)
                {
                    string[] sirImpartit = fisier.ReadLine().Split(" ");
                    production.Add((sirImpartit[0], "." + sirImpartit[1]));
                }

                cuvIntrare += fisier.ReadLine();
            }
            #region
            ////afisari
            //Console.WriteLine(nonterminals);
            //Console.WriteLine(terminals);
            //Console.WriteLine("Amu afisam matricea TA");
            //for (int i = 0; i < linesTA; i++)
            //{
            //    for (int j = 0; j < colTA; j++)
            //    {
            //        Console.Write(TA[i, j] + "\t");

            //    }
            //    Console.WriteLine();
            //}

            //Console.WriteLine("Amu afisam matricea TS");
            //for (int i = 0; i < linesTS; i++)
            //{
            //    for (int j = 0; j < colTS; j++)
            //    {
            //        Console.Write(TS[i, j] + "\t");

            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine("Amu afisam cuvant intrari " + cuvIntrare);
            #endregion


            //
            for (int i = 0; i < incCollection.Count; i++)
            {
                incCollection.Add(INC(production, production[i], nonterminals));
            }
            Salt(INC(production, production[0], nonterminals), 'E');
            //initializare stiva cu $ si 0
            stack.Push('$');
            stack.Push(0);

            bool acc = false;
            while (!acc)
            {
                ///daca e int
                if (stack.Peek().GetType() == typeof(int))
                {
                    indexLine = (int)(stack.Peek());
                    indexCol = terminals.IndexOf(cuvIntrare[0]);
                    value = TA[indexLine, indexCol];

                    if (value.Equals("acc"))
                    {
                        Console.WriteLine("Amu apartine gramaticii!! :)");
                        acc = true;
                    }

                    if (value.Equals("0"))
                    {
                        Console.WriteLine("Amu Nu apartine gramaticii! :(");
                        return;
                    }

                    if (value[0] == 'd')
                    {
                        stack.Push(cuvIntrare[0]);
                        stack.Push(int.Parse(value.Substring(1)));
                        cuvIntrare = cuvIntrare.Substring(1, cuvIntrare.Length - 1);
                    }

                    else if (value[0] == 'r')
                    {
                        string auxStack = "";
                        //parcurgem stiva (Care am facut o array)
                        foreach (var item in stack.ToArray())
                        {
                            stack.Pop();
                            //daca in stiva e caracter atunci adaug in sir
                            if (item.GetType() == typeof(char) || item.GetType() == typeof(string))
                            {
                                auxStack += item;
                                var altavar = production[int.Parse(value[1].ToString()) - 1].Item2;
                                //daca am gasit in stiva elementul de la care se face reducere se opreste daca nu tot scoate din stiva
                                if (Reverse(auxStack).Equals(altavar))
                                {
                                    break;
                                }
                            }
                        }

                        var peek = (int)(stack.Peek());
                        stack.Push(production[int.Parse(value[1].ToString()) - 1].Item1);
                        var auxVar = nonterminals.IndexOf(stack.Peek().ToString());
                        stack.Push(int.Parse(TS[peek, auxVar]));
                    }
                }
                //daca e string
                else
                {
                    //cauta in tabela de salt urmatoarea actiune
                    var auxCol = nonterminals.IndexOf((char)stack.Pop());
                    int action = Int32.Parse(TS[(int)stack.Peek(), auxCol]);
                    //si pune pe stiva
                    stack.Push(nonterminals[auxCol]);
                    stack.Push(action);
                }
            }
        }
        //afisare stiva 
        //Console.WriteLine("Amu  stiva");
        //foreach (var item in stack)
        //{ Console.Write(item + ","); }
    }
}
