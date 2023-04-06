
namespace Dominio

open System


// lista fixa
//São 5 racks. Acho que cada um vai de A a Z, cada um de 1 a 31
type Posicao = {
    NrRack: int
    Coluna: int
    Linha: string
}

module Posicao = 
    let create nrRack coluna linha = 
        { NrRack = nrRack; Coluna = coluna; Linha = linha }

// cadastrar somente
[<CLIMutable>]
type Sinal = {
    Nome: string
    Descricao: string
    Fonte: string
}



//256 lista permanente
type Source = {
    Numero:int
}



// 194 - lista permanente
type Destination = {
    Numero:int 
}

type EntradaPosicao = Source of Source| Destination of Destination | Sinal of Sinal     //não precisa repositório
with 
    member this.IsSignal = 
        match this with
        | Sinal _ -> true
        | _ -> false

    member this.TypeName = 
        match this with
        | Source _ -> "Source"
        | Destination _ -> "Destination"
        | Sinal _ -> "Sinal"

    member this.GetSinal =
        match this with
        | Sinal s -> s
        | _ -> failwith "Não é um sinal"

    member this.GetNumero =
        match this with
        | Source s -> s.Numero
        | Destination d -> d.Numero
        | Sinal s -> failwith "sinal não tem número"

//TODO: VERIFICAR SE OS MÉTODOS UTILIZADOS PARA VIEW PODEM FICAR NA CAMADA DE VIEW.

[<CLIMutable>]
type AssociacaoPosicao = {
    Posicao: Posicao
    EntradaPosicao: EntradaPosicao 
    DataDeCriacao: DateTime
}


[<CLIMutable>]
type AssociacaoPatch = {
    Entrada: AssociacaoPosicao
    Saida: AssociacaoPosicao
    DataDeCriacao: DateTime
    PrevisaoDeRemocao: DateTime
    Descricao: string
    Ativo: bool    //TODO: ADICINAR ESSE USE CASE NA PÁGINA DE  LISTA DE PATCHS
}

type SinalPlatinum = Source of Source | Destination of Destination         //não precisa repositório

//historico
// usuário pode adicionar
// usuário pode desassociar (criar uma nova associação sem sinal)
type AssociacaoPlatinum ={
    SinalPlatinum: SinalPlatinum * Posicao
    Sinal :  (Sinal * Posicao) option
    DataDeCriacao: DateOnly
}
module AssociacaoPlatinum = 
    let create sinalPlatinum sinal dataDeCriacao = 
        {SinalPlatinum = sinalPlatinum; Sinal = sinal; DataDeCriacao = dataDeCriacao}

//TODO: COLOCAR INTERFACES DENTRO DE DOMÍNIO.INTERFACES
namespace Interfaces

type ObterTodos<'a> = ObterTodos of (unit -> 'a list)
    with member this.Items() = 
        let (ObterTodos f) = this
        f()

type Adicionar<'a> = Adicionar of ('a -> unit)


//casos de uso
// 1 - cadastrar sinal
// 2 - cadastrar patch
// 3 - cadastrar AssociacaoPOsicao
// 5 -  listar tudo
// 3 = filtrar sources desassociados (sem assoiação ou patch atvios)
// 4 - filtrar destinations desassociados
// 5 - finalizar patch
// 6 - filtrar patches ativos
// 7 - Listar histórico   de sinais de uma posição


