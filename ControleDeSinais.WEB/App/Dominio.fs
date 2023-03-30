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
type Sinal = {
    Nome: string
    Descricao: string
    Fonte: string
}

module Sinal = 
    let create nome descricao fonte = 
        { Nome = nome; Descricao = descricao; Fonte = fonte }

//256 lista permanente
type Source = {
    Numero:int
}

module Source = 
    let create numero = { Numero = numero }

// 194 - lista permanente
type Destination = {
    Numero:int 
}

module Destination = 
    let create numero = { Numero = numero }

type EntradaPosicao = Source of Source| Destination of Destination | Sinal of Sinal     //não precisa repositório

module EntradaPosicao = 
    let createSource numero = Source (Source.create numero)
    let createDestination numero = Destination (Destination.create numero)
    let createSinal nome descricao fonte = Sinal (Sinal.create nome descricao fonte)

type PosicaoAssociacao = {
    Posicao: Posicao
    EntradaPosicao: EntradaPosicao
    DataDeCriacao: DateTime
}

module PosicaoAssociacao = 
    let create posicao entradaPosicao dataDeCriacao = 
        { Posicao = posicao; EntradaPosicao = entradaPosicao; DataDeCriacao = dataDeCriacao }

type AssociacaoPatch = {
    Entrada: PosicaoAssociacao
    Saida: PosicaoAssociacao
    DataDeCriacao: DateOnly
    PrevisaoDeRemocao: DateOnly
    Descricao: string
    Ativo: bool
}

module AssociacaoPatch = 
    let create entrada saida dataDeCriacao previsao descricao ativo = 
        {Entrada = entrada; Saida = saida; DataDeCriacao = dataDeCriacao; PrevisaoDeRemocao = previsao; Descricao = descricao; Ativo = ativo}

type SinalPlatinum = Source of Source | Destination of Destination         //não precisa repositório

module SinalPlatinum = 
    let createSource numero = Source (Source.create numero)
    let createDestination numero = Destination (Destination.create numero)

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



namespace Interfaces

type ObterTodos<'a> = ObterTodos of (unit -> 'a list)

type Adicionar<'a> = Adicionar of ('a -> unit)

////TODO: MAYBE CHANGE TO FUNCTION
//type IRepositorioDeLeitura<'a> =
//    abstract member ObterTodos: unit -> 'a list    

//type IRepositorioDeGravacao<'a> =
//    abstract member Adicionar: 'a -> unit
    
