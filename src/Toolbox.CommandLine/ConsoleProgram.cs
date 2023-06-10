using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.CommandLine
{
    /// <summary>
    /// Execute a programm on a <see cref="Console"/> and handle options automatically.
    /// </summary>
    /// <remarks>
    /// The help option will create a return code of 1, an error will return 2 and an <see cref="Exception"/> will return 3.
    /// </remarks>
    public class ConsoleProgram
    {
        public static int Run<T>(string[] args, Func<T, int> mainAction) where T : class, new()
        {
            try
            {
                var parser = Parser.Create<T>();
                var result = parser.Parse(args);

                return result
                        .OnError(ExecuteError)
                        .OnHelp(ExecuteHelp)
                        .On(mainAction)
                        .Return;
            }
            catch (Exception exception)
            {                
                Console.WriteLine(exception.Message);
                return 3;
            }
        }

        private static int ExecuteHelp(ParseResult result)
        {
            var width = Console.WindowWidth;
            if (width == 0) width = 80;

            Console.WriteLine(result.GetHelpText(width));
            return 1;
        }

        private static int ExecuteError(ParseResult result)
        {
            Console.WriteLine(result.Text); 
            return 2;
        }
    }
}
