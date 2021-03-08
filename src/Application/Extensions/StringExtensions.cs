using System;
using System.IO;

namespace tabbR.Application.Extensions
{
    public static class StringExtensions
    {
        public static string BuildIconPath(this string fileName) =>
            Path.Combine(AppContext.BaseDirectory, "resources", fileName);
    }
}
