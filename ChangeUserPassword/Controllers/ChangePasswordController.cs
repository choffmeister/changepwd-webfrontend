using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Security.Authentication;
using System.Web.Mvc;
using ChangeUserPassword.Models;

namespace ChangeUserPassword.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly string _domainDisplayName;
        private readonly ContextType _contextType;
        private readonly string _domain;
        private readonly string _container;
        private readonly IdentityType _identityType;

        public ChangePasswordController()
        {
            _domainDisplayName = ConfigurationManager.AppSettings["domainDisplayName"];
            _contextType = (ContextType)Enum.Parse(typeof(ContextType), ConfigurationManager.AppSettings["contextType"], true);
            _domain = ConfigurationManager.AppSettings["domain"];
            _container = ConfigurationManager.AppSettings["container"];
            _identityType = (IdentityType)Enum.Parse(typeof(IdentityType), ConfigurationManager.AppSettings["identityType"], true);
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DomainName = _domainDisplayName;

            return View();
        }

        [HttpPost]
        public ActionResult Index(ChangePasswordModel model)
        {
            ViewBag.DomainName = _domainDisplayName;

            if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new PrincipalContext(_contextType, _domain, _container, model.UserName, model.CurrentPassword))
                    {
                        using (var user = UserPrincipal.FindByIdentity(context, _identityType, model.UserName))
                        {
                            if (user != null)
                            {
                                user.ChangePassword(model.CurrentPassword, model.NewPassword);
                            }
                            else
                            {
                                ModelState.AddModelError("", "The user could not be found.");
                                return View(model);
                            }
                        }
                    }

                    ViewBag.Succeeded = true;
                    return View();
                }
                catch (AuthenticationException)
                {
                    ModelState.AddModelError("", "Username or current password is incorrect.");
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error of type " + ex.GetType().FullName + " occured while trying to change the password. " + ex.Message.Trim());
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