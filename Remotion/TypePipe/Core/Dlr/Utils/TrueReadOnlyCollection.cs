// This file is part of the re-motion TypePipe project (typepipe.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-motion TypePipe is free software; you can redistribute it 
// and/or modify it under the terms of the Apache License, Version 2.0
// as published by the Apache Software Foundation.
// 
// re-motion TypePipe is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// Apache License, Version 2.0 for more details.
// 
// You should have received a copy of the Apache License, Version 2.0
// along with re-motion; if not, see http://www.apache.org/licenses.
// 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Runtime.CompilerServices {
    sealed class TrueReadOnlyCollection<T> : ReadOnlyCollection<T> {
        /// <summary>
        /// Creates instnace of TrueReadOnlyCollection, wrapping passed in array.
        /// !!! DOES NOT COPY THE ARRAY !!!
        /// </summary>
        internal TrueReadOnlyCollection(T[] list)
            : base(list) {
        }
    }
}
