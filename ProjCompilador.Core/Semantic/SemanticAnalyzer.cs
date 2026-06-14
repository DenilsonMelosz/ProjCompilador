using System;
using System.Collections.Generic;
using ProjCompilador.Core.Lexer;
using ProjCompilador.Core.Parser;

namespace ProjCompilador.Core.Semantic;

public class SemanticAnalyzer
{
    private readonly SymbolTable _symbolTable = new();

    public void Analyze(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            VisitStatement(statement);
        }
    }

    private void VisitStatement(Statement stmt)
    {
        switch (stmt)
        {
            case VariableDeclaration varDecl:
                _symbolTable.DeclareVariable(varDecl.Name.Lexeme, varDecl.Type.Type);

                if (varDecl.Initializer != null)
                {
                    TokenType initType = VisitExpression(varDecl.Initializer);
                    if (initType != varDecl.Type.Type)
                    {
                        throw new Exception($"Erro Semântico: Tentativa de atribuir valor do tipo {initType} à variável '{varDecl.Name.Lexeme}' do tipo {varDecl.Type.Type}.");
                    }
                }
                break;

            case AssignmentStatement assignStmt:
                TokenType varType = _symbolTable.GetVariableType(assignStmt.Name.Lexeme);

                TokenType valueType = VisitExpression(assignStmt.Value);

                if (varType != valueType)
                {
                    throw new Exception($"Erro Semântico: Variável '{assignStmt.Name.Lexeme}' é do tipo {varType}, não pode receber {valueType}.");
                }
                break;

            case PrintStatement printStmt:
                VisitExpression(printStmt.Expression);
                break;

            case ReadStatement readStmt:
                TokenType readVarType = _symbolTable.GetVariableType(readStmt.Name.Lexeme);
                if (readVarType != TokenType.KW_INT)
                {
                    throw new Exception($"Erro Semântico: O comando 'read' só suporta variáveis do tipo inteiro. '{readStmt.Name.Lexeme}' é {readVarType}.");
                }
                break;

            case IfStatement ifStmt:
                TokenType conditionType = VisitExpression(ifStmt.Condition);
                if (conditionType != TokenType.KW_BOOL && conditionType != TokenType.BOOL_LITERAL)
                {
                    throw new Exception("Erro Semântico: A condição do 'if' deve resultar em um valor booleano.");
                }
                VisitStatement(ifStmt.ThenBranch);
                if (ifStmt.ElseBranch != null) VisitStatement(ifStmt.ElseBranch);
                break;

            case WhileStatement whileStmt:
                TokenType whileCondType = VisitExpression(whileStmt.Condition);
                if (whileCondType != TokenType.KW_BOOL && whileCondType != TokenType.BOOL_LITERAL)
                {
                    throw new Exception("Erro Semântico: A condição do 'while' deve resultar em um valor booleano.");
                }
                VisitStatement(whileStmt.Body);
                break;

            case BlockStatement blockStmt:
                foreach (var s in blockStmt.Statements)
                {
                    VisitStatement(s);
                }
                break;
        }
    }

    private TokenType VisitExpression(Expression expr)
    {
        return expr switch
        {
            LiteralExpression litExpr => litExpr.Value.Type == TokenType.BOOL_LITERAL ? TokenType.KW_BOOL : TokenType.KW_INT,

            VariableExpression varExpr => _symbolTable.GetVariableType(varExpr.Name.Lexeme),

            BinaryExpression binExpr => CheckBinaryExpression(binExpr),

            _ => throw new Exception("Erro Semântico: Expressão desconhecida.")
        };
    }

    private TokenType CheckBinaryExpression(BinaryExpression binExpr)
    {
        TokenType leftType = VisitExpression(binExpr.Left);
        TokenType rightType = VisitExpression(binExpr.Right);

        if (binExpr.Operator.Type is TokenType.OP_PLUS or TokenType.OP_MINUS or TokenType.OP_MULT or TokenType.OP_DIV)
        {
            if (leftType != TokenType.KW_INT || rightType != TokenType.KW_INT)
            {
                throw new Exception($"Erro Semântico: Operador '{binExpr.Operator.Lexeme}' exige números inteiros.");
            }
            return TokenType.KW_INT;
        }

        if (binExpr.Operator.Type is TokenType.OP_GT or TokenType.OP_LT)
        {
            if (leftType != TokenType.KW_INT || rightType != TokenType.KW_INT)
            {
                throw new Exception($"Erro Semântico: Operador '{binExpr.Operator.Lexeme}' exige números inteiros para comparação.");
            }
            return TokenType.KW_BOOL;
        }

        if (binExpr.Operator.Type is TokenType.OP_EQ or TokenType.OP_NEQ)
        {
            if (leftType != rightType)
            {
                throw new Exception($"Erro Semântico: Tipos incompativeis para comparação '{binExpr.Operator.Lexeme}'.");
            }
            return TokenType.KW_BOOL;
        }

        throw new Exception("Erro Semântico: Operador binário desconhecido.");
    }
}
