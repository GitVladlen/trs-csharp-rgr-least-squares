using System;
using System.Threading;
using System.Diagnostics;

namespace rgr
{
    class Program
    {
        /// <summary>
        /// Count of digits after comma in double numbers,
        /// using for rounding approximation results in Math.Round()
        /// </summary>
        private static int r_count = 3;
        /// <summary>
        /// Calculation the coefficients a1, a0 for linear approximation F(x) = a1*x + a0
        /// of method of least squeres
        /// </summary>
        /// <param name="x">function arguments</param>
        /// <param name="y">function values</param>
        /// <returns>array of 2 coefficients in order a1, a0</returns>
        public static double[] linear_approximation_params(double[] x, double[] y)
        {
            int n = x.Length;

            double sum_x = 0, sum_x_x = 0, sum_y = 0, sum_x_y = 0;
            for(int i = 0; i < n; i++)
            {
                sum_x += x[i];
                sum_y += y[i];
                sum_x_x += x[i] * x[i];
                sum_x_y += x[i] * y[i];
            }

            double a1 = (n*sum_x_y - sum_x*sum_y) / (n*sum_x_x - Math.Pow(sum_x, 2));
            double a0 = (sum_y*sum_x_x - sum_x_y*sum_x) / (n * sum_x_x - Math.Pow(sum_x, 2));

            return new double[] { a1, a0 };
        }
        /// <summary>
        /// Logarithmic approximation method of least squares
        /// for table-specified function
        /// </summary>
        /// <param name="x">array of function arguments</param>
        /// <param name="y">array of function values</param>
        /// <param name="x0">function argument for which need to approximate value</param>
        public static void logarithmic_approximation(double[] x, double[] y, double x0)
        {
            double[] log_x = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                log_x[i] = Math.Log(x[i]);
            }
            
            double[] coefficients = linear_approximation_params(log_x, y);

            // F(x) = a*log(x) + b
            double a = coefficients[0];
            double b = coefficients[1];
            double result = a * Math.Log(x0) + b;

            string msg = String.Format("F({0}) = {1}*log({0}) + {2} = {3}",
                x0, Math.Round(a, r_count), Math.Round(b, r_count), Math.Round(result, r_count));
            Console.WriteLine(msg);
        }
        /// <summary>
        /// Power approximation method of least squares
        /// for table-specified function
        /// </summary>
        /// <param name="x">array of function arguments</param>
        /// <param name="y">array of function values</param>
        /// <param name="x0">function argument for which need to approximate value</param>
        public static void power_approximation(double[] x, double[] y, double x0)
        {
            double[] log_x = new double[x.Length];
            double[] log_y = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                log_x[i] = Math.Log(x[i]);
                log_y[i] = Math.Log(y[i]);
            }

            double[] coefficients = linear_approximation_params(log_x, log_y);

            // F(x) = a * x**b
            double a = Math.Exp(coefficients[1]);
            double b = coefficients[0];
            double result = a * Math.Pow(x0, b);

            string msg = String.Format("F({0}) = {1} * {0}**{2} = {3}",
                x0, Math.Round(a, r_count), Math.Round(b, r_count), Math.Round(result, r_count));
            Console.WriteLine(msg);
        }
        /// <summary>
        /// Runs context with run time measurement
        /// </summary>
        /// <param name="msg">Message before running context</param>
        /// <param name="context">function (delegate) without return values and arguments</param>
        public static void stopwatch_context(String msg, Action context)
        {
            Stopwatch StopWatch = new Stopwatch();

            Console.WriteLine(msg);
            StopWatch.Start();

            context();

            StopWatch.Stop();
            Console.WriteLine(string.Concat("\nRunTime ", StopWatch.Elapsed.Milliseconds.ToString(), " ms\n"));
        }
        /// <summary>
        /// Entire point to the program
        /// </summary>
        /// <param name="args">shell arguments</param>
        static void Main(string[] args)
        {
            double[] x = { 1, 2, 3, 4, 5 };
            double x0 = 6;

            //test for logarithmic approximation
            double[] y1 = { 0.0, 0.69, 1.1, 1.4, 1.6 };

            //test for power approximation
            double[] y2 = { 0.6, 1.8, 5.5, 9.9, 18.2 };

            stopwatch_context(
                "Calculating without threads\n",
                () => {
                    logarithmic_approximation(x, y1, x0);
                    power_approximation(x, y2, x0);
                });

            stopwatch_context(
                "Calculating with threads\n",
                () => {
                    Thread t1 = new Thread(() => logarithmic_approximation(x, y1, x0));
                    Thread t2 = new Thread(() => power_approximation(x, y2, x0));

                    t1.Start();
                    t2.Start();

                    t1.Join();
                    t2.Join();
                });

            Console.ReadLine();
        }
    }
}
