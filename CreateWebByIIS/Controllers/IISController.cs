using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateWebByIIS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IISController : ControllerBase
    {

        private readonly ILogger<IISController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public IISController(ILogger<IISController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("GetIISVersion")]
        public int GetIISVersion()
        {
            return IISHelper.GetIISVersion();
        }

        [HttpGet("GetWebSiteList")]
        public IEnumerable<SiteDto> GetWebSiteList()
        {
            var list = IISHelper.GetWebSiteList().Select(x => new SiteDto()
            {
                SiteName = x.Name,
                Ports = x.Bindings.Select(x=>x.BindingInformation).ToArray(),
                Path = x.Applications.First().VirtualDirectories.First().PhysicalPath,
                State = x.State.ToString(),
            });
            return list;
        }

        [HttpPost("WebsiteStart")]
        public void WebsiteStart(string siteName)
        {
            IISHelper.WebsiteStart(siteName);
        }

        [HttpPost("WebsiteStop")]
        public void WebsiteStop(string siteName)
        {
            IISHelper.WebsiteStop(siteName);
        }

        [HttpPost("WebsiteRemove")]
        public void WebsiteRemove(string siteName)
        {
            IISHelper.WebsiteRemove(siteName);
        }

        [HttpPost("CreateWebSite")]
        public void CreateWebSite(string siteName, string path, int port)
        {
            IISHelper.InstallSiteByHTTP(siteName, path, port, true);
        }

        [HttpPost("CloneCurrentWebSite")]
        public void CloneWebSite(string siteName, int port)
        {
            var currentPath = _hostingEnvironment.ContentRootPath;
            string[] temp = currentPath.Split("\\".ToCharArray());
            string parentPath = null;
            for (int i = 0; i < temp.Length - 1; i++)
            {
                parentPath += temp[i];
                parentPath += "\\";
            }
            parentPath += siteName;

            IOUtil.CopyFolder(currentPath, parentPath);

            IISHelper.InstallSiteByHTTP(siteName, parentPath, port, true);
        }



    }
}
