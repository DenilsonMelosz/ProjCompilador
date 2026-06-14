namespace ProjCompilador.Core.IR;

public enum OpCode
{
    ADD, SUB, MUL, DIV,
    EQ, NEQ, LT, GT,
    ASSIGN,
    PRINT,
    READ,
    JUMP,
    JUMP_IF_FALSE,
    LABEL
}

public record TacInstruction(OpCode Op, string? Arg1, string? Arg2, string? Result)
{
    public override string ToString()
    {
        return Op switch
        {
            OpCode.ADD => $"{Result} = {Arg1} + {Arg2}",
            OpCode.SUB => $"{Result} = {Arg1} - {Arg2}",
            OpCode.MUL => $"{Result} = {Arg1} * {Arg2}",
            OpCode.DIV => $"{Result} = {Arg1} / {Arg2}",
            OpCode.EQ => $"{Result} = {Arg1} == {Arg2}",
            OpCode.NEQ => $"{Result} = {Arg1} != {Arg2}",
            OpCode.LT => $"{Result} = {Arg1} < {Arg2}",
            OpCode.GT => $"{Result} = {Arg1} > {Arg2}",
            OpCode.ASSIGN => $"{Result} = {Arg1}",
            OpCode.PRINT => $"print {Arg1}",
            OpCode.READ => $"read {Result}",
            OpCode.JUMP => $"goto {Result}",
            OpCode.JUMP_IF_FALSE => $"ifFalse {Arg1} goto {Result}",
            OpCode.LABEL => $"{Result}:",
            _ => $"{Op} {Arg1} {Arg2} {Result}"
        };
    }
}
