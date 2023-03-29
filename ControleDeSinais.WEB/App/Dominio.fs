namespace Dominio

open System


type Posicao = {
    NrRack: int
    Coluna: int
    Linha: char
}

// cadastrar somente
type Sinal = {
    Nome: string
    Descricao: string
}

//256 lista permanente
type Source = {
    Numero:int
}

// 194 - lista permanente
type Destination = {
    Numero:int 
}

type entradaPosicao = Source of Source| Destination of Destination | Sinal of Sinal

type PosicaoAssociacao = {
    Posicao: Posicao
    EntradaPosicao: entradaPosicao
}

type associacaoPatch = {
    Entrada: PosicaoAssociacao
    Saida: PosicaoAssociacao
    dataDeCriacao: DateOnly
    PrevisaoDeRemocao: DateOnly
    Descricao: string
    Ativo: bool
}

type SinalPlatinum = Source of Source | Destination of Destination 

//historico
// usuário pode adicionar
// usuário pode desassociar (criar uma nova associação sem sinal)
type AssociacaoPlatinum ={
    SinalPlatinum: SinalPlatinum * Posicao
    Sinal :  (Sinal * Posicao) option
    DataDeCriacao: DateOnly
}


//casos de uso
// 1 - cadastrar sinal
// 2 - cadastrar patch
// 5 -  listar tudo
// 3 = filtrar sources desassociados (sem assoiação ou patch atvios)
// 4 - filtrar destinations desassociados
// 5 - finalizar patch
// 6 - filtrar patches ativos
// 7 - Listar histórico   de sinais de uma posição



namespace Interfaces

type IRepositorioDeLeitura<'a> =
    abstract member GetAll: unit -> 'a list    

type IRepositorio<'a> =
    inherit IRepositorioDeLeitura<'a>
    abstract member Adicionar: 'a -> unit
    
