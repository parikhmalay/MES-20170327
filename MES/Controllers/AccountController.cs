using Account.DTO.Library;
using MES.Business.Library;
using MES.Business.Library.Extensions;
using MES.DTO.Library.Identity;
using MES.Identity.Data.Library;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class AccountController : SecuredWebControllerBase
    {
        public UserManager<MES.Identity.Data.Library.User> UserManager { get; private set; }

        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            using (UserStore u = new UserStore(new IdentityContext()))
            {
                using (UserManager userManager = new UserManager(u))
                {
                    User user = userManager.FindByName(User.Identity.Name);
                    if (user.IsDeleted == true || user.Active == false)
                    {
                        Request.GetOwinContext().Authentication.SignOut();
                        ViewBag.StatusMessageFlag = "block";
                        ViewBag.StatusMessage = Languages.GetResourceText("PasswordResetSuccess");
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.Culture.ToString());
                        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(user.Culture.ToString());
                        HttpCookie cookieCulture = new HttpCookie("culture", user.Culture.ToString());
                        cookieCulture.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookieCulture);
                    }
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = MES.Extended.ControllerExtensions.GetCurrentUser();
                if (currentUser != null)
                {
                    if (currentUser.Preferences != null && currentUser.Preferences.DefaultLandingPageId.HasValue)
                    {

                        return RedirectToAction(currentUser.Preferences.DefaultAction, currentUser.Preferences.DefaultController);
                    }
                    else
                    {
                        if (currentUser.DefaultObjectId.HasValue && currentUser.DefaultObjectId > 0)
                        {

                            return RedirectToAction(currentUser.DefaultAction, currentUser.DefaultController);
                        }
                    }
                }

                return RedirectToAction("Dashboard", "Dashboard", new { area = "" });

            }
            //return RedirectToAction("Index", "Home");
            return View();
        }

        public ActionResult LogOff()
        {
            Request.Cookies.Remove("culture");
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        //Controllers for the reset page
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            ViewBag.StatusMessageFlag = "none";
            ViewBag.StatusMessage = string.Empty;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ResetPassword(ResetPasswordModel Model)
        {

            string newPassword = GetPassword(8);
            try
            {
                ViewBag.StatusMessageFlag = "block";

                using (UserStore u = new UserStore(new IdentityContext()))
                {
                    using (UserManager userManager = new UserManager(u))
                    {
                        User user = userManager.FindByName(Model.UserName);
                        if (ModelState.IsValid && user != null)
                        {
                            string hashedNewPassword = userManager.PasswordHasher.HashPassword(newPassword);
                            userManager.RemovePassword(user.Id);
                            var res = userManager.AddPassword(user.Id, newPassword);

                            ViewBag.StatusMessage = (res.Succeeded) ? Languages.GetResourceText("PasswordResetSuccess")
                                          : Languages.GetResourceText("PasswordResetFailed") + "-" + string.Join(",", res.Errors.ToList().ToArray());

                            if (res.Succeeded)
                            {
                                /*Send Email*/
                                DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate = new DTO.Library.Setup.EmailTemplate.EmailTemplate();

                                emailTemplate.TestEmailAddress = user.Email;
                                emailTemplate.EmailSubject = "Reset Password";
                                emailTemplate.EmailBody = "<p>User Name: " + Model.UserName + "</p><p>Password: " + newPassword + "</p>";
                                bool IsSuccess = false;
                                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                                List<string> lstToAddress = new List<string>();
                                lstToAddress.Add(emailTemplate.TestEmailAddress);
                                emailRepository.SendEmail(lstToAddress, "", emailTemplate.EmailSubject, emailTemplate.EmailBody, out IsSuccess, null);

                                if (IsSuccess)
                                {

                                    ViewBag.StatusMessage = Languages.GetResourceText("PasswordResetSuccess");
                                }
                            }
                            else
                            {
                                ViewBag.StatusMessage = res.Errors;
                            }
                        }
                        else
                        {
                            ViewBag.StatusMessage = Languages.GetResourceText("InvalidUserName");
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                ViewBag.StatusMessage = RepositoryExtensions.GetEntityValidationException(e);
            }
            catch (Exception ex)
            {

                ViewBag.StatusMessage = ex.ToString();
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult doesnotexist(string UserName)
        {
            User user = UserManager.FindByName(UserName);
            return Json(user != null);
        }
        private string GetPassword(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var newPassword = new String(stringChars);

            return newPassword;
        }
    }
}