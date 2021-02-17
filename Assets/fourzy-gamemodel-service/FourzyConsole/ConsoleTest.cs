using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FourzyConsole
{
    static class FourzyConsoleTest
    {
        static void ShowExampleNames()
        {
            Console.WriteLine("Sample Generatated Names:");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(Constants.GenerateName());
            }
        }
    }
}
