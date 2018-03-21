using Diacarb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diacarb.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IDiacarbClient client = new DiacarbClient();

            Console.WriteLine("Query the carbohydrate machine");

            bool isRunning = true;
            while(isRunning)
            {
                Console.WriteLine();
                Console.Write("What do you want to ask?: ");
                string input = Console.ReadLine();

                IDietaryResult dietaryResult = client.GetDietaryResultAsync(input).Result;

                Console.WriteLine(dietaryResult);
                
            }
        }
    }
}
