using System;
using System.Security.Claims;

namespace hoteru_be.Services.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetHotelId(this ClaimsPrincipal user)
        {
            var v = user.FindFirst("hotelId")?.Value
                ?? throw new InvalidOperationException("hotelId claim missing");
            return int.Parse(v);
        }
    }
}
