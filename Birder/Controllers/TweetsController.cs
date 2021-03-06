﻿using AutoMapper;
using Birder.Data.Model;
using Birder.Data.Repository;
using Birder.Helpers;
using Birder.Services;
using Birder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Birder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TweetsController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISystemClockService _systemClock;
        private readonly ITweetDayRepository _tweetDayRepository;

        public TweetsController(ITweetDayRepository tweetDayRepository,
                                IMemoryCache memoryCache,
                                ILogger<TweetsController> logger,
                                ISystemClockService systemClock,
                                IMapper mapper)
        {
            _cache = memoryCache;
            _mapper = mapper;
            _logger = logger;
            _systemClock = systemClock;
            _tweetDayRepository = tweetDayRepository;
        }

        [HttpGet, Route("GetTweetDay")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTweetDayAsync()
        {
            try
            {
                if (_cache.TryGetValue(nameof(TweetDayViewModel), out TweetDayViewModel tweetDayCache))
                {
                    return Ok(tweetDayCache);
                }

                var tweet = await _tweetDayRepository.GetTweetOfTheDayAsync(_systemClock.GetToday);

                if (tweet is null)
                {
                    _logger.LogError(LoggingEvents.GetItemNotFound, "An error occurred getting tweet with date: {Date}", _systemClock.GetToday);
                    return StatusCode(500, $"tweets repository returned null");
                }

                var viewModel = _mapper.Map<TweetDay, TweetDayViewModel>(tweet);

                _cache.Set(nameof(TweetDayViewModel), viewModel, _systemClock.GetEndOfToday);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetItemNotFound, ex, "An error occurred getting tweet with date: {Date}", _systemClock.GetToday);
                return StatusCode(500, "an unexpected error occurred");
            }
        }

        [HttpGet, Route("GetTweetArchive")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTweetArchiveAsync(int pageIndex, int pageSize)
        {
            try
            {
                var tweets = await _tweetDayRepository.GetTweetArchiveAsync(pageIndex, pageSize, _systemClock.GetToday);

                if (tweets is null)
                {
                    _logger.LogError(LoggingEvents.GetItemNotFound, "An error occurred getting the tweets archive");
                    return StatusCode(500, $"tweets repository returned null");
                }

                var viewModel = _mapper.Map<QueryResult<TweetDay>, TweetArchiveDto>(tweets);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "An error occurred getting the tweets archive");
                return StatusCode(500, "an unexpected error occurred");
            }
        }
    }
}
