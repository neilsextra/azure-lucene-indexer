using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
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
            string directory = System.Environment.GetEnvironmentVariable("APPSETTING_directory");
  
            directory = directory ?? "c:\\temp\\Lucene_2";

            indexer = new LuceneIndexer(directory);

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
                string directory = System.Environment.GetEnvironmentVariable("APPSETTING_directory");
                directory = directory ?? "c:\\temp\\Lucene";

                NameValueCollection parameters = HttpUtility.ParseQueryString(queryString);
                
                if (context.Request.Path.Equals("/put") || context.Request.Method.Equals("PUT")) 
                {
                    if (parameters["id"] == null || parameters["name"] == null) {
                        HttpResponse response = context.Response;

                        response.StatusCode = 500;
                        await response.WriteAsync("Missing Parameter must have [id, name]");

                    }
                    else 
                    {
                        indexer.AddIndexEntry(parameters);

                        IndexPutAction indexAction = CreatePutIndexAction(200, "[put] operation successful",
                         parameters);

                        await context.Response.WriteAsync(SerializeIndexAction(indexAction));
                    }

                } 
                else if (context.Request.Path.Equals("/get")) 
                {
                    var result = indexer.Get(parameters["id"]);

                    if (result != null) 
                    {
                        var output = SerializeIndexItem(result);
                        await context.Response.WriteAsync(output);
                    } 
                    else 
                    {
                        HttpResponse response = context.Response;

                        response.StatusCode = 401;
                        await response.WriteAsync(parameters["id"] + ": Not Found");

                    }
                }
                else if (context.Request.Path.Equals("/delete") || context.Request.Method.Equals("DELETE")) 
                {
                    indexer.Delete(parameters["id"]);

                    await context.Response.WriteAsync(SerializeIndexAction(
                            createDeleteIndexAction(200, "[delete] operation successful",parameters["id"])));

                
                } 
                else if (context.Request.Path.Equals("/search")) 
                {
                    if (parameters["term"] == null) {
                        HttpResponse response = context.Response;

                        response.StatusCode = 500;
                        await response.WriteAsync("Missing Parameter must have [term]");

                    }
                    else
                    {

                        var result = indexer.Search(parameters["term"]);
                        var output = SerializeIndexItems(result);
                        await context.Response.WriteAsync(output);
                    }

                } 
                else 
                {
                    HttpResponse response = context.Response;
                    response.StatusCode = 500;
 
                    await response.WriteAsync("Unknown command");
                }

            });

        }

        private String SerializeIndexItem(IndexEntry indexEntry) {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IndexEntry));

            using (MemoryStream stream = new MemoryStream()) 
            {
                serializer.WriteObject(stream, indexEntry);

                return Encoding.Default.GetString(stream.ToArray());

            }

        }

        private String SerializeIndexItems(IndexEntry[] indexEntries) 
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IndexEntry[]));

            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, indexEntries);

                return Encoding.Default.GetString(stream.ToArray());

            }

        }       
        
        private String SerializeIndexAction<T>(T indexAction) 
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(indexAction.GetType());

            using (MemoryStream stream = new MemoryStream()) 
            {
                serializer.WriteObject(stream, indexAction);

                return Encoding.Default.GetString(stream.ToArray());

            }

        }

        private IndexPutAction CreatePutIndexAction(int status, String message, NameValueCollection parameters)
        {
            IndexPutAction indexAction = new IndexPutAction();

            indexAction.Operation = "put";
            indexAction.Status = status;
            indexAction.Message = message;
 
            IndexEntry indexEntry = new IndexEntry();

            indexEntry.Id = parameters["id"];
            indexEntry.Name = parameters["name"];
            indexEntry.Mobile = parameters["mobile"];

            indexAction.Entry = indexEntry;

            return indexAction;

        }

        private IndexAction createDeleteIndexAction(int status, String message, String id)
        {
            IndexDeleteAction indexAction = new IndexDeleteAction();

            indexAction.Operation = "delete";
            indexAction.Status = status;
            indexAction.Message = message;
            indexAction.Id = id;
   
            return indexAction;

        }

    }

}
