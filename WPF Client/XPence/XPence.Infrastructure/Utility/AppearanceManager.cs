/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro;

namespace XPence.Infrastructure.Utility
{
    /// <summary>
    /// A static class that manages the appearance of the application.
    /// </summary>
    public class AppearanceManager
    {
        internal static readonly ResourceDictionary LightChartResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/XPence.Infrastructure;component/Resources/ChartLight.xaml") };
        internal static readonly ResourceDictionary DarkChartResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/XPence.Infrastructure;component/Resources/ChartDark.xaml") };
        internal static readonly ResourceDictionary ChartAccentResources = new ResourceDictionary { Source = new Uri("pack://application:,,,/XPence.Infrastructure;component/Resources/ChartPaletteResources.xaml") };

        private static readonly string LightThemeText;
        private static readonly string DarkThemeText;

        #region Constructors

        /// <summary>
        /// Static constructor to initialize static variables.
        /// </summary>
        static AppearanceManager()
        {
            LightThemeText = "Light";
            DarkThemeText = "Dark";
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Gets the accent names.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAccentNames()
        {
            return ThemeManager.DefaultAccents.Select(a => a.Name).ToList();
        }

        /// <summary>
        /// Gets the theme names.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetThemeNames()
        {
            var themes = new[] { LightThemeText, DarkThemeText };
            return themes;
        }

        /// <summary>
        /// Gets the accent name that the application is displaying presently.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationAccent()
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            return theme.Item2.Name;
        }

        /// <summary>
        /// Gets the theme name that the app is displaying presently.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationTheme()
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            if (theme.Item1 == Theme.Dark)
                return DarkThemeText;
            if (theme.Item1 == Theme.Light)
                return LightThemeText;
            throw new Exception("Undetected theme.");
        }

        /// <summary>
        /// Changes the accent of the application.
        /// </summary>
        /// <param name="accentName">The name of the accent color.</param>
        public static void ChangeAccent(string accentName)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            var accent = ThemeManager.DefaultAccents.First(x => x.Name == accentName);
            ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1);
            RefreshChartResources(Application.Current.Resources);
        }

        /// <summary>
        /// Changes the theme of the application.
        /// </summary>
        /// <param name="themeName">The name of the theme.</param>
        public static void ChangeTheme(string themeName)
        {
            ChangeThemeForGraph(Application.Current.Resources, themeName);
            if (string.CompareOrdinal(LightThemeText, themeName) == 0)
            {
                var theme = ThemeManager.DetectTheme(Application.Current);
                ThemeManager.ChangeTheme(Application.Current, theme.Item2, Theme.Light);
            }
            else if (string.CompareOrdinal(DarkThemeText, themeName) == 0)
            {
                var theme = ThemeManager.DetectTheme(Application.Current);
                ThemeManager.ChangeTheme(Application.Current, theme.Item2, Theme.Dark);
            }
            else
            {
                throw new ValueUnavailableException("Theme name not known.");
            }
        }

        /// <summary>
        /// This is a little cheat method to refresh the palette colors with the very accent colors provided by MahApps metro.
        /// The below method just refreshes the resources that are responsible for the palette colors.
        /// </summary>
        /// <param name="resources"></param>
        private static void RefreshChartResources(ResourceDictionary resources)
        {
            var md = resources.MergedDictionaries.FirstOrDefault(d => d.Source == ChartAccentResources.Source);
            if (null != md)
            {
                resources.MergedDictionaries.Remove(md);
                resources.MergedDictionaries.Add(ChartAccentResources);
            }
        }

        /// <summary>
        /// Chnages the theme for graph.
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="themeName"></param>
        private static void ChangeThemeForGraph(ResourceDictionary resources, string themeName)
        {
            if (resources == null) 
                return;
            ResourceDictionary oldChartThemeResource;
            ResourceDictionary newChartThemeResource;
            if (string.CompareOrdinal(LightThemeText, themeName) == 0)
            {
                oldChartThemeResource = DarkChartResource;
                newChartThemeResource = LightChartResource;

            }
            else if (string.CompareOrdinal(DarkThemeText, themeName) == 0)
            {
                oldChartThemeResource = LightChartResource;
                newChartThemeResource = DarkChartResource;
            }
            else
            {
                throw new ValueUnavailableException("Theme resource not found for graph.");
            }
            if (oldChartThemeResource != null)
            {
                var md = resources.MergedDictionaries.FirstOrDefault(d => d.Source == oldChartThemeResource.Source);
                if(null!=md)
                {
                    resources.MergedDictionaries.Add(newChartThemeResource);
                    var chartThemeChanged = resources.MergedDictionaries.Remove(md);
                    if(!chartThemeChanged)
                    {
                        throw new Exception("Theme for chart could not be changed");
                    }
                }
            }
        }

        #endregion
    }
}
