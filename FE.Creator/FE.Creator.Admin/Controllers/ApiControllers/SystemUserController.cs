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

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    [UnknownErrorFilter]
    [Authorize]
    public class SystemUserController : ApiController
    {
        private ApplicationUserManager _userManager;

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
            var users = from u in this.UserManager.Users
                        select new
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            Email = u.Email,
                            EmailConfirmed = u.EmailConfirmed,
                            AccessFailedCount = u.AccessFailedCount,
                        };

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

            return this.Ok<string>(userId);
        }

        private String GetResetPassword()
        {
            return "1qaz!QAZ";
        }
        [HttpPost]
        public async void ResetUserPassword(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await this.UserManager.FindByIdAsync(id);
                if (user != null)
                {
                    var token = await this.UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    await this.UserManager.ResetPasswordAsync(user.Id, token, GetResetPassword());
                }
            }
        }
    }
}
