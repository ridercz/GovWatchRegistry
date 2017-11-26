using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Altairis.GovWatch.Registry.Data {
    public class ApplicationRole : IdentityRole<int> {

        public const string MonitorRoleName = "Monitor";
        public const string UserRoleName = "User";
        public const string AdminRoleName = "Admin";

    }
}
