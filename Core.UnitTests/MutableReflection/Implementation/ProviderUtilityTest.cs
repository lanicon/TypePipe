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
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  [TestFixture]
  public class ProviderUtilityTest
  {
    [Test]
    public void GetNonNullValue ()
    {
      var context = "hello";
      var value = new object();
      Func<string, object> provider = ctx =>
      {
        Assert.That (ctx, Is.SameAs (context));
        return value;
      };

      var result = ProviderUtility.GetNonNullValue (provider, context, providerArgumentName: "does not matter");

      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must not return null.\r\nParameter name: argumentName")]
    public void GetNonNullValue_ThrowsForNullReturningProvider ()
    {
      Func<string, object> provider = ctx => null;

      ProviderUtility.GetNonNullValue (provider, "", "argumentName");
    }
  }
}