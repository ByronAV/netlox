using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Runtime.InteropServices;

public class Lox {

    static bool hadError = false;
    public static void main(string[] args) {
        try {
            if (args.Length > 1 ) {
                Console.WriteLine("Usage: netlox [script]");
                Environment.Exit(64);
            } else if (args.Length == 1) {
                runFile(args[0]);
            } else {
                runPrompt();
            }
        } catch (System.IO.IOException e) {
            Console.WriteLine(e.Message);
        }
    }

    private static void runFile(string path) {
        try {
            byte[] bytes = File.ReadAllBytes(path);
            run(new string(Encoding.Default.GetString(bytes)));

            // Indicate an error in the exit code.
            if (hadError) Environment.Exit(65);
        } catch (System.IO.IOException e) {
            throw;
        }
    }

    private static void runPrompt() {
        try {
            using (StreamReader sr = new StreamReader(Console.OpenStandardInput())) {
                for(;;) {
                    Console.WriteLine("> ");
                    string? line = sr.ReadLine();
                    if (line == null) break;
                    //run(line);
                    hadError = false;
                }
            }
        } catch (System.IO.IOException e) {
            throw;
        }
    }

    private static void run(string source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        // For now, just print the tokens
        foreach (Token token in tokens) {
            Console.WriteLine(token);
        }
    }

    public static void error(int line, string message) {
        report(line, "", message);
    }

    private static void report(int line, string where, string message) {
        Console.Error.Write("[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }
}