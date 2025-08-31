using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
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

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value
            ?? user.FindFirst("role")!.Value;
        }

        public static int GetPersonId(this ClaimsPrincipal user)
        {

            var raw = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? throw new InvalidOperationException("sub (idPerson) claim missing");

            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                throw new InvalidOperationException("sub (idPerson) claim is not a valid integer");

            return id;
        }

    }
}
