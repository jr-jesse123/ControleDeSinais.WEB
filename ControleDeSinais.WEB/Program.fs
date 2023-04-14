namespace ControleDeSinais.WEB



open Microsoft.AspNetCore.Mvc.Rendering
open Interfaces
open Dominio
open InfraEstrutura.Persistencia
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Hosting
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http
type Program() =
    class end

module Program =
    let toListItem (x:obj) =
        new SelectListItem(x.ToString(), x.ToString())
    
    let toListItems (x:obj seq) =
        x |> Seq.map toListItem


    let exitCode = 0

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        //TODO: CRIAR HOT HELOADER PARA O BROWSER (REFRESH ON CSHTML FILE SAVED)

        builder
            .Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()

        builder.Services.AddRazorPages()
        builder.Services.AddSignalR()

        //TODO: CONFERIR LIFETIME DOS REPOSITÓRIOS
        //TODO: pensar em como registrar todos de uma vez
        //builder.Services.AddTransient(typeof<ObterTodos<>>,fun _ -> Adicionar )

        builder.Services.AddSingleton<ObterTodos<Sinal>>(Repositorios.ObterSinais)
        builder.Services.AddSingleton<Adicionar<Sinal>>(Repositorios.AdicionarSinal)
        
        builder.Services.AddSingleton<ObterTodos<AssociacaoPosicao>>(Repositorios.ObterAssociacaoPosicose)
        builder.Services.AddSingleton<Adicionar<AssociacaoPosicao>>(Repositorios.AdicionarAssociacaoPosicao)

        builder.Services.AddSingleton<ObterTodos<AssociacaoPatch>>(Repositorios.ObterAssociacaoPatch)
        builder.Services.AddSingleton<Adicionar<AssociacaoPatch>>(Repositorios.AdicionarAssociacaoPatch)
        
        
        builder.Services.AddSingleton<ObterTodos<AssociacaoPlatinum>>(Repositorios.ObterAssociacaoPlatinum)
        builder.Services.AddSingleton<Adicionar<AssociacaoPlatinum>>(Repositorios.AdicionarAssociacaoPlatinum)
        

        builder.Services.AddSingleton<ObterTodos<Posicao>>(ListasFixas.ObterPosices)
        builder.Services.AddSingleton<ObterTodos<Destination>>(ListasFixas.ObterDestinations)
        builder.Services.AddSingleton<ObterTodos<Source>>(ListasFixas.ObterSources)

        builder.Services.AddSingleton(toListItems)

        


        builder.Services.AddSingleton<IJsonSerializer>(JsonSerializer())


        
        //builder.Services.AddSingleton(typeof<IJsonUnSerializer<_>>, typeof<JsonUnSerializer<_>>)
        builder.Services.AddSingleton<IJsonUnSerializer>(JsonUnSerializer())
        //builder.services.Add(ServiceDescriptor.Scoped(typeof<IRepository<_>>, typeof(Repository<_>)) :> IServiceDescriptor)


//        builder.Services.AddSingleton(typeof<IJsonUnSerializer<>>, fun sp ->  {new IJsonUnSerializer<_> with
//                                                                                    member this.UnSerialize<_> obj =
//                                                                                        Newtonsoft.Json.JsonConvert.DeserializeObject<_> obj       })
////<IJsonUnSerializer<_>>()
        

        

        
        builder.Services.AddHostedService<Refresher.ViewWatherIHostedService>()


        let app = builder.Build()

        if not (builder.Environment.IsDevelopment()) then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts() |> ignore // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

        app.UseHttpsRedirection()

        app.UseStaticFiles()
        app.UseRouting()
        //app.UseAuthorization()

        app.MapControllerRoute(name = "default", pattern = "{controller=Home}/{action=Index}/{id?}")


        //TODO: TEST APP VIEW WITH RAZOR PAGES
        app.MapRazorPages()

        app.MapHub<RefreshHub>("/refreshHub")

        app.MapGet("/refresh", Func<IHubContext<RefreshHub>,IResult>(fun (x : IHubContext<RefreshHub> ) -> 
            x.Clients.All.SendAsync("refresh").Wait(); 
            Results.Ok()
        ))

        //app.MapGet("/", Func<HttpContext,IResult>(fun (x : HttpContext) -> x.Response.Redirect("/Sinais/Index"); Results.Ok()))

        app.Run()

        exitCode
