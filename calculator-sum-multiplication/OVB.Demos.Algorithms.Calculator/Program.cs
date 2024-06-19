namespace OVB.Demos.Algorithms.Calculator;

public sealed class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    public static (int SumResult, int MultiplicationResult) Operation(int x, int y)
        => (x + y, x * y);
}