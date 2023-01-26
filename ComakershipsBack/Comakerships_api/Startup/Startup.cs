using AutoMapper;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Comakerships_Api.Filters;
using ComakershipsApi.Filters;
using ComakershipsApi.Infrastructure;
using ComakershipsApi.Security;
using ComakershipsApi.Services;
using DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Service;
using ServiceLayer;
using ServiceLayer.User;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;

[assembly: FunctionsStartup(typeof(ComakershipsApi.Startup))]
namespace ComakershipsApi
{
    class Startup : FunctionsStartup {
		public override void Configure(IFunctionsHostBuilder Builder) {
			Builder.Services.AddSingleton<ITokenService, TokenService>();

			ConfigureSwagger(Builder);

			var mapperConfig = new MapperConfiguration(mc => {
				mc.AddProfile(new MappingProfile());
			});
			IMapper mapper = mapperConfig.CreateMapper();
			Builder.Services.AddSingleton(mapper);
			
			// Comakerships and Comakerships Status
			Builder.Services.AddScoped<IComakershipRepository, ComakershipRepository>();
			Builder.Services.AddScoped<IComakershipService, ComakershipService>();
			Builder.Services.AddScoped<IStatusRepository, StatusRepository>();
			Builder.Services.AddScoped<IStatusService, StatusService>();

			// Deliverables
			Builder.Services.AddScoped<IDeliverableRepository, DeliverableRepository>();
			Builder.Services.AddScoped<IDeliverableService, DeliverableService>();

			// Teams
			Builder.Services.AddScoped<ITeamRepository, TeamRepository>();
			Builder.Services.AddScoped<ITeamService, TeamService>();
			
			// Programs
			Builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
			Builder.Services.AddScoped<IProgramService, ProgramService>();

			// PurchaseKeys
			Builder.Services.AddScoped<IPurchaseKeyRepository, PurchaseKeyRepository>();
			Builder.Services.AddScoped<IKeyService, KeyService>();

			// Company
			Builder.Services.AddScoped<ICompanyService, CompanyService>();
			Builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();

			// University
			Builder.Services.AddScoped<IUniversityService, UniversityService>();
			Builder.Services.AddTransient<IUniversityRepository, UniversityRepository>();

			// Users
			Builder.Services.AddScoped<IUserService, UserService>();
			Builder.Services.AddScoped<IUserRepository, UserRepository>();

			// Review
			Builder.Services.AddScoped<IReviewService, ReviewService>();
			Builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

			// Azure
			Builder.Services.AddScoped<IAzureRepository, AzureRepository>();

			ConfigureAuthenticaton(Builder);

			ConfigureAuthorization(Builder);

			Builder.Services.AddDbContext<ComakershipsContext>();

			// Automatically perform database migration
			Builder.Services.BuildServiceProvider().GetService<ComakershipsContext>().Database.Migrate();
		}

		private void ConfigureAuthenticaton(IFunctionsHostBuilder Builder) {
			Builder.Services.AddAuthentication((AuthenticationOptions Options) => {
				Options.AddScheme<WebJobsAuthLevelHandler>(SecurityDefinition.WebJobsAuthLevel.ToString(), "");
				Options.AddScheme<BearerAuthenticationHandler>(SecurityDefinition.Bearer.ToString(), "");
			});
		}

		private void ConfigureAuthorization(IFunctionsHostBuilder Builder) {
			Builder.Services.AddAuthorization((AuthorizationOptions Options) => {
				//
			});

			Builder.Services.AddSingleton<IAuthorizationService, BearerAuthorizationService>();
		}

		private void ConfigureSwagger(IFunctionsHostBuilder Builder) {
			Builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), (SwaggerDocOptions Options) => {
				Options.SpecVersion = OpenApiSpecVersion.OpenApi3_0;
				Options.AddCodeParameter = false;
				Options.PrependOperationWithRoutePrefix = true;
				Options.XmlPath = "Comakerships Api.xml";
				Options.Documents = new[] {
					new SwaggerDocument{
						Name = "v1",
						Title = "Comakerships API",
						Description = "API endpoints for the Comakerships project of the minor \"Cloud\". Created by Group 7 a.k.a. xXCloudEnMobileProjectXx",
						Version = "v2"
					}
				};

				Options.Title = "Comakerships API";
				Options.ConfigureSwaggerGen = (SwaggerGenOptions Options) => {
					Options.IncludeXmlComments(GetLocalFilename("Models.xml"));			// Adding Models XML to swagger spec
					Options.OperationFilter<SecurityRequirementsOperationFilter>();
					Options.SchemaFilter<SwaggerRequiredFilter>();						// Adding the required statements to swagger
					Options.OperationFilter<FileUploadOperation>();						// added to support Multipart upload in Swagger ui
					Options.CustomOperationIds((ApiDescription ApiDesc) => {
						MethodInfo MethodInfo;
						if (ApiDesc.TryGetMethodInfo(out MethodInfo)) {
							return MethodInfo.Name;
						}
						else {
							return new Guid().ToString();
						}
					});

					OpenApiSecurityScheme SecurityScheme = new OpenApiSecurityScheme(); // Added
					SecurityScheme.Type = SecuritySchemeType.Http;                      // Added
					SecurityScheme.Scheme = "bearer";                                   // Added
					SecurityScheme.Description = "JWT for authorization";               // Added
					SecurityScheme.BearerFormat = "JWT";                                // Added
					Options.AddSecurityDefinition("BearerAuth", SecurityScheme);        // Added
				};
			});
		}

		private string GetLocalFilename(string Filename)
		{
			string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			DirectoryInfo AssemblyDirectory = Directory.CreateDirectory(AssemblyPath);
			string BasePath = AssemblyDirectory?.Parent?.FullName;
			return Path.Combine(BasePath, Filename);
		}

	}
}
