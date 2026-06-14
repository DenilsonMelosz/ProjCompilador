using System;
using System.Collections.Generic;
using ProjCompilador.Core.Lexer;

namespace ProjCompilador.Core.Semantic;

public class SymbolTable
{
    private readonly Dictionary<string, TokenType> _symbols = new();

    public void DeclareVariable(string name, TokenType type)
    {
        if (_symbols.ContainsKey(name))
        {
            throw new Exception($"Erro Semântico: A variável '{name}' já foi declarada neste escopo.");
        }

        _symbols[name] = type;
    }

    public TokenType GetVariableType(string name)
    {
        if (!_symbols.TryGetValue(name, out var type))
        {
            throw new Exception($"Erro Semântico: A variável '{name}' não foi declarada.");
        }

        return type;
    }
}
