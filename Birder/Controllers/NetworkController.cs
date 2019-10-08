﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Birder.Data.Model;
using Birder.Data.Repository;
using Birder.Helpers;
using Birder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Birder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NetworkController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INetworkRepository _networkRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public NetworkController(IMapper mapper
                                , IUnitOfWork unitOfWork
                                , ILogger<NetworkController> logger
                                , INetworkRepository networkRepository
                                , UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _networkRepository = networkRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetNetworkAsync()
        {
            try
            {
                var requestingUser = await _userManager.GetUserWithNetworkAsync(User.Identity.Name);

                if (requestingUser == null)
                {
                    _logger.LogError(LoggingEvents.GetItem, "User not found");
                    return NotFound("Requesting user not found");
                }

                var model = _mapper.Map<ApplicationUser, UserNetworkDto>(requestingUser);

                UserProfileHelper.UpdateFollowersCollection(model.Followers, requestingUser);

                UserProfileHelper.UpdateFollowingCollection(model.Following, requestingUser);

                return Ok(model);

            }
            catch(Exception ex)
            {
                _logger.LogError(LoggingEvents.GetItem, ex, "GetMyNetworkAsync");
                return BadRequest("An unexpected error occurred");
            }
        }

        [HttpGet, Route("NetworkSuggestions")]
        public async Task<IActionResult> GetNetworkSuggestionsAsync()
        {
            try
            {
                var requestingUser = await _userManager.GetUserWithNetworkAsync(User.Identity.Name);

                if (requestingUser == null)
                {
                    _logger.LogError(LoggingEvents.GetItemNotFound, "The user was not found");
                    return NotFound("Requesting user not found");
                }

                var followersNotBeingFollowed = UserProfileHelper.GetFollowersNotBeingFollowedUserNames(requestingUser);

                if (followersNotBeingFollowed.Count() == 0)
                {
                    var followingUsernamesList = UserProfileHelper.GetFollowingUserNames(requestingUser.Following);
                    var users = await _userManager.GetSuggestedBirdersToFollowAsync(requestingUser.UserName, followingUsernamesList);
                    return Ok(_mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<NetworkUserViewModel>>(users));
                }
                else
                {
                    var users = await _userManager.GetFollowersNotFollowedAsync(followersNotBeingFollowed);
                    return Ok(_mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<NetworkUserViewModel>>(users));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "Network");
                return BadRequest("An error occurred");
            }
        }

        [HttpGet, Route("SearchNetwork")]
        public async Task<IActionResult> GetSearchNetworkAsync(string searchCriterion)
        {
            try
            {
                if (string.IsNullOrEmpty(searchCriterion))
                {
                    _logger.LogError(LoggingEvents.GetListNotFound, "The search criterion is null or empty");
                    return BadRequest("No search criterion");
                }

                var requestingUser = await _userManager.GetUserWithNetworkAsync(User.Identity.Name);

                if (requestingUser == null)
                {
                    _logger.LogError(LoggingEvents.GetItemNotFound, "The user was not found");
                    return NotFound("Requesting user not found");
                }

                var followingUsernamesList = UserProfileHelper.GetFollowingUserNames(requestingUser.Following);
                followingUsernamesList.Add(requestingUser.UserName);

                var users = await _userManager.SearchBirdersToFollowAsync(searchCriterion, followingUsernamesList);
                return Ok(_mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<NetworkUserViewModel>>(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "Network");
                return BadRequest("An error occurred");
            }
        }

        [HttpPost, Route("Follow")]
        public async Task<IActionResult> PostFollowUserAsync(NetworkUserViewModel userToFollowDetails)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError(LoggingEvents.UpdateItem, ModelStateErrorsExtensions.GetModelStateErrorMessages(ModelState));
                    return BadRequest("Invalid modelstate");
                }

                var requestingUser = await _userManager.GetUserWithNetworkAsync(User.Identity.Name);

                var userToFollow = await _userManager.GetUserWithNetworkAsync(userToFollowDetails.UserName);

                if (requestingUser == null || userToFollow == null)
                {
                    _logger.LogError(LoggingEvents.UpdateItem, "User not found");
                    return NotFound("User not found");
                }

                if (requestingUser == userToFollow)
                {
                    return BadRequest("Trying to follow yourself");
                }

                _networkRepository.Follow(requestingUser, userToFollow);

                await _unitOfWork.CompleteAsync();

                var viewModel = _mapper.Map<ApplicationUser, NetworkUserViewModel>(userToFollow);

                viewModel.IsFollowing = UserProfileHelper.UpdateIsFollowing(viewModel.UserName, requestingUser.Following);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetItemNotFound, ex, "Follow action error");
                return BadRequest(String.Format("An error occurred trying to follow user: {0}", userToFollowDetails.UserName));
            }
        }

        [HttpPost, Route("Unfollow")]
        public async Task<IActionResult> PostUnfollowUserAsync(NetworkUserViewModel userToFollowDetails) //, int currentPage)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError(LoggingEvents.UpdateItem, ModelStateErrorsExtensions.GetModelStateErrorMessages(ModelState));
                    return BadRequest("Invalid modelstate");
                }

                var requestingUser = await _userManager.GetUserWithNetworkAsync(User.Identity.Name);
                var userToUnfollow = await _userManager.GetUserWithNetworkAsync(userToFollowDetails.UserName);

                if (requestingUser == null || userToUnfollow == null)
                {
                    _logger.LogError(LoggingEvents.UpdateItem, "User not found");
                    return NotFound("User not found");
                }

                if (requestingUser == userToUnfollow)
                {
                    return BadRequest("Trying to unfollow yourself");
                }

                _networkRepository.UnFollow(requestingUser, userToUnfollow);

                await _unitOfWork.CompleteAsync();

                var viewModel = _mapper.Map<ApplicationUser, NetworkUserViewModel>(userToUnfollow);

                viewModel.IsFollowing = UserProfileHelper.UpdateIsFollowing(viewModel.UserName, requestingUser.Following);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetItemNotFound, ex, "Unfollow action error");
                return BadRequest($"An error occurred trying to unfollow user: {userToFollowDetails.UserName}");
            }
        }
    }
}