using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Responses.Queries;
using TweetBook.Domain;

namespace TweetBook.Mapping
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQuery, PaginationFilter>();
            CreateMap<GetAllPostsQuery, GetAllPostsFilter>();
        }
    }
}
