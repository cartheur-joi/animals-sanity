using System;
using System.Threading.Tasks;
using Boagaphish;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("in here");

        await Task.CompletedTask;
    } 
}