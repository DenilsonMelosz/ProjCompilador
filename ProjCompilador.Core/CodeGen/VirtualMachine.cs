using System;
using System.Collections.Generic;
using ProjCompilador.Core.IR;

namespace ProjCompilador.Core.CodeGen;

public class VirtualMachine
{
    private readonly List<TacInstruction> _instructions;

    private readonly Dictionary<string, object> _memory = new();

    private readonly Dictionary<string, int> _labels = new();

    private int _pc = 0;

    public VirtualMachine(List<TacInstruction> instructions)
    {
        _instructions = instructions;

        for (int i = 0; i < _instructions.Count; i++)
        {
            if (_instructions[i].Op == OpCode.LABEL)
            {
                _labels[_instructions[i].Result!] = i;
            }
        }
    }

    public void Run()
    {
        while (_pc < _instructions.Count)
        {
            var inst = _instructions[_pc];

            switch (inst.Op)
            {
                case OpCode.ASSIGN:
                    _memory[inst.Result!] = GetValue(inst.Arg1!);
                    break;

                case OpCode.ADD:
                    _memory[inst.Result!] = (int)GetValue(inst.Arg1!) + (int)GetValue(inst.Arg2!);
                    break;
                case OpCode.SUB:
                    _memory[inst.Result!] = (int)GetValue(inst.Arg1!) - (int)GetValue(inst.Arg2!);
                    break;
                case OpCode.MUL:
                    _memory[inst.Result!] = (int)GetValue(inst.Arg1!) * (int)GetValue(inst.Arg2!);
                    break;
                case OpCode.DIV:
                    _memory[inst.Result!] = (int)GetValue(inst.Arg1!) / (int)GetValue(inst.Arg2!);
                    break;

                case OpCode.LT:
                    _memory[inst.Result!] = (int)GetValue(inst.Arg1!) < (int)GetValue(inst.Arg2!);
                    break;
                case OpCode.GT:
                    _memory[inst.Result!] = (int)GetValue(inst.Arg1!) > (int)GetValue(inst.Arg2!);
                    break;
                case OpCode.EQ:
                    _memory[inst.Result!] = GetValue(inst.Arg1!).Equals(GetValue(inst.Arg2!));
                    break;
                case OpCode.NEQ:
                    _memory[inst.Result!] = !GetValue(inst.Arg1!).Equals(GetValue(inst.Arg2!));
                    break;

                case OpCode.JUMP:
                    _pc = _labels[inst.Result!];
                    break;

                case OpCode.JUMP_IF_FALSE:
                    bool condition = (bool)GetValue(inst.Arg1!);
                    if (!condition)
                    {
                        _pc = _labels[inst.Result!]; 
                    }
                    break;

                case OpCode.PRINT:
                    Console.WriteLine(GetValue(inst.Arg1!));
                    break;

                case OpCode.READ: 
                    Console.Write($"Entrada para {inst.Result}: ");
                    string input = Console.ReadLine() ?? "0";
                    if (int.TryParse(input, out int parsedValue))
                    {
                        _memory[inst.Result!] = parsedValue;
                    }
                    else
                    {
                        throw new Exception("Erro na VM: Entrada inválida. Esperava-se um número inteiro.");
                    }
                    break;

                case OpCode.LABEL:
                    break;
            }

            _pc++; 
        }
    }

    private object GetValue(string arg)
    {
        if (int.TryParse(arg, out int intVal)) return intVal;
        if (bool.TryParse(arg, out bool boolVal)) return boolVal;
        if (_memory.TryGetValue(arg, out object? memVal)) return memVal;

        throw new Exception($"Erro na VM: Variável ou temporário não inicializado '{arg}'");
    }
}
