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
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using System.Linq;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  [TestFixture]
  public class MutableInfoCustomAttributeHelperTest
  {
    private MemberInfo _member;
    private IMutableInfo _mutableMember;
    private IMutableInfo _mutableParameter;
    private Func<ReadOnlyCollection<ICustomAttributeData>> _attributeProvider;
    private Func<bool> _canAddCustomAttributesDecider;

    private MutableInfoCustomAttributeHelper _helper;
    private MutableInfoCustomAttributeHelper _helperForParameter;

    private ConstructorInfo _attributeCtor;

    [SetUp]
    public void SetUp ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Member (7));
      var mutableMethod = MutableMethodInfoObjectMother.CreateForExisting (method);
      _member = method;
      _mutableMember = mutableMethod;
      // TODO method.MutableParameters
      _mutableParameter = (MutableParameterInfo) mutableMethod.GetParameters().Single();
      _attributeProvider = () => { throw new Exception ("should be lazy"); };
      _canAddCustomAttributesDecider = () => { throw new Exception ("should be lazy"); };

      _helper = new MutableInfoCustomAttributeHelper (_mutableMember, () => _attributeProvider(), () => _canAddCustomAttributesDecider());
      _helperForParameter = new MutableInfoCustomAttributeHelper (_mutableParameter, () => _attributeProvider(), () => _canAddCustomAttributesDecider());

      _attributeCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new UnrelatedAttribute());
    }

    [Test]
    public void AddedCustomAttributeDeclarations ()
    {
      Assert.That (_helper.AddedCustomAttributeDeclarations, Is.Empty);
    }

    [Test]
    public void AddCustomAttribute ()
    {
      var declaration = new CustomAttributeDeclaration (_attributeCtor, new object[0]);
      _canAddCustomAttributesDecider = () => true;

      _helper.AddCustomAttribute (declaration);

      Assert.That (_helper.AddedCustomAttributeDeclarations, Is.EqualTo (new[] { declaration }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Adding custom attributes to this element is not supported.")]
    public void AddCustomAttribute_CannotAdd ()
    {
      _canAddCustomAttributesDecider = () => false;
      _helper.AddCustomAttribute (new CustomAttributeDeclaration (_attributeCtor, new object[0]));
    }

    [Test]
    public void GetCustomAttributeData ()
    {
      var addedData = new CustomAttributeDeclaration (_attributeCtor, new object[0]);
      var existingData = new CustomAttributeDeclaration (_attributeCtor, new object[0]);
      _canAddCustomAttributesDecider = () => true;
      _helper.AddCustomAttribute (addedData);

      var callCount = 0;
      _attributeProvider = () =>
      {
        callCount++;
        return new ICustomAttributeData[] { existingData }.ToList().AsReadOnly();
      };

      var result = _helper.GetCustomAttributeData();

      Assert.That (result, Is.EqualTo (new[] { addedData, existingData }));

      Assert.That (callCount, Is.EqualTo (1));
      _helper.GetCustomAttributeData();
      Assert.That (callCount, Is.EqualTo (1), "should be cached");
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (_helper.GetCustomAttributes (inherit: false), Is.Empty);
      var result = _helper.GetCustomAttributes (inherit: true);

      Assert.That (result, Has.Length.EqualTo (1));
      var attribute = result.Single();
      Assert.That (attribute, Is.TypeOf<DerivedAttribute>());
    }

    [Test]
    public void GetCustomAttributes_NewInstance ()
    {
      var attribute1 = _helper.GetCustomAttributes (inherit: true).Single();
      var attribute2 = _helper.GetCustomAttributes (inherit: true).Single();

      Assert.That (attribute1, Is.Not.SameAs (attribute2));
    }

    [Test]
    public void GetCustomAttributes_Filter ()
    {
      Assert.That (_helper.GetCustomAttributes (typeof (UnrelatedAttribute), inherit: true), Is.Empty);
      Assert.That (_helper.GetCustomAttributes (typeof (DerivedAttribute), inherit: true), Has.Length.EqualTo (1));
      Assert.That (_helper.GetCustomAttributes (typeof (BaseAttribute), inherit: true), Has.Length.EqualTo (1));
      Assert.That (_helper.GetCustomAttributes (typeof (IBaseAttributeInterface), inherit: true), Has.Length.EqualTo (1));
    }

    [Test]
    public void GetCustomAttributes_ArrayType ()
    {
      // Standard reflection. Use as reference behavior.
      Assert.That (_member.GetCustomAttributes (false), Is.TypeOf (typeof (object[])));
      Assert.That (_member.GetCustomAttributes (typeof (BaseAttribute), false), Is.TypeOf (typeof (BaseAttribute[])));

      Assert.That (_helper.GetCustomAttributes (false), Is.TypeOf (typeof (object[])));
      Assert.That (_helper.GetCustomAttributes (typeof (BaseAttribute), false), Is.TypeOf (typeof (BaseAttribute[])));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (_helper.IsDefined (typeof (UnrelatedAttribute), inherit: true), Is.False);
      Assert.That (_helper.IsDefined (typeof (DerivedAttribute), inherit: true), Is.True);
      Assert.That (_helper.IsDefined (typeof (BaseAttribute), inherit: true), Is.True);
      Assert.That (_helper.IsDefined (typeof (IBaseAttributeInterface), inherit: true), Is.True);
    }

    [Test]
    public void GetCustomAttributes_Parameter ()
    {
      // Inherit must be false for parameters.
      var result = _helperForParameter.GetCustomAttributes (inherit: false);

      Assert.That (result, Has.Length.EqualTo (1));
      var attribute = result.Single();
      Assert.That (attribute, Is.TypeOf<DerivedAttribute>());
    }

    public class DomainTypeBase
    {
      [Derived]
      public virtual void Member (int arg) { Dev.Null = arg; }
    }

    public class DomainType : DomainTypeBase
    {
      public override void Member ([Derived] int arg) { Dev.Null = arg; }
    }

    interface IBaseAttributeInterface { }
    class BaseAttribute : Attribute, IBaseAttributeInterface { }
    class DerivedAttribute : BaseAttribute { }
    public class UnrelatedAttribute : Attribute { }
  }
}