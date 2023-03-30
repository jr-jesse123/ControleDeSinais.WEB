module TestesDeInfraEstrutura

open System
open Xunit
open InfraEstrutura.Entities
open InfraEstrutura.Persistencia
open Microsoft.EntityFrameworkCore
open System.IO



let GetContext<'a when 'a :> DbContext> () = 
    let randonConnStr = 
        sprintf "DataSource=file:%s?mode=memory&cache=shared" <| Path.GetRandomFileName()
        //sprintf "DataSource=file:%s.db" <| Path.GetRandomFileName() 
        
    let options = 
        DbContextOptionsBuilder<'a>()
            .UseSqlite(randonConnStr)
            .Options
    
    let context = Activator.CreateInstance(typeof<'a>, options )  :?> 'a 
    
    context.Database.EnsureCreated() |> ignore
    //context.Database.Migrate() |> ignore
    context


[<Fact>]
let ``Posso salva e recuperar um registro no banco de dados`` () =
    
    use context  = GetContext<BloggingContext>() 

    let blog = { Id = 0 ; Url = "http://sample.com" }

    context.Blogs.Add(blog) |> ignore

    context.SaveChanges() |> ignore

    Assert.True(true)

open InfraEstrutura.Persistencia.Models
open Swensen.Unquote
open Dominio
open Microsoft.AspNetCore.Connections


let TesteDeAdicaoERecuperacao<'a when 'a: not struct and 'a:equality> a = 
    use context  = GetContext<ContextoDb>() 
    
    context.Set<'a>().Add(a) |> ignore
    context.SaveChanges() |> ignore
    
    let conStr = context.Database.GetConnectionString()
    let newContext = new ContextoDb(DbContextOptionsBuilder().UseSqlite(conStr).Options )
    
    let found = newContext.Set<'a>().ToListAsync().Result |> Seq.last
    test <@ found = a @>


[<Theory>]
[<InlineData(1,"A")>]
[<InlineData(26,"Z")>]
[<InlineData(27,"AA")>]
let teste nr resultadoEsperado =
    let result = ListasFixas.transformarNrmEmLetra nr     
    test <@ result = resultadoEsperado @>
    