using System.Collections.Generic;

namespace ProjCompilador.Core.Lexer;

public class Scanner
{
    private readonly string _source;
    private int _position = 0;
    private int _line = 1;
    private int _column = 1;

    private readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "if", TokenType.KW_IF },
        { "else", TokenType.KW_ELSE },
        { "while", TokenType.KW_WHILE },
        { "print", TokenType.KW_PRINT },
        { "read", TokenType.KW_READ },
        { "int", TokenType.KW_INT },
        { "bool", TokenType.KW_BOOL },
        { "true", TokenType.BOOL_LITERAL },
        { "false", TokenType.BOOL_LITERAL }
    };

    public Scanner(string source)
    {
        _source = source;
    }

    private char Peek() => _position >= _source.Length ? '\0' : _source[_position];

    private char Advance()
    {
        if (_position >= _source.Length) return '\0';
        char current = _source[_position++];

        if (current == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return current;
    }

    public List<Token> ScanTokens()
    {
        var tokens = new List<Token>();

        while (Peek() != '\0')
        {
            char current = Peek();

            if (char.IsWhiteSpace(current))
            {
                Advance();
                continue;
            }

            int startLine = _line;
            int startColumn = _column;

            if (char.IsDigit(current))
            {
                string numberLiteral = "";
                while (char.IsDigit(Peek()))
                {
                    numberLiteral += Advance();
                }
                tokens.Add(new Token(TokenType.INT_LITERAL, numberLiteral, startLine, startColumn));
                continue;
            }

            if (char.IsLetter(current))
            {
                string text = "";
                while (char.IsLetterOrDigit(Peek()))
                {
                    text += Advance();
                }

                if (_keywords.TryGetValue(text, out TokenType keywordType))
                {
                    tokens.Add(new Token(keywordType, text, startLine, startColumn));
                }
                else
                {
                    tokens.Add(new Token(TokenType.IDENTIFIER, text, startLine, startColumn));
                }
                continue;
            }

            switch (current)
            {
                case '+': Advance(); tokens.Add(new Token(TokenType.OP_PLUS, "+", startLine, startColumn)); continue;
                case '-': Advance(); tokens.Add(new Token(TokenType.OP_MINUS, "-", startLine, startColumn)); continue;
                case '*': Advance(); tokens.Add(new Token(TokenType.OP_MULT, "*", startLine, startColumn)); continue;

                case '/':
                    Advance();
                    if (Peek() == '/')
                    {
                        Advance();
                        while (Peek() != '\n' && Peek() != '\0')
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.OP_DIV, "/", startLine, startColumn));
                    }
                    continue;

                case ';': Advance(); tokens.Add(new Token(TokenType.SEMICOLON, ";", startLine, startColumn)); continue;
                case '(': Advance(); tokens.Add(new Token(TokenType.LPAREN, "(", startLine, startColumn)); continue;
                case ')': Advance(); tokens.Add(new Token(TokenType.RPAREN, ")", startLine, startColumn)); continue;
                case '{': Advance(); tokens.Add(new Token(TokenType.LBRACE, "{", startLine, startColumn)); continue;
                case '}': Advance(); tokens.Add(new Token(TokenType.RBRACE, "}", startLine, startColumn)); continue;

                case '<': Advance(); tokens.Add(new Token(TokenType.OP_LT, "<", startLine, startColumn)); continue;
                case '>': Advance(); tokens.Add(new Token(TokenType.OP_GT, ">", startLine, startColumn)); continue;

                case '=':
                    Advance();
                    if (Peek() == '=') { Advance(); tokens.Add(new Token(TokenType.OP_EQ, "==", startLine, startColumn)); }
                    else { tokens.Add(new Token(TokenType.OP_ASSIGN, "=", startLine, startColumn)); }
                    continue;

                case '!':
                    Advance();
                    if (Peek() == '=') { Advance(); tokens.Add(new Token(TokenType.OP_NEQ, "!=", startLine, startColumn)); }
                    else { tokens.Add(new Token(TokenType.INVALID, "!", startLine, startColumn)); }
                    continue;

            }

            Advance();
            tokens.Add(new Token(TokenType.INVALID, current.ToString(), startLine, startColumn));
        }

        tokens.Add(new Token(TokenType.EOF, "", _line, _column));
        return tokens;
    }
}
