using KzsRest.Engine.Services.Abstract;
using Microsoft.AspNetCore.Hosting;

namespace KzsRest.Services.Implementation
{
    public class KzsSystem : ISystem
    {
        readonly IHostingEnvironment hostingEnvironment;
        public KzsSystem(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public string ContentRootPath => hostingEnvironment.ContentRootPath;
    }
}
