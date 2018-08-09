using System;

using Microsoft.Dism;

namespace WindowsFeatures.Implementations
{
    /// <summary>
    /// Microsoft Dism wrapper example.
    /// </summary>
    /// <remarks>
    /// Nuget package name "Microsoft.Dism".
    /// </remarks>
    internal class DismWrapperExample
    {
        /// <summary>
        /// Execute example.
        /// </summary>
        public void Run()
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            using (var session = DismApi.OpenOnlineSession())
            { 
                foreach (DismFeature feature in DismApi.GetFeatures(session))
                {
                    Console.WriteLine($"[Feature Name] {feature.FeatureName}");
                    Console.WriteLine($"[State] {feature.State}");
                    Console.WriteLine();
                }
            }

            DismApi.Shutdown();
        }
    }
}