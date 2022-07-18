using Data.Constants;
using Data.Entities.SMDEntities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ScheduleManagement.Extensions
{
    public static class ClaimExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(i => i.Type.Equals("Username"));
            if (idClaim != null)
            {
                return idClaim.Value;
            }
            return "";
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(i => i.Type.Equals("Id"));
            if (idClaim != null)
            {
                return idClaim.Value;
            }
            return "";
        }

        public static Role GetRole(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(i => i.Type.Equals("Role"));
            if (idClaim != null)
            {
                return idClaim.Value.Adapt<Role>();
            }
            return Role.UNKNOWN;
        }

        public static CustomUser GetUser(this ClaimsPrincipal claims)
        {
            var user = new CustomUser();
            var idClaim = claims.Claims.FirstOrDefault(i => i.Type.Equals("Role"));
            if (idClaim != null)
            {
                user.Role = idClaim.Value.Adapt<Role>();
            }
            idClaim = claims.Claims.FirstOrDefault(i => i.Type.Equals("Username"));
            if (idClaim != null)
            {
                user.Username = idClaim.Value;
            }
            return user;
        }
    }
}
