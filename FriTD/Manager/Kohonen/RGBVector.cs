using Manager.QLearning;
using System;

namespace Manager.Kohonen
{
    public class RgbVector : IVector<RgbVector> 
    {

        private readonly int[] _rgb;
        private static Random rnd = new Random();
      //  private readonly int[] _priorita = new[] {10000, 100, 1};

        public RgbVector(int r, int g, int b)
        {
            _rgb = new int[3];
            _rgb[0] = r;
            _rgb[1] = g;
            _rgb[2] = b;
        }

        public RgbVector()
        {
            _rgb = new int[3];
            _rgb[0] = /*0;//*/rnd.Next(255);
            _rgb[1] = rnd.Next(255);
            _rgb[2] = rnd.Next(255);
        }

        public double Difference(RgbVector vector)
        {
            double temp = 0;

            for (int i = 0; i < _rgb.Length; i++)
            {
                temp += Math.Pow(vector[i] - this[i], 2); //*_priorita[i];
            }
            return Math.Sqrt(temp);
        }

        public double Difference(RgbVector vector, double[] weight)
        {
            throw new NotImplementedException();
        }

        public double DifferenceCosine(RgbVector vector)
        {
            return 1.0 - this.Dot(vector) / (this.Norm() * vector.Norm());
        }

        public RgbVector Diff(RgbVector vector)
        {
            int[] second = vector.Rgb;
        //    Console.WriteLine(vector[0]+"  "+vector[0]+"  "+vector[1]);
            return new RgbVector((_rgb[0]-second[0]), (_rgb[1] - second[1]) , (_rgb[2] - second[2]) );
        }

        public RgbVector Add(RgbVector vector)
        {
            int[] second = vector.Rgb;
            return new RgbVector(Math.Abs(_rgb[0] + second[0]), Math.Abs(_rgb[1] + second[1]), Math.Abs(_rgb[2] + second[2]));

        }

        public RgbVector Multiply(Double factor)
        {
            //Console.WriteLine("multiplied by "+factor);
            return new RgbVector((int)(_rgb[0]*factor), (int)(_rgb[1]*factor), (int)(_rgb[2]*factor));

        }

        public double Dot(RgbVector vector)
        {
            double result = 0.0;
            for (int i = 0; i < _rgb.Length; ++i)
                result += _rgb[i] * vector._rgb[i];
            return result;
        }

        public double Norm()
        {
            double result = 0.0;
            for (int i = 0; i < _rgb.Length; ++i)
                result += _rgb[i] * _rgb[i];
            return result;
        }

        public int this[int index]
        {
            get { return _rgb[index];  }
            set { _rgb[index] = value; }
        }

        public int[] Rgb
        {
            get { return _rgb; }
        }

        public void Print()
        {
            Console.Write("("+_rgb[0] + "  " + _rgb[1] + "  " + _rgb[2]+")");
        }

        public void Load(string v)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }
    }
}
