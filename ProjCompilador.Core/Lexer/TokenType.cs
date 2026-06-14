namespace ProjCompilador.Core.Lexer;

public enum TokenType
{
    EOF,
    INVALID,

    IDENTIFIER,
    INT_LITERAL,
    BOOL_LITERAL,

    KW_IF,
    KW_ELSE,
    KW_WHILE,
    KW_PRINT,
    KW_READ,
    KW_INT,
    KW_BOOL,

    OP_PLUS,
    OP_MINUS,
    OP_MULT,
    OP_DIV,

    OP_ASSIGN,
    OP_EQ,
    OP_NEQ,
    OP_LT,
    OP_GT,

    LPAREN,
    RPAREN,
    LBRACE,
    RBRACE,
    SEMICOLON
}
