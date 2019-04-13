using Independence;
using System;

namespace DreamLifter.FR
{
    public sealed class MatrixProvider
    {
        private DoubleDenseMatrix _differentiate;
        private DoubleDenseMatrix _cfLeft, _cfRight;
        private DoubleDenseMatrix _coordinates;
        private readonly int _polynomialOrder;

        /// <summary>
        /// Return the coordinates of internal nodes.
        /// </summary>
        public DoubleDenseMatrix Coord => _coordinates;

        public MatrixProvider(int polynomialOrder)
        {
            if (polynomialOrder < 1 || polynomialOrder > 4) throw new NotSupportedException(nameof(polynomialOrder));
            _polynomialOrder = polynomialOrder;
            if (polynomialOrder == 1)
            {
                _coordinates = new DoubleDenseMatrix(polynomialOrder + 1, 1
                    , new double[] { -1, 1 });
            }
            else if (polynomialOrder == 2)
            {
                _coordinates = new DoubleDenseMatrix(polynomialOrder + 1, 1
                    , new double[] { -1, 0, 1 });
            }
            else if (polynomialOrder == 3)
            {
                _coordinates = new DoubleDenseMatrix(polynomialOrder + 1, 1
                    , new double[] { -1, -0.447213595499958, 0.447213595499958, 1 });
            }
            else if (polynomialOrder == 4)
            {
                _coordinates = new DoubleDenseMatrix(polynomialOrder + 1, 1
                    , new double[] { -1, -0.654653670707977, 0, 0.654653670707977, 1 });
            }
        }

        public DoubleDenseMatrix GetLocalAdvectionMatrix()
        {
            if (_differentiate == null)
            {
                _differentiate = CreateDifferentiationMatrix();
            }
            return _differentiate;
        }

        private DoubleDenseMatrix CreateDifferentiationMatrix()
        {
            var Dx = Polynomials.DiffLagrange(_coordinates);
            return Dx;
        }

        public DoubleDenseMatrix GetRightCorrectionFunc()
        {
            if (_cfRight == null)
            {
                _cfRight = new DoubleDenseMatrix(_polynomialOrder + 1, 1);
                for (var i = 0; i < _polynomialOrder + 1; i++)
                {
                    var deriv = Polynomials.Derivation(Polynomials.LeftRadau, _polynomialOrder + 1, _coordinates[i, 0]);
                    _cfRight[i, 0] = deriv;
                }
            }
            return _cfRight;
        }

        public DoubleDenseMatrix GetLeftCorrectionFunc()
        {
            if (_cfLeft == null)
            {
                _cfLeft = new DoubleDenseMatrix(_polynomialOrder + 1, 1);
                GetRightCorrectionFunc();
                for (var i = 0; i < _polynomialOrder + 1; i++)
                {
                    _cfLeft[_polynomialOrder - i, 0] = -(_cfRight[i, 0]);
                }
            }
            return _cfLeft;
        }
    }
}
