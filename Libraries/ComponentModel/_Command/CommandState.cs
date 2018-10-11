﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if !NET35
using System.Dynamic;
#endif
using System.Linq;

namespace KGySoft.ComponentModel
{
    /// <summary>
    /// Represents the states of a command for a specific command binding. When a state property is set it is tried to be applied for all of the command sources.
    /// By default, they are tried to be set as a property on the sources but this behavior can be overridden if an <see cref="ICommandStateUpdater"/> is added
    /// for the binding by the <see cref="ICommandBinding.AddStateUpdater">ICommandBinding.AddStateUpdater</see> method.
    /// <br/>See the <strong>Remarks</strong> section of <see cref="ICommand"/> for examples.
    /// </summary>
    /// <seealso cref="DynamicObject" />
    /// <seealso cref="KGySoft.ComponentModel.ICommandState" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public sealed class CommandState :
#if !NET35
        DynamicObject, 
#endif
        ICommandState, INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> stateProperties = new Dictionary<string, object> { [nameof(Enabled)] = true };

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandState"/> class from an initial configuration if provided.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration to use for initializing this <see cref="CommandState"/> instance.</param>
        /// <exception cref="ArgumentException"><paramref name="initialConfiguration"/> contains a non-<see cref="bool"/> <c>Enabled</c> entry.</exception>
        public CommandState(IDictionary<string, object> initialConfiguration = null)
        {
            if (initialConfiguration == null)
                return;

            foreach (var state in initialConfiguration)
            {
                if (state.Key == nameof(Enabled) && !(state.Value is bool))
                    throw new ArgumentException($"'{Enabled}' state must have a bool value", nameof(initialConfiguration));
                stateProperties[state.Key] = state.Value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            lock (stateProperties)
                return new Dictionary<string, object>(stateProperties).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            // not calling PropertyChanged because a removed property has no effect on update
            lock (stateProperties)
            {
                stateProperties.Clear();
                Enabled = true; // this may call PropertyChanged though
            }
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            lock (stateProperties)
                return ((ICollection<KeyValuePair<string, object>>)stateProperties).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            lock (stateProperties)
                ((ICollection<KeyValuePair<string, object>>)stateProperties).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            lock (stateProperties)
                return ((ICollection<KeyValuePair<string, object>>)stateProperties).Remove(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="CommandState" />.
        /// </summary>
        public int Count
        {
            get
            {
                lock (stateProperties)
                    return stateProperties.Count;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

        /// <summary>
        /// Determines whether the <see cref="CommandState" /> contains an element with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="CommandState" />.</param>
        /// <returns><see langword="true" /> if the <see cref="CommandState" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
        public bool ContainsKey(string key)
        {
            lock (stateProperties)
                return stateProperties.ContainsKey(key);
        }

        /// <summary>
        /// Adds a state element with the provided key and value to the <see cref="CommandState" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentException">An item with the same key has already been added</exception>
        public void Add(string key, object value)
        {
            lock (stateProperties)
            {
                if (stateProperties.ContainsKey(key))
                    throw new ArgumentException("An item with the same key has already been added.", nameof(key));
                //throw new ArgumentException(Res.Get(Res.DuplicateKey), nameof(key));

                stateProperties.Add(key, value);
                OnPropertyChanged(key);
            }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            // not calling PropertyChanged because a removed property has no effect on update
            lock (stateProperties)
                return stateProperties.Remove(key);
        }

        /// <summary>
        /// Gets the state element associated with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified <paramref name="key"/>, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the <see cref="CommandState"/> contains an element with the specified <paramref name="key"/>; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetValue(string key, out object value)
        {
            lock (stateProperties)
                return stateProperties.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets or sets the state value with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the state value to get or set.</param>
        public object this[string key]
        {
            get
            {
                lock (stateProperties)
                    return stateProperties[key];
            }
            set
            {
                if (key == nameof(Enabled) && !(value is bool))
                    throw new ArgumentException($"'{Enabled}' state must have a bool value");

                lock (stateProperties)
                {
                    bool differs = !stateProperties.TryGetValue(key, out object oldValue) || !Equals(oldValue, value);
                    stateProperties[key] = value;
                    if (differs)
                        OnPropertyChanged(key);
                }
            }
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                lock (stateProperties)
                    return stateProperties.Keys.ToArray();
            }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                lock (stateProperties)
                    return stateProperties.Values.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets whether the command is enabled in the current binding.
        /// <br/>Default value: <see langword="true"/>.
        /// </summary>
        /// <value><see langword="true" /> if the command enabled and can be executed; otherwise, <see langword="false" />.</value>
        public bool Enabled
        {
            get => (bool)this[nameof(Enabled)];
            set => this[nameof(Enabled)] = value;
        }

#if !NET35
        /// <summary>
        /// Gets the state as a dynamic object so the states can be set by simple property setting syntax.
        /// </summary>
        public dynamic AsDynamic => this;
#endif

        /// <summary>
        /// Occurs when a state entry value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

#if !NET35
        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;
        }
#endif
    }
}