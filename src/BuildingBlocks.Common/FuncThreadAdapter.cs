using System;
using System.Threading;

namespace BuildingBlocks.Common
{
    class FuncThreadAdapter<T, U>
    {
        readonly Thread _thread;
        readonly T _input;
        U _output;
        readonly Func<T, U> _func;

        public FuncThreadAdapter(Func<T, U> func, T input)
        {
            _input = input;
            _func = func;
            _thread = new Thread(Execute);
        }

        void Execute()
        {
            _output = _func(_input);
        }

        public Thread Thread
        {
            get { return _thread; }
        }

        public U Output
        {
            get { return _output; }
        }
    }
}