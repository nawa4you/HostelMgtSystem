using System.Diagnostics;
using HostelMgtSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using BCrypt.Net;

namespace HostelMgtSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        private const string ADMIN_RESET_DEFAULT_PASSWORD = "123456";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.HasApprovedRegistration = false;

            if (ViewBag.IsAuthenticated && !ViewBag.IsAdmin)
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userName))
                {
                    
                    ViewBag.HasApprovedRegistration = await _context.Registrations
                                                                    .AnyAsync(r => r.UserName == userName && r.IsApproved);
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Email),
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Role, user.Role),
                            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                            new Claim("Gender", user.Gender),
                            new Claim("Level", user.Level),
                            new Claim("UniqueId", user.UniqueId)
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        _logger.LogInformation($"User {user.Email} logged in with role {user.Role}.");

                        if (user.Role == "Admin")
                        {
                            return RedirectToAction("AdminDashboard");
                        }
                        return RedirectToAction("Saved");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email address.");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUser()
        {
            return View(new RegisterUserViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    return View(model);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, 11);

                var newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Level = model.Level,
                    UniqueId = model.UniqueId,
                    Role = "User"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            ViewBag.Message = $"Welcome, Admin {User.FindFirst(ClaimTypes.Name)?.Value}!";
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult ViewInfo()
        {
            var model = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View("ViewInfo", model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Invalid current password.");
                return View("ViewInfo", model);
            }

            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 11);

            user.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully!";
            Console.WriteLine("Successfully changed password");
            return RedirectToAction("ViewInfo");
        }

        [Authorize(Roles = "User")]
        public IActionResult RegisterHostel()
        {

           
            var viewModel = new RegistrationViewModel
            {
                UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,

                HostelOptions = _context.Hostels
                    .Select(h => new SelectListItem
                    {
                        Value = h.Id.ToString(),
                        Text = h.Name
                    }).ToList(),

                RoomOptions = new List<SelectListItem>()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEditRegistration(RegistrationViewModel model)
        {
            if (!model.Id.HasValue || model.Id.Value == 0) // Creating a new registration
            {
                if (!User.IsInRole("User"))
                {
                    return Forbid();
                }

                model.UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

                // Check if user already has an approved room
                var existingApprovedRoom = await _context.Registrations
                                                         .AnyAsync(r => r.UserName == model.UserName && r.IsApproved);
                if (existingApprovedRoom)
                {
                    TempData["ErrorMessage"] = "You already have an approved room. You cannot register a new one.";
                    // Reload HostelOptions and RoomOptions before returning view
                    model.HostelOptions = _context.Hostels
                        .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Name }).ToList();
                    model.RoomOptions = new List<SelectListItem>(); // Or fetch based on selected hostel
                    ViewData["AllRoomsJson"] = JsonSerializer.Serialize(_context.Rooms
                        .Select(r => new { Id = r.Id, RoomNumber = r.RoomNumber, Type = r.Type, HostelId = r.HostelId, HostelName = r.Hostel!.Name })
                        .ToList());
                    return View("RegisterHostel", model);
                }

                
                if (model.SelectedHostelId.HasValue)
                {
                    var availableRoomsInHostel = await _context.Rooms
                        .CountAsync(r => r.HostelId == model.SelectedHostelId && r.IsAvailable);

                    if (availableRoomsInHostel == 0)
                    {
                        ModelState.AddModelError("SelectedHostelId", "This hostel is currently fully booked out. Please choose another.");
                    }
                }
            }
            else // Editing an existing registration (likely by admin)
            {
                if (!User.IsInRole("Admin"))
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                Registration registration;

                if (model.Id.HasValue && model.Id.Value > 0) // Update existing
                {
                    var registrationToUpdate = await _context.Registrations
                                                       .FirstOrDefaultAsync(r => r.Id == model.Id.Value);

                    if (registrationToUpdate == null)
                    {
                        return NotFound();
                    }

                    registrationToUpdate.UserName = model.UserName; // User name should not change, but keeping for flexibility
                    registrationToUpdate.HostelId = model.SelectedHostelId ?? 0;
                    registrationToUpdate.RoomId = model.SelectedRoomId;
                    // IsApproved status is managed by specific Approve/Reject actions, not here.

                    _context.Registrations.Update(registrationToUpdate);
                }
                else // Add new
                {
                    registration = new Registration
                    {
                        UserName = model.UserName,
                        HostelId = model.SelectedHostelId ?? 0,
                        RoomId = null, // RoomId is null initially, assigned upon admin approval
                        RegisteredAt = DateTime.UtcNow,
                        IsApproved = false // NEW: Always set to false for new requests
                    };
                    _context.Registrations.Add(registration);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room registration request submitted successfully and is pending admin approval.";
                return RedirectToAction("Saved");
            }

            // If ModelState is not valid, re-populate select list options before returning view
            model.HostelOptions = _context.Hostels
                .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Name }).ToList();

            if (model.SelectedHostelId.HasValue)
            {
                model.RoomOptions = _context.Rooms
                    .Where(r => r.HostelId == model.SelectedHostelId.Value)
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = $"{r.RoomNumber} ({r.Type}) - {r.Hostel!.Name}"
                    }).ToList();
            }
            else
            {
                model.RoomOptions = new List<SelectListItem>();
            }

            ViewData["AllRoomsJson"] = JsonSerializer.Serialize(_context.Rooms
                .Select(r => new { Id = r.Id, RoomNumber = r.RoomNumber, Type = r.Type, HostelId = r.HostelId, HostelName = r.Hostel!.Name })
                .ToList());

            if (model.Id.HasValue && model.Id.Value > 0)
            {
                return View("Edit", model);
            }
            return View("RegisterHostel", model);
        }

        [Authorize]
        public async Task<IActionResult> Saved()
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            IQueryable<Registration> registrationsQuery = _context.Registrations;


            currentUserName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (currentUserRole != "Admin")
            {
                registrationsQuery = registrationsQuery.Where(r =>
                    r.UserName == currentUserName);
            }



            var registrationsFromDb = await registrationsQuery
                .Include(r => r.Hostel)
                .Include(r => r.Room)
                .OrderByDescending(r => r.RegisteredAt) 
                .ToListAsync();



           
            var registrations = registrationsFromDb.Select(r => new
            {
                r.Id,
                r.UserName,
                HostelName = r.Hostel != null ? r.Hostel.Name : "N/A",
                RoomNumber = r.Room != null ? r.Room.RoomNumber : "Pending (Admin Approval)",
                RoomType = r.Room != null ? r.Room.Type : "N/A",
                RegisteredAt = r.RegisteredAt,
                r.IsApproved 
            }).ToList();

            ViewBag.IsAdmin = currentUserRole == "Admin";

            
            bool hasActionsColumn = false;
            if (ViewBag.IsAdmin)
            {
                
                hasActionsColumn = registrations.Any(); // Only show if there's any registration at all
            }
            else
            {
                
                hasActionsColumn = registrations.Any(r => !r.IsApproved);
            }
            ViewBag.HasActionsColumn = hasActionsColumn;


            return View(registrations);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRegistration(int id)
        {
            var registrationToApprove = await _context.Registrations
                                                      .Include(r => r.Room)
                                                      .FirstOrDefaultAsync(r => r.Id == id && !r.IsApproved);

            if (registrationToApprove == null)
            {
                TempData["ErrorMessage"] = "Registration not found or already approved.";
                return RedirectToAction("Saved");
            }

            
            if (!registrationToApprove.RoomId.HasValue)
            {
                var availableRoom = await _context.Rooms
                    .Where(r => r.HostelId == registrationToApprove.HostelId && r.IsAvailable)
                    .FirstOrDefaultAsync();

                if (availableRoom == null)
                {
                    TempData["ErrorMessage"] = $"No available rooms in {registrationToApprove.Hostel?.Name}. Cannot approve this request.";
                    return RedirectToAction("Saved");
                }

                registrationToApprove.RoomId = availableRoom.Id;
                availableRoom.IsAvailable = false; 
            }

            registrationToApprove.IsApproved = true; 

            // Delete ALL other pending requests for THIS user
            var otherPendingRequests = await _context.Registrations
                .Where(r => r.UserName == registrationToApprove.UserName &&
                            r.Id != id && 
                            !r.IsApproved) 
                .ToListAsync();

            _context.Registrations.RemoveRange(otherPendingRequests); // Delete others

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Registration for {registrationToApprove.UserName} has been approved. Other pending requests for this user have been deleted.";
            return RedirectToAction("Saved");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectRegistration(int id)
        {
            var registrationToReject = await _context.Registrations
                                                     .FirstOrDefaultAsync(r => r.Id == id && !r.IsApproved);

            if (registrationToReject == null)
            {
                TempData["ErrorMessage"] = "Registration not found or already approved/rejected.";
                return RedirectToAction("Saved");
            }

            _context.Registrations.Remove(registrationToReject);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Registration for {registrationToReject.UserName} has been rejected and deleted.";
            return RedirectToAction("Saved");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var registration = _context.Registrations
                .Include(r => r.Hostel)
                .Include(r => r.Room)
                .FirstOrDefault(r => r.Id == id);

            if (registration == null) return NotFound();

            var viewModel = new RegistrationViewModel
            {
                Id = registration.Id,
                UserName = registration.UserName,
                SelectedHostelId = registration.HostelId,
                SelectedRoomId = registration.RoomId,
            };

            viewModel.HostelOptions = _context.Hostels
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                }).ToList();

            viewModel.RoomOptions = _context.Rooms
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.RoomNumber} ({r.Type}) - {r.Hostel!.Name}"
                }).ToList();

            ViewData["AllRoomsJson"] = JsonSerializer.Serialize(_context.Rooms
                .Select(r => new { Id = r.Id, RoomNumber = r.RoomNumber, Type = r.Type, HostelId = r.HostelId, HostelName = r.Hostel!.Name })
                .ToList());

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var registration = _context.Registrations.Find(id);
            if (registration != null)
            {
                // If a room was assigned to this registration, mark it available again
                if (registration.RoomId.HasValue)
                {
                    var room = _context.Rooms.Find(registration.RoomId.Value);
                    if (room != null)
                    {
                        room.IsAvailable = true;
                    }
                }
                _context.Registrations.Remove(registration);
                _context.SaveChanges();
            }
            TempData["SuccessMessage"] = "Registration deleted successfully.";
            return RedirectToAction("Saved");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestModel requestModel)
        {
            //string? authenticatedUserEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //if (!string.IsNullOrEmpty(authenticatedUserEmail) && authenticatedUserEmail != requestModel.Email)
            //{
            //    return Json(new { success = false, message = "Unauthorized request: Email mismatch for authenticated user." });
            //}

            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == requestModel.Email);
            //if (user == null)
            //{
            //    _logger.LogWarning($"Attempted password reset request for non-existent email: {requestModel.Email}");
            //    return Json(new { success = true, message = "If an account exists for that email address, a password reset request has been initiated." });
            //}

            //var existingPendingRequests = await _context.PasswordResetRequests
            //    .Where(r => r.UserEmail == requestModel.Email && !r.IsProcessed)
            //    .ToListAsync();
            //foreach (var req in existingPendingRequests)
            //{
            //    _context.PasswordResetRequests.Remove(req);
            //}
            //await _context.SaveChangesAsync();

            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();
            DateTime codeExpiry = DateTime.UtcNow.AddMinutes(5);

            var newRequest = new PasswordResetRequest
            {
                UserEmail = requestModel.Email,
                RequestDate = DateTime.UtcNow,
                IsProcessed = false,
                VerificationCode = verificationCode,
                CodeExpiry = codeExpiry
            };

            _context.PasswordResetRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password reset request initiated for {requestModel.Email}. Code: {verificationCode}, Expires: {codeExpiry}.");

            return Json(new { success = true, email = requestModel.Email, verificationCode = verificationCode });
        }

        public class PasswordResetRequestModel
        {
            public string Email { get; set; } = string.Empty;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                TempData["SuccessMessage"] = "If an account exists for that email address, a password reset request has been initiated.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var existingPendingRequests = await _context.PasswordResetRequests
                .Where(r => r.UserEmail == model.Email && !r.IsProcessed)
                .ToListAsync();
            foreach (var req in existingPendingRequests)
            {
                _context.PasswordResetRequests.Remove(req);
            }
            await _context.SaveChangesAsync();

            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();
            DateTime codeExpiry = DateTime.UtcNow.AddMinutes(5);

            var newRequest = new PasswordResetRequest
            {
                UserEmail = model.Email,
                RequestDate = DateTime.UtcNow,
                IsProcessed = false,
                VerificationCode = verificationCode,
                CodeExpiry = codeExpiry
            };

            _context.PasswordResetRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password reset request initiated from ForgotPassword page for {model.Email}. Code: {verificationCode}, Expires: {codeExpiry}.");

            return RedirectToAction("VerifyCode", new { email = model.Email, code = verificationCode });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyCode(string email, string code)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid request. Please start the password reset process again.";
                return RedirectToAction("ForgotPassword");
            }

            var viewModel = new VerifyCodeViewModel
            {
                Email = email,
                GeneratedCodeForDisplay = code
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var pendingRequest = await _context.PasswordResetRequests
                    .FirstOrDefaultAsync(r => r.UserEmail == model.Email && !r.IsProcessed);
                if (pendingRequest != null)
                {
                    model.GeneratedCodeForDisplay = pendingRequest.VerificationCode;
                }
                return View(model);
            }

            var passwordResetRequest = await _context.PasswordResetRequests
                .FirstOrDefaultAsync(r => r.UserEmail == model.Email && !r.IsProcessed);

            if (passwordResetRequest == null ||
                passwordResetRequest.VerificationCode != model.EnteredCode ||
                passwordResetRequest.CodeExpiry <= DateTime.UtcNow)
            {
                ModelState.AddModelError(nameof(model.EnteredCode), "Invalid or expired verification code.");
                if (passwordResetRequest != null)
                {
                    model.GeneratedCodeForDisplay = passwordResetRequest.VerificationCode;
                }
                return View(model);
            }

            TempData["ResetEmail"] = model.Email;
            return RedirectToAction("SetNewPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SetNewPassword()
        {
            string? email = TempData["ResetEmail"] as string;
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Password reset session expired or invalid. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            var viewModel = new SetNewPasswordViewModel { Email = email };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNewPassword(SetNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found for password reset.");
                return View(model);
            }

            var pendingRequest = await _context.PasswordResetRequests
                .FirstOrDefaultAsync(r => r.UserEmail == model.Email && !r.IsProcessed);

            if (pendingRequest == null || pendingRequest.CodeExpiry <= DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "Password reset session expired or invalid. Please start again.");
                return View(model);
            }

            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 11);
            user.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync();

            pendingRequest.IsProcessed = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your password has been reset successfully!";
            return RedirectToAction("Login");
        }

        // --- ManageUsers Action for Admin Dashboard ---
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.OrderBy(u => u.Name).ToListAsync();
            return View(users);
        }

        // --- Admin-Initiated Password Reset Actions ---
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetUserPassword()
        {
            var pendingRequestEmails = await _context.PasswordResetRequests
                                                    .Where(r => !r.IsProcessed)
                                                    .Select(r => r.UserEmail)
                                                    .Distinct()
                                                    .ToListAsync();

            var usersWithPendingRequests = await _context.Users
                                                        .Where(u => pendingRequestEmails.Contains(u.Email))
                                                        .OrderBy(u => u.Email)
                                                        .ToListAsync();

            var viewModel = new AdminResetPasswordViewModel
            {
                UsersList = usersWithPendingRequests.Select(u => new SelectListItem
                {
                    Value = u.Email,
                    Text = $"{u.Name} ({u.Email})"
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetUserPassword(AdminResetPasswordViewModel model)
        {
            var pendingRequestEmails = await _context.PasswordResetRequests
                                                    .Where(r => !r.IsProcessed)
                                                    .Select(r => r.UserEmail)
                                                    .Distinct()
                                                    .ToListAsync();

            model.UsersList = await _context.Users
                                            .Where(u => pendingRequestEmails.Contains(u.Email))
                                            .OrderBy(u => u.Email)
                                            .Select(u => new SelectListItem
                                            {
                                                Value = u.Email,
                                                Text = $"{u.Name} ({u.Email})"
                                            }).ToListAsync();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please select a user to reset their password.";
                return View(model);
            }

            var userToReset = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.SelectedUserEmail);

            if (userToReset == null)
            {
                TempData["ErrorMessage"] = "Selected user not found.";
                return View(model);
            }

            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(ADMIN_RESET_DEFAULT_PASSWORD, 11);

            userToReset.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync();

            var requestsToMarkProcessed = await _context.PasswordResetRequests
                                                        .Where(r => r.UserEmail == model.SelectedUserEmail && !r.IsProcessed)
                                                        .ToListAsync();
            foreach (var req in requestsToMarkProcessed)
            {
                req.IsProcessed = true;
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Password for {userToReset.Email} has been reset to '{ADMIN_RESET_DEFAULT_PASSWORD}'. Please inform the user to change it immediately after logging in.";
            return RedirectToAction("ResetUserPassword");
        }
    }
};