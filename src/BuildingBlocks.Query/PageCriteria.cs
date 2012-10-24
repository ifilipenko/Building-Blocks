using System;

namespace BuildingBlocks.Query
{
    public abstract class PageCriteria
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <exception cref="ArgumentOutOfRangeException"><c>pageNumber</c> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><c>pageSize</c> is out of range.</exception>
        protected PageCriteria(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "Page number number can not be less then 1");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "Page size can not be less then 1");

            PageSize = pageSize;
            PageNumber = pageNumber;
        }
        
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
    }
}