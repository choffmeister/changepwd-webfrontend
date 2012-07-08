using System;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using ChangeUserPassword.Models;

namespace ChangeUserPassword.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new PrincipalContext(ContextType.Machine))
                    {
                        using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, model.UserName))
                        {
                            if (user != null)
                            {
                                user.ChangePassword(model.OldPassword, model.NewPassword);
                            }
                            else
                            {
                                ModelState.AddModelError("", "The user could not be found");
                                return View(model);
                            }
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (PasswordException)
                {
                    ModelState.AddModelError("", "The old password provided is not correct");
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occured while trying to change the password (" + ex.Message + ")");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
    }
}