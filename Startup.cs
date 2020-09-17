
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using MeetingIntelgence.Data;
using MeetingIntelgence.GraphQl;
using MeetingIntelgence.GraphQl.Messaging;
using MeetingIntelgence.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MeetingIntelgence
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();



            services.AddSingleton<EngagmentReportDbContext>();
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<EngagmentReportManager>();
            services.AddScoped<EngagmentReportSchema>();
            // adding all the graphQL types 
            // option to se bounds on the complexity of query
            services.AddSingleton<EngagmentReportService>();

            services.AddGraphQL(o => { o.ExposeExceptions = true; /*o.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 1, }; */})
                .AddGraphTypes(ServiceLifetime.Scoped).AddUserContextBuilder(httpContext => httpContext.User)
                .AddWebSockets();
            //            services.AddGraphQL(o => { o.ExposeExceptions = false; o.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 1, }; })

        }

        public void Configure(IApplicationBuilder app)//, EngagmentReportDbContext engagmentReportDbContext)
        {

            app.UseWebSockets();
            app.UseGraphQLWebSockets<EngagmentReportSchema>("/graphql");
            app.UseGraphQL<EngagmentReportSchema>();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
        }
    }
}
