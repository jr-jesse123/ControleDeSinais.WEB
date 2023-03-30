module Testes.Integration

open ControleDeSinais.WEB
open Xunit
open Microsoft.AspNetCore.Mvc.Testing;
open System.Net
open System.Threading.Tasks
//class to do integration tests
type ControleDeSinaisWebFactory() =
    inherit WebApplicationFactory<Program>()

//TODO: LIDAR CORRETAEMNTE COM MÉTODOS ASSINCRONOS NA APLICAÇÃO
[<Fact>]
let teste1() =
    let factory = new ControleDeSinaisWebFactory()
    let client = factory.CreateClient()
    
    task {
        let! response = client.GetAsync("/")
        Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    }

    //Task.WaitAll(response)
        
    
