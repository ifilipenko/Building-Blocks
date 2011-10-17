using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildingBlocks.Common.Files
{
    public class CsvFile
    {
        private readonly string _filePath;
        private readonly string _separator;

        public CsvFile(string filePath, string separator)
        {
            _filePath = filePath;
            _separator = separator;

            if (string.IsNullOrEmpty(_separator))
            {
                _separator = ",";
            }
        }

        public IEnumerable<string []> ReadAllRows()
        {
            return File.ReadAllLines(_filePath)
                .Select(line => line.Split(_separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
        }

        public void ReadAndProcessEachRow(Action<int, string[]> rowProcessingFunction)
        {
            string[][] rows = (string[][]) ReadAllRows();
            for (int i = 0; i < rows.Length; i++)
            {
                string[] row = rows[i];
                rowProcessingFunction(i, row);
            }
        }

        public void SaveRows(IEnumerable<string []> rows)
        {
            var result = new StringBuilder();
            foreach (var row in rows)
            {
                result.AppendLine(string.Join(_separator, row));
            }
            File.WriteAllText(_filePath, result.ToString());
        }
    }
}