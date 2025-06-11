// BaseSetupStep.cs in GFSetupWizard.App.WinUI3.Core
namespace GFSetupWizard.App.WinUI3.Core
{
    public abstract class BaseSetupStep : ISetupStep
    {
        public abstract string Title { get; }
        public abstract string Description { get; }
        
        // StepId is used to uniquely identify each step
        public virtual string StepId => GetType().Name;
        
        // All steps are considered complete by default since we're removing completion tracking
        public virtual bool IsComplete => true;
        
        public virtual bool CanProceed { get; protected set; } = true;

        public virtual void Initialize() 
        {
            // No initialization needed
        }

        public virtual bool Validate()
        {
            return true;
        }

        public virtual void Execute()
        {
            // No execution needed
        }

        public virtual void Cleanup() { }
        
        // These methods are kept for compatibility but don't do anything
        public virtual void MarkAsComplete() { }
        
        public virtual void MarkAsSkipped() { }
    }
}
