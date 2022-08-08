using Microsoft.AspNetCore.Mvc;
using Veyesys.Web.Framework.Controllers;
//using Veyesys.Web.Framework.Mvc.Filters;

namespace Veyesys.Web.Controllers
{
    //[WwwRequirement]
    //[CheckLanguageSeoCode]
    //[CheckAccessPublicStore]
    //[CheckAccessClosedStore]
    //[CheckDiscountCoupon]
    //[CheckAffiliate]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual IActionResult InvokeHttp404()
        {
            Response.StatusCode = 404;
            return new EmptyResult();
        }
    }
}