using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScopeTest.Controllers;

namespace ScopeTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHttpClient("github", c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    // Github API versioning
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    // Github requires a user-agent
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
});
        }

        public void ConfigureContainer(IServiceContainer container)
        {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<SampleMiddleWare>();
             app.UseMvc();
        }
    }


    public class SampleMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly IHttpClientFactory clientFactory;

        public SampleMiddleWare(RequestDelegate next, IHttpClientFactory clientFactory)
        {
            this.next = next;
            this.clientFactory = clientFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
            "repos/aspnet/docs/pulls");

            var client = clientFactory.CreateClient("github");

            var response = await client.SendAsync(request);
            try
            {
                throw new Exception("");
            }
            catch (System.Exception)
            {

                await next(context);
            }




            //await next(context);
        }
    }
}
