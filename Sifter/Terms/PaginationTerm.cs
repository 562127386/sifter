using System;
using System.Linq;


namespace Sifter.Terms {

    internal class PaginationTerm {

        private readonly int page;
        private readonly int pageSize;



        public PaginationTerm(string pageStr, string pageSizeStr) {
            page = int.TryParse(pageStr, out var pageVal) ? Math.Max(pageVal, 1) : 1;

            pageSize = Math.Min(
                int.TryParse(pageSizeStr, out var pageSizeVal)
                    ? Math.Max(pageSizeVal, 1)
                    : Constants.DEFAULT_PAGE_SIZE,
                Constants.MAX_PAGE_SIZE
            );
        }



        public IQueryable<T> ApplyPagination<T>(IQueryable<T> query) {
            query = applyPage(query);
            query = applyPageSize(query);

            return query;
        }



        private IQueryable<T> applyPage<T>(IQueryable<T> query) {
            return page == 1 ? query : query.Skip((page - 1) * pageSize);
        }



        private IQueryable<T> applyPageSize<T>(IQueryable<T> query) {
            return query.Take(pageSize);
        }

    }

}