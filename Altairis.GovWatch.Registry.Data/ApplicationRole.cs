using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Altairis.GovWatch.Registry.Data {
    public class ApplicationRole : IdentityRole<int> {

        public const string MonitorRoleName = "Monitor";
        public const string OperatorRoleName = "Operator";
        public const string AdministratorRoleName = "Administrator";

    }
}
