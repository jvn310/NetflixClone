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

    public IActionResult WhoIsWatching()
    {
        int userId = 1;

        // Fetch profiles for the logged-in user
        var profiles = _profileService.GetProfilesByUserId(userId);

        profiles ??= new List<Profile>();

        return View("~/Views/Home/WhoIsWatching.cshtml",profiles);

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

        return RedirectToAction("WhoIsWatching");
    }

    [HttpPost]
    public IActionResult SelectProfile(int profileId)
    {
        var selectedProfile = _profileService.GetProfileById(profileId);

        if (selectedProfile != null)
        {
            HttpContext.Session.SetString("SelectedProfileName", selectedProfile.Name);
            HttpContext.Session.SetString("SelectedProfileIcon", selectedProfile.IconUrl);
        }

        return RedirectToAction("HomeNetflix", "Home");
    }

    public IActionResult ManageProfiles()
    {
        var profiles = _profileService.GetAllProfiles(); 
        return View(profiles); 
    }
}
