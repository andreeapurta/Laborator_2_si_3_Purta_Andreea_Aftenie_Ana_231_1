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
            string sirInitial = " ";
            int productionLength;
            int linesTA = 0, colTA = 0, linesTS, colTS;
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
                    for (int j = 0; j < colTA; j++)
                    {
                        TA[i, j] = ((char)fisier.Read()).ToString();

                    }
                }
            }
            Console.WriteLine(nonterminals);
            Console.WriteLine(terminals);
            Console.WriteLine("Amu afisam matricea");
            for (int i = 0; i < linesTA; i++)
            {
                for (int j = 0; j < colTA; j++)
                {
                    Console.Write(string.Format("{0} ", TA[i, j]));

                }
            }


        }
    }
}
