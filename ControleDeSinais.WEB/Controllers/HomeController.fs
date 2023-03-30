namespace ControleDeSinais.WEB.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open ControleDeSinais.WEB.Models
open Interfaces
open Dominio

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



type SinaisController (logger : ILogger<SinaisController>, repositorioLeitura : ObterTodos<Sinal>, repositorioGravacao: Adicionar<Sinal>) =
    inherit Controller()
    let(ObterTodos obtertodos) =  repositorioLeitura
    let(Adicionar adicionar) =  repositorioGravacao
    
    member this.Index () =
        let x = obtertodos()
        this.View(x)


    [<HttpGet>]
    member this.Create() =
        let newSinal = { Nome = ""; Descricao = ""; Fonte = ""}
        this.View(newSinal)


    [<HttpPost>]
    member this.Create(nome: string, descricao: string, fonte: string) =
        let newSinal = { Nome = nome; Descricao = descricao; Fonte = fonte }
        adicionar newSinal
        this.RedirectToAction("Index") 


    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id
        this.View({ RequestId = reqId })