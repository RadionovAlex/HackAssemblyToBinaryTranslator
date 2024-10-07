namespace HackAssemblyToBinaryTranslator
{
    public interface ICodeModule
    {
        string ComputationPart(string computation);
        string DestinationPart(string destination);
        string JumpPart(string jumpCondition);

        string TranslateInstructionToBinary(ComputeInstruction instruction);
    }
}