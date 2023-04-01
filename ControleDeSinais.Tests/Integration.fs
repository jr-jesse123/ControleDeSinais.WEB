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
let ``É possíve listar sinais``() =
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
        use formContent = 
            [("Nome", "Sinal 1"); ("Descricao", "Sinal 1"); ("Fonte", "Sinal 1")]
            |> List.map KeyValuePair
            |> fun x -> new FormUrlEncodedContent(x)     //TODO: PENSAR EM COMO FAZER ISSO DE FORMA EM RELAÇÃO AO DISPOSAL, TALVEZ CRIAR FUNÇÃO PARA NEW (let newup)
        
        
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

//TODO: GENERALIZAR O TESTE DE ACESSO POR TIPO

    //Task.WaitAll(response)
        
    
