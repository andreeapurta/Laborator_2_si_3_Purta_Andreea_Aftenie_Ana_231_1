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
        public static List<(string, string)> INC(List<(string, string)> production, List<(string, string)> startProductions, string nonterminals)
        {
            List<(string, string)> M = new List<(string, string)>();
            M.AddRange(startProductions);
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

        public static void GenerateCollection(List<(string, string)> productions, string nonterminals)
        {
            //retine ce  I s o generat din ce functie de salt  (de ex I0 si E == I1, I7 si ( == I4)
            List<Tuple<int, char, int>> f = new List<Tuple<int, char, int>>();
            //o lista de I 
            List<List<(string, string)>> collections = new List<List<(string, string)>>();
            var I0 = new List<(string, string)> { productions[0] };
            collections.Add(INC(productions, I0, nonterminals));
            var count = 1;
            //iei primul i si pentru fiecare item din din C[i] 
            for (int i = 0; i < collections.Count; i++)
            {
                // pt fiecare articol din I se executa functia salt daca punctul nu e la final
                foreach (var item in collections[i])
                {
                    //daca punctul nu e la final
                    if (item.Item2.IndexOf('.') + 1 < item.Item2.Length)
                    {
                        //primul caracter dupa punct
                        var character = item.Item2[item.Item2.IndexOf('.') + 1];
                        var I = Salt(productions, collections[i], character, nonterminals);
                        bool verif = true;
                        //verific daca mai exista I ul nou generat in colectie pt ca trebe sa fie doar odata
                        foreach (var inchidere in collections)
                        {
                            //daca au aceleasi elemente  //se genereaza un I care o existat deja 
                            if (inchidere.SequenceEqual(I))
                            {
                                //nu mai trebe adaugat in colectie
                                verif = false;
                                //"functiile de tranzitie"
                                f.Add(new Tuple<int, char, int>(i, character, collections.IndexOf(inchidere)));
                            }
                        }
                        if (verif) //se genereaza I nou
                        {
                            f.Add(new Tuple<int, char, int>(i, character, count));
                            count++;
                            collections.Add(I);
                        }
                    }
                }
            }
        }

        public static void GenerateTSAndTA(List<List<(string, string)>> collections, string terminals, string nonterminals)
        {
            string[,] TS = [100, 100];
            foreach (var item in collections)
            {
                //fiecare productie din colectie
                foreach (var i in item)
                {
                    var character = i.Item2[i.Item2.IndexOf('.') + 1];
                    if (character < i.Item2.Length) //daca punctul nu e la final
                    {
                        if (nonterminals.Contains(character))//daca e neterminal
                        {
                           
                        }
                        else if (terminals.Contains(character))
                        {

                        }
                    }                   
                    else
                    {

                    }
                }
            }
        }



        public List<string> URM(List<(string, string)> productions, string start, string caracter, string terminals, string nonterminals)
        {
            var productii = productions.Where(x => x.Item2.Contains(caracter)).ToList();
            var urm = new List<string>();
            if (caracter.Equals(start))
            {
                urm.Add("$");
            }

            foreach (var productie in productii)
            {
                var index = productie.Item2.IndexOf(caracter) + 1;
                if (index < productie.Item2.Length)
                {
                    var next = productie.Item2[index].ToString();
                    urm.AddRange(PRIM(productions, next, nonterminals, terminals));
                }
                else
                {
                    urm.AddRange(URM(productions, start, productie.Item1, nonterminals, terminals));
                }
            }
            return urm.Distinct().ToList();
        }

        public List<string> PRIM(List<(string, string)> productions, string caracter, string nonterminals, string terminals)
        {
            //iau toate productiile care se deriveaza din caracter 
            var productii = productions.Where(x => x.Item1.Equals(caracter)).ToList();
            var prim = new List<string>();
            //daca caracterul e terminal, il adaug in prim
            if (terminals.Contains(caracter))
            {
                prim.Add(caracter);
            }
            //trebe sa parcurg toate productiile
            foreach (var productie in productii)
            {
                var c = productie.Item2[0]; //iau primul caracter din partea dreapta si il derivez
                if (nonterminals.Contains(c.ToString())) //daca e neterminal
                {
                    if (!caracter.Equals(c.ToString())) //daca e diferit de ala initial sa nu merg pe E-E+T in continuu
                    {
                        prim.AddRange(PRIM(productions, c.ToString(), nonterminals, terminals));  //adaug la prim  prim de caracterul urm in care se  dervriveaza (practic acm merg pe T)
                    }
                }
            }
            return prim; //intoarce doar neterminalele cu care poate incepe un sir
        }

        public static List<(string, string)> Salt(List<(string, string)> production, List<(string, string)> I, char character, string nonterminals)
        {
            //caut toate articolele cu .caracter in partea dreapta
            List<(string, string)> temp = I.Where(i => i.Item2.Contains("." + character.ToString())).ToList();
            //lista rezultat in care adaug 
            List<(string, string)> result = new List<(string, string)>();
            for (int i = 0; i < temp.Count; i++)
            {
                var derivare = temp[i];
                derivare.Item2 = temp[i].Item2.Replace(("." + character), (character + "."));
                result.Add(derivare);
            }

            return INC(production, result, nonterminals);
        }

        static void Main(string[] args)
        {

            Stack stack = new Stack();
            string terminals = "";
            string nonterminals = "";
            var production = new List<(string, string)>();
            var incCollection = new List<List<(string, string)>>();
            string cuvIntrare = "";
            int productionLength;
            //int linesTA = 0, colTA = 0, linesTS = 0, colTS = 0;
            int indexLine = 1;
            int indexCol = 0;
            string value = "";
            string start = ""; // de ex E
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

            //initializare stiva cu $ si 0
            //    stack.Push('$');
            //    stack.Push(0);

            //    bool acc = false;
            //    while (!acc)
            //    {
            //        ///daca e int
            //        if (stack.Peek().GetType() == typeof(int))
            //        {
            //            indexLine = (int)(stack.Peek());
            //            indexCol = terminals.IndexOf(cuvIntrare[0]);
            //            value = TA[indexLine, indexCol];

            //            if (value.Equals("acc"))
            //            {
            //                Console.WriteLine("Amu apartine gramaticii!! :)");
            //                acc = true;
            //            }

            //            if (value.Equals("0"))
            //            {
            //                Console.WriteLine("Amu Nu apartine gramaticii! :(");
            //                return;
            //            }

            //            if (value[0] == 'd')
            //            {
            //                stack.Push(cuvIntrare[0]);
            //                stack.Push(int.Parse(value.Substring(1)));
            //                cuvIntrare = cuvIntrare.Substring(1, cuvIntrare.Length - 1);
            //            }

            //            else if (value[0] == 'r')
            //            {
            //                string auxStack = "";
            //                //parcurgem stiva (Care am facut o array)
            //                foreach (var item in stack.ToArray())
            //                {
            //                    stack.Pop();
            //                    //daca in stiva e caracter atunci adaug in sir
            //                    if (item.GetType() == typeof(char) || item.GetType() == typeof(string))
            //                    {
            //                        auxStack += item;
            //                        var altavar = production[int.Parse(value[1].ToString()) - 1].Item2;
            //                        //daca am gasit in stiva elementul de la care se face reducere se opreste daca nu tot scoate din stiva
            //                        if (Reverse(auxStack).Equals(altavar))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }

            //                var peek = (int)(stack.Peek());
            //                stack.Push(production[int.Parse(value[1].ToString()) - 1].Item1);
            //                var auxVar = nonterminals.IndexOf(stack.Peek().ToString());
            //                stack.Push(int.Parse(TS[peek, auxVar]));
            //            }
            //        }
            //        //daca e string
            //        else
            //        {
            //            //cauta in tabela de salt urmatoarea actiune
            //            var auxCol = nonterminals.IndexOf((char)stack.Pop());
            //            int action = Int32.Parse(TS[(int)stack.Peek(), auxCol]);
            //            //si pune pe stiva
            //            stack.Push(nonterminals[auxCol]);
            //            stack.Push(action);
            //        }
            //    }
        }
        //afisare stiva 
        //Console.WriteLine("Amu  stiva");
        //foreach (var item in stack)
        //{ Console.Write(item + ","); }
    }
}
