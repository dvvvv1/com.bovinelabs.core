﻿// <copyright file="ISingletonCollection.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.SingletonCollection
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities;

    public unsafe interface ISingletonCollection<T> : IComponentData
        where T : unmanaged, IDisposable
    {
        UnsafeList<T>* Collections { get; set; }

        Allocator Allocator { get; set; }
    }
}
