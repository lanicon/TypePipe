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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Remotion.Utilities;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Generics
{
  [TestFixture]
  public class MethodInstantiationTest
  {
    private Type _typeParameter;
    private CustomParameterInfo _parameter;
    private CustomMethodInfo _genericMethodDefinition;
    private Type _typeArgument;

    private MethodInstantiation _instantiation;

    [SetUp]
    public void SetUp ()
    {
      _typeParameter = GenericParameterObjectMother.Create();
      _parameter = CustomParameterInfoObjectMother.Create ();
      _genericMethodDefinition = CustomMethodInfoObjectMother.Create (
          parameters: new[] { _parameter }, isGenericMethod: true, typeArguments: new[] { _typeParameter });
      _typeArgument = CustomTypeObjectMother.Create();

      _instantiation = new MethodInstantiation (_genericMethodDefinition, new[] { _typeArgument });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_instantiation.DeclaringType, Is.SameAs (_genericMethodDefinition.DeclaringType));
      Assert.That (_instantiation.Name, Is.EqualTo (_genericMethodDefinition.Name));
      Assert.That (_instantiation.Attributes, Is.EqualTo (_genericMethodDefinition.Attributes));
      Assert.That (_instantiation.IsGenericMethod, Is.True);
      Assert.That (_instantiation.GetGenericMethodDefinition(), Is.SameAs (_genericMethodDefinition));
      Assert.That (_instantiation.GetGenericArguments(), Is.EqualTo (new[] { _typeArgument }));

      var returnParameter = _instantiation.ReturnParameter;
      Assertion.IsNotNull (returnParameter);
      Assert.That (returnParameter, Is.TypeOf<MemberParameterOnInstantiation>());
      Assert.That (returnParameter.Member, Is.SameAs (_instantiation));
      Assert.That (returnParameter.As<MemberParameterOnInstantiation>().MemberParameterOnGenericDefinition, Is.SameAs (_genericMethodDefinition.ReturnParameter));

      var parameter = _instantiation.GetParameters().Single();
      Assert.That (parameter, Is.TypeOf<MemberParameterOnInstantiation>());
      Assert.That (parameter.Member, Is.SameAs (_instantiation));
      Assert.That (parameter.As<MemberParameterOnInstantiation>().MemberParameterOnGenericDefinition, Is.SameAs (_parameter));
    }

    [Test]
    public void GetCustomAttributeData ()
    {
      var customAttributes = new[] { CustomAttributeDeclarationObjectMother.Create () };
      var method = CustomMethodInfoObjectMother.Create (isGenericMethod: true, customAttributes: customAttributes);
      var methodInstantiation = new MethodInstantiation (method, Type.EmptyTypes);

      Assert.That (methodInstantiation.GetCustomAttributeData (), Is.EqualTo (customAttributes));
    }

    [Test]
    public void SubstituteGenericParameters_RecursiveGenericType ()
    {
      var recursiveGeneric = typeof (List<>).MakeTypePipeGenericType (typeof (Func<>).MakeTypePipeGenericType (_typeParameter));
      
      var list = _instantiation.SubstituteGenericParameters (recursiveGeneric);

      Assert.That (list.GetGenericTypeDefinition(), Is.SameAs (typeof (List<>)));
      var func = list.GetGenericArguments().Single();
      Assert.That (func.GetGenericTypeDefinition(), Is.SameAs (typeof (Func<>)));
      var typeArgument = func.GetGenericArguments().Single();
      Assert.That (typeArgument, Is.SameAs (_typeArgument));
    }

    [Test]
    public void SubstituteGenericParameters_NonGenericType ()
    {
      var nonGeneric = ReflectionObjectMother.GetSomeNonGenericType ();
      var result = _instantiation.SubstituteGenericParameters (nonGeneric);

      Assert.That (result, Is.SameAs (nonGeneric));
    }

    [Test]
    public void SubstituteGenericParameters_RemembersSubstitutedTypes ()
    {
      var genericMethodDefinition = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => GenericMethod<Dev.T> (null, null));
      var instantiation = new MethodInstantiation (genericMethodDefinition, new[] { _typeArgument });

      var parameterTypes = instantiation.GetParameters().Select (p => p.ParameterType).ToList();
      Assert.That (parameterTypes, Has.Count.EqualTo (2));
      Assert.That (parameterTypes[0].GetGenericTypeDefinition(), Is.SameAs (typeof (List<>)));
      Assert.That (parameterTypes[0].GetGenericArguments().Single(), Is.SameAs (_typeArgument));
      Assert.That (parameterTypes[0], Is.SameAs (parameterTypes[1]));
    }

    void GenericMethod<T1> (List<T1> p1, List<T1> p2) {}
  }
}