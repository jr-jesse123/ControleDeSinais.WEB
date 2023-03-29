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
    
    let options = 
        DbContextOptionsBuilder<'a>()
            .UseSqlite(randonConnStr)
            .Options
    
    Activator.CreateInstance(typeof<'a>, options )  :?> 'a 


[<Fact>]
let ``Posso salva e recuperar um registro no banco de dados`` () =
    
    use context  = GetContext<BloggingContext>() 

    context.Database.EnsureCreated() |> ignore

    let blog = { Id = 1 ; Url = "http://sample.com" }

    context.Blogs.Add(blog) |> ignore

    context.SaveChanges() |> ignore

    Assert.True(true)



//type TestesSourceModel    
        