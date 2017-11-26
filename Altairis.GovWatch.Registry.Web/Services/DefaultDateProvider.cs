using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.GovWatch.Registry.Web.Services {
    public class DefaultDateProvider : IDateProvider {

        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;

    }
}
