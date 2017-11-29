using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Altairis.GovWatch.Registry.Web.Components {
    public class PagerViewComponent : ViewComponent {

        public IViewComponentResult Invoke(PagingInfo model) {
            return this.View(model);
        }

    }
}
