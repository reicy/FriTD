using System;

namespace Manager.Kohonen
{
    public class RgbVector : IVector<RgbVector>
    {

        private readonly int[] _rgb;
        private static readonly Random _rnd = new Random();
        //private readonly int[] _priorita = { 10000, 100, 1 };

        public RgbVector(int r, int g, int b)
        {
            _rgb = new[] { r, g, b };
        }

        public RgbVector()
        {
            _rgb = new[] { _rnd.Next(255), _rnd.Next(255), _rnd.Next(255) };
        }

        public double Difference(RgbVector vector)
        {
            double temp = 0;

            for (int i = 0; i < _rgb.Length; i++)
            {
                temp += Math.Pow(vector[i] - this[i], 2); // * _priorita[i];
            }
            return Math.Sqrt(temp);
        }

        public double Difference(RgbVector vector, double[] weight)
        {
            throw new NotImplementedException();
        }

        public double DifferenceCosine(RgbVector vector)
        {
            return 1.0 - Dot(vector) / (Norm() * vector.Norm());
        }

        public RgbVector Diff(RgbVector vector)
        {
            //Console.WriteLine(@"{0}  {1}  {2}", vector[0], vector[1], vector[2]);
            return new RgbVector(_rgb[0] - vector[0], _rgb[1] - vector[1], _rgb[2] - vector[2]);
        }

        public RgbVector Add(RgbVector vector)
        {
            return new RgbVector(Math.Abs(_rgb[0] + vector[0]), Math.Abs(_rgb[1] + vector[1]), Math.Abs(_rgb[2] + vector[2]));
        }

        public RgbVector Multiply(double factor)
        {
            //Console.WriteLine(@"multiplied by {0}", factor);
            return new RgbVector((int)(_rgb[0] * factor), (int)(_rgb[1] * factor), (int)(_rgb[2] * factor));
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
            get { return _rgb[index]; }
            set { _rgb[index] = value; }
        }

        public int[] Rgb
        {
            get { return _rgb; }
        }

        public void Print()
        {
            Console.Write(@"({0}  {1}  {2})", _rgb[0], _rgb[1], _rgb[2]);
        }

        public void FromString(string str)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }
    }
}
