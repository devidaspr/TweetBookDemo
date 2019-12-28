using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Contracts.V1.Responses.Queries;
using TweetBook.Domain;
using TweetBook.Services;

namespace TweetBook.Helpers
{
    public class PaginationHelpers
    {
        internal static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> response)
        {
            var nextPage = pagination.PageNumber >= 1
                ? uriService.GetAllPostsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString()
                : null;

            var previousPage = pagination.PageNumber - 1 >= 1
                ? uriService.GetAllPostsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}
