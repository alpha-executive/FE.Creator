using FE.Creator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity.Owin;
using FE.Creator.Admin.MVCExtension;
using NLog;
using System.Configuration;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    [UnknownErrorFilter]
    [Authorize]
    public class SystemUserController : ApiController
    {
        private ApplicationUserManager _userManager;
        private static  ILogger logger = LogManager.GetCurrentClassLogger();

        public ApplicationUserManager UserManager
        {
            get
            {
               
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [ResponseType(typeof(IEnumerable<object>))]
        [HttpGet]
        public async Task<IHttpActionResult> List(int? pageIndex = 1, int? pageSize = int.MaxValue)
        {
            logger.Debug("Start List");
            var users = from u in this.UserManager.Users
                        select new
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            Email = u.Email,
                            EmailConfirmed = u.EmailConfirmed,
                            AccessFailedCount = u.AccessFailedCount,
                        };
            logger.Debug("User count: " + users.Count());
            logger.Debug("End List");
            return this.Ok<IEnumerable<object>>(
                    users
                );
        }

        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetUserIdByUserLoginName()
        {
            var userId = (from u in this.UserManager.Users
                        where u.UserName.Equals(this.User.Identity.Name)
                        select u.Id).FirstOrDefault();
            logger.Debug("Login User ID: " + userId);
            return this.Ok<string>(userId);
        }

        [HttpGet]
        [ResponseType(typeof(string))]
        public string GetAdminLoginName()
        {
            var adminUser = ConfigurationManager.AppSettings["SuperAdmin"];

            return adminUser;
        }

        private String GetResetPassword()
        {
            return System.Web.Security.Membership.GeneratePassword(8, 1);
        }
        [HttpPost]
        public async void ResetUserPassword(string id)
        {
            logger.Debug("Start ResetUserPassword");

            if(RequestContext.Principal.Identity.Name.Equals(
                ConfigurationManager.AppSettings["SuperAdmin"], StringComparison.CurrentCultureIgnoreCase))
            {
                throw new UnauthorizedAccessException("Permission Required to Reset password : " + RequestContext.Principal.Identity.Name);
            }

            if (!string.IsNullOrEmpty(id))
            {
                var user = await this.UserManager.FindByIdAsync(id);
                if (user != null)
                {
                    var token = await this.UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    await this.UserManager.ResetPasswordAsync(user.Id, token, GetResetPassword());
                }
            }
            logger.Debug("End ResetUserPassword");
        }
    }
}
