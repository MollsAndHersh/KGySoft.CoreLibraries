﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ResXResourceReaderTest.cs
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using KGySoft.Resources;

using NUnit.Framework;

#endregion

namespace KGySoft.CoreLibraries.UnitTests.Resources
{
    [TestFixture]
    public class ResXResourceReaderTest : TestBase
    {
        #region Methods

        [Test]
        public void ParseData()
        {
            string path = Path.Combine(Files.GetExecutingPath(), "Resources\\TestRes.resx");
            var refReader = new System.Resources.ResXResourceReader(path, new[] { new AssemblyName("System.Drawing"), new AssemblyName("System") });
            refReader.BasePath = Files.GetExecutingPath();
            ResXResourceReader reader = new ResXResourceReader(path);

            Assert.AreNotEqual(
                refReader.Cast<object>().Count(), // this forces immediate enumeration
                reader.Cast<object>().Count()); // this returns duplicates as separated items

            Assert.AreNotEqual(
                refReader.Cast<object>().Count(), // cached
                reader.Cast<object>().Count()); // second enumeration is cached, though still returns duplicates

            reader = new ResXResourceReader(path) { AllowDuplicatedKeys = false };
            Assert.AreEqual(
                refReader.Cast<object>().Count(), // cached
                reader.Cast<object>().Count()); // duplication is off (not lazy now)
        }

        [Test]
        public void TestEnumerators()
        {
            string path = Path.Combine(Files.GetExecutingPath(), "Resources\\TestRes.resx");
            ResXResourceReader reader = new ResXResourceReader(path);

            // reading one element, then reset, read first element again
            var resEnumLazy = reader.GetEnumerator();
            resEnumLazy.MoveNext();
            var firstRes = resEnumLazy.Entry;
            resEnumLazy.Reset();
            resEnumLazy.MoveNext();
            Assert.AreEqual(firstRes, resEnumLazy.Entry);

            // getting enumerator again: cached
            var resEnumCached = reader.GetEnumerator();
            Assert.AreNotEqual(resEnumLazy.GetType(), resEnumCached.GetType());
            resEnumCached.MoveNext();
            Assert.AreEqual(firstRes, resEnumCached.Entry);

            // the lazy cached the rest of the elements into a buffer so they both see the second element now
            resEnumLazy.MoveNext();
            resEnumCached.MoveNext();
            Assert.AreEqual(resEnumLazy.Entry, resEnumCached.Entry);

            // getting the metadata returns a cached enumerator now
            var metaEnumCached = reader.GetMetadataEnumerator();
            Assert.AreEqual(resEnumCached.GetType(), metaEnumCached.GetType());

            // as well as alias
            var aliasEnumCached = reader.GetAliasEnumerator();
            Assert.AreEqual(resEnumCached.GetType(), aliasEnumCached.GetType());

            // alias enumerators are handled in a special way so they are tested separately
            // reader is recreated to get a lazy enumerator again
            reader = new ResXResourceReader(path);
            var aliasEnumLazy = reader.GetAliasEnumerator();
            aliasEnumLazy.MoveNext();
            var firstAlias = aliasEnumLazy.Entry;
            aliasEnumLazy.Reset();
            aliasEnumLazy.MoveNext();
            Assert.AreEqual(firstAlias, aliasEnumLazy.Entry);

            // getting enumerator again: cached
            aliasEnumCached = reader.GetAliasEnumerator();
            Assert.AreNotEqual(aliasEnumLazy.GetType(), aliasEnumCached.GetType());
            aliasEnumCached.MoveNext();
            Assert.AreEqual(firstAlias, aliasEnumCached.Entry);

            // the lazy cached the rest of the elements into a buffer so they both see the second element now
            aliasEnumLazy.MoveNext();
            aliasEnumCached.MoveNext();
            Assert.AreEqual(aliasEnumLazy.Entry, aliasEnumCached.Entry);

            // normal vs safe mode
            resEnumCached = reader.GetEnumerator();
            resEnumCached.MoveNext();
            Assert.IsNotInstanceOf<ResXDataNode>(resEnumCached.Value);
            reader.SafeMode = true;
            Assert.IsInstanceOf<ResXDataNode>(resEnumCached.Value);

            // however, aliases are always strings
            Assert.IsInstanceOf<string>(aliasEnumCached.Value);
            Assert.IsInstanceOf<string>(aliasEnumLazy.Value);
            reader.SafeMode = false;
            Assert.IsInstanceOf<string>(aliasEnumCached.Value);
            Assert.IsInstanceOf<string>(aliasEnumLazy.Value);
        }

        [Test]
        public void TestDataTypes()
        {
            // various types of embedded and referenced data
            string path = Path.Combine(Files.GetExecutingPath(), "Resources\\TestResourceResX.resx");
            //var refReader = new System.Resources.ResXResourceReader(path, new TypeResolver())
            //    {
            //        BasePath = Path.GetDirectoryName(path)
            //    };
            ResXResourceReader reader = new ResXResourceReader(path, new TestTypeResolver())
            {
                AllowDuplicatedKeys = false,
                BasePath = Path.GetDirectoryName(path)
            };
            //var refEnumerator = refReader.GetEnumerator(); // this reads now the whole xml BUG: System resx reader throws exception even with type resolver because the resolver is not used for file refs.
            var enumerator = reader.GetEnumerator(); // this reads now the whole xml
            while (/*refEnumerator.MoveNext() &&*/ enumerator.MoveNext())
            {
                //Console.Write("RefKey: {0}; RefValue: {1}; ", refEnumerator.Key, refEnumerator.Value);
                Console.WriteLine("Key: {0}; Value: {1}", enumerator.Key, enumerator.Value);
            }
        }

        [Test]
        public void TestException()
        {
            string resx = @"<?xml version='1.0' encoding='utf-8'?>
<root>
  <data>
    <value>Missing name</value>
  </data>
</root>";
            var reader = new ResXResourceReader(new StringReader(resx));

            Throws<XmlException>(() => reader.GetEnumerator().ToEnumerable().ToArray());
        }

        #endregion
    }
}