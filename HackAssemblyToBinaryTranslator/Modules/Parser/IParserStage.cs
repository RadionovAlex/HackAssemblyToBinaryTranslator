namespace HackAssemblyToBinaryTranslator.Modules.Parser
{
    public interface IParserStage
    {
        bool Finished { get; }

        void ProcessNext();

        string Name { get; }
       
        int Percents { get; }
    }
}