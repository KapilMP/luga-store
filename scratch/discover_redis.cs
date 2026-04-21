using System;
using System.Linq;
using System.Reflection;
using NRedisStack.RedisStackCommands;

public class Program
{
    public static void Main()
    {
        var type = typeof(ICuckooCommands);
        var methods = type.GetMethods().Select(m => m.Name).Distinct().OrderBy(n => n);
        foreach (var method in methods)
        {
            Console.WriteLine(method);
        }
    }
}
