using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    //Athorizatoin config at controller level thats applicable for all the endpoints within this controller
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //Roles sepereated by comma would be mean "OR"
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Poster")] 
    [Produces("application/json")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IMapper mapper)
        {
            this._postService = postService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all the tags in the system
        /// </summary>
        /// <response code="200">Returns all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();

            //without using AutoMapper
            //var tagResponses = tags.Select(tag => new TagResponse { Name = tag.Name }).ToList();
            //return Ok(tagResponses);

            //using AutoMapper
            return Ok(_mapper.Map<List<TagResponse>>(tags));
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);
            if (tag == null)
            {
                return NotFound();
            }

            //without using AutoMapper
            //return Ok(new TagResponse { Name = tag.Name });

            //using AutoMapper
            return Ok(_mapper.Map<TagResponse>(tag));
        }

        /// <summary>
        /// Creates a tag in the system
        /// </summary>
        /// <remarks>
        ///     Sample Request:
        ///     
        ///         POST api/v1/tags
        ///         {
        ///             "name" : "some name"
        ///         }
        /// </remarks>
        /// <response code="201">Creates a tag in the system</response>
        /// <response code="400">Unable to create a tag due to validation error</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody]CreateTagRequest request)
        {
            var newTag = new Tag
            {
                Name = request.TagName,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var created = await _postService.CreateTagAsync(newTag);
            if (!created)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>
                        { new ErrorModel { Message = "Unable to create tag"} }
                });
                //return BadRequest(new ErrorResponse(new ErrorModel { Message = "Unable to create tag"}));
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name);

            //without using AutoMapper
            //return Created(locationUri, new TagResponse { Name = newTag.Name });

            //using AutoMapper
            return Created(locationUri, _mapper.Map<TagResponse>(newTag));
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        //[Authorize(Roles = "Admin")] //Authorization cofigured at a specific endpoint to overwrite the one set at controller level
        //[Authorize(Roles = "Admin")] //one must be Admin
        //[Authorize(Roles = "Poster")]//as well as Poster
        //[Authorize(Roles = "Admin,Poster")] // Either Admin or Poster
        [Authorize(Policy = "MustWorkForTek")]
        public async Task<IActionResult> Delete([FromRoute]string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);

            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
