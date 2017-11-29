using System;

namespace Altairis.GovWatch.Registry.Web.Services {
    public interface IDateProvider {

        DateTime Now { get; }

        DateTime Today { get; }

    }
}
