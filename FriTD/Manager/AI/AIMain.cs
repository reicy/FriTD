using System;
using AI;

namespace AIMain
{
    class AIMain
    {
        static void Main()
        {
            Console.Write("zadaj cislo :");
            Int16 x = Int16.Parse(Console.ReadLine());
            State s = new State(x);



            Console.WriteLine("In binary : "+s.toString());
            Console.ReadKey();
        }
    }
}
