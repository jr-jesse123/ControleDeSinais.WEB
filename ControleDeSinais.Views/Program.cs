using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeSinais.Views
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //TODO: DESCOBRIR QUEM É QUE CHAMA A INDEX VIEW
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();


            //TODO: TEST CALLING F# VIEWS FROM HERE
            //app.MapGet("/home", async () =>
            //{
            //    var viewName = "Views/Home/Index.cshtml";
            //    var viewEngine = app.Services.GetRequiredService<ICompositeViewEngine>();
            //    var view = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);

            //    if (view.View == null)
            //    {
            //        throw new InvalidOperationException($"View not found: {viewName}");
            //    }

            //    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            //    viewData["Title"] = "Home";

            //    var viewResult = new ViewResult
            //    {
            //        ViewName = viewName,
            //        ViewData = viewData,
            //    };

            //    await viewResult.ExecuteResultAsync(new ActionContext
            //    {
            //        HttpContext = app.CONTEXT,
            //        RouteData = new(),
            //        ActionDescriptor = new(),
            //    });
            //});

            app.Run();
        }
    }
}