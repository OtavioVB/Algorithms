namespace OVB.Demos.Algorithms.DivisionRest;

public sealed class Program
{
    public int Mod(int x, int y)
        => x % y;

    public bool IsOdd(int number)
        => Mod(number, 2) == 1;

    /// <summary>
    /// O(n/4)
    /// </summary>
    /// <returns></returns>
    public int CalculateSumOfOddNumbers()
    {
        var sumOfOddNumbers = 0;

        var startCursor = 101;
        var endCursor = 199;

        while ((startCursor - endCursor) != 0)
        {
            if (IsOdd(startCursor))
                sumOfOddNumbers += startCursor;

            if (IsOdd(endCursor))
                sumOfOddNumbers += endCursor;

            endCursor -= 2;
            startCursor -= 2;
        }

        return sumOfOddNumbers;
    }
}
