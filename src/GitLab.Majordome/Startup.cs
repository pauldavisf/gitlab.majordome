using GitLab.Majordome.Abstractions;
using GitLab.Majordome.BotCommands;
using GitLab.Majordome.Configuration;
using GitLab.Majordome.Logic;
using GitLabApiClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GitLab.Majordome
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var credentials = Configuration.GetSection("Credentials");
            services.Configure<Credentials>(credentials);
            services.Configure<ChatOptions>(Configuration.GetSection("Chats"));
            services.Configure<GitLabOptions>(Configuration.GetSection("GitLab"));

            services.AddMvc();

            services.AddOptions();

            services.AddHttpContextAccessor();

            services.AddSingleton<IBotCommand, StartCommand>();
            services.AddSingleton<IBotCommand, LoginCommand>();
            services.AddSingleton<IBotCommand, ListReviewsCommand>();

            services.AddSingleton<IAdminService, AdminService>();
            services.AddSingleton<IUsersRepository, UsersRepository>();
            services.AddSingleton<IMergeRequestsProvider, MergeRequestsProvider>();

            services.AddSingleton<IGitLabClient>(x => new GitLabClient(
                "https://git.skbkontur.ru/",
                credentials["GitLabToken"]));

            services.AddHostedService<MergeRequestNotifier>();
            services.AddHostedService<PollingService>();

            services.AddScoped<IBotUpdateHandler, BotUpdateHandler>();
            services.AddSingleton<IBotService, BotService>();

            services
                .AddControllers()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
