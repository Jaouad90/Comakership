using Microsoft.IdentityModel.Tokens;

namespace ComakershipsApi.Security {
    public class TokenIdentityValidationParameters : TokenValidationParameters {
        public TokenIdentityValidationParameters(string Issuer, string Audience, SymmetricSecurityKey SecurityKey) {
            RequireSignedTokens = true;
            ValidAudience = Audience;
            ValidateAudience = true;
            ValidIssuer = Issuer;
            ValidateIssuer = true;
            ValidateIssuerSigningKey = true;
            ValidateLifetime = true;
            IssuerSigningKey = SecurityKey;
            AuthenticationType = SecurityDefinition.Bearer.ToString();
        }
    }

}
