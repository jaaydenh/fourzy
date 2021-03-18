using System.Collections.Generic;

public static class Extensions
{
    public static int StdDev(this IEnumerable<double> values)
    {
        // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/
        double mean = 0.0;
        double sum = 0.0;
        double stdDev = 0.0;
        int n = 0;
        foreach (double val in values)
        {
            n++;
            double delta = val - mean;
            mean += delta / n;
            sum += delta * (val - mean);
        }
        if (1 < n)
            stdDev = System.Math.Sqrt(sum / (n - 1));

        return (int)stdDev;
    }

}