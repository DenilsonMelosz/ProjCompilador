using System;
using System.Collections.Generic;
using ProjCompilador.Core.Lexer;

namespace ProjCompilador.Core.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Statement> ParseProgram()
    {
        List<Statement> statements = new List<Statement>();
        while (!IsAtEnd())
        {
            statements.Add(ParseDeclaration());
        }
        return statements;
    }

    private Statement ParseDeclaration()
    {
        if (Match(TokenType.KW_INT, TokenType.KW_BOOL))
        {
            Token typeToken = Previous();
            Token nameToken = Consume(TokenType.IDENTIFIER, "Esperava um nome para a variável.");

            Expression? initializer = null;
            if (Match(TokenType.OP_ASSIGN))
            {
                initializer = ParseExpression();
            }

            Consume(TokenType.SEMICOLON, "Esperava ';' após a declaração da variável.");
            return new VariableDeclaration(typeToken, nameToken, initializer);
        }

        return ParseStatement();
    }

    private Statement ParseStatement()
    {
        if (Match(TokenType.KW_IF)) return ParseIfStatement();
        if (Match(TokenType.KW_WHILE)) return ParseWhileStatement();
        if (Match(TokenType.KW_PRINT)) return ParsePrintStatement();
        if (Match(TokenType.KW_READ)) return ParseReadStatement();
        if (Match(TokenType.LBRACE)) return new BlockStatement(ParseBlockStatement());

        return ParseAssignmentStatement();
    }

    private List<Statement> ParseBlockStatement()
    {
        List<Statement> statements = new List<Statement>();

        while (!Check(TokenType.RBRACE) && !IsAtEnd())
        {
            statements.Add(ParseDeclaration());
        }

        Consume(TokenType.RBRACE, "Esperava '}' após o bloco de código.");
        return statements;
    }

    private Statement ParseIfStatement()
    {
        Consume(TokenType.LPAREN, "Esperava '(' após 'if'.");
        Expression condition = ParseExpression();
        Consume(TokenType.RPAREN, "Esperava ')' após a condição do if.");

        Statement thenBranch = ParseStatement();

        Statement? elseBranch = null;

        if (Match(TokenType.KW_ELSE))
        {
            elseBranch = ParseStatement();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    private Statement ParseWhileStatement()
    {
        Consume(TokenType.LPAREN, "Esperava '(' após 'while'.");
        Expression condition = ParseExpression();
        Consume(TokenType.RPAREN, "Esperava ')' após a condição do while.");

        Statement body = ParseStatement();

        return new WhileStatement(condition, body);
    }

    private Statement ParsePrintStatement()
    {
        Consume(TokenType.LPAREN, "Esperava '(' após 'print'.");
        Expression value = ParseExpression();
        Consume(TokenType.RPAREN, "Esperava ')' após a expressão.");
        Consume(TokenType.SEMICOLON, "Esperava ';' no final do comando print.");
        return new PrintStatement(value);
    }

    private Statement ParseReadStatement()
    {
        Consume(TokenType.LPAREN, "Esperava '(' após 'read'.");
        Token nameToken = Consume(TokenType.IDENTIFIER, "Esperava o nome da variável para leitura.");
        Consume(TokenType.RPAREN, "Esperava ')' após a variável.");
        Consume(TokenType.SEMICOLON, "Esperava ';' no final do comando read.");
        return new ReadStatement(nameToken);
    }

    private Statement ParseAssignmentStatement()
    {
        Token nameToken = Consume(TokenType.IDENTIFIER, "Esperava o nome da variável.");
        Consume(TokenType.OP_ASSIGN, "Esperava '=' na atribuição.");
        Expression value = ParseExpression();
        Consume(TokenType.SEMICOLON, "Esperava ';' após a atribuição.");
        return new AssignmentStatement(nameToken, value);
    }

    public Expression ParseExpression()
    {
        return Equality();
    }

    private Expression Equality()
    {
        Expression expr = Comparison();

        while (Match(TokenType.OP_EQ, TokenType.OP_NEQ))
        {
            Token operatorToken = Previous();
            Expression right = Comparison();
            expr = new BinaryExpression(expr, operatorToken, right);
        }

        return expr;
    }

    private Expression Comparison()
    {
        Expression expr = Term();

        while (Match(TokenType.OP_GT, TokenType.OP_LT))
        {
            Token operatorToken = Previous();
            Expression right = Term();
            expr = new BinaryExpression(expr, operatorToken, right);
        }

        return expr;
    }

    private Expression Term()
    {
        Expression expr = Factor();

        while (Match(TokenType.OP_MINUS, TokenType.OP_PLUS))
        {
            Token operatorToken = Previous();
            Expression right = Factor();
            expr = new BinaryExpression(expr, operatorToken, right);
        }

        return expr;
    }

    private Expression Factor()
    {
        Expression expr = Primary();

        while (Match(TokenType.OP_DIV, TokenType.OP_MULT))
        {
            Token operatorToken = Previous();
            Expression right = Primary();
            expr = new BinaryExpression(expr, operatorToken, right);
        }

        return expr;
    }

    private Expression Primary()
    {
        if (Match(TokenType.BOOL_LITERAL, TokenType.INT_LITERAL))
        {
            return new LiteralExpression(Previous());
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new VariableExpression(Previous());
        }

        if (Match(TokenType.LPAREN))
        {
            Expression expr = ParseExpression();
            Consume(TokenType.RPAREN, "Esperava ')' após a expressão.");
            return expr;
        }

        throw new Exception($"Erro de Sintaxe: Token inesperado '{Peek().Lexeme}' na linha {Peek().Line}.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EOF;
    private Token Peek() => _tokens[_current];
    private Token Previous() => _tokens[_current - 1];

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw new Exception(message);
    }
}
