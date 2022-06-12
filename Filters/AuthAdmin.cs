using KitapSatis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KitapSatis.Filters
{
    public class AuthAdmin : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentSession.User==null || (CurrentSession.User != null && CurrentSession.User.RolID != 1))
            {
                filterContext.Result = new RedirectResult("/Admin/HomePage/Login"); //admin değilse
            }
        }
    }
}