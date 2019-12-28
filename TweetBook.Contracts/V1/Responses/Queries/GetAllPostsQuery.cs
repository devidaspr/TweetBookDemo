using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBook.Contracts.V1.Responses.Queries
{
    public class GetAllPostsQuery
    {
        [FromQuery(Name ="profileId")]
        public string UserId { get; set; }
    }
}
