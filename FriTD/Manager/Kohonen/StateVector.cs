using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Manager.Kohonen
{
    public class StateVector:IVector<StateVector>
    {
        public static int Size = 20;
        private readonly double[] _vector;
        private int Count { get; }
        private static Random rnd = new Random();


        public StateVector()
        {
            _vector = new double[Size];
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = 0; //rnd.NextDouble();
            }
            Count = Size;
        }

        public void Load(string template)
        {
            
            var temp = template.Split(' ');
            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = double.Parse(temp[i]); //rnd.NextDouble();
            }
            

        }

        /* public StateVector(int size)
         {
             _vector = new double[size];
             for (int i = 0; i < _vector.Length; i++)
             {
                 _vector[i] = rnd.NextDouble();
             }
             Count = size;

         }*/

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
            Count = _vector.Length;
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

        public double Difference(StateVector vector, double[] weight)
        {
            double temp = 0;

            for (int i = 0; i < _vector.Length; i++)
            {
                temp += Math.Pow(vector[i] - this[i], 2) * weight[i]; //*_priorita[i];
            }
            return Math.Sqrt(temp);
        }

        public double DifferenceCosine(StateVector vector)
        {
            return 1.0 - this.Dot(vector) / (this.Norm() * vector.Norm());
        }

        public StateVector Diff(StateVector vector)
        {
            double[] second = vector.Vector;
            double[] result = new double[vector.Count];
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
            double[] result = new double[vector.Count];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = Math.Abs(_vector[i] + second[i]);
            }

            return new StateVector(result);
        }

      
        public StateVector Multiply(Double factor)
        {
            double[] result = new double[Count];
            for (int i = 0; i < _vector.Length; i++)
            {
                result[i] = _vector[i]*factor;
            }
            return new StateVector(result);

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
            return result;
        }

        public void Print()
        {
            for (int i = 0; i < _vector.Length; i++)
            {
                Console.Write(String.Format("{0:0.000} ",_vector[i]));
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

        public Boolean IsEmpty()
        {
            foreach (var d in _vector)
            {
                if (d != 0) return false;
            }
            return true;
        }
    }
}
