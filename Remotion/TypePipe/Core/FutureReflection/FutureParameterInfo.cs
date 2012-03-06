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
using Remotion.Utilities;

namespace Remotion.TypePipe.FutureReflection
{
  /// <summary>
  /// Represents a parameter for a member that does not exist yet. This is used to represent the signature of a member yet to be generatedwithin an
  /// expression tree.
  /// </summary>
  public class FutureParameterInfo : ParameterInfo
  {
    private readonly Type _parameterType;

    public FutureParameterInfo (Type parameterType)
    {
      ArgumentUtility.CheckNotNull ("parameterType", parameterType);

      _parameterType = parameterType;
    }

    public override Type ParameterType
    {
      get { return _parameterType; }
    }
  }
}