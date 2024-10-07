namespace HackAssemblyToBinaryTranslator
{
    public class SymbolTable : ISymbolTable
    {
        private const int StartRAMIndexingForVariables = 16;

        private Dictionary<string, int> _symbolAdressMap = new ();

        private int CurrentRamIndexForVariable;
 
        public SymbolTable()
        {
            Initialize();
            CurrentRamIndexForVariable = StartRAMIndexingForVariables;
        }

        /// <summary>
        /// Use for ROM labels only
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddROMEntry(string key, int value)
        {
            _symbolAdressMap.Add(key, value);
        }

        /// <summary>
        /// Use for RAM labels and variables only
        /// </summary>
        /// <param name="key"></param>

        public void AddRAMEntry(string key)
        {
            _symbolAdressMap.Add(key, CurrentRamIndexForVariable);
            CurrentRamIndexForVariable++;
        }

        public bool Contains(string key) =>
            _symbolAdressMap.ContainsKey(key);

        public int GetAddress(string key)
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            _symbolAdressMap.Add("SP", 0);
            _symbolAdressMap.Add("LCL", 1);
            _symbolAdressMap.Add("ARG", 2);
            _symbolAdressMap.Add("THIS", 3);
            _symbolAdressMap.Add("THAT", 4);
            _symbolAdressMap.Add("SCREEN", 16384);
            _symbolAdressMap.Add("KBD", 24576);
            for (int i = 0; i < 16; i++)
            {
                var index = i;
                _symbolAdressMap.Add($"R{i}", index);
            }
        }
    }
}