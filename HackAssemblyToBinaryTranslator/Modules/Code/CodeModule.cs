using System.Text;

namespace HackAssemblyToBinaryTranslator
{
    public class CodeModule : ICodeModule
    {
        // Computation dictionary for bitcodes
       private static Dictionary<string, string> _compToBitcode = new Dictionary<string, string>
       {
        // When a = 0 (for A)
        { "0",   "0101010" },
        { "1",   "0111111" },
        { "-1",  "0111010" },
        { "D",   "0001100" },
        { "A",   "0110000" },
        { "!D",  "0001101" },
        { "!A",  "0110001" },
        { "-D",  "0001111" },
        { "-A",  "0110011" },
        { "D+1", "0011111" },
        { "A+1", "0110111" },
        { "D-1", "0001110" },
        { "A-1", "0110010" },
        { "D+A", "0000010" },
        { "D-A", "0010011" },
        { "A-D", "0000111" },
        { "D&A", "0000000" },
        { "D|A", "0010101" },

        // When a = 1 (for M)
        { "M",   "1110000" },
        { "!M",  "1110001" },
        { "-M",  "1110011" },
        { "M+1", "1110111" },
        { "M-1", "1110010" },
        { "D+M", "1000010" },
        { "D-M", "1010011" },
        { "M-D", "1000111" },
        { "D&M", "1000000" },
        { "D|M", "1010101" }

       };
        public string ComputationPart(string computation)
        {
            var normalized = NormalizeComputation(computation);
            if (_compToBitcode.TryGetValue(normalized, out var bitcode))
                return bitcode;

            throw new Exception($"Cannot parse computation part: {computation}");
        }

        public string DestinationPart(string destination)
        {
            if (string.IsNullOrEmpty(destination))
                return "000";
            StringBuilder builder = new(3);
            builder.Append(destination.Contains("A") ? '1' : '0');
            builder.Append(destination.Contains("D") ? '1' : '0');
            builder.Append(destination.Contains("M") ? '1' : '0');

            return builder.ToString();
        }

        public string JumpPart(string jumpCondition)
        {
            if (string.IsNullOrEmpty(jumpCondition))
                return "000";
            switch (jumpCondition)
            {
                case "JGT":
                    return "001";
                case "JEQ":
                    return "010";
                case "JGE":
                    return "011";
                case "JLT":
                    return "100";
                case "JNE":
                    return "101";
                case "JLE":
                    return "110";
                case "JMP":
                    return "111";
                default:
                    throw new Exception("Not correct Jump condition");
            }
        }

        public string TranslateInstructionToBinary(ComputeInstruction instruction)
        {
            StringBuilder sb = new StringBuilder(16);
            try
            {
                sb.Append("111");
                sb.Append(ComputationPart(instruction.Computation));
                sb.Append(DestinationPart(instruction.Destination));
                sb.Append(JumpPart(instruction.Jump));
            }
            catch(Exception ex)
            {
                throw new Exception($"Cannot translate instruction to binary code, ex: {ex.Message}");
            }
            
            return sb.ToString();
        }

        public static string NormalizeComputation(string computation)
        {
            // Remove all whitespace characters
            computation = computation.Replace(" ", "");

            // Check if computation is already mapped directly
            if (_compToBitcode.ContainsKey(computation))
            {
                return computation;
            }

            // Handle commutative operations: +, |, &
            string[] operators = { "+", "|", "&" };

            foreach (var op in operators)
            {
                if (computation.Contains(op))
                {
                    // Split the computation by the operator
                    var parts = computation.Split(op);
                    if (parts.Length == 2)
                    {
                        string left = parts[0];
                        string right = parts[1];

                        // Swap the operands
                        string swappedComputation = $"{right}{op}{left}";

                        // Check if the swapped computation is mapped
                        if (_compToBitcode.ContainsKey(swappedComputation))
                        {
                            return swappedComputation;
                        }
                    }
                }
            }

            // If computation is not found in any form, throw an exception
            throw new Exception($"Invalid computation: {computation}");
        }

    }
}