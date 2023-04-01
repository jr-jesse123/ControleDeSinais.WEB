namespace ControleDeSinais.WEB.Controllers

open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open ControleDeSinais.WEB.Models
open Interfaces
open Dominio
open System.Security.Cryptography
open Newtonsoft.Json.Linq


type HomeController (logger : ILogger<HomeController>) =
    inherit Controller()

    member this.Index () =
        this.RedirectToAction("Index",controllerName="Sinais")

    member this.Privacy () =
        this.View()

    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id

        this.View({ RequestId = reqId })


type ControllerLeituraBase<'a>(repositorioLeitura : ObterTodos<'a>) =
    inherit Controller()
    let(ObterTodos obtertodos) =  repositorioLeitura
    member this.Index () : IActionResult =
        let x = obtertodos()
        this.View(x)  



type ControleLeituraGravaCaoBase<'a when 'a:(new:unit -> 'a)>(repositorioLeitura : ObterTodos<'a>, repositorioGravacao: Adicionar<'a>) =
    inherit ControllerLeituraBase<'a>(repositorioLeitura)
    
    [<HttpGet>]
    member inline this.Create() : IActionResult =
        let newRecord = new 'a()
        this.View(newRecord)
    


type PosicoesController (repositorioLeitura) =  
    inherit ControllerLeituraBase<Posicao>(repositorioLeitura ) 

type DestinationsController (repositorioLeitura) =  
    inherit ControllerLeituraBase<Destination>(repositorioLeitura ) 
        
type SourcesController (repositorioLeitura) =  
    inherit ControllerLeituraBase<Destination>(repositorioLeitura ) 


type SinaisController (repositorioLeitura , repositorioGravacao) =  
    inherit ControleLeituraGravaCaoBase<Sinal>(repositorioLeitura , repositorioGravacao) 
    let (Adicionar adicionar) =  repositorioGravacao
    [<HttpPost>]
    member this.Create(nome: string, descricao: string, fonte: string) =
        let novoSinal = {Nome=nome;Descricao = descricao;Fonte=fonte}
        adicionar novoSinal
        this.RedirectToAction "Index"




type AssociacoesController (repositorioLeitura , repositorioGravacao) =  
    inherit ControleLeituraGravaCaoBase<AssociacaoPosicao>(repositorioLeitura , repositorioGravacao) 
    //let (Adicionar adicionar) =  repositorioGravacao
    //[<HttpPost>]
    //member this.Create(nome: string, descricao: string, fonte: string) =
    //    let novoSinal = {Nome=nome;Descricao = descricao;Fonte=fonte}
    //    adicionar novoSinal
    //    this.RedirectToAction "Index"
        