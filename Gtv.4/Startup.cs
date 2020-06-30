using gtv.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoDb;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace gtv
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
            services.AddControllersWithViews();

            services.AddScoped<NoDb.IStoragePathResolver<Trip>, StoragePathResolver<Trip>>();
            services.AddScoped<NoDb.IStoragePathResolver<TripItem>, StoragePathResolver<TripItem>>();
            services.AddScoped<NoDb.IStoragePathResolver<Author>, StoragePathResolver<Author>>();
            services.AddNoDb<Trip>();
            services.AddNoDb<TripItem>();
            services.AddNoDb<Author>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    internal class StoragePathResolver<TObject> : IStoragePathResolver<TObject> where TObject : class
    {
        public StoragePathResolver(IStoragePathOptionsResolver storageOptionsResolver)
        {
            optionsResolver = storageOptionsResolver;
        }

        private IStoragePathOptionsResolver optionsResolver;

        public async Task<string> ResolvePath(string projectId, string key = "", string fileExtension = ".json", bool ensureFoldersExist = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pathOptions = await optionsResolver.Resolve(projectId).ConfigureAwait(false);

            var firstFolderPath = pathOptions.AppRootFolderPath
                + pathOptions.BaseFolderVPath.Replace("/", pathOptions.FolderSeparator);

            if (ensureFoldersExist && !Directory.Exists(firstFolderPath))
            {
                Directory.CreateDirectory(firstFolderPath);
            }

            var projectIdFolderPath = Path.Combine(firstFolderPath, projectId);

            if (ensureFoldersExist && !Directory.Exists(projectIdFolderPath))
            {
                Directory.CreateDirectory(projectIdFolderPath);
            }

            if (ensureFoldersExist && !Directory.Exists(projectIdFolderPath))
            {
                Directory.CreateDirectory(projectIdFolderPath);
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return projectIdFolderPath + pathOptions.FolderSeparator;
            }

            return Path.Combine(projectIdFolderPath, key + fileExtension);

        }

        public async Task<string> ResolvePath(
            string projectId,
            string key,
            TObject obj,
            string fileExtension = ".json",
            bool ensureFoldersExist = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            return await ResolvePath(
                projectId,
                key,
                fileExtension,
                ensureFoldersExist,
                cancellationToken).ConfigureAwait(false);

        }
    }
}
