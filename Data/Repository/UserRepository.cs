﻿using Birder.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birder.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Follow(ApplicationUser loggedinUser, ApplicationUser userToFollow)
        {
            userToFollow.Followers.Add(new Network { Follower = loggedinUser });
            //_dbContext.SaveChanges();
        }

        public void UnFollow(ApplicationUser loggedinUser, ApplicationUser userToUnfollow)
        {
            loggedinUser.Following.Remove(userToUnfollow.Followers.FirstOrDefault());
            //_dbContext.SaveChanges();
        }

        public async Task<ApplicationUser> GetUserAndNetworkAsyncByUserName(string userName)
        {
            return await _dbContext.Users
                         .Include(x => x.Followers)
                             .ThenInclude(x => x.Follower)
                         .Include(y => y.Following)
                             .ThenInclude(r => r.ApplicationUser)
                         .Where(x => x.UserName == userName)
                         .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetFollowersNotFollowedAsync(ApplicationUser user, IEnumerable<string> followersNotBeingFollowed)
        {
            // If followersNotBeingFollowed.Count() != 0
            return await _dbContext.Users.Where(users => followersNotBeingFollowed.Contains(users.UserName)).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetSuggestedBirdersToFollowAsync(ApplicationUser user, IEnumerable<string> followingList)
        {
            // If user is following every follower
            return await _dbContext.Users.Where(users => !followingList.Contains(users.UserName) && users.UserName != user.UserName).ToListAsync();
        }


        public async Task<IEnumerable<ApplicationUser>> SearchBirdersToFollowAsync(ApplicationUser user, string searchCriterion, IEnumerable<string> followingList)
        {
            return await _dbContext.Users
                .Where(users => users.NormalizedUserName.Contains(searchCriterion.ToUpper()) && !followingList.Contains(users.UserName)).ToListAsync();
        }
        //public IQueryable<NetworkUserViewModel> GetSuggestedBirdersToFollow(ApplicationUser user, string searchCriterion)
        //{
        //    var followingList = from following in user.Following
        //                        select following.ApplicationUser.UserName;

        //    //IEnumerable<NetworkUserViewModel> suggestedBirders = new List<NetworkUserViewModel>();
        //    //var suggestedBirders = new List<NetworkUserViewModel>();
        //    var suggestedBirders = from users in _dbContext.Users
        //                       where (users.UserName.ToUpper().Contains(searchCriterion.ToUpper()) && !followingList.Contains(users.UserName) && users.UserName != user.UserName) // .Contains(users.UserName) // != user.UserName)
        //                       select new NetworkUserViewModel
        //                       {
        //                           UserName = users.UserName,
        //                           ProfileImage = users.ProfileImage,
        //                           IsFollowing = users.Following.Any(cus => cus.ApplicationUser.UserName == user.UserName)
        //                       };


        //    return suggestedBirders;
        //}

        //public IQueryable<NetworkUserViewModel> GetSuggestedBirdersToFollow(ApplicationUser user) //, IEnumerable<string> followersNotBeingFollowed)
        //{
        //    //******************
        //    // move to controller (Automapper)
        //    var followerList = from follower in user.Followers
        //                       select follower.Follower.UserName;
        //    var followingList = from following in user.Following
        //                        select following.ApplicationUser.UserName;

        //    IEnumerable<string> followersNotBeingFollowed = followerList.Except(followingList); // list usernames
        //    //IEnumerable<NetworkUserViewModel> suggestedBirders = new List<NetworkUserViewModel>();
        //    //******************

        //    if (followersNotBeingFollowed.Count() != 0)
        //    {
        //        // Get followers who not be followed
        //        var suggestedBirders = from users in _dbContext.Users
        //                   .Where(users => followersNotBeingFollowed.Contains(users.UserName))
        //                               select new NetworkUserViewModel
        //                               {
        //                                   UserName = users.UserName,
        //                                   ProfileImage = users.ProfileImage,
        //                                   IsFollowing = users.Following.Any(cus => cus.ApplicationUser.UserName == users.UserName)
        //                               };
        //        return suggestedBirders;
        //    }
        //    else
        //    {
        //        var suggestedBirders = from users in _dbContext.Users
        //                           .Where(users => !followingList.Contains(users.UserName) && users.UserName != user.UserName)
        //                           select new NetworkUserViewModel
        //                           {
        //                               UserName = users.UserName,
        //                               ProfileImage = users.ProfileImage,
        //                               IsFollowing = users.Following.Any(cus => cus.ApplicationUser.UserName == users.UserName)
        //                           };
        //        return suggestedBirders;
        //    }

        //    ////return suggestedBirders.ToList();
        //}


        //public IEnumerable<UserViewModel> GetFollowingList(ApplicationUser user)
        //{
        //    var followingList = from following in user.Following
        //                            select new UserViewModel
        //                            {
        //                                UserName = following.ApplicationUser.UserName,
        //                                ProfileImage = following.ApplicationUser.ProfileImage
        //                            };
        //    return followingList;
        //}

        //public IEnumerable<UserViewModel> GetFollowersList(ApplicationUser user)
        //{
        //    var followerList = from follower in user.Followers
        //                          select new UserViewModel
        //                          {
        //                              UserName = follower.Follower.UserName,
        //                              ProfileImage = follower.Follower.ProfileImage,
        //                          };
        //    return followerList;
        //}
    }
}
