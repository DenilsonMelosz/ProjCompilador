using System;
using System.Collections.Generic;
using ProjCompilador.Core.Lexer;
using ProjCompilador.Core.Parser;

namespace ProjCompilador.Core.Optimization;

public class AstOptimizer
{
    public List<Statement> Optimize(List<Statement> statements)
    {
        var optimized = new List<Statement>();
        foreach (var stmt in statements)
        {
            optimized.Add(OptimizeStatement(stmt));
        }
        return optimized;
    }

    private Statement OptimizeStatement(Statement stmt)
    {
        return stmt switch
        {
            VariableDeclaration varDecl => varDecl with { Initializer = varDecl.Initializer != null ? OptimizeExpression(varDecl.Initializer) : null },
            AssignmentStatement assign => assign with { Value = OptimizeExpression(assign.Value) },
            PrintStatement print => print with { Expression = OptimizeExpression(print.Expression) },
            IfStatement ifStmt => ifStmt with
            {
                Condition = OptimizeExpression(ifStmt.Condition),
                ThenBranch = OptimizeStatement(ifStmt.ThenBranch),
                ElseBranch = ifStmt.ElseBranch != null ? OptimizeStatement(ifStmt.ElseBranch) : null
            },
            WhileStatement whileStmt => whileStmt with
            {
                Condition = OptimizeExpression(whileStmt.Condition),
                Body = OptimizeStatement(whileStmt.Body)
            },
            BlockStatement block => OptimizeBlock(block),
            ReadStatement read => read,
            _ => stmt
        };
    }

    private BlockStatement OptimizeBlock(BlockStatement block)
    {
        var optStmts = new List<Statement>();
        foreach (var s in block.Statements)
        {
            optStmts.Add(OptimizeStatement(s));
        }
        return new BlockStatement(optStmts);
    }

    private Expression OptimizeExpression(Expression expr)
    {
        if (expr is BinaryExpression binExpr)
        {
            var leftOpt = OptimizeExpression(binExpr.Left);
            var rightOpt = OptimizeExpression(binExpr.Right);

            if (leftOpt is LiteralExpression leftLit && rightOpt is LiteralExpression rightLit)
            {
                return FoldConstants(leftLit, binExpr.Operator, rightLit, binExpr);
            }

            return new BinaryExpression(leftOpt, binExpr.Operator, rightOpt);
        }

        return expr;
    }

    private Expression FoldConstants(LiteralExpression left, Token op, LiteralExpression right, BinaryExpression original)
    {
        if (left.Value.Type == TokenType.INT_LITERAL && right.Value.Type == TokenType.INT_LITERAL)
        {
            int l = int.Parse(left.Value.Lexeme);
            int r = int.Parse(right.Value.Lexeme);

            int mathResult = 0;
            bool isMath = true;
            switch (op.Type)
            {
                case TokenType.OP_PLUS: mathResult = l + r; break;
                case TokenType.OP_MINUS: mathResult = l - r; break;
                case TokenType.OP_MULT: mathResult = l * r; break;
                case TokenType.OP_DIV:
                    if (r == 0) throw new Exception("Otimizador detectou divisão por zero em tempo de compilação!");
                    mathResult = l / r;
                    break;
                default: isMath = false; break;
            }

            if (isMath)
            {
                var newToken = new Token(TokenType.INT_LITERAL, mathResult.ToString(), op.Line, op.Column);
                return new LiteralExpression(newToken);
            }

            bool boolResult = false;
            bool isComp = true;
            switch (op.Type)
            {
                case TokenType.OP_LT: boolResult = l < r; break;
                case TokenType.OP_GT: boolResult = l > r; break;
                case TokenType.OP_EQ: boolResult = l == r; break;
                case TokenType.OP_NEQ: boolResult = l != r; break;
                default: isComp = false; break;
            }

            if (isComp)
            {
                var newToken = new Token(TokenType.BOOL_LITERAL, boolResult ? "true" : "false", op.Line, op.Column);
                return new LiteralExpression(newToken);
            }
        }

        return original;
    }
}
