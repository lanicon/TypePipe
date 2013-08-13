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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  [TestFixture]
  public class MethodBaseDefinitionCacheTest
  {
    [Test]
    public void GetBaseDefinition_ForCustomMethodInfo_NotCached ()
    {
      var customMethodInfoMock = MockRepository.GenerateStrictMock<CustomMethodInfo> (
          ReflectionObjectMother.GetSomeType(), "method", MethodAttributes.Public, null, Type.EmptyTypes);

      var fakeMethodDefinition1 = ReflectionObjectMother.GetSomeMethod();
      customMethodInfoMock.Expect (mock => mock.GetBaseDefinition ()).Return (fakeMethodDefinition1).Repeat.Once();

      var fakeMethodDefinition2 = ReflectionObjectMother.GetSomeMethod();
      customMethodInfoMock.Expect (mock => mock.GetBaseDefinition ()).Return (fakeMethodDefinition2).Repeat.Once ();

      var result1 = MethodBaseDefinitionCache.GetBaseDefinition (customMethodInfoMock);
      var result2 = MethodBaseDefinitionCache.GetBaseDefinition (customMethodInfoMock);

      customMethodInfoMock.VerifyAllExpectations();

      Assert.That (result1, Is.SameAs (fakeMethodDefinition1));
      Assert.That (result2, Is.SameAs (fakeMethodDefinition2));
    }

    [Test]
    public void GetBaseDefinition_ForStandardMethodInfo_Cached ()
    {
      var standardMethodInfoMock = MockRepository.GenerateStrictMock<MethodInfo>();

      var fakeMethodDefinition = ReflectionObjectMother.GetSomeMethod ();
      standardMethodInfoMock.Expect (mock => mock.GetBaseDefinition ()).Return (fakeMethodDefinition).Repeat.Once ();

      var result1 = MethodBaseDefinitionCache.GetBaseDefinition (standardMethodInfoMock);
      var result2 = MethodBaseDefinitionCache.GetBaseDefinition (standardMethodInfoMock);

      standardMethodInfoMock.VerifyAllExpectations ();

      Assert.That (result1, Is.SameAs (fakeMethodDefinition));
      Assert.That (result2, Is.SameAs (fakeMethodDefinition));
    } 
  }
}