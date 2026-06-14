# 🚀 Projeto Compilador

Compilador didático desenvolvido em **C# (.NET)** como projeto da disciplina de **Compiladores**, abrangendo todas as etapas clássicas do processo de compilação, desde a análise léxica até a execução do código em uma Máquina Virtual própria.

O projeto foi desenvolvido com o objetivo de consolidar os conceitos de:

* Linguagens Formais
* Autômatos
* Compiladores
* Análise Léxica
* Análise Sintática
* Análise Semântica
* Otimização de Código
* Geração de Código Intermediário
* Execução em Máquina Virtual

---

# Objetivo

Este projeto demonstra a implementação prática de um compilador funcional completo, transformando código-fonte escrito em uma linguagem imperativa simplificada em instruções executáveis por uma Máquina Virtual desenvolvida especificamente para o projeto.

A arquitetura foi organizada seguindo o conceito de separação entre:

```text
Frontend
├── Análise Léxica
├── Análise Sintática
└── Análise Semântica

Middle-end
└── Otimização

Backend
├── Geração de Código Intermediário
└── Máquina Virtual
```

---

# Linguagem Implementada

A linguagem criada para o compilador possui tipagem estática e suporte para:

## Tipos de Dados

```c
int
bool
```

## Operadores Aritméticos

```c
+
-
*
/
```

## Operadores Relacionais

```c
==
!=
>
<
```

## Estruturas de Controle

```c
if
else
while
```

## Entrada e Saída

```c
read()
print()
```

---

# Arquitetura do Compilador

O compilador foi dividido em fases independentes e desacopladas.

## 1. Análise Léxica (Scanner)

Responsável por converter o código-fonte em uma sequência de tokens.

Funções:

* Remoção de espaços em branco
* Remoção de comentários
* Identificação de lexemas
* Geração de tokens
* Controle de linha e coluna para rastreamento de erros

Exemplo:

Código:

```c
int idade = 18;
```

Tokens gerados:

```text
INT
IDENTIFIER(idade)
ASSIGN
INTEGER(18)
SEMICOLON
```

---

## 2. Análise Sintática (Parser)

Implementada utilizando a técnica de **Recursive Descent Parser (Parser Descendente Recursivo)**.

Cada regra da gramática foi mapeada para métodos específicos responsáveis pela validação da estrutura sintática do programa.

Responsabilidades:

* Verificação da sintaxe
* Respeito à precedência de operadores
* Construção da AST

---

## 3. AST (Abstract Syntax Tree)

A Árvore de Sintaxe Abstrata foi modelada utilizando **Records do C#**, proporcionando:

* Imutabilidade
* Melhor organização estrutural
* Menor complexidade do código
* Utilização de Pattern Matching

Exemplo simplificado:

```text
      +
     / \
    x   *
       / \
      2   3
```

---

## 4. Análise Semântica (Type Checker)

Responsável pela validação semântica do programa.

Utiliza uma Tabela de Símbolos implementada com:

```csharp
Dictionary<string, Type>
```

Validações realizadas:

* Variáveis declaradas previamente
* Impedimento de re-declarações
* Compatibilidade de tipos
* Verificação de expressões
* Uso correto de read()
* Verificação de atribuições

Exemplo de erro:

```c
int numero;
bool ativo;

numero = ativo;
```

Resultado:

```text
Erro Semântico:
Tipos incompatíveis.
```

---

## 5. Otimização (Middle-end)

O compilador realiza uma etapa de otimização utilizando a técnica:

### Constant Folding

Expressões constantes são calculadas durante a compilação.

Antes:

```c
int limite = 5 * 2 + 5;
```

Depois:

```c
int limite = 15;
```

Essa otimização reduz operações desnecessárias durante a execução.

---

## 6. Geração de Código Intermediário

A AST é convertida para uma representação linear chamada:

### TAC (Three Address Code)

Exemplo:

```text
t0 = 5 * 2
t1 = t0 + 5
limite = t1
```

Estruturas de controle são transformadas em:

* Labels
* Saltos condicionais
* Saltos incondicionais

Semelhante ao funcionamento de Assembly.

---

## 7. Máquina Virtual

O backend do compilador consiste em uma Máquina Virtual própria.

Características:

* Program Counter (PC)
* Memória simulada
* Variáveis virtuais
* Execução de TAC
* Entrada via Console.ReadLine()
* Saída via Console.WriteLine()

A execução ocorre de forma sequencial sobre a lista de instruções geradas pelo compilador.

---

# Pipeline de Compilação

```text
Código Fonte
      │
      ▼
Análise Léxica
      │
      ▼
Análise Sintática
      │
      ▼
AST
      │
      ▼
Análise Semântica
      │
      ▼
Otimização (Constant Folding)
      │
      ▼
Geração TAC
      │
      ▼
Máquina Virtual
      │
      ▼
Resultado
```

---

# Exemplo de Programa

Código-fonte:

```c
int x = 0;

while (x < 3) {
    x = x + 1;
    print(x);
}
```

---

## TAC Gerado

```text
x = 0

L0:
t0 = x < 3
ifFalse t0 goto L1

t1 = x + 1
x = t1

print x

goto L0

L1:
```

---

# Estrutura do Projeto

```text
ProjCompilador
│
├── ProjCompilador.Console
│   └── Programa Principal
│
├── ProjCompilador.Core
│
├── Lexer
│   ├── Scanner.cs
│   ├── Token.cs
│   └── TokenType.cs
│
├── Parser
│   ├── Parser.cs
│   ├── Ast.cs
│   └── AstOptimizer.cs
│
├── Semantic
│   ├── SemanticAnalyzer.cs
│   └── SymbolTable.cs
│
├── IR
│   ├── IRGenerator.cs
│   └── TacInstruction.cs
│
├── CodeGen
│   └── VirtualMachine.cs
│
└── ProjCompilador.slnx
```

---

# Como Executar

## Clonar o projeto

```bash
git clone https://github.com/SEU-USUARIO/ProjCompilador.git
```

## Restaurar dependências

```bash
dotnet restore
```

## Executar

```bash
dotnet run --project ProjCompilador.Console
```

---

# Conceitos Aplicados

* Linguagens Formais
* Autômatos Finitos
* Análise Léxica
* Análise Sintática
* Recursive Descent Parser
* Abstract Syntax Tree (AST)
* Pattern Matching
* Type Checking
* Tabela de Símbolos
* Constant Folding
* Three Address Code (TAC)
* Máquina Virtual
* Compiladores

---

# Autores

**Denilson Melo**

**Larissa Santana**

Disciplina: **Compiladores**
