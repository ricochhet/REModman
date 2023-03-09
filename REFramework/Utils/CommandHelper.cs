using System;
using System.Linq;

namespace REFramework.Utils
{
    public class CommandHelper
    {
        private static string[] programArgs;
        private static string input;
        private static string[] arguments;

        public CommandHelper(string[] args)
        {
            programArgs = args;
        }

        private static string FlagParams(string[] Args, string Pattern, int Length)
        {
            int startIndex = Array.IndexOf(Args, Pattern);
            return string.Join(" ", Args, startIndex + 1, Length).Replace(Pattern, "");
        }

        public void ExpandArgs()
        {
            input = programArgs[0];
            arguments = programArgs.Skip(1).ToArray();
        }

        public void LineWriter(string[] lines, bool alwaysShow = false)
        {
            if (programArgs.Length < 1 && !alwaysShow)
            {
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            else if (alwaysShow)
            {
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public bool Flag(string flag)
        {
            return programArgs.Contains(flag);
        }

        public string StrArg(string flag, int length = 1)
        {
            if (programArgs.Contains(flag) && !(length > programArgs.Length))
            {
                return FlagParams(programArgs, flag, length);
            }

            return null;
        }

        public string StrArgVar(string strargvar, string setting, string fallback)
        {
            if (strargvar == setting)
            {
                return fallback;
            }
            else
            {
                return strargvar;
            }
        }
    }
}