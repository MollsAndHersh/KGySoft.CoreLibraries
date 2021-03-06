﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ByteArrayExtensionsTest.cs
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

using System;
using System.Linq;

using NUnit.Framework;

#endregion

namespace KGySoft.CoreLibraries.UnitTests.CoreLibraries.Extensions
{
    [TestFixture]
    public class ByteArrayExtensionsTest : TestBase
    {
        #region Methods

        [Test]
        public void ToBase64String()
        {
            byte[] bytes = Enumerable.Range(0, 255).Select(i => (byte)i).ToArray();
            string result = bytes.ToBase64String();
            Assert.IsFalse(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToBase64String(80);
            Assert.IsFalse(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToBase64String(80, 6);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));
        }

        [Test]
        public void ToHexString()
        {
            byte[] bytes = Enumerable.Range(1, 3).Select(i => (byte)i).ToArray();

            string result = bytes.ToHexValuesString(null, 0, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(null, 6, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(null, 4, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(null, 3, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(null, 1, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 0, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 10, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 8, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 6, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 4, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 3, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 2, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 1, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            bytes = new byte[1];
            result = bytes.ToHexValuesString(", ", 2, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToHexValuesString(", ", 1, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            Throws<ArgumentException>(() => bytes.ToHexValuesString("a"));
        }

        [Test]
        public void ToDecimalString()
        {
            byte[] bytes = Enumerable.Range(1, 3).Select(i => (byte)i).ToArray();

            Throws<ArgumentNullException>(() => bytes.ToDecimalValuesString(null));

            string result = bytes.ToDecimalValuesString();
            Assert.IsFalse(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 0, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 8, 2);
            Assert.IsFalse(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 6, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 4, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 3, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 2, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 1, 2);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsTrue(result.Contains(Environment.NewLine));

            bytes = new byte[1];
            result = bytes.ToDecimalValuesString(", ", 2, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            result = bytes.ToDecimalValuesString(", ", 1, 2, ' ', true);
            Assert.IsTrue(Char.IsWhiteSpace(result[0]));
            Assert.IsFalse(result.Contains(Environment.NewLine));

            Throws<ArgumentException>(() => bytes.ToDecimalValuesString("0"));
        }

        #endregion
    }
}
