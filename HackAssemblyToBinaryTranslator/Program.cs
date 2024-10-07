// See https://aka.ms/new-console-template for more information
using HackAssemblyToBinaryTranslator;

Console.WriteLine("Hello, World!");

string AssemblyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hack.hasm");
var text = System.IO.File.ReadAllText(AssemblyFilePath);

var symbolTable = new SymbolTable();
var parser = new ParserModule(symbolTable, text);
