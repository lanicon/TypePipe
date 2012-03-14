// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.BuilderAbstractions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class ReflectionEmitTypeModifierTest
  {
    private IModuleBuilder _moduleBuilderMock;
    private ISubclassProxyNameProvider _subclassProxyNameProviderMock;

    private ReflectionEmitTypeModifier _reflectionEmitTypeModifier;

    [SetUp]
    public void SetUp ()
    {
      _moduleBuilderMock = MockRepository.GenerateStrictMock<IModuleBuilder> ();
      _subclassProxyNameProviderMock = MockRepository.GenerateStrictMock<ISubclassProxyNameProvider> ();

      _reflectionEmitTypeModifier = new ReflectionEmitTypeModifier (_moduleBuilderMock, _subclassProxyNameProviderMock);
    }

    [Test]
    public void CreateMutableType ()
    {
      var underlyingSystemType = ReflectionObjectMother.GetSomeUnsealedType ();
      var mutableType = _reflectionEmitTypeModifier.CreateMutableType (underlyingSystemType);

      Assert.That (mutableType.UnderlyingSystemType, Is.SameAs (underlyingSystemType));
    }

    [Test]
    public void ApplyModifications ()
    {
      var mutableTypeMock = MutableTypeObjectMother.CreateStrictMock();
      var fakeResultType = ReflectionObjectMother.GetSomeType ();
      var fakeUnderlyingSystemType = ReflectionObjectMother.GetSomeType ();

      _subclassProxyNameProviderMock.Expect (mock => mock.GetSubclassProxyName (fakeUnderlyingSystemType)).Return ("foofoo");

      var typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder> ();
      bool acceptCalled = false;
      _moduleBuilderMock
          .Expect (mock => mock.DefineType ("foofoo", TypeAttributes.Public | TypeAttributes.BeforeFieldInit, fakeUnderlyingSystemType))
          .Return (typeBuilderMock);
      mutableTypeMock
          .Expect (mock => mock.Accept (Arg<TypeModificationHandler>.Matches (handler => handler.SubclassProxyBuilder == typeBuilderMock)))
          .WhenCalled (mi => acceptCalled = true);
      mutableTypeMock
          .Stub (stub => stub.UnderlyingSystemType)
          .Return (fakeUnderlyingSystemType);
      typeBuilderMock
          .Expect (mock => mock.CreateType ()).Return (fakeResultType)
          .WhenCalled (mi => Assert.That (acceptCalled, Is.True));

      var result = _reflectionEmitTypeModifier.ApplyModifications (mutableTypeMock);

      _subclassProxyNameProviderMock.VerifyAllExpectations ();
      _moduleBuilderMock.VerifyAllExpectations ();
      typeBuilderMock.VerifyAllExpectations ();
      mutableTypeMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (fakeResultType));
    }
  }
}