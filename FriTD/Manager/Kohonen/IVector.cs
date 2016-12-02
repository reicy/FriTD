namespace Manager.Kohonen
{
    public interface IVector<V> : IStringConstructible
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
        bool IsEmpty();
    }
}
