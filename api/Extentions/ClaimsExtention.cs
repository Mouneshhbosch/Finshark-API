using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extentions
{
    public static class ClaimsExtention
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
            return user.Claims.SingleOrDefault(x=>x.Type.Equals(type)).Value;
        }
    }
}