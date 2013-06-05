using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.Algorithms
{
    public class LargestCommonSubsequence<T>
    {
        public class Matrix
        {
            private readonly int[,] _data;

            public Matrix(int sequence1Length, int sequence2Length)
            {
                _data = new int[sequence1Length + 1, sequence2Length + 1];
            }

            public int MaxLength { get; private set; }

            public bool IsEmpty
            {
                get { return Sequence1Dimension == 1 || Sequence2Dimension == 1; }
            }

            public int Sequence1Dimension
            {
                get { return _data.GetLength(0); }
            }

            public int Sequence2Dimension
            {
                get { return _data.GetLength(0); }
            }

            public int this[int i, int j]
            {
                get { return _data[i, j]; }
                set
                {
                    _data[i, j] = value;
                    if (MaxLength < value)
                    {
                        MaxLength = value;
                    }
                }
            }
        }

        private readonly T[] _sequence1;
        private readonly T[] _sequence2;
        private readonly IEqualityComparer<T> _equalityComparer;

        public LargestCommonSubsequence(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            _sequence1 = sequence1;
            _sequence2 = sequence2;
            _equalityComparer = equalityComparer ?? new DefaultEqualityComparer<T>();
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <returns></returns>
        public Matrix CreateMatrix()
        {
            var matrix = new Matrix(_sequence1.Length, _sequence2.Length);
            if (matrix.IsEmpty)
                return matrix;

            for (int i = 0; i <= _sequence1.Length; i++)
            {
                for (int j = 0; j <= _sequence2.Length; j++)
                {
                    if (j == 0 || i == 0)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (_equalityComparer.Equals(_sequence1[i - 1], _sequence2[j - 1]))
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i, j - 1]);
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <returns></returns>
        public T[] GetSubsequence()
        {
            var matrix = CreateMatrix();
            return GetSubsequenceCore(matrix, _sequence1.Length, _sequence2.Length).ToArray();
        }

        /// <summary>
        /// Implementation algorithm described in 
        /// http://en.wikipedia.org/wiki/Longest_common_subsequence_problem
        /// </summary>
        /// <param name="processEquals">processing function identical items from both sequences</param>
        /// <param name="processAdded">processing function added items to sequence2</param>
        /// <param name="processDeleted">processing function deleted items from sequence1</param>
        public void Backtrack(Action<T, T> processEquals, Action<T> processAdded, Action<T> processDeleted)
        {
            var matrix = CreateMatrix();
            BacktrackCore(matrix, _sequence1.Length, _sequence2.Length, processEquals, processAdded, processDeleted);
        }

        private void BacktrackCore(Matrix matrix, int i, int j, Action<T, T> processEquals, Action<T> processAdded, Action<T> processDeleted)
        {
            if (i > 0 && j > 0 && _equalityComparer.Equals(_sequence1[i - 1], _sequence2[j - 1]))
            {
                BacktrackCore(matrix, i - 1, j - 1, processEquals, processAdded, processDeleted);
                processEquals(_sequence1[i - 1], _sequence2[j - 1]);
            }
            else if (j > 0 && (i == 0 || matrix[i, j - 1] >= matrix[i - 1, j]))
            {
                BacktrackCore(matrix, i, j - 1, processEquals, processAdded, processDeleted);
                processAdded(_sequence2[j - 1]);
            }
            else if (i > 0 && (j == 0 || matrix[i, j - 1] < matrix[i - 1, j]))
            {
                BacktrackCore(matrix, i - 1, j, processEquals, processAdded, processDeleted);
                processDeleted(_sequence1[i - 1]);
            }
        }

        private IEnumerable<T> GetSubsequenceCore(Matrix matrix, int i, int j)
        {
            if (i == 0 || j == 0)
                return Enumerable.Empty<T>();

            if (_equalityComparer.Equals(_sequence1[i - 1], _sequence2[j - 1]))
            {
                var subsequence = GetSubsequenceCore(matrix, i - 1, j - 1);
                return subsequence.Concat(_sequence1[i - 1].ToEnumerable());
            }

            return matrix[i - 1, j] > matrix[i, j - 1]
                       ? GetSubsequenceCore(matrix, i - 1, j)
                       : GetSubsequenceCore(matrix, i, j - 1);
        }
    }
}
