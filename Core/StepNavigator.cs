using System.Collections.Generic;
using System.Linq;

namespace GFSetupWizard.App.WinUI3.Core
{
    public class StepNavigator : IStepNavigator
    {
        private readonly List<ISetupStep> _steps;
        private int _currentIndex = 0;

        public StepNavigator(IEnumerable<ISetupStep> steps)
        {
            _steps = steps.ToList();
        }

        public ISetupStep CurrentStep => _steps.Count > 0 ? _steps[_currentIndex] : null;

        public bool CanMoveNext => _currentIndex < _steps.Count - 1 && CurrentStep?.CanProceed == true;

        public bool CanMovePrevious => _currentIndex > 0;

        public void MoveNext()
        {
            if (CanMoveNext)
            {
                CurrentStep?.Execute();
                _currentIndex++;
                CurrentStep?.Initialize();
            }
        }

        public void MovePrevious()
        {
            if (CanMovePrevious)
            {
                CurrentStep?.Cleanup();
                _currentIndex--;
                CurrentStep?.Initialize();
            }
        }

        public void Reset()
        {
            _currentIndex = 0;
            foreach (var step in _steps)
            {
                step.Cleanup();
            }
            CurrentStep?.Initialize();
        }
    }
}
