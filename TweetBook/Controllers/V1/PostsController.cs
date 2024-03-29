﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Cache;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Contracts.V1.Responses.Queries;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Helpers;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        //[Route("api/v1/posts")]
        //[HttpGet("api/v1/posts")]
        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsQuery query, [FromQuery] PaginationQuery paginationQuery)
        {
            //for paginated result
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllPostsFilter>(query);
            var posts = await _postService.GetPostsAsync(filter, pagination);

            //for without paginated result
            //var posts = await _postService.GetPostsAsync();

            //without using AutoMapper
            //var postResponses = posts.Select(post => new PostResponse
            //{
            //    Id = post.Id,
            //    Name = post.Name,
            //    UserId = post.UserId,
            //    Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName }).ToList()
            //}).ToList();
            //return Ok(postResponses);

            //Using AutoMapper
            //return Ok(_mapper.Map<List<PostResponse>>(posts));

            //using response wrapper
            //return Ok(new Response<List<PostResponse>>(_mapper.Map<List<PostResponse>>(posts)));

            //using response wrapper for pagination
            var postsResponse = _mapper.Map<List<PostResponse>>(posts);
            

            if(pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PostResponse>(postsResponse));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, postsResponse);
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            //without using AutoMapper
            //return Ok(new PostResponse
            //{
            //    Id = post.Id,
            //    Name = post.Name,
            //    UserId = post.UserId,
            //    Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName })
            //});

            //using AutoMapper
            //return Ok(_mapper.Map<PostResponse>(post));

            //using response wrapper for pagination
            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new ErrorResponse(new ErrorModel { Message = "you do not own this post" } ));
            }

            var deleted = await _postService.DeletePostAsync(postId);

            if (deleted)
                return NoContent();

            return NotFound();
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody]UpdatePostRequest request)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new ErrorResponse(new ErrorModel { Message = "you do not own this post" }));
            }

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = request.Name;

            var udpated = await _postService.UpdatePostAsync(post);

            
            if (udpated)
            {
                //without using AutoMapper
                //    return Ok(new PostResponse
                //    {
                //        Id = post.Id,
                //        Name = post.Name,
                //        UserId = post.UserId,
                //        Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName })
                //    });

                //using AutoMapper
                //return Ok(_mapper.Map<PostResponse>(post));

                //using response wrapper for pagination
                return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
            }

            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var newPostId = Guid.NewGuid();
            var post = new Post
            {
                Id = newPostId,
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(x => new PostTag { PostId = newPostId, TagName = x }).ToList()
            };

            await _postService.CreatePostAsync(post);

            //var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            //var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            var locationUri = _uriService.GetPostUri(post.Id.ToString());

            ////without using AutoMapper
            //var response = new PostResponse
            //{
            //    Id = post.Id,
            //    Name = post.Name,
            //    UserId = post.UserId,
            //    Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName })
            //};

            //return Created(locationUri, response);

            //using AutoMapper
            //return Created(locationUri, _mapper.Map<PostResponse>(post));

            //using response wrapper for pagination
            return Created(locationUri, new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }
    }
}
