namespace HackAssemblyToBinaryTranslator
{
    public interface ISymbolTable
    {
        void AddROMEntry(string key, int value);
        void AddRAMEntry(string key);
        bool Contains(string key);
        int GetAddress(string key);
    }
}