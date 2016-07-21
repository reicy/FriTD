using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.Kohonen
{
    public interface IVector<V>
    {
        double Difference(V vector);
        double Difference(V vector, double[] weight);
        double DifferenceCosine(V vector);

        V Diff(V vector);
        V Add(V vector);
        V Multiply(double factor);
        double Dot(V vector);
        double Norm();
        void Print();
        void Load(string v);
        bool IsEmpty();
    }
}
