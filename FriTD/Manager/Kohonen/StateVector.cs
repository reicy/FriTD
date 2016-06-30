using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.Kohonen
{
    public class StateVector:IVector<StateVector>
    {
        public static int Size = 8;
        private readonly double[] _vector;
        private static Random rnd = new Random();


        public StateVector()
        {
            _vector = new double[Size];
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = rnd.NextDouble();
            }
        }

        public StateVector(double[] _vector, double factor)
        {
            this._vector = _vector;
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] *= factor;
            }
        }

        public StateVector(double[] _vector)
        {
            this._vector = _vector;
        }

        public double Difference(StateVector vector)
        {
            double temp = 0;

            for (int i = 0; i < _vector.Length; i++)
            {
                temp += Math.Pow(vector[i] - this[i], 2); //*_priorita[i];
            }
            return Math.Sqrt(temp);
        }

        public StateVector Diff(StateVector vector)
        {
            double[] second = vector.Vector;
            double[] result = new double[Size];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = _vector[i] - second[i];
            }

            return new StateVector(result);

        }

        public StateVector Add(StateVector vector)
        {
          /*  Console.WriteLine("add");
            this.Print();
            Console.Write("   ");
            vector.Print();
            Console.WriteLine();*/
            double[] second = vector.Vector;
            double[] result = new double[Size];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = Math.Abs(_vector[i] + second[i]);
            }

            return new StateVector(result);
        }

      
        public StateVector Multiply(Double factor)
        {
            double[] result = new double[Size];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = _vector[i]*factor;
            }
            return new StateVector(result);

        }

        public void Print()
        {
            for (int i = 0; i < _vector.Length; i++)
            {
                Console.Write(String.Format("{0:0.00} ",_vector[i]));
            }
            Console.Write(" ---|");
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

      
    }
}
