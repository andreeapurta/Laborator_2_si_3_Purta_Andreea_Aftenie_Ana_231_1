using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Laborator_2_Purta_Andreea_Aftenie_Ana_231_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Stack stack = new Stack();
            string terminals = "";
            string nonterminals = "";
            var production = new List<(string, string)>();
            string[,] TS = new string[100, 100];
            string[,] TA = new string[100, 100];
            string sirInitial = "";
            string cuvIntrare = "";
            int productionLength;
            int linesTA = 0, colTA = 0, linesTS = 0, colTS = 0;
            int indexLine = 0;
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
                    sirInitial += fisier.ReadLine();
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
            Console.WriteLine("Amu afisam cuvant intrari" + cuvIntrare);

            //initializare stiva cu $ si 0
            stack.Push('0');
            stack.Push('$');
        
            for (int i = 0; i < cuvIntrare.Length; i++)
            {

                indexLine = 0;
                indexCol = cuvIntrare.IndexOf(cuvIntrare[0]);
                value = TA[indexLine, indexCol];

                if (value[0] == 'd')
                {
                    stack.Push(cuvIntrare[0]);
                    stack.Push(value[1]);
                }

                else if (value[0] == 'r')
                {
                    stack.Pop();
                    stack.Pop();
                    stack.Push(production[value[1]].Item1);
                    stack.Push(TS[indexLine, nonterminals.IndexOf(production[value[1]].Item1)]);
                    Console.WriteLine("Amu Iar stiva");
                    foreach (var item in stack)
                    { Console.Write(item + ","); }
                }
         
                else if (value == "acc")
                {
                    Console.WriteLine("Acceptare");
                    break;
                }
                else
                {
                    Console.WriteLine("Eroare");
                    break;
                }

            }
           

        }
    }
}
