using Independence;
using System;
using System.Linq;

namespace DreamLifter.Executable
{
    internal static class MainEngine
    {
        static void Main()
        {
            const int polyOrder = 2;
            const int numberOfElements = 100;
            var adv1d = new FR.Tests.Advection1D(new FR.MatrixProvider(polyOrder), numberOfElements, polyOrder + 1);
            var rho = new DoubleDenseMatrix(polyOrder + 1, numberOfElements);
            {
                for (var k = 0; k < numberOfElements; k++)
                {
                    for (var i = 0; i < polyOrder + 1; i++)
                    {
                        var x = k * 2.0 + i - 1.0;
                        rho[i, k] = Math.Sin(0.2 * Math.PI * x);
                    }
                }
            }
            foreach (var it in System.Linq.Enumerable.Range(0, 1000))
            {
                rho = RK4(adv1d.EvaluateRHS, rho, 1.0e-1);
                Console.WriteLine($"MAX: {rho.ToArray().Max().ToString("0.00")}, MIN: {rho.ToArray().Min().ToString("0.00")}");
            }
        }

        static DoubleDenseMatrix RK4(Func<DoubleDenseMatrix, DoubleDenseMatrix> func, DoubleDenseMatrix initial, double dt)
        {
            var k1 = func(initial) * dt;
            var k2 = func(initial + 0.5 * k1) * dt;
            var k3 = func(initial + 0.5 * k2) * dt;
            var k4 = func(initial + k3) * dt;
            var delta = (k1 + 2.0 * (k2 + k3) + k4) / 6.0;
            return initial + delta;
        }
    }
}
