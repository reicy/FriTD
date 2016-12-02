using System;
using System.Text;

namespace Manager.Kohonen
{
    public class StateVector : IVector<StateVector>
    {
        public static int Size = 20;
        //private static Random _rnd = new Random();
        private readonly double[] _vector;
        private readonly int _count;

        public StateVector()
        {
            _vector = new double[Size];
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = 0; // _rnd.NextDouble();
            }
            _count = Size;
        }

        public void FromString(string str)
        {
            var temp = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = double.Parse(temp[i]); // _rnd.NextDouble();
            }
        }

        /*public StateVector(int size)
        {
            _vector = new double[size];
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = _rnd.NextDouble();
            }
            _count = size;
        }*/

        public StateVector(double[] vector, double factor)
        {
            _vector = vector;
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] *= factor;
            }
        }

        public StateVector(double[] vector)
        {
            _vector = vector;
            _count = vector.Length;
        }

        public double Difference(StateVector vector)
        {
            double temp = 0;

            for (int i = 0; i < _vector.Length; i++)
            {
                temp += Math.Pow(vector[i] - this[i], 2); // * _priorita[i];
            }
            return Math.Sqrt(temp);
        }

        public double Difference(StateVector vector, double[] weight)
        {
            double temp = 0;

            for (int i = 0; i < _vector.Length; i++)
            {
                temp += Math.Pow(vector[i] - this[i], 2) * weight[i]; // * _priorita[i];
            }
            return Math.Sqrt(temp);
        }

        public StateVector Diff(StateVector vector)
        {
            double[] second = vector.Vector;
            double[] result = new double[vector._count];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = _vector[i] - second[i];
            }

            return new StateVector(result);
        }

        public StateVector Add(StateVector vector)
        {
            /*Console.WriteLine(@"add");
            Print();
            Console.Write(@"   ");
            vector.Print();
            Console.WriteLine();*/
            double[] second = vector.Vector;
            double[] result = new double[vector._count];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = Math.Abs(_vector[i] + second[i]);
            }

            return new StateVector(result);
        }

        public StateVector Multiply(Double factor)
        {
            double[] result = new double[_count];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = _vector[i] * factor;
            }

            return new StateVector(result);
        }

        public double DifferenceCosine(StateVector vector)
        {
            return 1.0 - Dot(vector) / (Norm() * vector.Norm());
        }

        public double Dot(StateVector vector)
        {
            double result = 0.0;
            for (int i = 0; i < _vector.Length; ++i)
                result += _vector[i] * vector._vector[i];
            return result;
        }

        public double Norm()
        {
            double result = 0.0;
            for (int i = 0; i < _vector.Length; ++i)
                result += _vector[i] * _vector[i];
            result = Math.Sqrt(result);
            return result;
        }

        public void Print()
        {
            for (int i = 0; i < _vector.Length; i++)
            {
                Console.Write(@"{0:0.000} ", _vector[i]);
            }
            Console.Write(@" ---|");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < _vector.Length; i++)
            {
                sb.Append($"{_vector[i]:0.000} ");
            }
            sb.Append(" ---|");

            return sb.ToString();
        }

        public double this[int index]
        {
            get { return _vector[index]; }
            set { _vector[index] = value; }
        }

        public double[] Vector
        {
            get { return _vector; }
        }

        public bool IsEmpty()
        {
            foreach (var d in _vector)
            {
                if (d != 0) return false;
            }
            return true;
        }

        public void CheckForNorm()
        {
            for (int i = 0; i < _vector.Length; i++)
            {
                if (_vector[i] > 1)
                {
                    Console.WriteLine(@"Bad normalization error !");
                    Print();
                    Console.WriteLine();
                }
            }
        }
    }
}
