using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Runtime.InteropServices;

public class Lox {

    private static readonly Interpreter interpreter = new Interpreter();
    static bool hadError = false;
    static bool hadRuntimeError = false;
    public static void Main(string[] args) {
        try {
            if (args.Length > 1 ) {
                Console.WriteLine("Usage: netlox [script]");
                Environment.Exit(64);
            } else if (args.Length == 1) {
                RunFile(args[0]);
            } else {
                RunPrompt();
            }
        } catch (System.IO.IOException e) {
            Console.WriteLine(e.Message);
        }
    }

    private static void RunFile(string path) {
        try {
            byte[] bytes = File.ReadAllBytes(path);
            Run(new string(Encoding.Default.GetString(bytes)));

            // Indicate an error in the exit code.
            if (hadError) Environment.Exit(65);
            if (hadRuntimeError) Environment.Exit(70);
        } catch (System.IO.IOException e) {
            throw;
        }
    }

    private static void RunPrompt() {
        try {
            using (StreamReader sr = new StreamReader(Console.OpenStandardInput())) {
                for(;;) {
                    Console.Write("> ");
                    string? line = sr.ReadLine();
                    if (line == null) break;
                    Run(line);
                    hadError = false;
                }
            }
        } catch (System.IO.IOException e) {
            throw;
        }
    }

    private static void Run(string source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser<object> parser = new Parser<object>(tokens);
        Expr<object>? expression = parser.Parse();

        if (hadError) return;

        interpreter.Interpet(expression);
    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }

    public static void Error(Token token, string message) {
        if (token.Type == TokenType.EOF) {
            Report(token.Line, " at end", message);
        } else {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    public static void RuntimeError(RunTimeError error) {
        Console.Error.WriteLine(error.Message + "\n[line " + error.Token.Line + "]");
        hadRuntimeError = true;
    }
}