namespace ProjCompilador.Core.Lexer;

public record Token(TokenType Type, string Lexeme, int Line, int Column)
{
    public override string ToString() =>
        $"[{Type}] Lexeme: '{Lexeme}' at Line {Line}, Col {Column}";
}
