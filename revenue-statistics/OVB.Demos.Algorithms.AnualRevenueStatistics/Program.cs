using System.Globalization;
using System.Text.Json;

namespace OVB.Demos.Algorithms.AnualRevenueStatistics;

public sealed class Program
{
    static void Main(string[] args)
    {
        var result = GetAnnualRevenueStatistics([0, 0, 5_000, 2_500, 3_250]);

        Console.WriteLine($"*Estatísticas Anual de Faturamento Diário*\n\n" +
            $"Menor Faturamento Diário do Ano\t\t{result.GetFriendlyLowestAnnualRevenueAsLocalCurrency()}\n" +
            $"Maior Faturamento Diário do Ano\t\t{result.GetFriendlyHighestAnnualRevenueAsLocalCurrency()}\n" +
            $"Número de Dias que o Faturamento superou a Média Anual\t\t{result.NumberOfDaysThatRevenueGreaterThanTheAnnualAverage} dia(s)");
    }

    /// <summary>
    /// Modelo que Encapsula Estatísticas Anual de Faturamento
    /// </summary>
    public readonly struct RevenueStatisticsModel
    {
        /// <summary>
        /// Menor Faturamento Diário no Ano
        /// </summary>
        public decimal LowestAnnualRevenue { get; }

        /// <summary>
        /// Maior Faturamento Diário do Ano
        /// </summary>
        public decimal HighestAnnualRevenue { get; }

        /// <summary>
        /// Número de Dias que o Faturamento Diário superou a Média Anual
        /// </summary>
        public int NumberOfDaysThatRevenueGreaterThanTheAnnualAverage { get; }

        public const string DEFAULT_CULTURE_INFO = "pt-br";

        private RevenueStatisticsModel(decimal lowestAnnualRevenue, decimal highestAnnualRevenue, int numberOfDaysGreaterThanTheAnnualAverage)
        {
            LowestAnnualRevenue = lowestAnnualRevenue;
            HighestAnnualRevenue = highestAnnualRevenue;
            NumberOfDaysThatRevenueGreaterThanTheAnnualAverage = numberOfDaysGreaterThanTheAnnualAverage;
        }

        public string GetFriendlyLowestAnnualRevenueAsLocalCurrency(string? cultureInfo = null)
            => Math.Round(LowestAnnualRevenue, 2).ToString("C", CultureInfo.GetCultureInfo(cultureInfo ?? DEFAULT_CULTURE_INFO));

        public string GetFriendlyHighestAnnualRevenueAsLocalCurrency(string? cultureInfo = null)
            => Math.Round(HighestAnnualRevenue, 2).ToString("C", CultureInfo.GetCultureInfo(cultureInfo ?? DEFAULT_CULTURE_INFO));

        public static RevenueStatisticsModel Factory(decimal lowestAnnualRevenue, decimal highestAnnualRevenue, int numberOfDaysGreaterThanTheAnnualAverage)
            => new(lowestAnnualRevenue, highestAnnualRevenue, numberOfDaysGreaterThanTheAnnualAverage);
    }

    /// <summary>
    /// Consultar as Estatísticas Anuais.
    /// 
    /// A consulta das estatísticas anuais possui Big O Notation igual a O(n)
    /// </summary>
    /// <param name="annualRevenuesPerDay">Vetor que Contempla o Faturamento Diário de cada dia.</param>
    /// <returns>Modelo que Encapsula Estatística Anual de Faturamento</returns>
    public static RevenueStatisticsModel GetAnnualRevenueStatistics(decimal[] annualRevenuesPerDay)
    {
        const bool IGNORE_DAYS_WITHOUT_REVENUE = true;

        int annualRevenuesPerDayArrayLength = annualRevenuesPerDay.Length;

        int numberOfTheDaysWithoutRevenue = 0;

        decimal totalAnnualRevenue = 0;
        decimal? lowestDailyRevenue = null;
        decimal? highestDailyRevenue = null;

        for (int i = 0; i < annualRevenuesPerDayArrayLength; i++)
        {
            var leftCursorIndex = i;
            var rightCursorIndex = annualRevenuesPerDayArrayLength - i - 1;
            var differenceBetweenCursor = rightCursorIndex - leftCursorIndex;

            if ((leftCursorIndex + rightCursorIndex + 1) == annualRevenuesPerDayArrayLength && (differenceBetweenCursor) == 0)
                break;

            DefineDailyRevenueComparisonIfIsNotDefined(
                dailyRevenue: annualRevenuesPerDay[leftCursorIndex],
                comparisonDailyRevenue: ref lowestDailyRevenue,
                ignoreDaysWithoutRevenue: IGNORE_DAYS_WITHOUT_REVENUE);
            DefineDailyRevenueComparisonIfIsNotDefined(
                dailyRevenue: annualRevenuesPerDay[leftCursorIndex],
                comparisonDailyRevenue: ref highestDailyRevenue,
                ignoreDaysWithoutRevenue: IGNORE_DAYS_WITHOUT_REVENUE);

            if (lowestDailyRevenue > annualRevenuesPerDay[leftCursorIndex] && (annualRevenuesPerDay[leftCursorIndex] != 0 && IGNORE_DAYS_WITHOUT_REVENUE))
                lowestDailyRevenue = annualRevenuesPerDay[leftCursorIndex];
            if (highestDailyRevenue < annualRevenuesPerDay[leftCursorIndex] && (annualRevenuesPerDay[leftCursorIndex] != 0 && IGNORE_DAYS_WITHOUT_REVENUE))
                highestDailyRevenue = annualRevenuesPerDay[leftCursorIndex];

            if (annualRevenuesPerDay[rightCursorIndex] == 0 && (annualRevenuesPerDay[rightCursorIndex] == 0 && IGNORE_DAYS_WITHOUT_REVENUE))
                numberOfTheDaysWithoutRevenue++;

            totalAnnualRevenue += annualRevenuesPerDay[leftCursorIndex];

            if (differenceBetweenCursor > 1)
            {
                DefineDailyRevenueComparisonIfIsNotDefined(
                    dailyRevenue: annualRevenuesPerDay[rightCursorIndex],
                    comparisonDailyRevenue: ref lowestDailyRevenue,
                    ignoreDaysWithoutRevenue: IGNORE_DAYS_WITHOUT_REVENUE);
                DefineDailyRevenueComparisonIfIsNotDefined(
                    dailyRevenue: annualRevenuesPerDay[rightCursorIndex],
                    comparisonDailyRevenue: ref highestDailyRevenue,
                    ignoreDaysWithoutRevenue: IGNORE_DAYS_WITHOUT_REVENUE);

                if (lowestDailyRevenue > annualRevenuesPerDay[rightCursorIndex] && (annualRevenuesPerDay[rightCursorIndex] != 0 && IGNORE_DAYS_WITHOUT_REVENUE))
                    lowestDailyRevenue = annualRevenuesPerDay[rightCursorIndex];
                if (highestDailyRevenue < annualRevenuesPerDay[rightCursorIndex] && (annualRevenuesPerDay[rightCursorIndex] != 0 && IGNORE_DAYS_WITHOUT_REVENUE))
                    highestDailyRevenue = annualRevenuesPerDay[rightCursorIndex];

                if (annualRevenuesPerDay[leftCursorIndex] == 0 && (annualRevenuesPerDay[leftCursorIndex] == 0 && IGNORE_DAYS_WITHOUT_REVENUE))
                    numberOfTheDaysWithoutRevenue++;

                totalAnnualRevenue += annualRevenuesPerDay[rightCursorIndex];
            }
        }

        decimal annualRevenueAverage = totalAnnualRevenue / (annualRevenuesPerDayArrayLength - numberOfTheDaysWithoutRevenue);

        int numberOfDaysThatTheRevenueIsGreaterThanTheAverage = 0;

        for (int i = 0; i < annualRevenuesPerDayArrayLength; i++)
        {
            var leftCursorIndex = i;
            var rightCursorIndex = annualRevenuesPerDayArrayLength - i - 1;
            var differenceBetweenCursor = rightCursorIndex - leftCursorIndex;

            if ((leftCursorIndex + rightCursorIndex + 1) == annualRevenuesPerDayArrayLength && (differenceBetweenCursor) == 0)
                break;

            if (annualRevenuesPerDay[leftCursorIndex] > annualRevenueAverage)
                numberOfDaysThatTheRevenueIsGreaterThanTheAverage++;

            if (annualRevenuesPerDay[rightCursorIndex] > annualRevenueAverage && differenceBetweenCursor > 1)
                numberOfDaysThatTheRevenueIsGreaterThanTheAverage++;
        }

        return RevenueStatisticsModel.Factory(
            lowestAnnualRevenue: lowestDailyRevenue ?? 0,
            highestAnnualRevenue: highestDailyRevenue ?? 0,
            numberOfDaysGreaterThanTheAnnualAverage: numberOfDaysThatTheRevenueIsGreaterThanTheAverage);
    }

    /// <summary>
    /// Definir um Valor de Comparação para o Menor ou Maior Faturamento em um dia do Ano, caso não há.
    /// </summary>
    /// <param name="dailyRevenue">Faturamento que ocorreu no dia.</param>
    /// <param name="comparisonDailyRevenue">Referência a Variável que Guarda o Atual Dia que Menos Houve Faturamento</param>
    /// <param name="ignoreDaysWithoutRevenue">Indica se irá ignorar dias que houve faturamento ou não.</param>
    public static void DefineDailyRevenueComparisonIfIsNotDefined(
        decimal dailyRevenue,
        ref decimal? comparisonDailyRevenue,
        bool ignoreDaysWithoutRevenue)
    {
        if (comparisonDailyRevenue is null && (dailyRevenue != 0 && ignoreDaysWithoutRevenue == true))
            comparisonDailyRevenue = dailyRevenue;

        if (comparisonDailyRevenue is null && ignoreDaysWithoutRevenue == false)
            comparisonDailyRevenue = dailyRevenue;
    }
}