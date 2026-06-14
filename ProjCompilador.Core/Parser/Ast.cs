using System.Collections.Generic;
using ProjCompilador.Core.Lexer;

namespace ProjCompilador.Core.Parser;

public abstract record AstNode;

public abstract record Expression : AstNode;

public record LiteralExpression(Token Value) : Expression;
public record VariableExpression(Token Name) : Expression;
public record BinaryExpression(Expression Left, Token Operator, Expression Right) : Expression;

public abstract record Statement : AstNode;

public record VariableDeclaration(Token Type, Token Name, Expression? Initializer) : Statement;

public record AssignmentStatement(Token Name, Expression Value) : Statement;
public record PrintStatement(Expression Expression) : Statement;

public record ReadStatement(Token Name) : Statement;

public record IfStatement(Expression Condition, Statement ThenBranch, Statement? ElseBranch) : Statement;

public record WhileStatement(Expression Condition, Statement Body) : Statement;
public record BlockStatement(List<Statement> Statements) : Statement;
