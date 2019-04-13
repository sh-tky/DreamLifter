using Independence;
using Independence.Ex;
using System;

namespace DreamLifter.FR
{
    /// <summary>
    /// Publish polynomials used in matrix evaluation with FR.
    /// </summary>
    public static class Polynomials
    {
        private const double _eps = 1.0e-4;

        public static double Legendre(int k, double xi)
        {
            if (k == -1)
            {
                return 0;
            }
            else if (k == 0)
            {
                return 1;
            }
            else
            {
                return (2 * k - 1) / (double)k * xi * Legendre(k - 1, xi)
                    - (k - 1) / (double)k * Legendre(k - 2, xi);
            }
        }

        public static double Lobatto(int k, double xi)
        {
            return Legendre(k, xi) - Legendre(k - 2, xi);
        }

        public static double RightRadau(int k, double xi)
        {
            return 0.5 * (Legendre(k, xi) - Legendre(k - 1, xi)) * Math.Pow(-1, k);
        }

        public static double LeftRadau(int k, double xi)
        {
            return 0.5 * (Legendre(k, xi) + Legendre(k - 1, xi));
        }

        public static double Derivation(Func<int, double, double> tgt, int k, double xi)
        {
            return (tgt(k, xi + _eps) - tgt(k, xi - _eps)) / (2.0 * _eps);
        }

        public static DoubleDenseMatrix Lagrange(DoubleDenseMatrix points, double xi)
        {
            var l = new DoubleDenseMatrix(points.RowNum, 1);
            for (var i = 0; i < points.RowNum; i++)
            {
                l[i, 0] = 1.0;
                for (var j = 0; j < points.RowNum; j++)
                {
                    if (i != j)
                    {
                        l[i, 0] *= ((xi - points[j, 0]) / (points[i, 0] - points[j, 0]));
                    }
                }
            }
            return l;
        }

        public static DoubleDenseMatrix DiffLagrange(DoubleDenseMatrix points, double xi)
        {
            return (Lagrange(points, xi + _eps) - Lagrange(points, xi - _eps)) / (2.0 * _eps);
        }

        public static DoubleDenseMatrix DiffLagrange(DoubleDenseMatrix points)
        {
            var yield = new DoubleDenseMatrix(points.RowNum, points.RowNum);
            for (var i = 0; i < points.RowNum; i++)
            {
                var subset = DiffLagrange(points, points[i, 0]);
                yield.SubMatrix(subset.Transpose(), i, 0, i, points.RowNum - 1);
            }
            return yield;
        }
    }
}
