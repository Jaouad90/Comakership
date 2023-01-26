using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ComakershipsApi.Security {
    public class BearerAuthorizationService : IAuthorizationService {
        public BearerAuthorizationService() {
            //
        }

        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal User, object Resource,
                                                              IEnumerable<IAuthorizationRequirement> Requirements) {
            if (User.Identity.IsAuthenticated) {
                return await Task.FromResult(AuthorizationResult.Success());
            } else {
                return await Task.FromResult(AuthorizationResult.Failed());
            }
        }

        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName) {
            return await Task.FromResult(AuthorizationResult.Failed());
        }
    }

}
