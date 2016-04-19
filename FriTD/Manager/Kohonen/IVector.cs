using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.Kohonen
{
    public interface IVector<V>
    {
        double Difference(V vector);

        V Diff(V vector);
        V Add(V vector);
        V Multiply(double factor);
        void Print();

    }
}
