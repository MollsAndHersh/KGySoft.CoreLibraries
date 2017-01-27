﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace KGySoft.Libraries.Resources
{
    /// <summary>
    /// Represents a resource set of hybrid sources (both resx and compiled source).
    /// </summary>
    [Serializable]
    internal sealed class HybridResourceSet : ResourceSet, IExpandoResourceSet, IExpandoResourceSetInternal, IEnumerable
    {
        /// <summary>
        /// An enumerator for a HybridResourceSet. If both resx and compiled resources contain the same key, returns only the value from the resx.
        /// Must be implemented because yield return does not work for IDictionaryEnumerator.
        /// Cannot be serializable because the compiled enumerator is not serializable (supports reset, though).
        /// </summary>
        private class Enumerator: IDictionaryEnumerator
        {
            enum State
            {
                NotStarted = -1,
                EnumeratingResX,
                EnumeratingCompiled,
                Finished = -2
            }

            private readonly int version;
            private ResXResourceEnumerator resxEnumerator;
            private IDictionaryEnumerator compiledEnumerator;
            private State state;
            private HybridResourceSet owner;
            private HashSet<string> resxKeys;
            private HashSet<string> compiledKeys;

            internal Enumerator(HybridResourceSet owner, ResXResourceEnumerator resx, IDictionaryEnumerator compiled, int version)
            {
                this.owner = owner;
                state = State.NotStarted;
                this.version = version;
                resxEnumerator = resx;
                compiledEnumerator = compiled;
            }

            public DictionaryEntry Entry
            {
                get
                {
                    switch (state)
                    {
                        case State.EnumeratingResX:
                            return resxEnumerator.Entry;
                        case State.EnumeratingCompiled:
                            return compiledEnumerator.Entry;
                        default:
                            throw new InvalidOperationException(Res.Get(Res.EnumerationNotStartedOrFinished));
                    }
                }
            }

            public object Key
            {
                get
                {
                    switch (state)
                    {
                        case State.EnumeratingResX:
                            return resxEnumerator.Key;
                        case State.EnumeratingCompiled:
                            return compiledEnumerator.Key;
                        default:
                            throw new InvalidOperationException(Res.Get(Res.EnumerationNotStartedOrFinished));
                    }
                }
            }

            public object Value
            {
                get
                {
                    switch (state)
                    {
                        case State.EnumeratingResX:
                            return resxEnumerator.Value;
                        case State.EnumeratingCompiled:
                            return compiledEnumerator.Value;
                        default:
                            throw new InvalidOperationException(Res.Get(Res.EnumerationNotStartedOrFinished));
                    }
                }
            }

            public object Current
            {
                get { return Entry; }
            }

            public bool MoveNext()
            {
                switch (state)
                {
                    case State.NotStarted:
                        state = State.EnumeratingResX;
                        resxKeys = new HashSet<string>();
                        goto case State.EnumeratingResX;

                    case State.EnumeratingResX:
                        // version is checked internally here
                        if (resxEnumerator.MoveNext())
                        {
                            resxKeys.Add(resxEnumerator.Key.ToString());
                            return true;
                        }

                        state = State.EnumeratingCompiled;
                        compiledKeys = new HashSet<string>();
                        goto case State.EnumeratingCompiled;

                    case State.EnumeratingCompiled:
                        if (version != resxEnumerator.OwnerVersion)
                            throw new InvalidOperationException(Res.Get(Res.EnumerationCollectionModified));

                        while (compiledEnumerator.MoveNext())
                        {
                            compiledKeys.Add(compiledEnumerator.Key.ToString());
                            if (resxKeys.Contains(compiledEnumerator.Key.ToString()))
                                continue;

                            return true;
                        }

                        resxKeys = null;
                        state = State.Finished;
                        if (owner.compiledKeys == null)
                            owner.compiledKeys = compiledKeys;
                        return false;

                    case State.Finished:
                        return false;

                    default:
                        // internal error, no res is needed
                        throw new InvalidOperationException("Invalid state");
                }
            }

            public void Reset()
            {
                resxEnumerator.Reset();
                compiledEnumerator.Reset();
                resxKeys = null;
                state = State.NotStarted;
            }
        }

        private ResXResourceSet resxResourceSet;
        private ResourceSet compiledResourceSet;
        [NonSerialized] private HashSet<string> compiledKeys;
        [NonSerialized] private HashSet<string> compiledKeysCaseInsensitive;

        internal HybridResourceSet(ResXResourceSet resx, ResourceSet compiled)
        {
            if (resx == null)
                throw new ArgumentNullException("resx", Res.Get(Res.ArgumentNull));
            if (compiled == null)
                throw new ArgumentNullException("compiled", Res.Get(Res.ArgumentNull));
            resxResourceSet = resx;
            compiledResourceSet = compiled;

            // base ctor allocates a Hashtable and the dummy base ctor(bool), which avoids that, is not available from here
            Table = null;
        }

        protected override void Dispose(bool disposing)
        {
            ResourceSet resx = resxResourceSet;
            ResourceSet compiled = compiledResourceSet;
            if (resx == null || compiled == null)
                return;

            // not disposing the wrapped resource sets just nullifying them because their life cycle can be longer
            // as the hybrid one (eg. changing source from mixed to single one).
            resxResourceSet = null;
            compiledResourceSet = null;
            compiledKeys = null;
            compiledKeysCaseInsensitive = null;
            base.Dispose(disposing);
        }

        public override Type GetDefaultReader()
        {
            // actually there is no HybridResourceReader so returning the more dynamic XML version here
            return typeof(ResXResourceReader);
        }

        public override Type GetDefaultWriter()
        {
            // actually there is no HybridResourceWriter so returning the more dynamic XML version here
            return typeof(ResXResourceWriter);
        }

        public override IDictionaryEnumerator GetEnumerator()
        {
            ResXResourceSet resx = resxResourceSet;
            ResourceSet compiled = compiledResourceSet;
            if (resx == null || compiled == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            // changing is checked in resx resource set
            return new Enumerator(this, (ResXResourceEnumerator)resx.GetEnumerator(), compiled.GetEnumerator(), ((IResXResourceContainer)resx).Version);
        }

        public override object GetObject(string name)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return GetResource(name, false, false, resx.SafeMode);
        }

        public override object GetObject(string name, bool ignoreCase)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return GetResource(name, ignoreCase, false, resx.SafeMode);
        }

        public override string GetString(string name)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return (string)GetResource(name, false, true, resx.SafeMode);
        }

        public override string GetString(string name, bool ignoreCase)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return (string)GetResource(name, ignoreCase, true, resx.SafeMode);
        }

        public bool ContainsResource(string name, bool ignoreCase)
        {
            ResXResourceSet resx = resxResourceSet;
            ResourceSet compiled = compiledResourceSet;
            if (resx == null || compiled == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            if (resx.ContainsResource(name, ignoreCase))
                return true;

            HashSet<string> binKeys = compiledKeys;
            if (binKeys == null)
            {
                // no foreach because that would read the values, too
                binKeys = new HashSet<string>();
                IDictionaryEnumerator compiledEnumerator = compiled.GetEnumerator();
                while (compiledEnumerator.MoveNext())
                {
                    binKeys.Add(compiledEnumerator.Key.ToString());
                }

                compiledKeys = binKeys;
            }

            if (binKeys.Contains(name))
                return true;

            if (!ignoreCase)
                return false;

            HashSet<string> binKeysIgnoreCase = compiledKeysCaseInsensitive;
            if (binKeysIgnoreCase == null)
            {
                compiledKeysCaseInsensitive = binKeysIgnoreCase = new HashSet<string>(binKeys, StringComparer.OrdinalIgnoreCase);
            }

            return binKeysIgnoreCase.Contains(name);
        }

        public bool ContainsMeta(string name, bool ignoreCase)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return resx.ContainsMeta(name, ignoreCase);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IExpandoResourceSetInternal Members

        public object GetResource(string name, bool ignoreCase, bool isString, bool asSafe)
        {
            ResXResourceSet resx = resxResourceSet;
            ResourceSet compiled = compiledResourceSet;
            if (resx == null || compiled == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            object result = resx.GetResourceInternal(name, ignoreCase, isString, asSafe);

            if (result != null)
                return result;

            // if the null result is because it is explicitly stored, hiding the compiled value
            if (resx.ContainsResource(name, ignoreCase))
                return null;

            return isString ? compiled.GetString(name, ignoreCase) : compiled.GetObject(name, ignoreCase);
        }

        bool IExpandoResourceSetInternal.SafeMode
        {
            set { resxResourceSet.SafeMode = value; }
        }

        public object GetMeta(string name, bool ignoreCase, bool isString, bool asSafe)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return resx.GetMetaInternal(name, ignoreCase, isString, asSafe);
        }

        #endregion

        #region IExpandoResourceSet Members

        bool IExpandoResourceSet.SafeMode
        {
            get
            {
                ResXResourceSet resx = resxResourceSet;
                if (resx == null)
                    throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));
                
                return resx.SafeMode;
            }
            set
            {
                ResXResourceSet resx = resxResourceSet;
                if (resx == null)
                    throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));
                
                resx.SafeMode = value;
            }
        }

        public bool IsModified
        {
            get
            {
                ResXResourceSet resx = resxResourceSet;
                if (resx == null)
                    throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));
                return resx.IsModified;
            }
        }

        public IDictionaryEnumerator GetMetadataEnumerator()
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return resx.GetMetadataEnumerator();
        }

        public IDictionaryEnumerator GetAliasEnumerator()
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return resx.GetAliasEnumerator();
        }

        public object GetMetaObject(string name, bool ignoreCase = false)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return resx.GetMetaInternal(name, ignoreCase, false, resx.SafeMode);
        }

        public string GetMetaString(string name, bool ignoreCase = false)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return (string)resx.GetMetaInternal(name, ignoreCase, true, resx.SafeMode);
        }

        public string GetAliasValue(string alias)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            return resx.GetAliasValue(alias);
        }

        public void SetObject(string name, object value)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.SetObject(name, value);
        }

        public void SetMetaObject(string name, object value)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.SetMetaObject(name, value);
        }

        public void SetAliasValue(string alias, string assemblyName)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.SetAliasValue(alias, assemblyName);
        }

        public void RemoveObject(string name)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.RemoveObject(name);
        }

        public void RemoveMetaObject(string name)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.RemoveMetaObject(name);
        }

        public void RemoveAliasValue(string alias)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.RemoveAliasValue(alias);
        }

        public void Save(string fileName, bool compatibleFormat = false, bool forceEmbeddedResources = false, string basePath = null)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.Save(fileName, compatibleFormat, forceEmbeddedResources, basePath);
        }

        public void Save(Stream stream, bool compatibleFormat = false, bool forceEmbeddedResources = false, string basePath = null)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.Save(stream, compatibleFormat, forceEmbeddedResources, basePath);
        }

        public void Save(TextWriter textWriter, bool compatibleFormat = false, bool forceEmbeddedResources = false, string basePath = null)
        {
            ResXResourceSet resx = resxResourceSet;
            if (resx == null)
                throw new ObjectDisposedException(null, Res.Get(Res.ObjectDisposed));

            resx.Save(textWriter, compatibleFormat, forceEmbeddedResources, basePath);
        }

        #endregion
    }
}
