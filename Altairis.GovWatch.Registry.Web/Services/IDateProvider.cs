using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.GovWatch.Registry.Web.Services {
    public interface IDateProvider {

        DateTime Now { get; }

        DateTime Today { get; }

    }
}
