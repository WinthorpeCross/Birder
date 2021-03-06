﻿using AutoMapper;
using Birder.Controllers;
using Birder.Data;
using Birder.Data.Model;
using Birder.Data.Repository;
using Birder.TestsHelpers;
using Birder.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Birder.Tests.Controller
{
    public class PostUnfollowUserAsyncTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<NetworkController>> _logger;
        //private readonly UserManager<ApplicationUser> userManager;

        public PostUnfollowUserAsyncTests()
        {
            //// remove after refactor.....
            //userManager = SharedFunctions.InitialiseUserManager();
            ////
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BirderMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();
            _logger = new Mock<ILogger<NetworkController>>();
        }

        #region Unfollow action tests

        //[Fact]
        //public async Task PostUnfollowUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var mockUserManager = SharedFunctions.InitialiseMockUserManager();
        //    var mockRepo = new Mock<INetworkRepository>();

        //    var mockUnitOfWork = new Mock<IUnitOfWork>();

        //    var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, mockUserManager.Object);

        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal("example name") }
        //    };

        //    //Add model error
        //    controller.ModelState.AddModelError("Test", "This is a test model error");

        //    // Act
        //    var result = await controller.PostUnfollowUserAsync(SharedFunctions.GetTestNetworkUserViewModel("Test User"));

        //    var modelState = controller.ModelState;
        //    Assert.Equal(1, modelState.ErrorCount);
        //    Assert.True(modelState.ContainsKey("Test"));
        //    Assert.True(modelState["Test"].Errors.Count == 1);
        //    Assert.Equal("This is a test model error", modelState["Test"].Errors[0].ErrorMessage);

        //    // test response
        //    var objectResult = result as ObjectResult;
        //    Assert.NotNull(objectResult);
        //    Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.True(objectResult is BadRequestObjectResult);
        //    Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        //    //
        //    var actual = Assert.IsType<string>(objectResult.Value);

        //    //Assert.Contains("This is a test model error", "This is a test model error");
        //    Assert.Equal("Invalid modelstate", actual);
        //}


        [Fact]
        public async Task PostUnfollowUserAsync_ReturnsNotFound_WhenRequestingUserIsNullFromRepository()
        {
            var options = this.CreateUniqueClassOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                //You have to create the database
                context.Database.EnsureClean();
                context.Database.EnsureCreated();
                //context.SeedDatabaseFourBooks();

                //context.ConservationStatuses.Add(new ConservationStatus { ConservationList = "Red", Description = "", CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now });

                context.Users.Add(SharedFunctions.CreateUser("testUser1"));
                context.Users.Add(SharedFunctions.CreateUser("testUser2"));

                context.SaveChanges();

                context.Users.Count().ShouldEqual(2);
                // Arrange
                //*******************
                var userManager = SharedFunctions.InitialiseUserManager(context);
                //**
                var mockRepo = new Mock<INetworkRepository>();

                var mockUnitOfWork = new Mock<IUnitOfWork>();

                var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, userManager);

                string requestingUser = "This requested user does not exist";

                string userToUnfollow = "This requested user does not exist";

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
                };

                // Act
                var result = await controller.PostUnfollowUserAsync(SharedFunctions.GetTestNetworkUserViewModel(userToUnfollow));

                // Assert
                var objectResult = result as ObjectResult;
                Assert.NotNull(objectResult);
                Assert.IsType<NotFoundObjectResult>(result);
                Assert.True(objectResult is NotFoundObjectResult);
                Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
                var actual = Assert.IsType<string>(objectResult.Value);
                Assert.Equal("Requesting user not found", actual);
            }
        }

        [Fact]
        public async Task PostUnfollowUserAsync_ReturnsNotFound_WhenUserToFollowIsNullFromRepository()
        {
            var options = this.CreateUniqueClassOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                //You have to create the database
                context.Database.EnsureClean();
                context.Database.EnsureCreated();
                //context.SeedDatabaseFourBooks();

                //context.ConservationStatuses.Add(new ConservationStatus { ConservationList = "Red", Description = "", CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now });

                context.Users.Add(SharedFunctions.CreateUser("testUser1"));
                context.Users.Add(SharedFunctions.CreateUser("testUser2"));

                context.SaveChanges();

                context.Users.Count().ShouldEqual(2);

                // Arrange
                //*******************
                var userManager = SharedFunctions.InitialiseUserManager(context);
                //**
                var mockRepo = new Mock<INetworkRepository>();
                //mockRepo.Setup(x => x.GetUserAndNetworkAsync(It.IsAny<string>()))
                //        .Returns(Task.FromResult<ApplicationUser>(null));

                var mockUnitOfWork = new Mock<IUnitOfWork>();

                var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, userManager);

                string requestingUser = "testUser1";

                string userToUnfollow = "This requested user does not exist";

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
                };

                // Act
                var result = await controller.PostUnfollowUserAsync(SharedFunctions.GetTestNetworkUserViewModel(userToUnfollow));

                // Assert
                var objectResult = result as ObjectResult;
                Assert.NotNull(objectResult);
                Assert.IsType<NotFoundObjectResult>(result);
                Assert.True(objectResult is NotFoundObjectResult);
                Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
                var actual = Assert.IsType<string>(objectResult.Value);
                Assert.Equal("User to Unfollow not found", actual);
            }
        }

        [Fact]
        public async Task PostUnfollowUserAsync_ReturnsBadRequest_FollowerAndToFollowAreEqual()
        {
            var options = this.CreateUniqueClassOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                //You have to create the database
                context.Database.EnsureClean();
                context.Database.EnsureCreated();
                //context.SeedDatabaseFourBooks();

                //context.ConservationStatuses.Add(new ConservationStatus { ConservationList = "Red", Description = "", CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now });

                context.Users.Add(SharedFunctions.CreateUser("testUser1"));
                context.Users.Add(SharedFunctions.CreateUser("testUser2"));

                context.SaveChanges();

                context.Users.Count().ShouldEqual(2);

                // Arrange
                //*******************
                var userManager = SharedFunctions.InitialiseUserManager(context);
                //**
                var mockRepo = new Mock<INetworkRepository>();

                var mockUnitOfWork = new Mock<IUnitOfWork>();

                var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, userManager);

                string requestingUser = "testUser1";

                string userToUnfollow = requestingUser;

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
                };

                // Act
                var result = await controller.PostUnfollowUserAsync(SharedFunctions.GetTestNetworkUserViewModel(userToUnfollow));

                // Assert
                var objectResult = result as ObjectResult;
                Assert.NotNull(objectResult);
                Assert.IsType<BadRequestObjectResult>(result);
                Assert.True(objectResult is BadRequestObjectResult);
                Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
                var actual = Assert.IsType<string>(objectResult.Value);
                Assert.Equal("Trying to unfollow yourself", actual);
            }
        }

        [Fact]
        public async Task PostUnfollowUserAsync_Returns_500_On_Internal_Error()
        {
            //var options = this.CreateUniqueClassOptions<ApplicationDbContext>();

            //using (var context = new ApplicationDbContext(options))
            //{
            //    //You have to create the database
            //    context.Database.EnsureClean();
            //    context.Database.EnsureCreated();
            //    //context.SeedDatabaseFourBooks();

            //    //context.ConservationStatuses.Add(new ConservationStatus { ConservationList = "Red", Description = "", CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now });

            //    context.Users.Add(SharedFunctions.CreateUser("testUser1"));
            //    context.Users.Add(SharedFunctions.CreateUser("testUser2"));

            //    context.SaveChanges();

            //    context.Users.Count().ShouldEqual(2);

            //    // Arrange
            UserManager<ApplicationUser> userManager = null; //to cause internal error
            var mockRepo = new Mock<INetworkRepository>();
            mockRepo.Setup(repo => repo.Follow(It.IsAny<ApplicationUser>(), It.IsAny<ApplicationUser>()))
                .Verifiable();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.CompleteAsync())
                .ThrowsAsync(new InvalidOperationException());

            var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, userManager);

            string requestingUser = "testUser1";

            string userToUnfollow = "testUser2";

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
            };

            // Act
            var result = await controller.PostUnfollowUserAsync(SharedFunctions.GetTestNetworkUserViewModel(userToUnfollow));

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal($"an unexpected error occurred", objectResult.Value);
        }

        [Fact]
        public async Task PostUnfollowUserAsync_ReturnsOkObject_WhenRequestIsValid()
        {
            var options = this.CreateUniqueClassOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                //You have to create the database
                context.Database.EnsureClean();
                context.Database.EnsureCreated();
                //context.SeedDatabaseFourBooks();

                //context.ConservationStatuses.Add(new ConservationStatus { ConservationList = "Red", Description = "", CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now });

                context.Users.Add(SharedFunctions.CreateUser("testUser1"));
                context.Users.Add(SharedFunctions.CreateUser("testUser2"));

                context.SaveChanges();

                context.Users.Count().ShouldEqual(2);

                // Arrange
                //*******************
                var userManager = SharedFunctions.InitialiseUserManager(context);
                //**
                var mockRepo = new Mock<INetworkRepository>();
                mockRepo.Setup(repo => repo.Follow(It.IsAny<ApplicationUser>(), It.IsAny<ApplicationUser>()))
                    .Verifiable();

                var mockUnitOfWork = new Mock<IUnitOfWork>();
                mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

                var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, userManager);

                string requestingUser = "testUser1";

                string userToUnfollow = "testUser2";

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
                };

                // Act
                var result = await controller.PostUnfollowUserAsync(SharedFunctions.GetTestNetworkUserViewModel(userToUnfollow));

                // Assert
                var objectResult = result as ObjectResult;
                Assert.NotNull(objectResult);
                Assert.IsType<OkObjectResult>(result);
                Assert.True(objectResult is OkObjectResult);
                Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
                Assert.IsType<NetworkUserViewModel>(objectResult.Value);

                var model = objectResult.Value as NetworkUserViewModel;
                Assert.Equal(userToUnfollow, model.UserName);
            }
        }

        #endregion
    }
}
