using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equiparts.Interfaces
{
    public interface INavigationService
    {
        Task NaviagteAsync<TPage>(bool isRootPage = false, IDictionary<string, object>? parameters = null, bool replaceCurrent = false) where TPage : Page;
        Task GoBackAsync(int depth = 0, IDictionary<string, object>? parameters = null);
        Task NavigateBackToPageAsync(Type sourcePageType, IDictionary<string, object>? parameters = null);
    }
}