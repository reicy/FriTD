using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;

namespace Manager.Kohonen
{
    public class KohonenCore<V> where V : IVector<V>, new()
    {
        private readonly V[,] _arr;
        private readonly int[,] _accesses;
        private double _radius;
        private double _learningRate;
        private double[] _weight;
        private double _distFactor;
        private readonly int _rows;
        private readonly int _cols;
        private double _aHN; // 0 - 1
        private double _rAHN;
        private bool _nonEmptyModeActive;
        private SortCandidatesHeuristicG _candidatesHeuristic = new SortCandidatesHeuristicG();

        public delegate double DistanceDelegate(V v1, V v2);

        // euclidean distance between two vectors
        public double DistEuclidean(V v1, V v2)
        {


            if (_weight == null)
            {
                return v1.Difference(v2);
            }
            else
            {
                return v1.Difference(v2, _weight);
            }
        }

        // cosine distance between two vectors
        public double DistCosine(V v1, V v2)
        {
            return v1.DifferenceCosine(v2);
        }

        private class SortCandidatesHeuristic : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                KeyValuePair<double, int[]> c1 = (KeyValuePair<double, int[]>)a;
                KeyValuePair<double, int[]> c2 = (KeyValuePair<double, int[]>)a;

                if (c1.Key > c2.Key)
                    return 1;
                if (c1.Key < c2.Key)
                    return -1;
                else
                    return 0;
            }
        }

        private class SortCandidatesHeuristicG : IComparer<KeyValuePair<double, int[]>>
        {
            public int Compare(KeyValuePair<double, int[]> c1, KeyValuePair<double, int[]> c2)
            {
                if (c1.Key > c2.Key)
                    return 1;
                if (c1.Key < c2.Key)
                    return -1;
                else
                    return 0;
            }
        }

        public KohonenCore(int rows, int cols, double radius, double learningRate, double distFactor, double aHN,
            double rAHN, Boolean nonEmptyModeActive)
        {
            _arr = new V[rows, cols];
            _radius = radius;
            _learningRate = learningRate;
            _distFactor = distFactor;
            _rows = rows;
            _cols = cols;
            _aHN = aHN;
            _rAHN = rAHN;
            _accesses = new int[rows, cols];
            _nonEmptyModeActive = nonEmptyModeActive;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _arr[i, j] = new V();
                }
            }
        }

        public int[] Winner(V input)
        {
            int[] result = { 0, 0 };
            double minDiff = input.Difference(_arr[0, 0]);


            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    if (_nonEmptyModeActive) if ((i > 0 || j > 0) && _arr[i, j].IsEmpty()) continue;
                    double temp;
                    if (_weight == null)
                    {
                        temp = input.Difference(_arr[i, j]);
                    }
                    else
                    {
                        temp = input.Difference(_arr[i, j], _weight);
                    }


                    if (temp < minDiff)
                    {
                        minDiff = temp;
                        result[0] = i;
                        result[1] = j;
                    }
                }
            }

            _accesses[result[0], result[1]]++;
            return result;
        }

        /// <param name="percentage">Percentage in interval [0.0, 1.0]</param>
        internal void DecreaseLRBy(double percentage)
        {
            _learningRate -= percentage * _learningRate;
        }

        internal int[] Winner(V input, DistanceDelegate distFun, bool weightenedFunction)
        {
            int[] result = { 0, 0 };
            double minDiff = distFun(input, _arr[0, 0]);


            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    if (_nonEmptyModeActive) if ((i > 0 || j > 0) && _arr[i, j].IsEmpty()) continue;
                    double temp;
                    if (_weight == null)
                    {
                        temp = distFun(input, _arr[i, j]);
                    }
                    else
                    {
                        if (weightenedFunction)
                        {
                            temp = input.Difference(_arr[i, j], _weight);
                        }
                        else
                        {
                            temp = distFun(input, _arr[i, j]);
                        }
                    }


                    if (temp < minDiff)
                    {
                        minDiff = temp;
                        result[0] = i;
                        result[1] = j;
                    }
                }
            }

            _accesses[result[0], result[1]]++;
            return result;
        }

        public int[] WinnerHeuristic(V input, DistanceDelegate distFun)
        {
            int hsqrt = (int)Math.Sqrt(_rows); // height square root
            int wsqrt = (int)Math.Sqrt(_cols); // width square root
            int nrows = hsqrt; // <----------------------------- you can change heuristic attributes here
            int ncols = wsqrt; // <----------------------------- you can change heuristic attributes here
            int itemh = _rows / nrows;
            int itemw = _cols / ncols;
            List<KeyValuePair<double, int[]>> candidates = new List<KeyValuePair<double, int[]>>(nrows * ncols);
            for (int r = 0; r < nrows; ++r)
            {
                for (int c = 0; c < ncols; ++c)
                {
                    int rr = (int)Math.Round(0.5 + r) * itemh;
                    int cc = (int)Math.Round(0.5 + c) * itemw;
                    double dist = distFun(_arr[rr, cc], input);
                    candidates.Add(new KeyValuePair<double, int[]>(dist, new int[] { rr, cc }));
                }
            }
            candidates.Sort(_candidatesHeuristic);
            double bestDist = candidates[0].Key;
            int[] best = candidates[0].Value;
            int search = 5; // <----------------------------- you can change heuristic attributes here
            int search_candidates = 5; // <----------------------------- you can change heuristic attributes here
            Random rnd = new Random();
            while (true)
            {
                bool found = false;
                for (int i = 0; i < search; ++i)
                {
                    int[] coords = candidates[0].Value;
                    candidates.RemoveAt(0);
                    for (int j = 0; j < search_candidates; ++j)
                    {
                        int r = coords[0] + rnd.Next(-itemh / 2, itemh / 2);
                        int c = coords[1] + rnd.Next(-itemw / 2, itemw / 2);
                        if (0 <= r && r < _rows && 0 <= c && c < _cols)
                        {
                            V v = _arr[r, c];
                            double dist = distFun(v, input);
                            if (dist < bestDist)
                            {
                                bestDist = dist;
                                best[0] = r;
                                best[1] = c;
                                candidates.Add(new KeyValuePair<double, int[]>(dist, new int[] { r, c }));
                                candidates.Sort(_candidatesHeuristic);
                                found = true;
                            }
                        }
                    }
                }
                if (!found) break;
            }
            ++_accesses[best[0], best[1]];
            return best;
        }

        public void ResetAccesses()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    _accesses[i, j] = 1;
                }
            }
        }

        public void PrintAccesses()
        {
            Console.WriteLine();
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    Console.Write("{0,7:########}", _accesses[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void ReArrange(int row, int col, V value)
        {
            //  Console.WriteLine(_radius);
            //   _radius *= 0.99995;

            //Displ();

            int offset = (int)Math.Ceiling(_radius - 0.5);
            double temp;
            double radSqrt = Math.Sqrt(_radius);
            double midr = row + 0.5, midc = col + 0.5;

            int start = (int)Math.Floor(col + 0.5 - _radius);
            int end = (int)Math.Ceiling(col + 0.5 + _radius);

            //     Console.WriteLine("main c"+(col-offset)+" "+(col+offset));
            for (int i = start; i <= end; i++)
            {
                if (IsWithinTable(row, i)) UpdateVector(row, i, row, col, value);
            }

            /*   for (int i = row - offset; i <= row + offset; i++)
            {
                if (IsWithinTable(row, i)) UpdateVector(i, col, row, col, value);
            }*/


            for (int i = 1; i < (int)Math.Ceiling(0.5 + _radius); i++)
            {
                temp = Math.Sqrt(Math.Pow(_radius, 2) - Math.Pow(i - 0.5, 2));
                //     Console.WriteLine(temp);
                start = (int)Math.Floor(col + 0.5 - temp);
                end = (int)Math.Ceiling(col + 0.5 + temp);
                //     Console.WriteLine(start+" "+end);
                ProcessRow(start, end, row + i, row, col, value);
                ProcessRow(start, end, row - i, row, col, value);
            }

            // Displ();
        }

        internal void Displ(int v)
        {
            Console.WriteLine();
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    //TODO remove
                    Console.Write("{0,5:###}", (_arr[i, j] as StateVector)[v]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public Boolean IsWithinTable(int row, int col)
        {
            return row >= 0 && col >= 0 && row < _rows && col < _cols;
        }

        private void UpdateVector(int i, int j, int row, int col, V value)
        {
            /*  Console.Write("pred ");
            _arr[i,j].Print();
            Console.WriteLine();
            Console.WriteLine("lr: "+ _learningRate);
            Console.WriteLine("NF: "+ NFunction(new int[] { i, j }, new int[] { row, col }));*/
            _arr[i, j] =
                _arr[i, j].Add(
                    value.Diff(_arr[i, j])
                        .Multiply(_learningRate)
                        .Multiply(NFunction(new int[] { i, j }, new int[] { row, col })));
            /*  Console.Write("po ");
            _arr[i,j].Print();
            Console.WriteLine();*/
        }

        private double NFunction(int[] dim1, int[] dim2)
        {
            //  Console.WriteLine("dist "+ Dist(dim1, dim2)+" radius "+_radius);
            //double r = _aHN*Math.Pow(Math.E, Math.Pow((Dist(dim1,dim2))/_radius,2));
            double r = 1 / Math.Pow(Math.E, Math.Pow((Dist(dim1, dim2)) / _radius, 2));
            if (r > 1)
            {
                return 1;
            }
            else
            {
                return r;
            }
        }

        public void Save(string filename)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _cols; j++)
                {
                    //_arr[i, j].Print();
                    sb.Append($"{_arr[i, j]} ");
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append($"{_radius}{Environment.NewLine}");

            File.WriteAllText(filename, sb.ToString());
        }

        private double Dist(int[] dim1, int[] dim2)
        {
            return Math.Sqrt(Math.Pow(dim1[0] - dim2[0], 2) + Math.Pow(dim1[1] - dim2[1], 2));
        }

        private void ProcessRow(int start, int end, int rowToProcess, int row, int col, V value)
        {
            for (int i = start; i <= end; i++)
            {
                //TODO recheck and remove Dist
                if (IsWithinTable(rowToProcess, i) /*&& _radius>=Dist(new [] {i,rowToProcess},new[] {row,col})*/)
                    UpdateVector(rowToProcess, i, row, col, value);
            }
        }

        public V this[int i, int j]
        {
            get { return _arr[i, j]; }
        }

        public void Displ()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    _arr[i, j].Print();
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


        public void SetWeight(double[] weight)
        {
            _weight = weight;
        }

        public void Load(string location)
        {
            string text = System.IO.File.ReadAllText(location);
            using (var reader = new StringReader(text))
            {
                var rows = _arr.GetLength(0);
                var cols = _arr.GetLength(1);
                string[] separator = new string[] { "  ---|" };
                string line;

                for (int i = 0; i < rows; i++)
                {
                    line = reader.ReadLine();
                    var temp = line.Split(separator, StringSplitOptions.None);
                    for (int j = 0; j < cols; j++)
                    {
                        _arr[i, j].Load(temp[j]);
                    }
                }
            }
        }
    }
}
