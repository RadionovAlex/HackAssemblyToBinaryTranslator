namespace HackAssemblyToBinaryTranslator.Modules.Parser.Stages
{
    public enum ParseResult
    {
        Success,
        Error
    }
    public class ParserStageProcessor
    {
        private List<IParserStage> _stages;
        private int _currentIndex = 0;
        ParseResult _parseResult = ParseResult.Success;

        public ParserStageProcessor(List<IParserStage> stages)
        {
            _stages = stages;
        }

        public bool Finished()
        {
            if (_parseResult == ParseResult.Error)
                return true;

            if (_currentIndex >= _stages.Count) return true;

            return _currentIndex == _stages.Count - 1 && _stages[_currentIndex].Finished;
        }

        public void Process()
        {
            if (Finished()) return;

            if (_stages[_currentIndex].Finished)
                _currentIndex++;

            _stages[_currentIndex].ProcessNext();
        }

        public string Progress()
        {
            var curStage = _stages[_currentIndex];
            if (Finished())
                return $"Parsing finished on stage {_currentIndex} / {_stages.Count} with result: {_parseResult}";

            return $"Stage {_currentIndex} / {_stages.Count}, {curStage.Name}, Progress {curStage.Percents}";
        }
    }
}