namespace InfraEstrutura




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

type PosicaoModel = {
    NrRack: int
    Coluna: int
    Linha: char
}

// cadastrar somente
type SinalModel = {
    nome: string
    descricao: string
}

//256 lista permanente
type SourceModel = {
    numero:int
}

// 194 - lista permanente
type DestinationModel = {
    numero:int 
}

type EntradaPosicao = Source of SourceModel| Destination of DestinationModel | Sinal of SinalModel

type PosicaoAssociacaoModel = {
    Posicao: PosicaoModel
    entradaPosicao: EntradaPosicao
}

type AssociacaoPatchModel = {
    Entrada: PosicaoAssociacaoModel
    Saida: PosicaoAssociacaoModel
    dataDeCriacao: DateOnly
    PrevisaoDeRemocao: DateOnly
    Descricao: string
    Ativo: bool
}

type SinalPlatinumModel = Source of SourceModel | Destination of DestinationModel 

//historico
// usuário pode adicionar
// usuário pode desassociar (criar uma nova associação sem sinal)
type associacaoPlatinum ={
    SinalPlatinum: SinalPlatinumModel * PosicaoModel    
    sinal :  (SinalModel * PosicaoModel) option
    dataDeCriacao: DateOnly
}


namespace InfraEstrutura.Persistencia

open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open Microsoft.EntityFrameworkCore.Design
open InfraEstrutura.Entities
open InfraEstrutura.Persistencia.Models


type ContextoDb(options) =  
    inherit DbContext(options)
    
    [<DefaultValue>]  val mutable private sources : DbSet<SourceModel>
    member this.Sources with get() = this.sources and set v = this.sources <- v

    
    //[<DefaultValue>]  val mutable private blogs : DbSet<SinalModel>
    //member this.Blogs with get() = this.blogs and set v = this.blogs <- v


    //[<DefaultValue>]  val mutable private Destinations : DbSet<DestinationModel>
    //[<DefaultValue>]  val mutable private Sinais : DbSet<SinalModel>
    

    //[<DefaultValue>]  val mutable private Patchs : DbSet<AssociacaoPatchModel>




    override _.OnModelCreating builder =
        builder.RegisterOptionTypes() // enables option values for all entities
        

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