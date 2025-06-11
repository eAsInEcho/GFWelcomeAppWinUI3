// IStepNavigator.cs in GFSetupWizard.App.WinUI3.Core
namespace GFSetupWizard.App.WinUI3.Core
{
    public interface IStepNavigator
    {
        ISetupStep CurrentStep { get; }
        bool CanMoveNext { get; }
        bool CanMovePrevious { get; }
        
        void MoveNext();
        void MovePrevious();
        void Reset();
    }
}
