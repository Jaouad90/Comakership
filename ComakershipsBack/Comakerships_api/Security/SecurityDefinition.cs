using System;
using System.Collections.Generic;
using System.Text;

namespace ComakershipsApi.Security {
    public enum SecurityDefinition {
        WebJobsAuthLevel, // Defined by default by the Azure functions runtime
        Bearer, // We will use this for our JWT tokens
    }

}
