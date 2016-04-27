using System;
using System.Data;
using System.Threading;

namespace Manager.Kohonen
{
    public class KohonenCore<V> where V:IVector<V>, new() 
    {
        private readonly V[,] _arr;
        private double _radius;
        private double _learningRate;
        private double _distFactor;
        private readonly int _rows;
        private readonly int _cols;
        private double _aHN;// 0 - 1
        private double _rAHN;

        public KohonenCore(int rows, int cols, double radius, double learningRate, double distFactor, double aHN, double rAHN)
        {
            _arr = new V[rows,cols];
            _radius = radius;
            _learningRate = learningRate;
            _distFactor = distFactor;
            _rows = rows;
            _cols = cols;
            _aHN = aHN;
            _rAHN = rAHN;


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _arr[i,j] = new V();
                }
            }
        }

        public int[] Winner(V input)
        {
            int [] result = {0,0};
            double minDiff = input.Difference(_arr[0, 0]);


            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    var temp = input.Difference(_arr[i,j]);
                    if (temp < minDiff)
                    {
                        minDiff = temp;
                        result[0] = i;
                        result[1] = j;
                    }
                }
            }


            return result;
        }

        public void ReArrange(int row, int col, V value)
        {
          //  Console.WriteLine(_radius);
         //   _radius *= 0.99995;
            int offset = (int) Math.Ceiling(_radius - 0.5);
            double temp;
            double radSqrt = Math.Sqrt(_radius);
            double midr = row+0.5, midc=col+0.5;

            int start = (int)Math.Floor(col + 0.5 - _radius);
            int end = (int)Math.Ceiling(col + 0.5 + _radius);
            
       //     Console.WriteLine("main c"+(col-offset)+" "+(col+offset));
            for (int i = start; i <= end; i++)
            {
               
                if(IsWithinTable(row,i))UpdateVector(row, i, row, col, value);
            }
            
         /*   for (int i = row - offset; i <= row + offset; i++)
            {
                if (IsWithinTable(row, i)) UpdateVector(i, col, row, col, value);
            }*/

            

            for (int i = 1; i < (int)Math.Ceiling( 0.5 + _radius); i++)
            {
                temp = Math.Sqrt(Math.Pow(_radius,2) - Math.Pow(i - 0.5, 2));
           //     Console.WriteLine(temp);
                start = (int) Math.Floor(col + 0.5 - temp);
                end = (int)Math.Ceiling(col + 0.5 + temp);
           //     Console.WriteLine(start+" "+end);
                ProcessRow(start,end,row+i,row,col,value);
                ProcessRow(start, end, row - i, row, col, value);
            }

          
        }

        public Boolean IsWithinTable(int row, int col)
        {
            return row>=0 && col>=0 && row < _rows && col < _cols;
        }

        private void UpdateVector(int i, int j, int row, int col, V value)
        {
            _arr[i,j] = _arr[i,j].Add(value.Diff(_arr[i,j]).Multiply(_learningRate).Multiply(NFunction(new int[]{i,j},new int[]{ row,col})));
        }

        private double NFunction(int [] dim1, int [] dim2)
        {
            //  Console.WriteLine("dist "+ Dist(dim1, dim2)+" radius "+_radius);
            //double r = _aHN*Math.Pow(Math.E, Math.Pow((Dist(dim1,dim2))/_radius,2));
            double r = 1/ Math.Pow(Math.E, Math.Pow((Dist(dim1, dim2)) / _radius, 2));
            if (r > 1)
            {
                return 1;
            }
            else
            {
                return r;
            }
        }

        private double Dist(int[] dim1, int[] dim2)
        {
            return Math.Sqrt(Math.Pow(dim1[0] - dim2[0], 2) + Math.Pow(dim1[1] - dim2[1], 2));
        }

        private void ProcessRow(int start, int end, int rowToProcess, int row, int col, V value )
        {
            for (int i = start; i <= end; i++)
            {
                //TODO recheck and remove Dist
                if (IsWithinTable(rowToProcess, i) /*&& _radius>=Dist(new [] {i,rowToProcess},new[] {row,col})*/) UpdateVector(rowToProcess, i, row, col, value);
            }
        }

        public V this[int i, int j]
        {
            get { return _arr[i, j]; }
        }

        public void Displ()
        {

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    _arr[i,j].Print();
                }
                Console.WriteLine();
                
            }
            Console.WriteLine(_radius);
        }

        //////////////////////////////////////////////////////////////////////////////

        public V getApproximatedState(V input)
        {
            int[] winnerCoords = Winner(input);
            return _arr[winnerCoords[0], winnerCoords[1]];
        }




    }
}
