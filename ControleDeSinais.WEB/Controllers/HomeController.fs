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
open System.IO
open InfraEstrutura.Persistencia.ListasFixas
open InfraEstrutura.Persistencia.ListasFixas



type HomeController (logger : ILogger<HomeController>) =
    inherit Controller()

    member this.Index () =
        this.View()

    //member this.Privacy () =
    //    this.View()

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
    
    abstract member Create : unit -> IActionResult 
    [<HttpGet>]
    default this.Create() : IActionResult =
        
        this.View()
    
//TODO: REMOVER ISTO JÁ QUE PODEMOS PASSAR NULO PARA AS MODELS
type ControleLeituraGravaCaoComModeloEspecializadoBase<'a, 'b when 'b:(new:unit -> 'b) and 'b:(new:unit -> 'b)>(repositorioLeitura : ObterTodos<'a>, repositorioGravacao: Adicionar<'a>) =
    inherit ControllerLeituraBase<'a>(repositorioLeitura)
    
    abstract member Create : unit -> IActionResult 
    [<HttpGet>]
    default this.Create() : IActionResult =
        this.View()
    



type PosicoesController (repositorioLeitura) =  
    inherit ControllerLeituraBase<Posicao>(repositorioLeitura ) 

type DestinationsController (repositorioLeitura) =  
    inherit ControllerLeituraBase<Destination>(repositorioLeitura ) 
        
type SourcesController (repositorioLeitura) =  
    inherit ControllerLeituraBase<Source>(repositorioLeitura ) 


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



[<CLIMutable>]
type AssociaCaoPosicaoModel  =     {
        NrRack : int
        NrColuna : int
        NrLinha : string
        TipoEntrada : Nullable<TipoEntradaPosicao>
        IndiceSource : int
        IndiceDestination : int
        IndiceSinal : int
}



//TODO: REGISTRAR AS LISTAS FIXAS DIRETAMENTE SEM O REPOSITORIO
//TODO: CHECAR COMO USAR CONSTRUTORES SECUNDÁRIOS PARA DESCONSTRUIR OS SINGLE CASES UNIONS
//TODO: CHECHAR COMO PASSAR O VALIDADOOR POR MIDDLEWARES, ACTIONFILTER, OU HERANÇA DE MANEIRA QUE OS MODELOS POSSAM SER 
//VALIDADOS AUTOMATICAMENTE MESMO QUE TENHAM DEPENDENCIAS PRA ISSO.
type AssociacaoPosicaoController (repositorioLeitura , repositorioGravacao, repositorioSinais: ObterTodos<Sinal>) =  
    inherit ControleLeituraGravaCaoComModeloEspecializadoBase<AssociacaoPosicao,AssociaCaoPosicaoModel>(repositorioLeitura , repositorioGravacao) 
    let (Adicionar adicionar) =  repositorioGravacao
    let sinais = repositorioSinais.Items()
    
    //TODO: LIDAR COM O INDICE + - 1 NA HORA DE ASSOCIAR POR INDICE
    //TODO: CRIAR TESTES PARA CRIAÇÃO DE ASSOCIAÇÃO COM SINAIS E INDICE
    [<HttpPost>]
    member this.Create([<FromForm>](assModel: AssociaCaoPosicaoModel)) =
        let ass = 
            let entrada = 
                match (int assModel.TipoEntrada.Value) with
                //| null -> failwith "Escolha um tipo de Entrada"
                | 0 -> EntradaPosicao.Source({ Numero= assModel.IndiceSource + 1 })
                | 1 -> EntradaPosicao.Destination({ Numero=  assModel.IndiceDestination + 1})
                | 2 -> EntradaPosicao.Sinal(sinais[assModel.IndiceSinal])
                | _ -> failwith "Tipo de entrada não reconhecido"

            let posicao = {NrRack = assModel.NrRack; Coluna = assModel.NrColuna; Linha = assModel.NrLinha}
            {Posicao = posicao; EntradaPosicao = entrada; DataDeCriacao = DateTime.Now}

        adicionar ass
        this.RedirectToAction "Index"
        

//TODO: CRIAR MÓDELS ESPECIALIZADOS PARA TUDO
type AssociacaoPatchModel ()  =
    [<DefaultValue>] val mutable IndiceEntrada : int
    [<DefaultValue>] val mutable IndiceSaida : int
    [<DefaultValue>] val mutable Ativo : bool
    [<DefaultValue>] val mutable Descricao : string
    member val PrevisaoDeRemocao = DateTime.Now with get, set
    

type AssociacaoPatchController (repositorioLeitura, repositorioGravacao, assPosicaoRepositorio : ObterTodos<AssociacaoPosicao>) =
    inherit ControleLeituraGravaCaoBase<AssociacaoPatch>(repositorioLeitura, repositorioGravacao)
    let assPosicoes = assPosicaoRepositorio.Items()
    //TODO: EXPERIMENTAR USAR OS SERIALIZADORES
    let (Adicionar adicionar ) = repositorioGravacao

    override this.Create() =
        let x = new AssociacaoPatchModel()
        this.View(x)

    [<HttpPost>]
    member this.Create(entradaIndice, saidaIndice, previsaoDeRemocao ,  descricao )= 
        let entrada = assPosicoes[entradaIndice]
        let saida = assPosicoes[saidaIndice]
        let patch = {AssociacaoPatch.Entrada = entrada; Saida = saida; PrevisaoDeRemocao = previsaoDeRemocao; Descricao = descricao; DataDeCriacao = System.DateTime.Now; Ativo = true}

        adicionar patch

        this.RedirectToAction("Index")

//TODO: TESTAR SE TROCANDO O SERIALIZADOR PADRÃO PARA NEWTONSOFT, É POSSÍVEL TER O BIND DE RECORDS E DU AUTOMATICAMENTE
           
           
open InfraEstrutura.Persistencia

type AssociacaoPlatinumModel() =
    member val TipoPlatinum ="" with get,set
    //[<DefaultValue>] val mutable TipoPlatinum : int
    member val Source = "" with  get,set
    member val Destination = ""  with get,set
    member val Posicao : string = "" with get,set
    member val TemSinal: bool = false with get,set
    member val Sinal : string  = "" with get,set
    member val PosicaoSinal : string = "" with get,set

    //member this.FromDoamin (source: AssociacaoPlatinum) (serializer:IJsonSerializer) =
    //    let tipoPlatinum, posicaoPlatinum, numeroPLatinum = 
    //        match source.SinalPlatinum with
    //        | (SinalPlatinum.Source s, p ) -> "source" , serializer.Serialize p , s.Numero
    //        | (SinalPlatinum.Destination d, p ) -> "destination", serializer.Serialize  p, d.Numero
    //        | _ -> exn("sinal platinum desconhecido") |> raise

    //    let temSinal, sinal, PosicaoSinal =
    //        match source.Sinal with
    //        |Some (s, p) -> true , serializer.Serialize s, serializer.Serialize p
    //        |None -> false, "", ""

    //    new AssociacaoPlatinumModel(
    //        TipoPlatinum = tipoPlatinum
    //        Source = 
    //    )





open System.Collections.Generic

type AssociacaoPlatinumController (leitura : ObterTodos<AssociacaoPlatinum>, gravacao, unserializer:IJsonUnSerializer) =
    //inherit ControleLeituraGravaCaoComModeloEspecializadoBase<AssociacaoPlatinum,AssociacaoPlatinumModel>(leitura,gravacao)
    inherit Controller()
    
    let (Adicionar adicionar) = gravacao
    let (ObterTodos obter)  = leitura

    member this.Index() =



        this.View(obter())

    member this.Create() =
        this.View()
 
    [<HttpPost>]
    member this.Create(model:AssociacaoPlatinumModel) =
        ()
        let platinum = 
            if model.TipoPlatinum = "source" then (SinalPlatinum.Source <|  unserializer.UnSerialize<Source>  model.Source, unserializer.UnSerialize<Posicao>  model.Posicao) 
            else (SinalPlatinum.Destination <| unserializer.UnSerialize<Destination>  model.Destination , unserializer.UnSerialize<Posicao>  model.Posicao)
        
        let sinal = 
            if model.TemSinal then 
                Some ( unserializer.UnSerialize<Sinal>  model.Sinal, unserializer.UnSerialize<Posicao>  model.PosicaoSinal)
            else
                None

        let newAss =  AssociacaoPlatinum.create platinum  sinal  DateTime.Now
        adicionar newAss

        this.RedirectToAction("Index")