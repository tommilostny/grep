using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace grep
{
    class Program
    {
        private static int pocet = 0;
        private static bool c = false, h = false, v = false, a = false, i = false;
        private static string soubor = "", pattern = "";

        private static void Vypis(string line)
        {
            MatchCollection matches;
            if (i) matches = Regex.Matches(line, pattern, RegexOptions.IgnoreCase);
            else matches = Regex.Matches(line, pattern);

            if (matches.Count == 0 && v) pocet++;
            else if (matches.Count > 0 && c && !v) pocet += matches.Count;

            if (matches.Count > 0 && !v && !c || a)
            {
                int i = 0;
                foreach (Match match in matches)
                {
                    while (i < match.Index)
                    {
                        Console.ResetColor();
                        Console.Write(line[i]);
                        i++;
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(match.Value);
                    i += match.Value.Length;
                }
                if (i < line.Length)
                {
                    Console.ResetColor();
                    Console.Write(line.Remove(0, i));
                }
                Console.Write("\n");
            }
            else if ((a && matches.Count == 0) || (v && matches.Count == 0) && !c)
            {
                Console.ResetColor();
                Console.WriteLine(line);
            }
            Console.ResetColor();
        }

        private static void Main(string[] args)
        {
            try
            {
                if (args.Count() > 0)
                {
                    foreach (string argument in args)
                    {
                        switch (argument)
                        {
                            case "-c": c = true; break;
                            case "-h": h = true; break;
                            case "-v": v = true; break;
                            case "-a": a = true; break;
                            case "-i": i = true; break;
                            default:
                                if (File.Exists(argument) && soubor == "") soubor = argument;
                                else if (pattern == "") pattern = argument;
                                else Console.WriteLine("Invalid argument " + argument + "\n");
                                break;
                        }
                    }
                    if (h)
                    {
                        Console.WriteLine("grep searches for PATTERN in a FILE.\nPATTERN can be in form of a regular expression.\nIf no FILE is given, grep searches read standard input.\nBy default, grep prints matching lines.");
                        Console.WriteLine("\nUSAGE:\n   grep [OPTION]... PATTERN [FILE]");
                        Console.WriteLine("\nOPTIONS:\n   -c ..... Prints count of PATTERN matches found.\n   -h ..... Displays help.\n   -v ..... Prints lines that do not contain the PATTERN match.\n   -a ..... Prints all lines.\n   -i ..... Ignore case.");
                        Console.WriteLine("\nAUTHOR:\n   Tom Milostny, PSA4");
                        return;
                    }
                    if (a && v)
                    {
                        Console.WriteLine("You cannot combine -a and -v attributes.");
                        return;
                    }
                    if (soubor.Contains("\"")) soubor = soubor.Remove(0, 1).Remove(soubor.Length - 1, 1);
                    if (pattern.Contains("\"")) pattern.Remove(0, 1).Remove(pattern.Length - 1, 1);
                }
                else throw new Exception(); //nelze bez argumentů
                if (pattern == "") throw new Exception(); //nelze bez patternu

                if (soubor == "") //Výpis z | pipe
                {
                    string line;
                    while ((line = Console.ReadLine()) != null) Vypis(line);
                }
                else if (File.Exists(soubor)) //Výpis ze souboru
                {
                    StreamReader sr = new StreamReader(soubor);
                    while (!sr.EndOfStream) Vypis(sr.ReadLine());
                    sr.Close();
                }
                else Console.WriteLine(string.Format("grep: {0}: No such file or directory", soubor));

                if (c && v) Console.WriteLine(string.Format("Search term \'{0}\' was not found on {1} line(s).", pattern, pocet));
                else if (c) Console.WriteLine(string.Format("Search term \'{0}\' was found {1} time(s).", pattern, pocet));
            }
            catch { Console.WriteLine("Usage: grep [OPTION]... PATTERN [FILE].\nTry 'grep -h' for more information."); }
        }
    }
}