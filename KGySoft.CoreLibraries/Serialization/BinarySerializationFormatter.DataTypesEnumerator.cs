﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BinarySerializationFormatter.DataTypesEnumerator.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;
using KGySoft.Collections;

#endregion

namespace KGySoft.Serialization
{
    public sealed partial class BinarySerializationFormatter
    {
        /// <summary>
        /// A special lightweight enumerator for encoded <see cref="DataTypes"/> collections.
        /// </summary>
        private sealed class DataTypesEnumerator
        {
            #region Fields

            private readonly IList<DataTypes> dataTypes;
            private DataTypes current;
            private int index;

            #endregion

            #region Properties

            internal DataTypes CurrentSeparated => IsCollectionType(current) ? GetCollectionDataType(current) : GetElementDataType(current);

            internal DataTypes Current => current;

            #endregion

            #region Constructors

            internal DataTypesEnumerator(IList<DataTypes> source, bool moveFirst = false)
            {
                dataTypes = source;
                if (moveFirst)
                    MoveNext();
            }

            #endregion

            #region Methods

            #region Public Methods

            public override string ToString() => DataTypeToString(current);

            #endregion

            #region Internal Methods

            internal bool MoveNext()
            {
                if (index < dataTypes.Count)
                {
                    current = dataTypes[index++];
                    return true;
                }

                current = DataTypes.Null;
                return false;
            }

            internal bool MoveNextExtracted()
            {
                if (current == DataTypes.Null && index >= dataTypes.Count)
                    return false;

                if (!IsCollectionType(current))
                    return MoveNext();

                current = GetElementDataType(current);
                return current != DataTypes.Null || MoveNext();
            }

            internal DataTypesEnumerator Clone() =>
                new DataTypesEnumerator(dataTypes)
                {
                    current = current,
                    index = index
                };

            internal void Reset()
            {
                index = 0;
                current = DataTypes.Null;
            }

            internal void MoveToFirst()
            {
                Reset();
                MoveNext();
            }

            internal IList<DataTypes> GetCurrentSegment()
            {
                int end = index - 1;
                int skip = 1;
                do
                {
                    DataTypes dt = dataTypes[end++];
                    if (IsDictionary(dt))
                    {
                        if (!IsElementType(dt))
                            skip++;
                    }
                    else if (IsElementType(dt))
                        skip--;
                } while (skip > 0);

                return new ListSegment<DataTypes>(dataTypes, index - 1, end - index + 1);
            }

            #endregion

            #endregion
        }
    }
}
