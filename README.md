# Introduction

This is the C# implementation of a Lox interpreter. You can learn more about this project [here](https://github.com/munificent/craftinginterpreters). This project is built using **.NET 8.0**

# Build

In order to build the project you need to run:
```
dotnet build
```
in the main repository. This should build the solution. To build only for `Debug` or `Release` builds use
```
dotnet build -c <Debug|Release>
```

# Execute REPL

To execute the interpreter and run it in a REPL you need to run the following command.
```
dotnet run
```
The REPL will be launched and you can run any given expression supported by the Lox interpreter

# Execute script

To execute the interpreter and run it in a REPL you need to run the following command.
```
dotnet run <path_to_lox_file>
```
This should execute the script file and provide the result in the terminal window.