using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;

namespace BuildingBlocks.Common.Excel
{
    public class ExcelAppHelper : IDisposable
    {
        private readonly ApplicationClass _excelApp;
        private readonly ExcelWorkbookHelper _excelWorkbookHelper;

        public ExcelAppHelper()
        {
            _excelApp = new ApplicationClass {Visible = false};
            _excelWorkbookHelper = new ExcelWorkbookHelper(this);
        }

        internal ApplicationClass ExcelAppClass
        {
            get { return _excelApp; }
        }

        public bool FileIsLoaded
        {
            get { return !string.IsNullOrEmpty(LoadedFilePath); }
        }

        public string LoadedFilePath
        {
            get { return _excelWorkbookHelper.OpenedFileName; }
        }

        public void OpenVisible()
        {
            _excelWorkbookHelper.VisibleOpen();
        }

        public void OpenForCreate(string fileNameOnClose, byte listCount)
        {
            _excelWorkbookHelper.New(listCount, fileNameOnClose);
        }

        public void OpenForChange(string fileName)
        {
            _excelWorkbookHelper.Open(fileName);
        }

        public void OpenForRead(string fileName)
        {
            _excelWorkbookHelper.ReadOnlyOpen(fileName);
            if (_excelWorkbookHelper.IsEmptyWorkbook())
            {
                throw new InvalidOperationException("Загруженная книга пуста");
            }
        }

        public int[] GetWorksheetNumbers()
        {
            return Enumerable.Range(1, _excelWorkbookHelper.WorksheetsCount).ToArray();
        }

        public ExcelWorksheetHelper GetWorksheet(int worksheetNo)
        {
            return _excelWorkbookHelper.GetWorksheet(worksheetNo);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            _excelWorkbookHelper.Dispose();
        }

        #endregion
    }
}