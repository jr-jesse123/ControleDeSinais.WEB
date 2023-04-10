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
    
    abstract member Create : unit -> IActionResult 
    [<HttpGet>]
    default this.Create() : IActionResult =
        let newRecord = new 'a()
        this.View(newRecord)
    

type ControleLeituraGravaCaoComModeloEspecializadoBase<'a, 'b when 'b:(new:unit -> 'b) and 'b:(new:unit -> 'b)>(repositorioLeitura : ObterTodos<'a>, repositorioGravacao: Adicionar<'a>) =
    inherit ControllerLeituraBase<'a>(repositorioLeitura)
    
    abstract member Create : unit -> IActionResult 
    [<HttpGet>]
    default this.Create() : IActionResult =
        let newRecord = new 'b()
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


type AssociacaoPlatinumController (leitura, gravacao) =
    inherit ControleLeituraGravaCaoBase<AssociacaoPlatinum>(leitura,gravacao)


