using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace BuildingBlocks.Common.Excel
{
    public static class ExcelUtil
    {
        public static string GetRangeAddress(Range range)
        {
            Debug.Assert(range != null);

            return range.get_Address(
                false,
                false,
                XlReferenceStyle.xlA1,
                Missing.Value,
                Missing.Value);
        }

        public static Range GetEntryWorksheetRange(Worksheet worksheet)
        {
            Debug.Assert(worksheet != null);

            Range entireWorksheet = worksheet.get_Range(
                worksheet.Cells[1, 1],
                worksheet.Cells[worksheet.Rows.Count, worksheet.Columns.Count]);
            return entireWorksheet;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <remarks>ћетод не гарантирует поиск в скрытых €чейках</remarks>
        public static int GetLastNonEmptyRowNo(Range range)
        {
            Debug.Assert(range != null);

            Range cell = range.Find("*",
                                    Missing.Value,
                                    XlFindLookIn.xlFormulas,
                                    XlLookAt.xlWhole,
                                    XlSearchOrder.xlByRows,
                                    XlSearchDirection.xlPrevious,
                                    true,
                                    true,
                                    Missing.Value);

            if (cell == null)
            {
                return 0;
            }

            return cell.Row;
        }

        public static Range FindTextCellRange(Worksheet worksheet, string columnName)
        {
            Range entry = GetEntryWorksheetRange(worksheet);
            Range cell = entry.Find(columnName,
                                    entry.Cells[entry.Rows.Count, 1],
                                    XlFindLookIn.xlValues,
                                    XlLookAt.xlPart,
                                    XlSearchOrder.xlByRows,
                                    XlSearchDirection.xlNext,
                                    false, false, Missing.Value);
            return cell;
        }

        public static object[,] GetRangeValues(Range range)
        {
            Debug.Assert(range != null);

            if (range.Rows.Count > 1 || range.Columns.Count > 1)
            {
                return (Object[,]) range.get_Value(Missing.Value);
            }

            object[,] cellValues = CreateSingleColumn2DArray(1);
            cellValues[1, 1] = range.get_Value(Missing.Value);
            return cellValues;
        }

        public static Range ScanFromStartRowToEmptyRow(int startRow, Range searchArea)
        {
            const string wildCard = "*";
            Debug.Assert(searchArea != null);

            Range firstRow = searchArea.Find(wildCard,
                                             searchArea.Cells[searchArea.Rows.Count, 1],
                                             XlFindLookIn.xlValues,
                                             XlLookAt.xlPart,
                                             XlSearchOrder.xlByRows,
                                             XlSearchDirection.xlNext,
                                             false, false, Missing.Value);
            if (firstRow == null)
            {
                return null;
            }

            Range firstColumn = searchArea.Find(wildCard,
                                                searchArea.Cells[1, searchArea.Columns.Count], XlFindLookIn.xlValues,
                                                XlLookAt.xlPart, XlSearchOrder.xlByColumns,
                                                XlSearchDirection.xlNext, false, false, Missing.Value);

            Range lastRow = searchArea.Find(wildCard,
                                            searchArea.Cells[1, 1], XlFindLookIn.xlValues,
                                            XlLookAt.xlPart, XlSearchOrder.xlByRows,
                                            XlSearchDirection.xlPrevious, false, false, Missing.Value);

            Range lastColumn = searchArea.Find(wildCard,
                                               searchArea.Cells[1, 1], XlFindLookIn.xlValues,
                                               XlLookAt.xlPart, XlSearchOrder.xlByColumns,
                                               XlSearchDirection.xlPrevious, false, false, Missing.Value);

            Debug.Assert(firstColumn != null);
            Debug.Assert(lastRow != null);
            Debug.Assert(lastColumn != null);

            Int32 iFirstRow = firstRow.Row;
            Int32 iFirstColumn = firstColumn.Column;
            Int32 iLastRow = lastRow.Row;
            Int32 iLastColumn = lastColumn.Column;

            Worksheet worksheet = searchArea.Worksheet;

            return worksheet.get_Range(
                worksheet.Cells[iFirstRow, iFirstColumn],
                worksheet.Cells[iLastRow, iLastColumn]);
        }

        public static void ParseCellAddress(string cellAddressA1Style,
                                            out int oneBasedRowNumber, out string columnLetter)
        {
            Debug.Assert(!string.IsNullOrEmpty(cellAddressA1Style));
            Debug.Assert(cellAddressA1Style.IndexOf(':') == -1);

            oneBasedRowNumber = int.MinValue;
            columnLetter = null;

            for (int i = 0; i < cellAddressA1Style.Length; i++)
            {
                if (char.IsDigit(cellAddressA1Style[i]))
                {
                    oneBasedRowNumber = int.Parse(cellAddressA1Style.Substring(i));
                    columnLetter = cellAddressA1Style.Substring(0, i);

                    return;
                }
            }
        }

        public static Object[,] CreateSingleColumn2DArray(int rows)
        {
            Debug.Assert(rows > 0);
            return (Object[,])Array.CreateInstance(
                                  typeof(object),
                                  new int[] { rows, 1 },
                                  new int[] { 1, 1 });
        }

        public static Range GetNonEmptyRowCell(Worksheet worksheet, int startRow, int columnNo)
        {
            Range entry = GetEntryWorksheetRange(worksheet);

            int row = startRow;
            Range cell;
            do
            {
                cell = worksheet.get_Range(
                    worksheet.Cells[row, columnNo],
                    worksheet.Cells[row, columnNo]);
                row++;
            } while (cell.Value2 != null && !string.IsNullOrEmpty(cell.Value2.ToString()));

            if (row - 1 == startRow)
            {
                return null;
            }

            return worksheet.get_Range(
                worksheet.Cells[startRow, columnNo],
                worksheet.Cells[row - 2, columnNo]);
        }

        public static Range GetEmptyColumnCell(Worksheet worksheet, int row, int startColumn)
        {
            int column = startColumn;
            Range cell;
            do
            {
                cell = worksheet.get_Range(
                    worksheet.Cells[row, column],
                    worksheet.Cells[row, column]);
                column++;
            } while (cell.Value2 != null && !string.IsNullOrEmpty(cell.Value2.ToString()));

            return worksheet.get_Range(
                worksheet.Cells[row, cell.Column],
                worksheet.Cells[row, cell.Column]);
        }

        public static Range GetRange(Worksheet worksheet, int startRow, int columnNo, int stopRow)
        {
            if (startRow > stopRow)
            {
                throw new ArgumentException("Stop row can not be less than start row");
            }

            return worksheet.get_Range(
                worksheet.Cells[startRow, columnNo],
                worksheet.Cells[stopRow, columnNo]);
        }
    }
}