﻿using Birder.Controllers;
using Birder.Data.Model;
using Birder.Services;
using Birder.TestsHelpers;
using Birder.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Birder.Tests.Controller
{
    public class LifeListTests
    {
        private readonly Mock<ILogger<ListController>> _logger;
        private readonly ISystemClockService _systemClock;

        public LifeListTests()
        {
            _systemClock = new SystemClockService();
            _logger = new Mock<ILogger<ListController>>();
        }

        [Fact]
        public async Task Returns_Ok_With_Viewmodel()
        {
            // Arrange
            var mockListService = new Mock<IListService>();
            mockListService.Setup(obs => obs.GetLifeListsAsync(It.IsAny<Expression<Func<Observation, bool>>>()))
                    .ReturnsAsync(new List<LifeListViewModel>());

            var controller = new ListController(_logger.Object, _systemClock, mockListService.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                { User = SharedFunctions.GetTestClaimsPrincipal("test_username") }
            };

            // Act
            var result = await controller.GetLifeListAsync();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var actual = Assert.IsAssignableFrom<IEnumerable<LifeListViewModel>>(objectResult.Value);
        }

        [Fact]
        public async Task Returns_500_When_Exception_Is_Raised()
        {
            // Arrange
            var mockListService = new Mock<IListService>();
            mockListService.Setup(obs => obs.GetLifeListsAsync(It.IsAny<Expression<Func<Observation, bool>>>()))
                .ThrowsAsync(new InvalidOperationException());

            var controller = new ListController(_logger.Object, _systemClock, mockListService.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                { User = SharedFunctions.GetTestClaimsPrincipal("test_username") }
            };

            // Act
            var result = await controller.GetLifeListAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var actual = Assert.IsType<string>(objectResult.Value);
            Assert.Equal($"an unexpected error occurred", actual);
        }

        [Fact]
        public async Task Returns_Unauthorised_When_Username_Is_Empty()
        {
            // Arrange
            var mockListService = new Mock<IListService>();

            var controller = new ListController(_logger.Object, _systemClock, mockListService.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                { User = SharedFunctions.GetTestClaimsPrincipal(string.Empty) }
            };

            // Act
            var result = await controller.GetLifeListAsync();

            // Assert
            var objectResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);
            var actual = Assert.IsType<string>(objectResult.Value);
            Assert.Equal($"requesting username is null or empty", actual);
        }

        [Fact]
        public async Task Returns_500_When_Repository_Returns_Null()
        {
            // Arrange
            var mockListService = new Mock<IListService>();
            mockListService.Setup(obs => obs.GetLifeListsAsync(It.IsAny<Expression<Func<Observation, bool>>>()))
                    .Returns(Task.FromResult<IEnumerable<LifeListViewModel>>(null));

            var controller = new ListController(_logger.Object, _systemClock, mockListService.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                { User = SharedFunctions.GetTestClaimsPrincipal("test_username") }
            };

            // Act
            var result = await controller.GetLifeListAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var actual = Assert.IsType<string>(objectResult.Value);
            Assert.Equal($"listService returned null", actual);
        }
    }
}

