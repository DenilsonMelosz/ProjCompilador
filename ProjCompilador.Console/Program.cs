using System;
using ProjCompilador.Core.Lexer;
using ProjCompilador.Core.Parser;
using ProjCompilador.Core.Semantic;
using ProjCompilador.Core.Optimization;
using ProjCompilador.Core.IR;
using ProjCompilador.Core.CodeGen;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("    COMPILADOR DIDÁTICO - PROJETO COMPLETO       ");
        Console.ResetColor();

        // Código exempl que utiliza as funcionalidades implementadas
        string codigoFonte = @"
            // 1. Teste de Otimização (Constant Folding)
            int limite = 5 * 2 + 5; 
            
            // 2. Teste de Leitura
            int atual = 0;
            read(atual);

            // 3. Teste de Laço de Repetição e Condicionais
            while (atual < limite) {
                if (atual == 10) {
                    // 4. Teste de Matemática e Escrita
                    print(9999); 
                } else {
                    print(atual);
                }
                atual = atual + 1;
            }
        ";

        Console.WriteLine(">>> RETORNO CODIGO FONTE:");
        Console.ResetColor();
        Console.WriteLine(codigoFonte);

        try
        {
            Scanner scanner = new Scanner(codigoFonte);
            var tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            var statements = parser.ParseProgram();

            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer();
            semanticAnalyzer.Analyze(statements);

            // (Bônus): Otimizador (Constant Folding)
            AstOptimizer optimizer = new AstOptimizer();
            statements = optimizer.Optimize(statements);

            IRGenerator irGenerator = new IRGenerator();
            irGenerator.Generate(statements);

            Console.WriteLine(">>> CÓDIGO TAC OTIMIZADO:");
            Console.ResetColor();
            foreach (var instruction in irGenerator.Instructions)
            {
                if (instruction.Op == OpCode.LABEL) Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(instruction);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("INICIANDO MÁQUINA VIRTUAL...");
            Console.ResetColor();

            Console.WriteLine("[A VM encontrou um comando 'read'. Por favor, digite um número inteiro menor que 15 e aperte ENTER]");

            VirtualMachine vm = new VirtualMachine(irGenerator.Instructions);
            vm.Run();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[Execução finalizada com sucesso. Máquina Virtual desligada.]");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[OPA OPA: ERRO DE COMPILAÇÃO]: {ex.Message}");
            Console.ResetColor();
        }
    }
}