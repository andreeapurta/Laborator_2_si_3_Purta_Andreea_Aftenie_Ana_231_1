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
                if (M[i].Item2.IndexOf('.') + 1 < M[i].Item2.Length)
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
            }
            return M;
        }

        public static void GenerateCollection(List<(string, string)> productions, string start, string terminals, string nonterminals)
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
            GenerateTSAndTA(collections, productions, f, start, terminals, nonterminals);
        }

        public static void GenerateTSAndTA(List<List<(string, string)>> collections, List<(string, string)> productions, List<Tuple<int, char, int>> f, string start, string terminals, string nonterminals)
        {
            List<Tuple<int, int, string>> TS = new List<Tuple<int, int, string>>();
            List<Tuple<int, int, string>> TA = new List<Tuple<int, int, string>>();
            List<(string, string)> P = new List<(string, string)>(productions);

            for (int i = 0; i < P.Count; i++)
            {
                var x = P[i];
                x.Item2 = P[i].Item2.Remove(P[i].Item2.IndexOf('.'), 1);
                P[i] = x;
            }

            foreach (var item in collections)
            {
                //fiecare productie din colectie
                foreach (var i in item)
                {

                    if (i.Item2.IndexOf('.') + 1 < i.Item2.Length) //daca punctul nu e la final
                    {
                        var character = i.Item2[i.Item2.IndexOf('.') + 1];
                        //iau functia care am generat-o cu ajutorul i-ului curent din colectie si caracter 
                        var column = f.FirstOrDefault(x => x.Item1.Equals(collections.IndexOf(item)) && x.Item2.Equals(character));
                        if (nonterminals.Contains(character))//daca e neterminal
                        {
                            //numarul la I ul rezultat e elementul din matrice
                            //TS[collections.IndexOf(item), nonterminals.IndexOf(character)] = column.Item3.ToString();
                            TS.Add(new Tuple<int, int, string>(collections.IndexOf(item), nonterminals.IndexOf(character), column.Item3.ToString()));
                        }
                        else if (terminals.Contains(character))
                        {
                            TA.Add(new Tuple<int, int, string>(collections.IndexOf(item), terminals.IndexOf(character), "d" + column.Item3.ToString()));
                            //TA[collections.IndexOf(item), terminals.IndexOf(character)] = "d" + column.Item3.ToString();
                        }
                    }
                    else
                    {
                        //aici stergem punctul din fiecare productie
                        var production = i;
                        var index = production.Item2.IndexOf('.'); //pozitia elementului punctului.
                        production.Item2 = production.Item2.Remove(index, 1);
                        var productionIndex = P.IndexOf(production);
                        foreach (var x in URM(P, start, production.Item1, terminals, nonterminals))
                        {
                            if (production.Item1.Equals(start))
                            {
                                //TA[collections.IndexOf(item), terminals.IndexOf(x)] = "acc";
                                TA.Add(new Tuple<int, int, string>(collections.IndexOf(item), terminals.IndexOf(x), "acc"));
                            }
                            else
                            {
                                //TA[collections.IndexOf(item), terminals.IndexOf(x)] = "r" + productionIndex.ToString();
                                TA.Add(new Tuple<int, int, string>(collections.IndexOf(item), terminals.IndexOf(x), "r" + productionIndex.ToString()));
                            }
                        }

                    }
                }
            }
            AfisareTSSiTA(TS, TA, terminals);
        }

        public static List<string> URM(List<(string, string)> productions, string start, string caracter, string terminals, string nonterminals)
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

        public static List<string> PRIM(List<(string, string)> productions, string caracter, string nonterminals, string terminals)
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

        public static void AfisareTSSiTA(List<Tuple<int, int, string>> TS, List<Tuple<int, int, string>> TA, string terminals)
        {

            foreach (var item in TA.Distinct())
            {
                Console.WriteLine("L:" + item.Item1 + " C:" + item.Item2 + " " + item.Item3); ;
            }
            Console.WriteLine();

            foreach (var item in TS.Distinct())
            {
                Console.WriteLine("L:" + item.Item1 + " C:" + item.Item2 + " " + item.Item3);
            }

        }

        static void Main(string[] args)
        {
            string terminals = "";
            string nonterminals = "";
            var production = new List<(string, string)>();
            var incCollection = new List<List<(string, string)>>();
            string cuvIntrare = "";
            int productionLength;
            string start = "";
            //citire fisier
            using (StreamReader fisier = new StreamReader(@"C:\Users\Andreea Purta\Source\Repos\Laborator_2_Purta_Andreea_Aftenie_Ana_231_12\Laborator_2_Purta_Andreea_Aftenie_Ana_231_1\textFile.txt"))
            {
                nonterminals += fisier.ReadLine();
                terminals += fisier.ReadLine();
                start += fisier.ReadLine();
                Int32.TryParse(fisier.ReadLine(), out productionLength);
                for (int i = 0; i < productionLength; i++)
                {
                    string[] sirImpartit = fisier.ReadLine().Split(" ");
                    production.Add((sirImpartit[0], "." + sirImpartit[1]));
                }

                cuvIntrare += fisier.ReadLine();
            }
            GenerateCollection(production, start, terminals, nonterminals);
        }
    }
}
