using System.Text;
using Equiparts.Interfaces;

namespace Equiparts.Services
{
    public class NavigationService : INavigationService
    {
        public Task NaviagteAsync<TPage>(bool isRootPage = false, IDictionary<string, object>? parameters = null, bool replaceCurrent = false) where TPage : Page
        {
            var route = typeof(TPage).Name;
            if (isRootPage)
                route = $"//{route}";
            else if (replaceCurrent)
                route = $"../{route}";

            return parameters != null ? Shell.Current.GoToAsync(route, parameters) : Shell.Current.GoToAsync(route);
        }

        public Task GoBackAsync(int depth = 0, IDictionary<string, object>? parameters = null)
        {
            var routeBuilder = new StringBuilder("..");
            for (int i = 0; i < depth; i++)
            {
                routeBuilder.Append("/..");
            }

            string route = routeBuilder.ToString();
            return parameters != null ? Shell.Current.GoToAsync(route, parameters) : Shell.Current.GoToAsync(route);
        }

        public Task NavigateBackToPageAsync(Type sourcePageType, IDictionary<string, object>? parameters = null)
        {
            var navStack = Shell.Current.Navigation.NavigationStack;

            int stepsBack = navStack.Reverse()
                                    .TakeWhile(page => page.GetType() != sourcePageType)
                                    .Count();

            if (stepsBack == navStack.Count)
                return Task.CompletedTask;

            if (stepsBack == 0)
                return Task.CompletedTask;

            string route = string.Join("/", Enumerable.Repeat("..", stepsBack));

            return parameters != null ? Shell.Current.GoToAsync(route, parameters) : Shell.Current.GoToAsync(route);
        }
    }
}