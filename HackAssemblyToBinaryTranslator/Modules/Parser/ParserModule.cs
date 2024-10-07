using System.Text.RegularExpressions;

namespace HackAssemblyToBinaryTranslator
{
    public class ComputeInstruction
    {
        public string Destination;
        public string Computation;
        public string Jump;
    }

    public class ParserModule : IParserModule
    {
        private SymbolTable _symbolTable;
        private string _raw;
        private List<string> _parsedLines;
        private int _currentInstructionIndex;

        const string PseudoCommandPattern = @"^\(([A-Za-z][A-Za-z0-9.]*)\)$";
        const string VariableAddressingPattern = @"^\@([A-Za-z][A-Za-z0-9.]*)$";
        const string DestinationPart = "destination";
        const string ComputationPart = "computation";
        const string JumpPart = "jumpCondition";
        const string ComputationInstructionPattern= $@"^(?:(?<{DestinationPart}>[AMD]+)=)?(?<{ComputationPart}>[^;=]+)?(?:;(?<{JumpPart}>.*))?$";

        private Dictionary<int, CommandType> linesCommandTypes = new();
        private Dictionary<int, ComputeInstruction> linesInstructions= new();

        public ParserModule(SymbolTable symbolTable, string raw)
        {
            _symbolTable = symbolTable;
            _raw = raw;
            ReadLines();            
        }

        private void ReadLines()
        {
            var lines = _raw.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Where(l => !l.StartsWith("//")).ToList();

            HandlePseudoCommands(lines);
            HandleVariables(lines);
            var a = 5;
        }

        private Match MatchPseudoCommand(string line) =>
            Regex.Match(line, PseudoCommandPattern);
        private Match MatchVariableAddressing(string line) =>
            Regex.Match(line, VariableAddressingPattern);

        private int FindNearestNotPseudoCommand(List<string> lines, int pseudoCommandLineIndex)
        {
            var nearestNotPseudoCommandLine = -1;
            for (int i = pseudoCommandLineIndex + 1; i < lines.Count; i++)
            {
                if (!MatchPseudoCommand(lines[i]).Success)
                {
                    nearestNotPseudoCommandLine = i;
                    break;
                }
            }
            if (nearestNotPseudoCommandLine == -1)
                nearestNotPseudoCommandLine = pseudoCommandLineIndex + 1;

            return nearestNotPseudoCommandLine;
        }

        private void HandlePseudoCommands(List<string> lines)
        {
            // go through all the lines, read all (GOTO symbols), add them into the symbolsTable and remove these lines;
            var pseudoCommandsCount = lines.Where(l => MatchPseudoCommand(l).Success).Count();
            List<int> indexesToRemove = new List<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                var match = MatchPseudoCommand(lines[i]);
                if (match.Success)
                {
                    int index = i;
                    indexesToRemove.Add(index);
                    var nearestCommandPos = FindNearestNotPseudoCommand(lines, index);
                    var jumpIndexToWrite = nearestCommandPos - pseudoCommandsCount;
                    var key = match.Groups[1].Value;
                    if (_symbolTable.Contains(key))
                    {
                        throw new Exception($"Pseudocommad already exists:  {key}");
                        // throw exception or what?
                    }
                    else
                    {
                        _symbolTable.AddROMEntry(key, jumpIndexToWrite);
                    }
                }
            }

            foreach (var index in indexesToRemove)
                lines.RemoveAt(index);

            _parsedLines = lines;
        }

        private void HandleVariables(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var match = MatchVariableAddressing(lines[i]);
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    if (!_symbolTable.Contains(key))
                    {
                        // throw exception or what?
                    }
                }
            }
        }

        public CommandType GetCommandType()
        {
            if (linesCommandTypes.ContainsKey(_currentInstructionIndex))
                return linesCommandTypes[_currentInstructionIndex];

            CommandType result;

            if (MatchVariableAddressing(_parsedLines[_currentInstructionIndex]).Success)
                result = CommandType.A_Command;
            else if (MatchPseudoCommand(_parsedLines[_currentInstructionIndex]).Success)
                result = CommandType.Pseudo_Command;

            else result = CommandType.C_Command;

            linesCommandTypes[_currentInstructionIndex] = result;

            return result;
        }

        public string GetComp()
        {
            if (GetCommandType() != CommandType.C_Command)
                throw new Exception();

            return GetComputeInstruction().Computation;
        }

        public string GetDestination()
        {
            if (GetCommandType() != CommandType.C_Command)
                throw new Exception();

            return GetComputeInstruction().Destination;
        }

        public string GetJump()
        {
            if (GetCommandType() != CommandType.C_Command)
                throw new Exception();

            return GetComputeInstruction().Jump;
        }

        public string GetSymbol()
        {
            var commandType = GetCommandType();
            if (commandType == CommandType.A_Command)
            {
                var match = MatchVariableAddressing(_parsedLines[_currentInstructionIndex]);
                return match.Groups[1].Value;
            }
            else if (commandType == CommandType.Pseudo_Command)
            {
                var match = MatchPseudoCommand(_parsedLines[_currentInstructionIndex]);
                return match.Groups[1].Value;
            }

            else throw new Exception("Trying to get symbol for not addressing or pseudo commands");

        }

        public bool HasMoreCommands() =>
            _currentInstructionIndex < _parsedLines.Count - 1;

        public bool IsValid()
        {
            return true;   
        }

        public void ReadNext()
        {
            _currentInstructionIndex++;
        }

        private ComputeInstruction GetComputeInstruction()
        {
            if (linesInstructions.ContainsKey(_currentInstructionIndex))
                return linesInstructions[_currentInstructionIndex];

            var match = Regex.Match(_parsedLines[_currentInstructionIndex], ComputationInstructionPattern);
            if (!match.Success)
                throw new Exception("Cannot parse Comp command");

            ComputeInstruction result = new ComputeInstruction();
            result.Destination = match.Groups[DestinationPart].Value;
            result.Computation = match.Groups[ComputationPart].Value;
            result.Jump = match.Groups[JumpPart].Value;

            linesInstructions.Add(_currentInstructionIndex, result);
            return result;
        }
    }
}