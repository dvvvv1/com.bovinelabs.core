﻿// <copyright file="ArchetypeChunkInternals.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Internal
{
    using System;
    using System.Diagnostics;
    using Unity.Burst.CompilerServices;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities;

    public static class ArchetypeChunkInternals
    {
        public static unsafe void SetChangeFilter<T>(this ArchetypeChunk chunk, ComponentTypeHandle<T> handle)
            where T : unmanaged, IComponentData
        {
            SetChangeFilterCheckWriteAndThrow(handle);

            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, handle.m_TypeIndex);
            if (Hint.Unlikely(typeIndexInArchetype == -1))
            {
                return;
            }

            // This should (=S) be thread safe int writes are atomic in c#
            chunk.m_Chunk->SetChangeVersion(typeIndexInArchetype, handle.GlobalSystemVersion);
        }

        public static unsafe void SetChangeFilter<T>(this ArchetypeChunk chunk, ComponentTypeHandle<T> handle, uint version)
            where T : unmanaged, IComponentData
        {
            SetChangeFilterCheckWriteAndThrow(handle);

            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, handle.m_TypeIndex);
            if (Hint.Unlikely(typeIndexInArchetype == -1))
            {
                return;
            }

            // This should (=S) be thread safe int writes are atomic in c#
            chunk.m_Chunk->SetChangeVersion(typeIndexInArchetype, version);
        }

        public static unsafe void SetChangeFilter<T>(this ArchetypeChunk chunk, BufferTypeHandle<T> handle)
            where T : unmanaged, IBufferElementData
        {
            SetChangeFilterCheckWriteAndThrow(handle);

            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, handle.m_TypeIndex);
            if (Hint.Unlikely(typeIndexInArchetype == -1))
            {
                return;
            }

            // This should (=S) be thread safe int writes are atomic in c#
            chunk.m_Chunk->SetChangeVersion(typeIndexInArchetype, handle.GlobalSystemVersion);
        }

        public static unsafe void SetChangeFilter<T>(this ArchetypeChunk chunk, BufferTypeHandle<T> handle, uint version)
            where T : unmanaged, IBufferElementData
        {
            SetChangeFilterCheckWriteAndThrow(handle);

            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, handle.m_TypeIndex);
            if (Hint.Unlikely(typeIndexInArchetype == -1))
            {
                return;
            }

            // This should (=S) be thread safe int writes are atomic in c#
            chunk.m_Chunk->SetChangeVersion(typeIndexInArchetype, version);
        }

        public static unsafe void SetChangeFilter(this ArchetypeChunk chunk, DynamicComponentTypeHandle handle)
        {
            SetChangeFilterCheckWriteAndThrow(handle);

            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, handle.m_TypeIndex);
            if (Hint.Unlikely(typeIndexInArchetype == -1))
            {
                return;
            }

            chunk.m_Chunk->SetChangeVersion(typeIndexInArchetype, handle.GlobalSystemVersion);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void SetChangeFilterCheckWriteAndThrow<T>(ComponentTypeHandle<T> chunkComponentType)
            where T : IComponentData
        {
            if (chunkComponentType.IsReadOnly)
            {
                throw new ArgumentException("SetChangeFilter used on read only type handle");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void SetChangeFilterCheckWriteAndThrow<T>(BufferTypeHandle<T> chunkComponentType)
            where T : unmanaged, IBufferElementData
        {
            if (chunkComponentType.IsReadOnly)
            {
                throw new ArgumentException("SetChangeFilter used on read only type handle");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void SetChangeFilterCheckWriteAndThrow(DynamicComponentTypeHandle chunkComponentType)
        {
            if (chunkComponentType.IsReadOnly)
            {
                throw new ArgumentException("SetChangeFilter used on read only type handle");
            }
        }
    }
}
