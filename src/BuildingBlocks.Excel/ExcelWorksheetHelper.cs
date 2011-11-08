using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;

namespace BuildingBlocks.Common.Excel
{
    public delegate T ConvertValue<T>(object value);

    public class ExcelWorksheetHelper
    {
        private readonly ExcelAppHelper _excelAppHelper;
        private readonly Worksheet _worksheet;

        internal ExcelWorksheetHelper(ExcelAppHelper excelAppHelper, Worksheet worksheet)
        {
            Debug.Assert(excelAppHelper != null);
            Debug.Assert(worksheet != null);

            _excelAppHelper = excelAppHelper;
            _worksheet = worksheet;
        }

        public ExcelAppHelper ExcelAppHelper
        {
            get { return _excelAppHelper; }
        }

        public Worksheet Worksheet
        {
            get { return _worksheet; }
        }

        public IList<T> GetColumnData<T>(string columnName, ConvertValue<T> convertor)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException("columnName");
            }

            return LoadColumnData(columnName, convertor, GetColumnsRowData);
        }

        public IList<T> GetOptionalColumnData<T>(string columnName, ConvertValue<T> convertor, int expectedItemsCount)
        {
            if (columnName == null)
                throw new ArgumentNullException("columnName");
            if (expectedItemsCount <= 0)
                throw new ArgumentException("Expected items count can not be less or equal 0", "expectedItemsCount");

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException("columnName");
            }

            return LoadColumnData(columnName, convertor, (columnRange, valueConvertor) => GetColumnsRowData(columnRange, valueConvertor, expectedItemsCount));
        }

        public void SetColumnData<T>(string columnName, IEnumerable<T> data)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException("columnName");
            }
            if (_worksheet == null)
            {
                return;
            }

            Range columnRange = EnsureColumnRange(columnName);
            int lastNonEmptyRow = ExcelUtil.GetLastNonEmptyRowNo(columnRange);

            int row = lastNonEmptyRow < 2 ? 2 : lastNonEmptyRow + 1;
            foreach (T dataItem in data)
            {
                Range cell = _worksheet.get_Range(_worksheet.Cells[row, columnRange.Column],
                                                  _worksheet.Cells[row, columnRange.Column]);
                cell.Value2 = dataItem;
                row++;
            }
        }

        private IList<T> LoadColumnData<T>(string columnName, ConvertValue<T> convertor, Func<Range, ConvertValue<T>, IList<T>> columnRangeDataGetter)
        {
            if (_worksheet == null)
                return new List<T>(0);

            Range columnRange = GetColumnRange(columnName);
            return columnRangeDataGetter(columnRange, convertor);
        }

        private Range EnsureColumnRange(string columnName)
        {
            Range range;
            try
            {
                range = GetColumnRange(columnName);
            }
            catch (ArgumentException)
            {
                range = ExcelUtil.GetEmptyColumnCell(_worksheet, 1, 1);
                range.Value2 = columnName;
            }
            return range;
        }

        public IList<T> GetColumnData<T>(string columnName)
        {
            return GetColumnData<T>(columnName, ConvertTo<T>);
        }

        public IEnumerable<T> GetOptionalColumnData<T>(string columnName, int expectedItemsCount)
        {
            return GetOptionalColumnData<T>(columnName, ConvertTo<T>, expectedItemsCount);
        }

        private IList<T> GetColumnsRowData<T>(Range columnRange, ConvertValue<T> convertor)
        {
            Range nonEmptyRows = ExcelUtil.GetNonEmptyRowCell(_worksheet, columnRange.Row + 1, columnRange.Column);
            if (nonEmptyRows == null)
            {
                return new List<T>(0);
            }
            object[,] dataItems = ExcelUtil.GetRangeValues(nonEmptyRows);

            List<T> result = new List<T>();
            for (int i = 1; i <= dataItems.GetLength(0); i++)
            {
                T value = convertor(dataItems[i, 1]);
                result.Add(value);
            }
            return result;
        }

        private IList<T> GetColumnsRowData<T>(Range columnRange, ConvertValue<T> convertor, int itemsCount)
        {
            Range dataRows = ExcelUtil.GetRange(_worksheet,
                                                columnRange.Row + 1,
                                                columnRange.Column,
                                                columnRange.Row + itemsCount);
            if (dataRows == null)
            {
                return new List<T>(0);
            }

            List<T> result = new List<T>();
            object[,] dataItems = ExcelUtil.GetRangeValues(dataRows);
            for (int i = 1; i <= Math.Min(itemsCount, dataItems.GetLength(0)); i++)
            {
                T value = convertor(dataItems[i, 1]);
                result.Add(value);
            }
            return result;
        }

        private Range GetColumnRange(string columnName)
        {
            Range columnRange = ExcelUtil.FindTextCellRange(_worksheet, columnName);
            if (columnRange == null)
            {
                throw new ArgumentException(
                    string.Format("Колонка \"{0}\" не найдена в листе \"{1}\"",
                                  columnName, _worksheet.Name));
            }
            return columnRange;
        }

        private static T ConvertTo<T>(object value)
        {
            if (value is T)
            {
                return (T)value;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}