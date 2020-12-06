using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Cyre.Api.Kernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyre.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            // TODO: Move ConfigureServices to extension methods

            #region MVC
            services.AddMvcCore(serup =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                serup.Filters.Add(new AuthorizeFilter(policy));
                serup.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
            })
            .AddNewtonsoftJson(setup =>
            {
                //cfg.UseCamelCasing(true);
                setup.SerializerSettings.ContractResolver = new DefaultContractResolver();
            })
            .AddJsonOptions(setup => 
            {
                setup.JsonSerializerOptions.PropertyNamingPolicy = null; // JsonNamingPolicy.CamelCase;
                setup.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            })
            .AddApiExplorer();
            #endregion

            #region API Options
            services.Configure<ApiBehaviorOptions>(setup =>
            {
                setup.SuppressModelStateInvalidFilter = false;
                setup.SuppressInferBindingSourcesForParameters = false;

                setup.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = $"https://httpstatuses.com/400",
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes =
                        {
                            "application/problem+json",
                            "application/problem+xml"
                        }
                    };
                };
            });
            #endregion

            #region Localization
            var locationConfig = LocalizationConfiguration.BuildDefault();
            services.AddJsonLocalization(setup =>
            {
                setup.ResourcesPath = "i18n";
                setup.UseBaseName = false;
                setup.CacheDuration = TimeSpan.FromHours(1);
                setup.DefaultCulture = new CultureInfo(locationConfig.DefaultLanguage);
                setup.DefaultUICulture = new CultureInfo(locationConfig.DefaultLanguage);
                setup.SupportedCultureInfos = locationConfig.SupportedCultures.ToHashSet();
                setup.FileEncoding = Encoding.UTF8;
                setup.IsAbsolutePath = true;
                setup.LocalizationMode = LocalizationMode.I18n;
            })
            .Configure<RequestLocalizationOptions>(setup =>
            {
                setup.DefaultRequestCulture = new RequestCulture(locationConfig.DefaultLanguage);
                setup.SupportedCultures = locationConfig.SupportedCultures;
                setup.SupportedUICultures = locationConfig.SupportedCultures;
                setup.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(context =>
                {
                    var lang = context.User?.FindFirst("language")?.Value ?? locationConfig.DefaultLanguage;
                    return Task.FromResult(new ProviderCultureResult(lang));
                }));
            });
            #endregion

            #region HashIDs
            services.AddHashids(setup =>
            {
                //setup.MinHashLength = 
            });
            #endregion

        }

        public void Configure(IApplicationBuilder app)
        {

        }
    }
}
