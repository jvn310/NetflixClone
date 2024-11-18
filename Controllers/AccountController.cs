using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;

namespace NetflixClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly NetflixCloneDbContext _context;

        public AccountController(NetflixCloneDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser == null)
                {
                    // Hash the password before saving it 
                    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    // Save the user in the database
                    _context.Users.Add(model);
                    _context.SaveChanges();

                    // Redirect to the movie page after successful sign-up
                    return RedirectToAction("HomeNetflix");
                }
                else
                {
                    ModelState.AddModelError("", "Email already registered.");
                }
            }

            return View(model);
        }
    }
}
