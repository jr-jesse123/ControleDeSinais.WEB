namespace Testes.InfraEstrutura
open System
open Xunit
open InfraEstrutura.Persistencia
open System.IO
open Swensen.Unquote


module Listas =

    [<Theory>]
    [<InlineData(1,"A")>]
    [<InlineData(26,"Z")>]
    [<InlineData(27,"AA")>]
    let ``N�meros s�o corretamente mapeados para Letras`` nr resultadoEsperado =
        let result = ListasFixas.transformarNrmEmLetra nr     
        test <@ result = resultadoEsperado @>
    

open Dominio
open RepositoriosJsonGenericos

module RepositoriosJson  =

    let DeleteJsonTestFile<'a> () =
        let path = obterCaminho<'a>()
        if File.Exists(path) then
            File.Delete(path)

    [<Fact>]
    let ``Tentar ler de um arquivo Vazio Retorna uma lista vazia`` () =
        DeleteJsonTestFile<AssociacaoPosicao>()
        let result = ObterTodasEnteidadesDoJson<AssociacaoPosicao>()

        test <@ result = [] @>


    [<Fact>]
    let ``� Poss�vel Adicionar �tems e recuperalos`` () =
        DeleteJsonTestFile<Posicao>()
        let posicao = Posicao.create 1 1 "A"
        
        AdicionarEntidadeAoJson<Posicao> posicao

        let result = ObterTodasEnteidadesDoJson<Posicao>()

        test <@ result = [posicao] @>


    [<Fact>]
    let ``� Poss�vel Adicionar multiplos �tems e recuperalos`` () =
        DeleteJsonTestFile<Posicao>()
        let posicao = Posicao.create 1 1 "A"

        let posicao2 = Posicao.create 1 1 "B"
            
        AdicionarEntidadeAoJson<Posicao> posicao

        AdicionarEntidadeAoJson<Posicao> posicao2
    
        let result = ObterTodasEnteidadesDoJson<Posicao>()
    
        test <@ result = [posicao2; posicao ] @>


    [<Fact>]
    let ``N�o � Poss�vel Adicionar dois registros iguais ao reposit�rio`` () =
        DeleteJsonTestFile<Posicao>()
        let posicao = Posicao.create 1 1 "A"

        let posicao2 = Posicao.create 1 1 "A"
            
        AdicionarEntidadeAoJson<Posicao> posicao

        let action () = AdicionarEntidadeAoJson<Posicao> posicao2
    
        Assert.Throws<Exception>(action)