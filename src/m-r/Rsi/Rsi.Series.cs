namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (SERIES)
public static partial class Indicator
{
    internal static IEnumerable<RsiResult> CalcRsi<T>(
        this IEnumerable<T> tpList,
        Func<T, DateTime> dateSelector,
        Func<T, double> valueSelector,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateRsi(lookbackPeriods);

        // initialize
        double avgGain = 0;
        double avgLoss = 0;

        double lastValue = 0;
        bool isFirst = true;
        double alpha = 1.0d / lookbackPeriods;

        // roll through quotes
        int i = 0;

        foreach (T obj in tpList)
        {
            DateTime date = dateSelector(obj);
            double value = valueSelector(obj);
            RsiResult r = new(date);

            if (isFirst)
            {
                lastValue = value;
                isFirst = false;
                yield return r;
                continue;
            }

            double gain = (value > lastValue) ? (value - lastValue) : 0;
            double loss = (value < lastValue) ? (lastValue - value) : 0;

            i++;
            if (i <= lookbackPeriods)
            {
                avgGain += gain / lookbackPeriods;
                avgLoss += loss / lookbackPeriods;
            }
            else if (i > lookbackPeriods)
            {
                avgGain = (alpha * gain) + ((1 - alpha) * avgGain);
                avgLoss = (alpha * loss) + ((1 - alpha) * avgLoss);
            }

            if (i >= lookbackPeriods)
            {
                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }
            }

            lastValue = value;

            yield return r;
        }
    }

    internal static IEnumerable<RsiResult> CalcRsi(
        this IEnumerable<(DateTime Date, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateRsi(lookbackPeriods);

        // initialize
        double avgGain = 0;
        double avgLoss = 0;

        double lastValue = 0;
        bool isFirst = true;
        double alpha = 1.0d / lookbackPeriods;

        // roll through quotes
        int i = 0;

        foreach ((DateTime date, double value) in tpList)
        {
            RsiResult r = new(date);

            if (isFirst)
            {
                lastValue = value;
                isFirst = false;
                yield return r;
                continue;
            }

            double gain = (value > lastValue) ? (value - lastValue) : 0;
            double loss = (value < lastValue) ? (lastValue - value) : 0;

            i++;
            if (i <= lookbackPeriods)
            {
                avgGain += gain / lookbackPeriods;
                avgLoss += loss / lookbackPeriods;
            }
            else if (i > lookbackPeriods)
            {
                avgGain = (alpha * gain) + ((1 - alpha) * avgGain);
                avgLoss = (alpha * loss) + ((1 - alpha) * avgLoss);
            }

            if (i >= lookbackPeriods)
            {
                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }
            }

            lastValue = value;

            yield return r;
        }
    }

    internal static List<RsiResult> CalcRsiOld(
        this List<(DateTime Date, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateRsi(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        double avgGain = 0;
        double avgLoss = 0;

        List<RsiResult> results = new(length);
        double[] gain = new double[length]; // gain
        double[] loss = new double[length]; // loss
        double lastValue;

        if (length == 0)
        {
            return results;
        }
        else
        {
            lastValue = tpList[0].Value;
        }

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            RsiResult r = new(date);
            results.Add(r);

            gain[i] = (value > lastValue) ? value - lastValue : 0;
            loss[i] = (value < lastValue) ? lastValue - value : 0;
            lastValue = value;

            // calculate RSI
            if (i > lookbackPeriods)
            {
                avgGain = ((avgGain * (lookbackPeriods - 1)) + gain[i]) / lookbackPeriods;
                avgLoss = ((avgLoss * (lookbackPeriods - 1)) + loss[i]) / lookbackPeriods;

                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }
            }

            // initialize average gain
            else if (i == lookbackPeriods)
            {
                double sumGain = 0;
                double sumLoss = 0;

                for (int p = 1; p <= lookbackPeriods; p++)
                {
                    sumGain += gain[p];
                    sumLoss += loss[p];
                }

                avgGain = sumGain / lookbackPeriods;
                avgLoss = sumLoss / lookbackPeriods;

                r.Rsi = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateRsi(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for RSI.");
        }
    }
}