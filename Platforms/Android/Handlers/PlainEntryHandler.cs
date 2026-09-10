using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Equiparts.Platforms.Handlers
{
    public class PlainEntryHandler : EntryHandler
    {
        protected override MauiAppCompatEditText CreatePlatformView()
        {
            var editText = new MauiAppCompatEditText(Context)
            {
                Background = null
            };

            editText.SetBackgroundColor(Colors.Transparent.ToPlatform());
            editText.SetPadding(0, 0, 0, 0);
            editText.SetSingleLine(true);
            editText.Background = null;

            return editText;
        }
    }
}