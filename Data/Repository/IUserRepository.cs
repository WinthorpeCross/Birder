﻿using Birder.Data.Model;
using Birder.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birder.Data.Repository
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<ApplicationUser> GetUserAndNetworkAsyncByUserName(ApplicationUser user);
        Task<ApplicationUser> GetUserAndNetworkAsyncByUserName(string userName);
        IEnumerable<UserViewModel> GetFollowingList(ApplicationUser user);
        IEnumerable<UserViewModel> GetFollowersList(ApplicationUser user);
        void Follow(ApplicationUser loggedinUser, ApplicationUser userToFollow);
        void UnFollow(ApplicationUser loggedinUser, ApplicationUser userToUnfollow);
        IEnumerable<UserViewModel> GetSuggestedBirdersToFollow(ApplicationUser user);
        IEnumerable<UserViewModel> GetSuggestedBirdersToFollow(ApplicationUser user, string searchCriterion);
        IQueryable<Observation> GetUsersObservationsList(string userId);
        Task<int> UniqueSpeciesCount(ApplicationUser user);
    }
}