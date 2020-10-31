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

        static void Main(string[] args)
        {
            Stack stack = new Stack();
            string terminals = "";
            string nonterminals = "";
            var production = new List<(string, string)>();
            string[,] TS = new string[100, 100];
            string[,] TA = new string[100, 100];
            string cuvIntrare = "";
            int productionLength;
            int linesTA = 0, colTA = 0, linesTS = 0, colTS = 0;
            int indexLine = 1;
            int indexCol = 0;
            string value = "";

            //citire fisier
            using (StreamReader fisier = new StreamReader(@"C:\Users\Andreea Purta\Desktop\Laborator_2_Purta_Andreea_Aftenie_Ana_231_1\Laborator_2_Purta_Andreea_Aftenie_Ana_231_1\textFile.txt"))
            {
                nonterminals += fisier.ReadLine();
                terminals += fisier.ReadLine();
                Int32.TryParse(fisier.ReadLine(), out productionLength);
                for (int i = 0; i < productionLength; i++)
                {
                    string[] sirImpartit = fisier.ReadLine().Split(" ");
                    production.Add((sirImpartit[0], sirImpartit[1]));
                }

                Int32.TryParse(fisier.ReadLine(), out linesTA);
                Int32.TryParse(fisier.ReadLine(), out colTA);

                for (int i = 0; i < linesTA; i++)
                {
                    string line = fisier.ReadLine();
                    string[] terms = line.Split(" ");
                    for (int j = 0; j < colTA; j++)
                    {
                        TA[i, j] = terms[j];
                    }
                }

                Int32.TryParse(fisier.ReadLine(), out linesTS);
                Int32.TryParse(fisier.ReadLine(), out colTS);

                for (int i = 0; i < linesTS; i++)
                {
                    string lineTS = fisier.ReadLine();
                    string[] termsTS = lineTS.Split(" ");
                    for (int j = 0; j < colTS; j++)
                    {
                        TS[i, j] = termsTS[j];
                    }
                }
                cuvIntrare += fisier.ReadLine();
            }

            //afisari
            Console.WriteLine(nonterminals);
            Console.WriteLine(terminals);
            Console.WriteLine("Amu afisam matricea TA");
            for (int i = 0; i < linesTA; i++)
            {
                for (int j = 0; j < colTA; j++)
                {
                    Console.Write(TA[i, j] + "\t");

                }
                Console.WriteLine();
            }

            Console.WriteLine("Amu afisam matricea TS");
            for (int i = 0; i < linesTS; i++)
            {
                for (int j = 0; j < colTS; j++)
                {
                    Console.Write(TS[i, j] + "\t");

                }
                Console.WriteLine();
            }
            Console.WriteLine("Amu afisam cuvant intrari " + cuvIntrare);

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
                        stack.Push(int.Parse(value[1].ToString()));
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
