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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.MutableReflection;
using System.Collections.Generic;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class SignatureDebugStringGeneratorTest
  {
    [Test]
    public void GetMethodSignatureString ()
    {
      var method = ReflectionObjectMother.GetMethod ((DomainType dt) => dt.Method (ref Dev<int>.Dummy, null));

      var result = SignatureDebugStringGenerator.GetMethodSignatureString (method);

      Assert.That (result, Is.EqualTo ("String Method(Int32&, Dictionary`2[Int32,DateTime])"));
    }

    [Test]
    public void GetConstructorSignatureString ()
    {
      var constructor = ReflectionObjectMother.GetConstructor (() => new DomainType(7, ref Dev<string>.Dummy));

      var result = SignatureDebugStringGenerator.GetConstructorSignatureString (constructor);

      Assert.That (result, Is.EqualTo ("Void .ctor(Int32, String&)"));
    }

    class DomainType
    {
      public DomainType (int i, ref string s)
      {
        Dev.Null = i;
        s = null;
      }

      public string Method (ref int i, Dictionary<int, DateTime> dictionary)
      {
        i = 0;
        Dev.Null = dictionary;
        return ""; 
      }
    }
  }
}