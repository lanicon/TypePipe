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
using System.Reflection.Emit;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Expressions
{
  /// <summary>
  /// Represents an <see cref="OpCodes.Unbox_Any"/> operation.
  /// </summary>
  public class UnboxExpression : UnaryExpressionBase
  {
    public UnboxExpression (Expression operand, Type toType)
        : base (operand, toType)
    {
    }

    public override UnaryExpressionBase Update (Expression operand)
    {
      ArgumentUtility.CheckNotNull ("operand", operand);

      if (operand == Operand)
        return this;

      return new UnboxExpression (operand, Type);
    }

    public override Expression Accept (IPrimitiveTypePipeExpressionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      return visitor.VisitUnbox (this);
    }
  }
}