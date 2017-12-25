using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Mvc;

namespace Altairis.GovWatch.Registry.Web.Components {
    public class MenuViewComponent : ViewComponent {

        public IViewComponentResult Invoke() {
            if (!this.User.Identity.IsAuthenticated) {
                return this.View("Anonymous");
            }
            else if (this.User.IsInRole(ApplicationRole.AdministratorRoleName)) {
                return this.View("Administrator");
            } else if (this.User.IsInRole(ApplicationRole.OperatorRoleName)) {
                return this.View("Operator");
            } else {
                throw new Exception("Unsupported user role for menu display");
            }
        }

    }
}
