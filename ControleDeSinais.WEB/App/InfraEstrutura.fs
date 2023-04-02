namespace InfraEstrutura

namespace InfraEstrutura.Persistencia
open System.IO
open System
open Dominio
open Interfaces

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
        File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

    let ObterTodasEnteidadesDoJson<'a when 'a: equality> () =
        use stream = getStream<'a>()
        let json = StreamReader(stream).ReadToEnd()
        let result = Newtonsoft.Json.JsonConvert.DeserializeObject<'a seq>(json)
        if result = null then [] else result |> Seq.toList

    let AdicionarEntidadeAoJson<'a when 'a: equality> entidade = 
        let actual = ObterTodasEnteidadesDoJson<'a>()
        let path = obterCaminho<'a>()

        if List.contains entidade actual then 
            raise <| Exception("Entidade já existe")

        let result = List.append actual [entidade]
        let json = Newtonsoft.Json.JsonConvert.SerializeObject(result)
        use stream = getStream<'a>()
        use writter = StreamWriter(stream)
        
        writter.Write(json)


open RepositoriosJsonGenericos


module Repositorios = 
    
    let AdicionarSinal = Adicionar AdicionarEntidadeAoJson<Sinal>
    let ObterSinais = ObterTodos ObterTodasEnteidadesDoJson<Sinal>

    let AdicionarEntradaPosicao = Adicionar AdicionarEntidadeAoJson<EntradaPosicao>
    let ObterEntradasPosicao = ObterTodos ObterTodasEnteidadesDoJson<EntradaPosicao>

    let AdicionarAssociacaoPosicao = Adicionar AdicionarEntidadeAoJson<AssociacaoPosicao>
    let ObterAssociacaoPosicose = ObterTodos ObterTodasEnteidadesDoJson<AssociacaoPosicao>

    let AdicionarAssociacaoPatch = Adicionar AdicionarEntidadeAoJson<AssociacaoPatch>
    let ObterAssociacaoPatch = ObterTodos ObterTodasEnteidadesDoJson<AssociacaoPatch>

    let AdicionarAssociacaoPlatinum = Adicionar AdicionarEntidadeAoJson<AssociacaoPlatinum>
    let ObterAssociacaoPlatinum = ObterTodos ObterTodasEnteidadesDoJson<AssociacaoPlatinum>


