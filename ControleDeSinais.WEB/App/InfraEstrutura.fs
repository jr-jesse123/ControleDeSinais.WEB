module InfraEstrutura
open Dominio
open Interfaces

//type RepositorioDeLeitura<'a>

open System.ComponentModel.DataAnnotations
open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open Microsoft.EntityFrameworkCore.Design

[<CLIMutable>]
type Blog = {
    [<Key>] Id: int
    Url: string
}



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
open Microsoft.EntityFrameworkCore.Design
open EntityFrameworkCore.FSharp

type DesignTimeServices() =
    interface IDesignTimeServices with 
        member __.ConfigureDesignTimeServices(serviceCollection: IServiceCollection) = 
            let fSharpServices = EFCoreFSharpServices.Default
            fSharpServices.ConfigureDesignTimeServices serviceCollection
            ()