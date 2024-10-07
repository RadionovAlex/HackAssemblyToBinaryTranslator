namespace HackAssemblyToBinaryTranslator
{
    public interface IParserModule
    {
        bool IsValid();
        bool HasMoreCommands();
        void ReadNext();
        CommandType GetCommandType();

        string GetSymbol();
        string GetDestination();
        string GetComp();
        string GetJump();
    }
}