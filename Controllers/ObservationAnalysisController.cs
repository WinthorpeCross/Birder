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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Birder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ObservationAnalysisController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IObservationRepository _observationRepository;

        public ObservationAnalysisController(IObservationRepository observationRepository
                                            , ILogger<ObservationAnalysisController> logger
                                            , IMemoryCache memoryCache
                                            , ISystemClock systemClock
                                            , IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _cache = memoryCache;
            _systemClock = systemClock;
            _observationRepository = observationRepository;
        }

        [HttpGet, Route("GetObservationAnalysis")]
        public async Task<IActionResult> GetObservationAnalysis()
        {
            try
            {
                var username = User.Identity.Name;

                if (username == null)
                {
                    return Unauthorized();
                }

                if (_cache.TryGetValue(CacheEntries.ObservationsList, out IEnumerable<Observation> observationsCache))
                {
                    return Ok(_mapper.Map<IEnumerable<Observation>, ObservationAnalysisViewModel>(observationsCache));
                }

                var observations = await _observationRepository.GetObservationsAsync(x => x.ApplicationUser.UserName == username);

                _cache.Set(CacheEntries.ObservationsList, observations, _systemClock.GetEndOfToday);

                var viewModel = _mapper.Map<IEnumerable<Observation>, ObservationAnalysisViewModel>(observations);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "An error occurred getting the Observations Analysis");
                return BadRequest();
            }
        }

        [HttpGet, Route("GetTopObservationAnalysis")]
        public async Task<IActionResult> GetTopObservationAnalysisAsync()
        {
            try
            {
                var username = User.Identity.Name;

                if (username == null)
                {
                    return Unauthorized();
                }

                if (_cache.TryGetValue(CacheEntries.ObservationsList, out IEnumerable<Observation> observationsCache))
                {
                    var viewModelCache = ObservationsAnalysisHelper.MapTopObservations(observationsCache, _systemClock.GetToday.AddDays(-30));
                    return Ok(viewModelCache);
                }

                var observations = await _observationRepository.GetObservationsAsync(a => a.ApplicationUser.UserName == username);

                _cache.Set(CacheEntries.ObservationsList, observations, _systemClock.GetEndOfToday);

                var viewModel = ObservationsAnalysisHelper.MapTopObservations(observations, _systemClock.GetToday.AddDays(-30));

               return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "An error occurred getting the Top Observations Analysis");
                return BadRequest();
            }
        }

        [HttpGet, Route("GetLifeList")]
        public async Task<IActionResult> GetLifeList()
        {
            try
            {
                var username = User.Identity.Name;

                if (username == null)
                {
                    return Unauthorized();
                }

                if (_cache.TryGetValue(CacheEntries.ObservationsList, out IEnumerable<Observation> observationsCache))
                {
                    var viewModelCache = ObservationsAnalysisHelper.MapLifeList(observationsCache);
                    return Ok(viewModelCache);
                }

                var observations = await _observationRepository.GetObservationsAsync(a => a.ApplicationUser.UserName == username);

                _cache.Set(CacheEntries.ObservationsList, observations, _systemClock.GetEndOfToday);

                var viewModel = ObservationsAnalysisHelper.MapLifeList(observations);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "An error occurred getting the Life List");
                return BadRequest();
            }
        }
    }
}
