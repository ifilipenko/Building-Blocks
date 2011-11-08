using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace BuildingBlocks.Common.Excel
{
    class ExcelWorkbookHelper : IDisposable
    {
        #region [Srategy classes]

        public interface IOpenedWorkbook
        {
            bool FileOpened { get; }
            bool IsEmptyWorkbook { get; }
            int SheetsCount { get; }
            string OpenedFileName { get; }
            bool CloseAppWhenDisposed { get; }
            Worksheet GetWorksheet(int worksheetNo);
            void OpenFile(string fileName);
            void Close();
        }

        abstract class WorkbookBase : IOpenedWorkbook
        {
            private readonly ApplicationClass _excelApp;
            protected Workbook _workbook;
            private string _fileName;

            protected WorkbookBase(ApplicationClass excelApp)
            {
                _excelApp = excelApp;
            }

            protected ApplicationClass ExcelApp
            {
                get { return _excelApp; }
            }

            #region Implementation of IOpenedWorkbook

            public bool FileOpened
            {
                get { return _workbook != null; }
            }

            public bool IsEmptyWorkbook
            {
                get { return _workbook.Sheets.Count == 0; }
            }

            public int SheetsCount
            {
                get { return _workbook.Worksheets.Count; }
            }

            public string OpenedFileName
            {
                get { return _fileName; }
                protected set { _fileName = value; }
            }

            public virtual bool CloseAppWhenDisposed
            {
                get { return true; }
            }

            public Worksheet GetWorksheet(int worksheetNo)
            {
                return (Worksheet) _workbook.Worksheets[worksheetNo];
            }

            public abstract void OpenFile(string fileName);

            public virtual void Close()
            {
                CloseBook();
            }

            #endregion

            protected void CloseBook()
            {
                if (_workbook != null)
                {
                    _workbook.Close(false, string.Empty, false);
                }
            }

            protected void CloseAndSave()
            {
                if (_workbook != null)
                {
                    try
                    {
                        _workbook.Close(true, OpenedFileName, Type.Missing);
                    }
                    catch(COMException ex)
                    {
                        if (ex.ErrorCode == -2146827284)
                        {
                            throw new InvalidOperationException("Не удалось сохранить файл, т.к. файл с таким именем уже существует");
                        }
                        throw;
                    }
                }
            }
        }

        class VisibleWorkbook : IOpenedWorkbook
        {
            private readonly ApplicationClass _excelApp;
            private Workbook _workbook;

            public VisibleWorkbook(ApplicationClass excelApp)
            {
                _excelApp = excelApp;
            }

            #region Implementation of IOpenedWorkbook

            public bool FileOpened
            {
                get { return _excelApp != null; }
            }

            public bool IsEmptyWorkbook
            {
                get { return false; }
            }

            public int SheetsCount
            {
                get { return _workbook.Sheets.Count; }
            }

            public string OpenedFileName
            {
                get { return string.Empty; }
            }

            public bool CloseAppWhenDisposed
            {
                get { return false; }
            }

            public Worksheet GetWorksheet(int worksheetNo)
            {
                return (Worksheet) _workbook.Sheets[worksheetNo];
            }

            public void OpenFile(string fileName)
            {
                _workbook = _excelApp.Workbooks.Add(Type.Missing);
                _workbook.Sheets.Add(Type.Missing, Type.Missing, 1, XlSheetType.xlWorksheet);
                _excelApp.Visible = true;
            }

            public void Close()
            {
            }

            #endregion
        }

        class NewWorkbook : WorkbookBase
        {
            private readonly int _listCount;

            public NewWorkbook(int listCount, ApplicationClass excelApp)
                : base(excelApp)
            {
                _listCount = listCount;
            }

            #region Overrides of WorkbookBase

            public override void OpenFile(string fileName)
            {
                if (File.Exists(fileName))
                {
                    throw new ArgumentException(
                        string.Format("Файл \"{0}\" уже существует", fileName),
                        "fileName");
                }

                OpenedFileName = fileName;
                _workbook = ExcelApp.Workbooks.Add(Type.Missing);
                _workbook.Sheets.Add(Type.Missing, Type.Missing, _listCount, XlSheetType.xlWorksheet);
            }

            #endregion

            public override void Close()
            {
                CloseAndSave();
            }
        }

        class FileWorkbook : WorkbookBase
        {
            private bool _readOnly;

            public FileWorkbook(ApplicationClass excelApp)
                : base(excelApp)
            {
            }

            public override void OpenFile(string fileName)
            {
                if (!File.Exists(fileName))
                {
                    throw new ArgumentException(
                        string.Format("Файл \"{0}\" не существует", fileName),
                        "fileName");
                }

                Open(fileName, false);
            }

            public override void Close()
            {
                if(_readOnly)
                {
                    CloseBook();
                }
                else
                {
                    CloseAndSave();
                }
            }

            protected void Open(string fileName, bool readOnly)
            {
                OpenedFileName = fileName;
                _readOnly = readOnly;
                _workbook = ExcelApp.Workbooks.Open(fileName,
                                                    0,
                                                    readOnly,
                                                    1,
                                                    string.Empty,
                                                    string.Empty,
                                                    true,
                                                    XlPlatform.xlWindows,
                                                    ",",
                                                    !readOnly,
                                                    false,
                                                    1,
                                                    false,
                                                    null,
                                                    null);
            }
        }

        class ReadOnlyWorkbook : FileWorkbook
        {
            public ReadOnlyWorkbook(ApplicationClass excelApp)
                : base(excelApp)
            {
            }

            public override void OpenFile(string fileName)
            {
                Open(fileName, true);
            }
        }

        class NullWorkbook : IOpenedWorkbook
        {
            #region Implementation of IOpenedWorkbook

            public bool FileOpened
            {
                get { return false; }
            }

            public bool IsEmptyWorkbook
            {
                get { return true; }
            }

            public int SheetsCount
            {
                get { return 0; }
            }

            public string OpenedFileName
            {
                get { return string.Empty; }
            }

            public bool CloseAppWhenDisposed
            {
                get { return true; }
            }

            public Worksheet GetWorksheet(int worksheetNo)
            {
                return null;
            }

            public void OpenFile(string fileName)
            {
            }

            public void Close()
            {
            }

            #endregion
        }

        #endregion

        private readonly ExcelAppHelper _excelAppHelper;
        private readonly ApplicationClass _excelApp;
        private IOpenedWorkbook _workbook;

        public ExcelWorkbookHelper(ExcelAppHelper excelAppHelper)
        {
            _excelAppHelper = excelAppHelper;
            _excelApp = excelAppHelper.ExcelAppClass;
            _workbook = new NullWorkbook();
        }

        public int WorksheetsCount
        {
            get { return _workbook.SheetsCount; }
        }

        public string OpenedFileName
        {
            get { return _workbook.OpenedFileName; }
        }

        public void VisibleOpen()
        {
            OpenCore(string.Empty, new VisibleWorkbook(_excelApp));
        }

        public void New(byte listCount, string fileNameOnClose)
        {
            OpenCore(fileNameOnClose, new NewWorkbook(listCount, _excelApp));
        }

        public void ReadOnlyOpen(string fileName)
        {
            OpenCore(fileName, new ReadOnlyWorkbook(_excelApp));
        }

        public void Open(string fileName)
        {
            OpenCore(fileName, new FileWorkbook(_excelApp));
        }

        void OpenCore(string fileName, IOpenedWorkbook workbook)
        {
            workbook.OpenFile(fileName);

            _workbook.Close();
            _workbook = workbook;
        }

        public bool IsEmptyWorkbook()
        {
            return _workbook.IsEmptyWorkbook;
        }

        public ExcelWorksheetHelper GetWorksheet(int worksheetNo)
        {
            if (worksheetNo < 1 || worksheetNo > _workbook.SheetsCount)
            {
                throw new ArgumentException(
                    string.Format("Неверный номер листа в книге. Всего листов {0}, но указан номер {1}",
                                  _workbook.SheetsCount, worksheetNo),
                    "worksheetNo");
            }

            return new ExcelWorksheetHelper(_excelAppHelper, _workbook.GetWorksheet(worksheetNo));
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_excelApp != null && _workbook.CloseAppWhenDisposed)
            {
                _workbook.Close();
                _excelApp.Quit();
            }
        }

        #endregion
    }
}