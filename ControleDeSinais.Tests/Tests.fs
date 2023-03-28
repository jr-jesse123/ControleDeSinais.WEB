module InfraEstruturaTests

open System
open Xunit

open InfraEstrutura
open Microsoft.EntityFrameworkCore

[<Fact>]
let ``Posso salva e recuperar um registro no banco de dados`` () =
    let options = 
        DbContextOptionsBuilder<BloggingContext>()
            //.UseSqlite("DataSource=:memory:")
            .UseSqlite("Data Source=blogging.db")
            .Options
    
    use context = new BloggingContext(options)

    context.Database.EnsureDeleted() |> ignore

    context.Database.EnsureCreated() |> ignore

    let blog = { Id = 1 ; Url = "http://sample.com" }

    context.Blogs.Add(blog) |> ignore

    context.SaveChanges() |> ignore

    context.Blogs.ToListAsync().Result |> Seq.last |> fun x -> Assert.Equal(blog,x)

    Assert.True(true)

