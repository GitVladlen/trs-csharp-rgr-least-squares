using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Determinant of 2x2 matrix
        /// </summary>
        /// <param name="matrix">
        /// 2x2 matrix
        /// </param>
        /// <returns>
        /// determinant value
        /// </returns>
        private static double det_2_2(double[][] matrix)
        {
            return matrix[0][0] * matrix[1][1] - matrix[1][0] * matrix[0][1];
        }
        /// <summary>
        /// Calculates roots of a system of linear equations
        /// with 2 unknowns using Kramer's rule
        /// </summary>
        /// <param name="matrix">
        /// martix in form (A|B) with size 2 rows x 3 cols
        /// where A is matrix of coefficients near unknowns,
        /// B is vector of right part of system
        /// </param>
        /// <returns>
        /// array with 2 roots of current system
        /// </returns>
        private static double[] kramer_2(double[][] matrix)
        {
            double[][] matrix_det = new double[2][]
            {
                new double[2] {matrix[0][0], matrix[0][1]},
                new double[2] {matrix[1][0], matrix[1][1]},
            };
            double det = det_2_2(matrix_det);

            double[][] matrix_det_1 = new double[2][]
            {
                new double[2] {matrix[0][2], matrix[0][1]},
                new double[2] {matrix[1][2], matrix[1][1]},
            };
            double det_1 = det_2_2(matrix_det_1);

            double[][] matrix_det_2 = new double[2][]
            {
                new double[2] {matrix[0][0], matrix[0][2]},
                new double[2] {matrix[1][0], matrix[1][2]},
            };
            double det_2 = det_2_2(matrix_det_2);

            double[] solutions = new double[2] {
                det_1 / det,
                det_2 / det
            };

            return solutions;
        }
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

            double[][] matrix = new double[2][]
            {
                new double[3] {sum_x_x, sum_x, sum_x_y},
                new double[3] {sum_x, n, sum_y},
            };

            return kramer_2(matrix);
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
        static void Main(string[] args)
        {
            double[] x = { 1, 2, 3, 4, 5 };
            double x0 = 6;

            //test for logarithmic approximation
            double[] y1 = { 0.0, 0.69, 1.1, 1.4, 1.6 };
            logarithmic_approximation(x, y1, x0);

            //test for power approximation
            double[] y2 = { 0.6, 1.8, 5.5, 9.9, 18.2 };
            power_approximation(x, y2, x0);

            Console.ReadLine();
        }
    }
}
