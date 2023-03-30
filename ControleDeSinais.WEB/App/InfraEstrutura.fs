namespace InfraEstrutura

open System.IO




//type RepositorioDeLeitura<'a>





namespace InfraEstrutura.Entities
open System.ComponentModel.DataAnnotations




[<CLIMutable>]
type Blog = {
    [<Key>] Id: int
    Url: string
}


namespace InfraEstrutura.Persistencia.Models
open System
open Microsoft.EntityFrameworkCore
open System.ComponentModel.DataAnnotations

type ModelBase() =
    member val Id : int = 0  with get , set
    //let mutable id:int = id
    //member this.Id : int = id
    
//256 lista permanente
//[<CLIMutable>]
//type SourceModel = {
//    [<Key>] Numero:int
//}
type SourceModel() =
    inherit ModelBase()

//type DestinationModel() =
//    inherit ModelBase()
              
              
              
// 194 - lista permanente
//[<CLIMutable>]
//type DestinationModel = {
//    [<Key>] Numero:int 
//}


//São 5 racks. Acho que cada um vai de A a Z, cada um de 1 a 31
type PosicaoModel = {
    NrRack: int
    Coluna: int
    Linha: char
}

// cadastrar somente
[<CLIMutable>] //TODO: PODEMOS EDITAR O SINAL? JÁ QUE PROVAVELMENTE NÃO PODEMOS TER DOIS COM O MESMO NOME...
type SinalModel = {
    [<Key>] Nome: string
    Descricao: string
}


//type EntradaPosicao = Source of SourceModel| Destination of DestinationModel | Sinal of SinalModel

//type PosicaoAssociacaoModel = {
//    Posicao: PosicaoModel
//    EntradaPosicao: EntradaPosicao
//}

//type AssociacaoPatchModel = {
//    Entrada: PosicaoAssociacaoModel
//    Saida: PosicaoAssociacaoModel
//    DataDeCriacao: DateOnly
//    PrevisaoDeRemocao: DateOnly
//    Descricao: string
//    Ativo: bool
//}

//type SinalPlatinumModel = Source of SourceModel | Destination of DestinationModel 

////historico
//// usuário pode adicionar
//// usuário pode desassociar (criar uma nova associação sem sinal)
//type associacaoPlatinum ={
//    SinalPlatinum: SinalPlatinumModel * PosicaoModel    
//    Sinal :  (SinalModel * PosicaoModel) option
//    DataDeCriacao: DateOnly
//}


namespace InfraEstrutura.Persistencia
open System.IO
open System
open Interfaces
open Dominio




module ListasFixas =     
    
    //TODO: FAZER CONFIGURÁVEL todas as listas fixas
    let transformarNrmEmLetra nr = 
        let rec inner acc nr = 
            if nr <= 26 then 
                nr :: acc
            else
                let letra = nr % 26
                let nr = nr / 26
                inner (letra :: acc) nr
        
        inner [] nr
        |> List.map (fun x -> Char.ConvertFromUtf32(x + 64))
        |> String.concat ""

    let private posicoes = 
        [for i in 1..5 do
            for j in 1..31 do
                for k in 1..28 do
                    {NrRack = i; Coluna = j; Linha = transformarNrmEmLetra k}
        ]   

    let ObterPosices  = ObterTodos(fun _ -> posicoes)

    
    let private sources : Source list = 
            [for i in 1..256 do
                {Source.Numero = i}
            ]

    let ObterSources = ObterTodos(fun _ -> sources)

    let private destinations : Destination list = 
        [for i in 1..194 do
            {Numero = i}
        ]

    let ObterDestinations = ObterTodos(fun _ -> destinations)


module RepositoriosJsonGenericos = 
    
    let obterCaminho<'a>() = 
        let fileName = typeof<'a>.Name + ".json"
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)

    let getStream<'a>() =
        let path = obterCaminho<'a>()
        File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)

    let ObterTodasEnteidadesDoJson<'a> () =
        use stream = getStream<'a>()
        let json = StreamReader(stream).ReadToEnd()
        let result = Newtonsoft.Json.JsonConvert.DeserializeObject<'a list>(json)
        result

    let AdicionarEntidadeAoJson<'a> entidade = 
        let actual = ObterTodasEnteidadesDoJson<'a>()
        let path = obterCaminho<'a>()
        let result = entidade :: actual
        let json = Newtonsoft.Json.JsonConvert.SerializeObject(result)
        use stream = getStream<'a>()
        StreamWriter(stream).Write(result)

open RepositoriosJsonGenericos


module Repositorios = 
    let AdicionarSinal = Adicionar AdicionarEntidadeAoJson<Sinal>
    let ObterSinais = ObterTodos ObterTodasEnteidadesDoJson<Sinal>

    let AdicionarEntradaPosicao = Adicionar AdicionarEntidadeAoJson<EntradaPosicao>
    let ObterEntradasPosicao = ObterTodos ObterTodasEnteidadesDoJson<EntradaPosicao>

    let AdicionarPosicaoAssociacao = Adicionar AdicionarEntidadeAoJson<PosicaoAssociacao>
    let ObterPosicoseAssociacao = ObterTodos ObterTodasEnteidadesDoJson<PosicaoAssociacao>

    let AdicionarAssociacaoPatch = Adicionar AdicionarEntidadeAoJson<AssociacaoPatch>
    let ObterAssociacaoPatch = ObterTodos ObterTodasEnteidadesDoJson<AssociacaoPatch>

    let AdicionarAssociacaoPlatinum = Adicionar AdicionarEntidadeAoJson<AssociacaoPlatinum>
    let ObterAssociacaoPlatinum = ObterTodos ObterTodasEnteidadesDoJson<AssociacaoPlatinum>


open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open Microsoft.EntityFrameworkCore.Design
open InfraEstrutura.Entities
open InfraEstrutura.Persistencia.Models


type ContextoDb(options) =  
    inherit DbContext(options)
    
    [<DefaultValue>]  val mutable private sources : DbSet<SourceModel>
    member this.Sources with get() = this.sources and set v = this.sources <- v
    
    //[<DefaultValue>]  val mutable private destinations : DbSet<DestinationModel>
    //member this.Destinations with get() = this.destinations and set v = this.destinations <- v

    //[<DefaultValue>]  val mutable private sinais : DbSet<SinalModel>
    //member this.Sinais with get() = this.sinais and set v = this.sinais <- v

    //[<DefaultValue>]  val mutable private Patchs : DbSet<AssociacaoPatchModel>




    override _.OnModelCreating builder =
        builder.RegisterOptionTypes() // enables option values for all entities
        builder.Entity<SourceModel>().Property(fun x -> x.Id).ValueGeneratedOnAdd() |> ignore
        

    //override __.OnConfiguring(options: DbContextOptionsBuilder) : unit =
        //options.UseSqlite("Data Source=blogging.db") |> ignore


type BloggingContext(options) =  
    inherit DbContext(options)
    
    [<DefaultValue>]  val mutable private blogs : DbSet<Blog>
    member this.Blogs with get() = this.blogs and set v = this.blogs <- v

    override _.OnModelCreating builder =
        builder.RegisterOptionTypes() // enables option values for all entities
        

    //override __.OnConfiguring(options: DbContextOptionsBuilder) : unit =
        //options.UseSqlite("Data Source=blogging.db") |> ignore

type DesignTimeDbContextFactory() =
        interface IDesignTimeDbContextFactory<BloggingContext> with
            member __.CreateDbContext(args) =
                let optionsBuilder = DbContextOptionsBuilder<BloggingContext>()
                optionsBuilder.UseSqlite("Data Source=blogging.db") |> ignore
                new BloggingContext(optionsBuilder.Options)


//TODO: CREATE CONFIGURATION FILES TO INCLUDE FIXED ENTITIES


open Microsoft.Extensions.DependencyInjection
open EntityFrameworkCore.FSharp

type DesignTimeServices() =
    interface IDesignTimeServices with 
        member __.ConfigureDesignTimeServices(serviceCollection: IServiceCollection) = 
            let fSharpServices = EFCoreFSharpServices.Default
            fSharpServices.ConfigureDesignTimeServices serviceCollection
            ()