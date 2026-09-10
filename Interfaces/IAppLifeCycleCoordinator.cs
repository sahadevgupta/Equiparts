using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equiparts.Interfaces
{
    public interface IAppLifeCycleCoordinator
    {
        Task OnWindowCreatedAsync();
        void RegisterGlobalExceptionHandlers();

        // Decides Login vs Home based on stored session state and navigates there.
        // Called by the splash screen once its entrance animation has finished playing.
        Task NavigateToInitialDestinationAsync();
    }
}