using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tangehrine.DataLayer;
using Tangehrine.DataLayer.DbContext;

[assembly: HostingStartup(typeof(Tangehrine.WebLayer.Areas.Identity.IdentityHostingStartup))]
namespace Tangehrine.WebLayer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}