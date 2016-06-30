using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TD.Entities;

namespace TD.Core
{
    class MapBuilder
    {

        private char[,] _map;
        private PathSquare _spawn;
        private List<TowerPlace> _towerPlaces;
        public const int SQUARE_SIZE = 100;

        public void Build(string mapFile)
        {
            _towerPlaces = new List<TowerPlace>();
            int rows;
            using (var reader = new StringReader(mapFile))
            {
                rows = int.Parse(reader.ReadLine());
                string line;
                _map = new char[rows, rows];
                for (int i = 0; i < rows; i++)
                {
                    line = reader.ReadLine();
                    for (int j = 0; j < line.Length; j++)
                    {
                        _map[i, j] = line[j];
                    }
                }
            }

            for (int i = 0; i < _map.Length; i++)
            {
                if (_map[i / rows, i % rows] == 'S')
                {
                    _spawn = (new PathSquare() { X = i % rows, Y = i / rows });


                }
                if (_map[i / rows, i % rows] == 'T')
                {
                    _towerPlaces.Add(new TowerPlace() { X = i % rows * SQUARE_SIZE + SQUARE_SIZE / 2, Y = i / rows * SQUARE_SIZE + SQUARE_SIZE / 2 });
                }

            }
           
            BuildPath();

            AdjustSize(_spawn);
            var tmp = _spawn;
            do
            {
                tmp = tmp.Next;
                AdjustSize(tmp);
                //Debug.WriteLine(tmp.X + " " + tmp.Y);
                
            } while (tmp.Next != null);

           // Debug.WriteLine(_spawn.Next.X+" "+_spawn.Next.Y );

        }

        private PathSquare AdjustSize(PathSquare pathSquare)
        {
            pathSquare.X = pathSquare.X * SQUARE_SIZE + SQUARE_SIZE / 2;
            pathSquare.Y = pathSquare.Y * SQUARE_SIZE + SQUARE_SIZE / 2;
            return pathSquare;
        }

        private void BuildPath()
        {
            LinkedList<PathSquare> queue = new LinkedList<PathSquare>();
            queue.AddLast(_spawn);
            PathSquare current;
            while (queue.Count() != 0)
            {
                current = queue.First();
                queue.RemoveFirst();
                char val;
                PathSquare next;
                //up
                val = _map[current.Y - 1, current.X];
                if (val == 'P' || val == 'S' || val == 'E')
                {
                    if (current.Previous == null ||
                        (current.Previous.X != current.X || current.Previous.Y != current.Y - 1))
                    {
                        next = (new PathSquare() { Y = current.Y - 1, X = current.X, Previous = current });
                        current.Next = next;
                        queue.AddLast(next);
                    }

                }
                //down
                val = _map[current.Y + 1, current.X];
                if (val == 'P' || val == 'S' || val == 'E')
                {
                    if (current.Previous == null ||
                        (current.Previous.X != current.X || current.Previous.Y != current.Y + 1))
                    {
                        next = (new PathSquare() { Y = current.Y + 1, X = current.X, Previous = current });
                        current.Next = next;
                        queue.AddLast(next);
                    }

                }
                //left
                val = _map[current.Y, current.X - 1];
                if (val == 'P' || val == 'S' || val == 'E')
                {
                    if (current.Previous == null ||
                        (current.Previous.X != current.X - 1 || current.Previous.Y != current.Y))
                    {
                        next = (new PathSquare() { Y = current.Y, X = current.X - 1, Previous = current });
                        current.Next = next;
                        queue.AddLast(next);
                    }

                }

                //right
                val = _map[current.Y, current.X + 1];
                if (val == 'P' || val == 'S' || val == 'E')
                {
                    if (current.Previous == null ||
                        (current.Previous.X != current.X + 1 || current.Previous.Y != current.Y))
                    {
                        next = (new PathSquare() { Y = current.Y, X = current.X + 1, Previous = current });
                        current.Next = next;
                        queue.AddLast(next);
                    }

                }
            }



        }

        public void Info(out PathSquare spawn, out char[,] map, out List<TowerPlace> towers)
        {
            spawn = _spawn;
            map = _map;
            towers = _towerPlaces;
        }
    }
}
