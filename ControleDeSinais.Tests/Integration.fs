module Testes.Integration

open ControleDeSinais.WEB
open Xunit
open Microsoft.AspNetCore.Mvc.Testing;
open System.Net
open System.Threading.Tasks
open System.Net.Http
open System.Collections.Generic

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


[<Fact>]
let ``É possível Criar um sinal``() =
    let factory = new ControleDeSinaisWebFactory()
    let client = factory.CreateClient()
    
    task {

        //let Keyvaluep (key, value) = new KeyValuePair<string, string>(key, value)
        let formContent = 
            [("Nome", "Sinal 1"); ("Descricao", "Sinal 1"); ("Fonte", "Sinal 1")]
            |> List.map KeyValuePair
            |> FormUrlEncodedContent
        
        
        let! response = client.PostAsync("/Sinais/Create", formContent)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    }



[<Fact>]
let ``É possível chegar à página que cria um sinal``() =
    let factory = new ControleDeSinaisWebFactory()
    let client = factory.CreateClient()
    
    task {
        let! response = client.GetAsync("/Sinais/Create")
        Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    }



    //Task.WaitAll(response)
        
    
