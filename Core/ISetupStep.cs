// ISetupStep.cs in GFSetupWizard.App.WinUI3.Core
namespace GFSetupWizard.App.WinUI3.Core
{
    public interface ISetupStep
    {
        string Title { get; }
        string Description { get; }
        bool IsComplete { get; }
        bool CanProceed { get; }
        
        void Initialize();
        bool Validate();
        void Execute();
        void Cleanup();
    }
}
