using System;

namespace Altairis.GovWatch.Registry.Web.Services {
    public class DefaultDateProvider : IDateProvider {

        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;

    }
}
