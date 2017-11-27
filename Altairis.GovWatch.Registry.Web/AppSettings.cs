using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.GovWatch.Registry.Web {
    public class AppSettings {
        public MailingConfig Mailing { get; set; }

        public class MailingConfig {

            public string PickupFolder { get; set; }

            public string SenderName { get; set; }

            public string SenderAddress { get; set; }

        }
    }
}
