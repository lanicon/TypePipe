﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Provides a synchronization wrapper around an implementation of <see cref="IReflectionEmitCodeGenerator"/>.
  /// </summary>
  public class LockingReflectionEmitCodeGeneratorDecorator : IReflectionEmitCodeGenerator
  {
    private readonly object _lock = new object();
    private readonly IReflectionEmitCodeGenerator _innerCodeGenerator;

    [CLSCompliant (false)]
    public LockingReflectionEmitCodeGeneratorDecorator (IReflectionEmitCodeGenerator innerCodeGenerator)
    {
      ArgumentUtility.CheckNotNull ("innerCodeGenerator", innerCodeGenerator);

      _innerCodeGenerator = innerCodeGenerator;
    }

    public string AssemblyDirectory
    {
      get { lock (_lock) return _innerCodeGenerator.AssemblyDirectory; }
    }

    public string AssemblyName
    {
      get { lock (_lock) return _innerCodeGenerator.AssemblyName; }
    }

    public DebugInfoGenerator DebugInfoGenerator
    {
      get { lock (_lock) return _innerCodeGenerator.DebugInfoGenerator; }
    }

    public void SetAssemblyDirectory (string assemblyDirectory)
    {
      lock (_lock) _innerCodeGenerator.SetAssemblyDirectory (assemblyDirectory);
    }

    public void SetAssemblyName (string assemblyName)
    {
      lock (_lock) _innerCodeGenerator.SetAssemblyName (assemblyName);
    }

    public string FlushCodeToDisk (string participantConfigurationID)
    {
      lock (_lock) return _innerCodeGenerator.FlushCodeToDisk (participantConfigurationID);
    }

    public IEmittableOperandProvider CreateEmittableOperandProvider ()
    {
      lock (_lock) return _innerCodeGenerator.CreateEmittableOperandProvider();
    }

    [CLSCompliant (false)]
    public ITypeBuilder DefineType (string name, TypeAttributes attributes, IEmittableOperandProvider emittableOperandProvider)
    {
      lock (_lock) return _innerCodeGenerator.DefineType (name, attributes, emittableOperandProvider);
    }
  }
}