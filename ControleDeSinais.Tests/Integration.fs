module Testes.Integration

open Microsoft.Extensions.DependencyInjection
open ControleDeSinais.WEB
open Xunit
open Microsoft.AspNetCore.Mvc.Testing;
open System.Net
open System.Threading.Tasks
open System.Net.Http
open System.Collections.Generic
open Interfaces
open Dominio

//class to do integration tests
type ControleDeSinaisWebFactory() =
    inherit WebApplicationFactory<Program>()
    //configure DI
    
    //TODO ABSTRACT MEMORY REPOSITOIES


    //override this.ConfigureWebHost(host) =
    //    host.ConfigureServices(fun sc -> 
            
    //        sc.Remove(new ServiceDescriptor(typeof<ObterTodos<Sinal>>, typeof<ObterTodos<Sinal>>))
    //        sc.AddSingleton<Adicionar<Sinal>>(Adicionar ignore)
    //        |> ignore
    //    )
    //    |> ignore
    

//TODO: LIDAR CORRETAEMNTE COM MÉTODOS ASSINCRONOS NA APLICAÇÃO
[<Fact>]
let ``É possíve listar sinais``() =
    let factory = new ControleDeSinaisWebFactory()
    let client = factory.CreateClient()
    
    task {
        let! response = client.GetAsync("/")
        Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    }

//TODO: LIDAR CORRETAMENTE COM HTML AQUI. PODE SER QUE TENHA QUE USAR O HTMLAGILITYPACK
[<Fact>]
let ``É possível Criar um sinal``() =
    let factory = new ControleDeSinaisWebFactory()
    let client = factory.CreateClient()
    
    task {

        //let Keyvaluep (key, value) = new KeyValuePair<string, string>(key, value)
        use formContent = 
            [("Nome", "Sinal 3"); ("Descricao", "Sinal 1"); ("Fonte", "Sinal 1")]
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



    
    
[<Fact>]
let ``É possível chegar à página que cria uma assoiacao``() =
    let factory = new ControleDeSinaisWebFactory()
    let client = factory.CreateClient()
        
    task {
        let! response = client.GetAsync("/AssociacaoPosicao/Create")
        Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    }
    
//TODO: GENERALIZAR O TESTE DE ACESSO POR TIPO

    //Task.WaitAll(response)
        
    
