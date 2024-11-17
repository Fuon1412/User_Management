using Microsoft.IdentityModel.Tokens;
using back_end.Models.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security;

namespace back_end.Utils
{
    public class JwtTokenGenerator
    {
        private readonly string _jwtKey;
        private readonly string _issuer;

        public JwtTokenGenerator(string jwtKey, string issuer)
        {
            _jwtKey = jwtKey;
            _issuer = issuer;
        }
        // Generate JWT Token khi user dang nhap hoac dang ky 1 account moi
        public string GenerateJwtToken(User account)
        {
            if (string.IsNullOrEmpty(_jwtKey))
            {
                throw new ArgumentNullException(nameof(_jwtKey), "JWT Key is not configured properly.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", account.Id),
                new Claim("Status", account.Status),
                new Claim("Username", account.Username),
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Kiem tra token co hop le hay khong
        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtKey);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = _issuer,

                    ValidateAudience = true,
                    ValidAudience = _issuer,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                var claimPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken &&
                        jwtSecurityToken.Header.Alg.Equals(
                            SecurityAlgorithms.HmacSha256,
                            StringComparison.InvariantCultureIgnoreCase
                        )
                    )
                {
                    return claimPrincipal;

                }

                else throw new SecurityException("Token is not valid");
            }
            catch (Exception e)
            {
                throw new SecurityException($"Error while validating token: {e.Message}");
            }
        }
    }
}
