using KzsRest.Engine.Services.Abstract;
using Microsoft.AspNetCore.Hosting;

namespace KzsRest.Services.Implementation
{
    public class KzsSystem : ISystem
    {
        readonly IWebHostEnvironment hostingEnvironment;
        public KzsSystem(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public string ContentRootPath => hostingEnvironment.ContentRootPath;
    }
}
