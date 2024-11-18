using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Services;
using System.Collections.Generic;
using NetflixClone.Data;

public class ProfileController : Controller
{
    private readonly ProfileService _profileService;
    private readonly NetflixCloneDbContext _context;

    public ProfileController(ProfileService profileService, NetflixCloneDbContext context)
    {
        _profileService = profileService;
        _context = context;
    }

    [HttpPost]
    public IActionResult AddProfile(string profileName, string profileColor)
    {
        int userId = 1;

        var newProfile = new Profile
        {
            Name = profileName,
            IconUrl = profileColor,
            UserId = userId
        };

        _context.Profiles.Add(newProfile);
        _context.SaveChanges();

        return RedirectToAction("WhoIsWatching", "Home");
    }

    [HttpPost]
    public IActionResult SelectProfile([FromBody] int profileId)
    {
        var profile = _context.Profiles.FirstOrDefault(p => p.Id == profileId);

        if (profile != null)
        {
            HttpContext.Session.SetInt32("SelectedProfileId", profile.Id);
            HttpContext.Session.SetString("SelectedProfileName", profile.Name);
            return Json(new { success = true });
        }

        Console.WriteLine($"Profile not found for ID: {profileId}");
        return Json(new { success = false, message = "Profile not found." });
    }

    public IActionResult ManageProfiles()
    {
        var profiles = _profileService.GetAllProfiles();
        ViewData["BodyClass"] = "black-background";
        ViewData["Page"] = "Home";
        ViewData["ShowSignIn"] = false;
        return View("~/Views/Home/ManageProfiles.cshtml", profiles); 
    }

    [HttpPost]
    public IActionResult EditProfile([FromBody] Profile updatedProfile)
    {
        var profile = _context.Profiles.FirstOrDefault(p => p.Id == updatedProfile.Id);
        if (profile == null) return NotFound();

        profile.Name = updatedProfile.Name;
        profile.IconUrl = updatedProfile.IconUrl;
        _context.SaveChanges();

        return Ok();
    }

    [HttpPost]
    public IActionResult DeleteProfile([FromBody] int id)
    {
        try
        {
            var profile = _context.Profiles.FirstOrDefault(p => p.Id == id);
            if (profile == null)
            {
                return Json(new { success = false, message = "Profile not found." });
            }

            _context.Profiles.Remove(profile);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
