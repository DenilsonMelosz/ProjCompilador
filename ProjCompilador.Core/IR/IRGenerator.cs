using System;
using System.Collections.Generic;
using ProjCompilador.Core.Lexer;
using ProjCompilador.Core.Parser;

namespace ProjCompilador.Core.IR;

public class IRGenerator
{
    private int _tempCount = 0;
    private int _labelCount = 0;

    public List<TacInstruction> Instructions { get; } = new();

    private string NewTemp() => $"t{_tempCount++}";
    private string NewLabel() => $"L{_labelCount++}";

    public void Generate(List<Statement> statements)
    {
        foreach (var stmt in statements)
        {
            GenerateStatement(stmt);
        }
    }

    private void GenerateStatement(Statement stmt)
    {
        switch (stmt)
        {
            case VariableDeclaration varDecl:
                {
                    if (varDecl.Initializer != null)
                    {
                        string initResult = GenerateExpression(varDecl.Initializer);
                        Instructions.Add(new TacInstruction(OpCode.ASSIGN, initResult, null, varDecl.Name.Lexeme));
                    }
                    break;
                }

            case AssignmentStatement assignStmt:
                {
                    string exprResult = GenerateExpression(assignStmt.Value);
                    Instructions.Add(new TacInstruction(OpCode.ASSIGN, exprResult, null, assignStmt.Name.Lexeme));
                    break;
                }

            case PrintStatement printStmt:
                {
                    string printArg = GenerateExpression(printStmt.Expression);
                    Instructions.Add(new TacInstruction(OpCode.PRINT, printArg, null, null));
                    break;
                }

            case ReadStatement readStmt:
                {
                    Instructions.Add(new TacInstruction(OpCode.READ, null, null, readStmt.Name.Lexeme));
                    break;
                }

            case BlockStatement blockStmt:
                {
                    foreach (var s in blockStmt.Statements)
                    {
                        GenerateStatement(s);
                    }
                    break;
                }

            case IfStatement ifStmt:
                {
                    string condResult = GenerateExpression(ifStmt.Condition);
                    string falseLabel = NewLabel();
                    string endLabel = NewLabel();

                    Instructions.Add(new TacInstruction(OpCode.JUMP_IF_FALSE, condResult, null, falseLabel));

                    GenerateStatement(ifStmt.ThenBranch);

                    if (ifStmt.ElseBranch != null)
                    {
                        Instructions.Add(new TacInstruction(OpCode.JUMP, null, null, endLabel));
                        Instructions.Add(new TacInstruction(OpCode.LABEL, null, null, falseLabel));
                        GenerateStatement(ifStmt.ElseBranch);
                        Instructions.Add(new TacInstruction(OpCode.LABEL, null, null, endLabel));
                    }
                    else
                    {
                        Instructions.Add(new TacInstruction(OpCode.LABEL, null, null, falseLabel));
                    }
                    break;
                }

            case WhileStatement whileStmt:
                {
                    string startLabel = NewLabel();
                    string endLabel = NewLabel();

                    Instructions.Add(new TacInstruction(OpCode.LABEL, null, null, startLabel));

                    string wCondResult = GenerateExpression(whileStmt.Condition);
                    Instructions.Add(new TacInstruction(OpCode.JUMP_IF_FALSE, wCondResult, null, endLabel));

                    GenerateStatement(whileStmt.Body);

                    Instructions.Add(new TacInstruction(OpCode.JUMP, null, null, startLabel));
                    Instructions.Add(new TacInstruction(OpCode.LABEL, null, null, endLabel));
                    break;
                }
        }
    }

    private string GenerateExpression(Expression expr)
    {
        return expr switch
        {
            LiteralExpression lit => lit.Value.Lexeme,
            VariableExpression varExpr => varExpr.Name.Lexeme,
            BinaryExpression bin => GenerateBinaryExpression(bin),
            _ => throw new Exception("Expressão desconhecida na geração de código.")
        };
    }

    private string GenerateBinaryExpression(BinaryExpression binExpr)
    {
        string left = GenerateExpression(binExpr.Left);
        string right = GenerateExpression(binExpr.Right);
        string result = NewTemp();

        OpCode op = binExpr.Operator.Type switch
        {
            TokenType.OP_PLUS => OpCode.ADD,
            TokenType.OP_MINUS => OpCode.SUB,
            TokenType.OP_MULT => OpCode.MUL,
            TokenType.OP_DIV => OpCode.DIV,
            TokenType.OP_EQ => OpCode.EQ,
            TokenType.OP_NEQ => OpCode.NEQ,
            TokenType.OP_LT => OpCode.LT,
            TokenType.OP_GT => OpCode.GT,
            _ => throw new Exception("Operador desconhecido.")
        };

        Instructions.Add(new TacInstruction(op, left, right, result));
        return result;
    }
}
