﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn;

namespace RoslynPad.Hosting
{
    [DataContract]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    internal class InitializationParameters
    {
        public InitializationParameters(IList<string> references, IList<string> imports, string workingDirectory, bool shadowCopyAssemblies = true, OptimizationLevel optimizationLevel = OptimizationLevel.Debug, bool checkOverflow = false, bool allowUnsafe = true)
        {
            References = references;
            Imports = imports;
            WorkingDirectory = workingDirectory;
            ShadowCopyAssemblies = shadowCopyAssemblies;
            OptimizationLevel = optimizationLevel;
            CheckOverflow = checkOverflow;
            AllowUnsafe = allowUnsafe;
        }

        [DataMember]
        public IList<string> References { get; set; }
        [DataMember]
        public IList<string> Imports { get; set; }
        [DataMember]
        public string WorkingDirectory { get; private set; }
        [DataMember]
        public bool ShadowCopyAssemblies { get; private set; }
        [DataMember]
        public OptimizationLevel OptimizationLevel { get; private set; }
        [DataMember]
        public bool CheckOverflow { get; private set; }
        [DataMember]
        public bool AllowUnsafe { get; private set; }
    }
}