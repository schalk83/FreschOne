﻿using FreschOne.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization; 


namespace FreschOne.Controllers
{
    public class AccountController : BaseController
    {
       // public AccountController(DatabaseHelper dbHelper) : base(dbHelper) { }
        public AccountController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult Login()
        {
            var licenseEndDateString = GetDecodedLicenseKey();
            if (DateTime.TryParse(licenseEndDateString, out DateTime endDate))
            {
                var daysRemaining = (endDate - DateTime.Now).Days;
                if (daysRemaining < 0) daysRemaining = 0; // Avoid negative days

                ViewBag.LicenseStatus = $"License valid until: {endDate:yyyy-MM-dd} <br> (Days remaining: {daysRemaining})";
            }
            else
            {
                ViewBag.LicenseStatus = "❌ Invalid license.";
            }

            return View();
        }



        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var userId = _dbHelper.GetUserID(username);
            if (userId == null)
            {
                ViewBag.Message = "Invalid username";
                return View();
            }

            if (_dbHelper.CheckPasswordReset((long)userId))
            {
                return RedirectToAction("ResetPassword", new { username });
            }

            var user = _dbHelper.AuthenticateUser(username, password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", (int)user.ID);  // 👈 Store login
                HttpContext.Session.SetString("IsLoggedIn", "true");

                _dbHelper.LogUserLogin(user.ID);
                return RedirectToAction("Index", "Home", new { userId });
            }

            ViewBag.Message = "Invalid credentials";
            return View();
        }

        public IActionResult ResetPassword(string username)
        {
            ViewBag.Username = username;
            ViewBag.userId = _dbHelper.GetUserID(username);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(long userId, string username, string newPassword)
        {
            _dbHelper.UpdateUserPassword(userId, newPassword);
            return RedirectToAction("Login");
        }


    }
}