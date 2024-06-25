using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Runtime.InteropServices;

public class Lox {

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
        Parser<string> parser = new Parser<string>(tokens);
        Expr<string>? expression = parser.Parse();

        if (hadError) return;

        Console.WriteLine(new AstPrinter().Print(expression));

        // For now, just print the tokens
        // foreach (Token token in tokens) {
        //     Console.WriteLine(token.ToString());
        // }
    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }

    public static void Error(Token token, string message) {
        if (token.type == TokenType.EOF) {
            Report(token.line, " at end", message);
        } else {
            Report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    public static void RuntimeError(RunTimeError error) {
        Console.Error.WriteLine(error.Message + "\n[line " + error.token.line + "]");
        hadRuntimeError = true;
    }
}