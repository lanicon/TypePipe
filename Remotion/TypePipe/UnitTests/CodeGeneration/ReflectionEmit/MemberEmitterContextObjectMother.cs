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
using System.Runtime.CompilerServices;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  public static class MemberEmitterContextObjectMother
  {
    public static MemberEmitterContext GetSomeContext (
        MutableType mutableType = null,
        ITypeBuilder typeBuilder = null,
        DebugInfoGenerator debugInfoGenerator = null,
        IEmittableOperandProvider emittableOperandProvider = null,
        IMethodTrampolineProvider methodTrampolineProvider = null,
        DeferredActionManager postDeclarationsActionManager = null)
    {
      return new MemberEmitterContext (
          mutableType ?? MutableTypeObjectMother.CreateForExistingType(),
          typeBuilder ?? MockRepository.GenerateStub<ITypeBuilder>(),
          debugInfoGenerator ?? MockRepository.GenerateStub<DebugInfoGenerator>(),
          emittableOperandProvider ?? MockRepository.GenerateStub<IEmittableOperandProvider>(),
          methodTrampolineProvider ?? MockRepository.GenerateStub<IMethodTrampolineProvider>(),
          postDeclarationsActionManager ?? new DeferredActionManager());
    }
  }
}