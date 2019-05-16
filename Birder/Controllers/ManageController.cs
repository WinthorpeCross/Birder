﻿using AutoMapper;
using Birder.Data.Model;
using Birder.Services;
using Birder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Birder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ManageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISystemClock _systemClock;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageController(IMapper mapper,
                                ISystemClock systemClock,
                                UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _systemClock = systemClock;
            _userManager = userManager;
        }

        [HttpGet, Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var viewModel = _mapper.Map<ApplicationUser, ManageProfileViewModel>(user);

            return Ok(viewModel);

        }

        [HttpPost, Route("UpdateProfile")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ManageProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            //var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("GetUser", $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return BadRequest(ModelState);
            }

            var userName = user.UserName;
            if (model.UserName != userName)
            {
                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    // Can use this in check username...
                    ModelState.AddModelError("Username", $"Username '{model.UserName}' is already taken.");

                    return BadRequest(ModelState);
                }
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    //throw new ApplicationException($"Unexpected error occurred setting username for user with ID '{user.Id}'.");
                    ModelState.AddModelError("Username", $"Unexpected error occurred setting username for user with ID '{user.Id}'.");
                    return BadRequest(ModelState);
                }
            }


            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    ModelState.AddModelError("Email", $"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                    return BadRequest(ModelState);
                }
                // send confirm email message
            }

            //***********************************************


            //***********************************************

            //if (model.ProfileImage != null)
            //{
            //    try
            //    {
            //        string filepath = string.Concat(user.UserName, Path.GetExtension(model.ProfileImage.FileName.ToString()));
            //        var imageArray = await _stream.GetByteArray(model.ProfileImage);
            //        imageArray = _stream.ResizePhoto(imageArray, 64, 64);
            //        var imageUpload = _imageService.StoreProfileImage(filepath, imageArray, "profile");

            //        imageUpload.Wait();
            //        if (imageUpload.IsCompletedSuccessfully == true)
            //        {
            //            user.ProfileImage = imageUpload.Result;
            //        }
            //    }
            //    catch
            //    {
            //        ModelState.AddModelError("ProfileImage", $"Unexpected error occurred processing the profile photo for user with ID '{user.Id}'.");
            //return BadRequest(ModelState);
            //    }
            //}

            await _userManager.UpdateAsync(user);

            return Ok(model);

            //StatusMessage = "Your profile has been updated";
            //return RedirectToAction(nameof(Index));
        }

        [HttpPost, Route("SetLocation")]
        public async Task<IActionResult> SetLocation(SetLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            //var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return BadRequest(ModelState);
            }

            var coOrdinate = user.DefaultLocationLatitude + "," + user.DefaultLocationLongitude;
            if (model.DefaultLocationLatitude + "," + model.DefaultLocationLongitude != coOrdinate)
            {
                user.DefaultLocationLatitude = model.DefaultLocationLatitude;
                user.DefaultLocationLongitude = model.DefaultLocationLongitude;

                var setCoOrdinate = await _userManager.UpdateAsync(user);
                if (!setCoOrdinate.Succeeded)
                {
                    //throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                    return BadRequest(ModelState);
                }
            }

            return Ok(model);
        }


        [HttpPost, Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(model);
        }
    }
}