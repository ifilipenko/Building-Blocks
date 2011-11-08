using System;

namespace BuildingBlocks.Common
{
    public struct Pair<T1, T2>
    {
        private readonly T1 _first;
        private readonly T2 _second;

        public Pair(T1 fisrt, T2 second)
        {
            _first = fisrt;
            _second = second;
        }

        public T1 First
        {
            get { return _first; }
        }

        public T2 Second
        {
            get { return _second; }
        }

        public override int GetHashCode()
        {
            return _first.GetHashCode() ^ _second.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Pair<T1, T2>)
            {
                Pair<T1, T2> pair = (Pair<T1, T2>)obj;
                return pair._first.Equals(_first) && pair._second.Equals(_second);
            }
            return false;
        }
    }

    [Serializable]
    public struct Pair<T>
    {
        private readonly T _fisrt;
        private readonly T _second;

        public Pair(T first, T second)
        {
            _fisrt = first;
            _second = second;
        }

        public T First
        {
            get { return _fisrt; }
        }

        public T Second
        {
            get { return _second; }
        }
    }
}