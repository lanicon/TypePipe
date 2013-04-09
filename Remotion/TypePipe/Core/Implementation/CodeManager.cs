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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.Implementation
{
  // TODO review: updat edocs
  /// <summary>
  /// Manages the code generated by the pipeline by delegating to the contained <see cref="IGeneratedCodeFlusher"/> and <see cref="ITypeCache"/> instance.
  /// Note that the <see cref="IGeneratedCodeFlusher"/> instance is guarded by the specified lock.
  /// </summary>
  public class CodeManager : ICodeManager, ICodeGenerator
  {
    private static readonly ConstructorInfo s_typePipeAssemblyAttributeCtor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new TypePipeAssemblyAttribute ("participantConfigurationID"));

    private readonly IGeneratedCodeFlusher _generatedCodeFlusher;
    private readonly object _codeGenerationLock;
    private readonly ITypeCache _typeCache;

    public CodeManager (IGeneratedCodeFlusher generatedCodeFlusher, object codeGenerationLock, ITypeCache typeCache)
    {
      ArgumentUtility.CheckNotNull ("generatedCodeFlusher", generatedCodeFlusher);
      ArgumentUtility.CheckNotNull ("codeGenerationLock", codeGenerationLock);
      ArgumentUtility.CheckNotNull ("typeCache", typeCache);

      _generatedCodeFlusher = generatedCodeFlusher;
      _codeGenerationLock = codeGenerationLock;
      _typeCache = typeCache;
    }

    public string AssemblyDirectory
    {
      get { lock (_codeGenerationLock) return _generatedCodeFlusher.AssemblyDirectory; }
    }

    public string AssemblyNamePattern
    {
      get { lock (_codeGenerationLock) return _generatedCodeFlusher.AssemblyNamePattern; }
    }

    public void SetAssemblyDirectory (string assemblyDirectory)
    {
      lock (_codeGenerationLock)
        _generatedCodeFlusher.SetAssemblyDirectory (assemblyDirectory);
    }

    public void SetAssemblyNamePattern (string assemblyNamePattern)
    {
      lock (_codeGenerationLock)
        _generatedCodeFlusher.SetAssemblyNamePattern (assemblyNamePattern);
    }

    public string FlushCodeToDisk ()
    {
      var participantConfigurationID = _typeCache.ParticipantConfigurationID;
      var assemblyAttribute = new CustomAttributeDeclaration (s_typePipeAssemblyAttributeCtor, new object[] { participantConfigurationID });

      lock (_codeGenerationLock)
        return _generatedCodeFlusher.FlushCodeToDisk (assemblyAttribute);
    }

    public void LoadFlushedCode (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      LoadFlushedCode ((_Assembly) assembly);
    }

    [CLSCompliant (false)]
    public void LoadFlushedCode (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      var assemblyAttribute = (TypePipeAssemblyAttribute) assembly.GetCustomAttributes (typeof (TypePipeAssemblyAttribute), inherit: false).SingleOrDefault();
      if (assemblyAttribute == null)
        throw new ArgumentException ("The specified assembly was not generated by the pipeline.", "assembly");

      if (assemblyAttribute.ParticipantConfigurationID != _typeCache.ParticipantConfigurationID)
      {
        var message = string.Format (
            "The specified assembly was generated with a different participant configuration: '{0}'.", assemblyAttribute.ParticipantConfigurationID);
        throw new ArgumentException (message, "assembly");
      }

      _typeCache.LoadTypes (assembly.GetTypes());
    }

    public Type GetOrGenerateType (
        ConcurrentDictionary<object[], Type> types,
        object[] key,
        ITypeAssembler typeAssembler,
        Type requestedType,
        IDictionary<string, object> participantState,
        IMutableTypeBatchCodeGenerator mutableTypeBatchCodeGenerator)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("typeAssembler", typeAssembler);
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);
      ArgumentUtility.CheckNotNull ("participantState", participantState);
      ArgumentUtility.CheckNotNull ("mutableTypeBatchCodeGenerator", mutableTypeBatchCodeGenerator);

      Type generatedType;
      lock (_codeGenerationLock)
      {
        if (types.TryGetValue (key, out generatedType))
          return generatedType;

        generatedType = typeAssembler.AssembleType (requestedType, participantState, mutableTypeBatchCodeGenerator);
        types.Add (key, generatedType);
      }

      return generatedType;
    }
  }
}