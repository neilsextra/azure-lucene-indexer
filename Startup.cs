﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace azure_lucene_indexer
{
    public class Startup
    {
        LuceneIndexer indexer;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            indexer = new LuceneIndexer("/home/lucene");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                string queryString = context.Request.QueryString.ToString();
               
                NameValueCollection parameters = HttpUtility.ParseQueryString(queryString);
                string directory = System.Environment.GetEnvironmentVariable("APPSETTING_directory");
                await context.Response.WriteAsync("Swagger/Lucene/Example: " + directory + ":" + parameters["name"]);

            });
        }
    }
}
