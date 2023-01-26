using ComakershipsApi.Security;
using ComakershipsApi.Utils;
using Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ComakershipsApi.Services {
    public interface ITokenService {
        Task<Token> CreateToken(Login Login, UserBody user);
        Task<ClaimsPrincipal> GetByValue(string Value);
    }

    public class TokenService : ITokenService {
        private ILogger Logger { get; }

        private string Issuer { get; }
        private string Audience { get; }
        private TimeSpan ValidityDuration { get; }

        private SigningCredentials Credentials { get; }
        private TokenIdentityValidationParameters ValidationParameters { get; }

        public TokenService(IConfiguration Configuration, ILogger<TokenService> Logger) {
            this.Logger = Logger;

            Issuer = Configuration.GetClassValueChecked("JWTIssuer", "DebugIssuer", Logger);
            Audience = Configuration.GetClassValueChecked("JWTAudience", "DebugAudience", Logger);
            ValidityDuration = TimeSpan.FromDays(1);// Todo: configure
            string Key = Configuration.GetClassValueChecked("JWTKey", "DebugKey DebugKey", Logger);

            SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

            Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

            ValidationParameters = new TokenIdentityValidationParameters(Issuer, Audience, SecurityKey);
        }

        public async Task<Token> CreateToken(Login Login, UserBody user) {
            if(user is StudentUser) {
                var studentUser = (StudentUser)user;
                return await CreateToken(new Claim[] {
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.Name, studentUser.Name),
                    new Claim(ClaimTypes.Email, studentUser.Email),
                    new Claim("UniversityId", studentUser.UniversityId.ToString()),
                    new Claim("UserType", "StudentUser"),
                    new Claim("UserId", studentUser.Id.ToString())
                });
            } else {
                var companyUser = (CompanyUser)user;
                return await CreateToken(new Claim[] {
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.Name, companyUser.Name),
                    new Claim(ClaimTypes.Email, companyUser.Email),
                    new Claim("CompanyId", companyUser.CompanyId.ToString()),
                    new Claim("UserType", "CompanyUser"),
                    new Claim("UserId", companyUser.Id.ToString()),
                    new Claim("IsCompanyAdmin", companyUser.IsCompanyAdmin.ToString())
                });
            }
        }

        private async Task<Token> CreateToken(Claim[] Claims) {
            JwtHeader Header = new JwtHeader(Credentials);

            JwtPayload Payload = new JwtPayload(Issuer, Audience, Claims, DateTime.UtcNow,
                                                DateTime.UtcNow.Add(ValidityDuration), DateTime.UtcNow);

            JwtSecurityToken SecurityToken = new JwtSecurityToken(Header, Payload);

            return await Task.FromResult(CreateToken(SecurityToken));
        }

        private Token CreateToken(JwtSecurityToken SecurityToken) {
            return new Token(SecurityToken);
        }

        public async Task<ClaimsPrincipal> GetByValue(string Value) {
            if (Value == null) {
                throw new Exception("No Token supplied");
            }

            JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler();

            try {
                SecurityToken ValidatedToken;
                ClaimsPrincipal Principal = Handler.ValidateToken(Value, ValidationParameters, out ValidatedToken);

                return await Task.FromResult(Principal);
            } catch (Exception e) {
                throw e;
            }
        }
    }
}
