﻿// <copyright file="KAttribute.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_CONFIG
namespace BovineLabs.Core.Keys
{
    using System;
    using UnityEngine;

    /// <summary> Apply to a byte/integer field to display the name defined in the <see cref="Settings" /> file. </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KAttribute : PropertyAttribute
    {
        /// <summary> Initializes a new instance of the <see cref="KAttribute" /> class. </summary>
        /// <param name="settings"> The name of the settings file. </param>
        /// <param name="flags"> Is K used as flags. </param>
        public KAttribute(string settings, bool flags = false)
        {
            this.Settings = settings;
            this.Flags = flags;
        }

        /// <summary> Gets the name of the settings file. </summary>
        public string Settings { get; }

        /// <summary> Gets a value indicating whether it should be drawn as flags or not. </summary>
        public bool Flags { get; }
    }
}
#endif
