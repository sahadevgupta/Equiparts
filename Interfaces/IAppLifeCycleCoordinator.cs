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
    }
}