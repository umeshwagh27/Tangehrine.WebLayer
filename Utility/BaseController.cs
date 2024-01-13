
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Tangehrine.WebLayer.Models;
using Tangehrine.ServiceLayer.Interface;

namespace Tangehrine.WebLayer.Utility
{
    public class BaseController<T> : Controller where T : BaseController<T>
    {
        #region Fields
        protected JsonResponse JsonResponse => new JsonResponse();

        private IErrorLogService _errorLog;
        protected IErrorLogService ErrorLog => _errorLog ?? (_errorLog = HttpContext.RequestServices.GetService<IErrorLogService>());

        private IHttpContextAccessor _accessor;
        protected IHttpContextAccessor Accessor => _accessor ?? (_accessor = HttpContext.RequestServices.GetService<IHttpContextAccessor>());

        private IWebHostEnvironment _webHostingEnvironment;
        protected IWebHostEnvironment WebHostingEnvironment => _webHostingEnvironment ?? (_webHostingEnvironment = HttpContext.RequestServices.GetService<IWebHostEnvironment>());

        private IMapper _mapper;
        protected IMapper Mapper => _mapper ?? (_mapper = HttpContext.RequestServices.GetService<IMapper>());

        private IConfiguration _config;
        protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        #endregion

        public string GetSortingColumnName(int sortColumnNo)
        {
            return Accessor.HttpContext.Request.Query["mDataProp_" + sortColumnNo][0]??"Id";
        }

        public string GetPhysicalUrl()
        {
            return Config.GetValue<string>("CommonProperty:PhysicalUrl");
        }

        public string GetClientAppUrl()
        {
            return Config.GetValue<string>("CommonProperty:ClientAppUrl");
        }

        public int GetDefaultSubscriptionPlanDuration()
        {
            return Config.GetValue<int>("CommonProperty:DefaultSubscriptionDuration");
        }

        public string GetConfigValue(string key)
        {
            return Config.GetValue<string>(key);
        }

        
    }
}