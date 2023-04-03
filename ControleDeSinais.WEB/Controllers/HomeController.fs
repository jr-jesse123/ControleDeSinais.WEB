namespace ControleDeSinais.WEB.Controllers

open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open ControleDeSinais.WEB.Models
open Interfaces
open Dominio
open System.Security.Cryptography
open Newtonsoft.Json.Linq
open System


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


//TODO: CRIAR SEGUNDO ARGUMENTO GENÉRICO PARA O MODEL UTILIZADO EM GRAVAÇÃO QUE PODE SER O MESMO DA LEITURA
//CONTROLE<posicaoassociacao,posicaoAssociacaoMOdel>
type ControleLeituraGravaCaoBase<'a when 'a:(new:unit -> 'a)>(repositorioLeitura : ObterTodos<'a>, repositorioGravacao: Adicionar<'a>) =
    inherit ControllerLeituraBase<'a>(repositorioLeitura)
    
    [<HttpGet>]
    abstract member Create : unit -> IActionResult 
    default this.Create() : IActionResult =
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

type TipoEntradaPosicao = 
    | Source = 0
    | Destination = 1
    | Sinal = 2





//TODO: PENSAR EM INTERNACIONALIZAÇÃO DA DATA
type AssociaCaoPosicaoModel () = 
    [<DefaultValue>] val mutable NrRack : int
    [<DefaultValue>] val mutable NrColuna : int
    [<DefaultValue>] val mutable NrLinha : int
    [<DefaultValue>] val mutable TipoEntrada : TipoEntradaPosicao
    [<DefaultValue>] val mutable EntradaIndice : Nullable<int>


    
    



//TODO: CHECAR COMO USAR CONSTRUTORES SECUNDÁRIOS PARA DESCONSTRUIR OS SINGLE CASES UNIONS
type AssociacaoPosicaoController (repositorioLeitura , repositorioGravacao, listaSinais : ObterTodos<Sinal>) =  
    inherit ControleLeituraGravaCaoBase<AssociacaoPosicao>(repositorioLeitura , repositorioGravacao) 
    let (Adicionar adicionar) =  repositorioGravacao
    let sinais =  
        let (ObterTodos obter ) = listaSinais
        obter()
    
    override this.Create() : IActionResult =
        let newRecord  =  AssociaCaoPosicaoModel()
        this.View(newRecord)
    
    
    //[<HttpPost>]
    //member this.Create(nome: string, descricao: string, fonte: string) =
    //    let novoSinal = {Nome=nome;Descricao = descricao;Fonte=fonte}
    //    adicionar novoSinal
    //    this.RedirectToAction "Index"
        