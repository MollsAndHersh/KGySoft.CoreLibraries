﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IExpandoResourceSet.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2017 - All Rights Reserved
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
using System.Collections;
using System.IO;
using System.Resources;

#endregion

namespace KGySoft.Libraries.Resources
{
    /// <summary>
    /// Represents a <see cref="ResourceSet"/> class that can hold replaceable resources.
    /// </summary>
    public interface IExpandoResourceSet : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether the <see cref="IExpandoResourceSet"/> works in safe mode. In safe mode the retrieved
        /// objects returned from .resx sources are not deserialized automatically. See Remarks section for details.
        /// <br/>Default value: <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>When <c>SafeMode</c> is <c>true</c>, the <see cref="GetObject"/> and <see cref="GetMetaObject"/> methods
        /// return <see cref="ResXDataNode"/> instances instead of deserialized objects, if they are returned from .resx resource. You can retrieve the deserialized
        /// objects on demand by calling the <see cref="ResXDataNode.GetValue"/> method on the <see cref="ResXDataNode"/> instance.</para>
        /// <para>When <c>SafeMode</c> is <c>true</c>, the <see cref="GetString"/> and <see cref="GetMetaString"/> methods
        /// will return a <see cref="string"/> for non-string objects, too, if they are from a .resx resource.
        /// For non-string elements the raw XML string value will be returned.</para>
        /// </remarks>
        bool SafeMode { get; set; }

        /// <summary>
        /// Gets whether this <see cref="IExpandoResourceSet"/> instance is modified.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        bool IsModified { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that can iterate through the resources of the <see cref="IExpandoResourceSet" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for this <see cref="IExpandoResourceSet" />.
        /// </returns>
        IDictionaryEnumerator GetEnumerator();

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that can iterate through the metadata of the <see cref="IExpandoResourceSet" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for this <see cref="IExpandoResourceSet" />.
        /// </returns>
        IDictionaryEnumerator GetMetadataEnumerator();

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that can iterate through the aliases of the <see cref="IExpandoResourceSet" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for this <see cref="IExpandoResourceSet" />.
        /// </returns>
        IDictionaryEnumerator GetAliasEnumerator();

        /// <summary>
        /// Searches for a resource object with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the resource to search for.</param>
        /// <param name="ignoreCase">Indicates whether the case of the specified <paramref name="name"/> should be ignored.</param>
        /// <returns>
        /// The requested resource, or when <see cref="SafeMode"/> is <c>true</c> and the resource is found in a .resx source, a <see cref="ResXDataNode"/> instance
        /// from which the resource can be obtained. If the requested <paramref name="name"/> cannot be found, <see langword="null"/> is returned.
        /// </returns>
        /// <remarks>
        /// When <see cref="SafeMode"/> is <c>true</c> and the resource is found in a .resx source, the returned object is a <see cref="ResXDataNode"/> instance
        /// from which the resource can be obtained.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        object GetObject(string name, bool ignoreCase = false);

        /// <summary>
        /// Searches for a <see cref="string" /> resource with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the resource to search for.</param>
        /// <param name="ignoreCase">Indicates whether the case of the specified <paramref name="name"/> should be ignored.</param>
        /// <returns>
        /// The <see cref="string"/> value of a resource.
        /// If <see cref="SafeMode"/> is <c>false</c>, or the result is not from a .resx resource, an <see cref="InvalidOperationException"/> will be thrown for
        /// non-string resources. If <see cref="SafeMode"/> is <c>true</c> and the result is found in a .resx resource, the raw XML value will be returned for non-string resources.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="InvalidOperationException">The type of the resource is not <see cref="string"/> (when <see cref="SafeMode"/> is <c>false</c> or the resource is not from a .resx resource).</exception>
        string GetString(string name, bool ignoreCase = false);

        /// <summary>
        /// Searches for a metadata object with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the metadata to search for.</param>
        /// <param name="ignoreCase">Indicates whether the case of the specified <paramref name="name"/> should be ignored.</param>
        /// <returns>
        /// The requested metadata, or when <see cref="SafeMode"/> is <c>true</c>, a <see cref="ResXDataNode"/> instance
        /// from which the metadata can be obtained. If the requested <paramref name="name"/> cannot be found, <see langword="null"/> is returned.
        /// </returns>
        /// <remarks>
        /// When <see cref="SafeMode"/> is <c>true</c>, the returned object is a <see cref="ResXDataNode"/> instance
        /// from which the metadata can be obtained.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        object GetMetaObject(string name, bool ignoreCase = false);

        /// <summary>
        /// Searches for a <see cref="string" /> metadata with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the metadata to search for.</param>
        /// <param name="ignoreCase">Indicates whether the case of the specified <paramref name="name"/> should be ignored.</param>
        /// <returns>
        /// The <see cref="string"/> value of a metadata.
        /// If <see cref="SafeMode"/> is <c>false</c>, an <see cref="InvalidOperationException"/> will be thrown for
        /// non-string metadata. If <see cref="SafeMode"/> is <c>true</c>, the raw XML value will be returned for non-string metadata.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="InvalidOperationException"><see cref="SafeMode"/> is <c>false</c> and the type of the metadata is not <see cref="string"/>.</exception>
        string GetMetaString(string name, bool ignoreCase = false);

        /// <summary>
        /// Gets the assembly name for the specified <paramref name="alias"/>.
        /// </summary>
        /// <param name="alias">The alias of the assembly name, which should be retrieved.</param>
        /// <returns>The assembly name of the <paramref name="alias"/>, or <see langword="null"/> if there is no such alias defined.</returns>
        /// <remarks>If an alias is redefined in the .resx file, then this method returns the last occurrence of the alias value.</remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="alias"/> is <see langword="null"/>.</exception>
        string GetAliasValue(string alias);

        /// <summary>
        /// Gets whether the current <see cref="IExpandoResourceSet"/> contains a resource with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the resource to check.</param>
        /// <param name="ignoreCase">Indicates whether the case of the specified <paramref name="name"/> should be ignored.</param>
        /// <returns><c>true</c>, if the current <see cref="IExpandoResourceSet"/> contains a resource with name <paramref name="name"/>; otherwise, <c>false</c>.</returns>
        bool ContainsResource(string name, bool ignoreCase = false);

        /// <summary>
        /// Gets whether the current <see cref="IExpandoResourceSet"/> contains a metadata with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the metadata to check.</param>
        /// <param name="ignoreCase">Indicates whether the case of the specified <paramref name="name"/> should be ignored.</param>
        /// <returns><c>true</c>, if the current <see cref="IExpandoResourceSet"/> contains a metadata with name <paramref name="name"/>; otherwise, <c>false</c>.</returns>
        bool ContainsMeta(string name, bool ignoreCase = false);

        /// <summary>
        /// Adds or replaces a resource object in the current <see cref="IExpandoResourceSet"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the resource value to set.</param>
        /// <param name="value">The resource value to set. If <see langword="null"/>, a null reference will be explicitly stored.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// <para>If <paramref name="value"/> is <see langword="null"/>, and this <see cref="IExpandoResourceSet"/> instance
        /// is a hybrid resource set, <see cref="GetObject"/> will always return <see langword="null"/>, even if <paramref name="name"/> is
        /// defined in the original binary resource set. Thus you can force to take the parent resource set for example in case of a <see cref="HybridResourceManager"/>.</para>
        /// <para>To remove the user-defined content and reset the original resource defined in the binary resource set (if any), use
        /// the <see cref="RemoveObject"/> method.</para>
        /// </remarks>
        void SetObject(string name, object value);

        /// <summary>
        /// Adds or replaces a metadata object in the current <see cref="IExpandoResourceSet"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the metadata value to set.</param>
        /// <param name="value">The metadata value to set. If <see langword="null"/>, a null reference will be explicitly stored.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// <para>To remove the user-defined content use the <see cref="RemoveMetaObject"/> method.</para>
        /// </remarks>
        void SetMetaObject(string name, object value);

        /// <summary>
        /// Adds or replaces an assembly alias value in the current <see cref="IExpandoResourceSet"/>.
        /// </summary>
        /// <param name="alias">The alias name to use instead of <paramref name="assemblyName"/> in the saved .resx file.</param>
        /// <param name="assemblyName">The fully or partially qualified name of the assembly.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyName"/> or <paramref name="alias"/> is <see langword="null"/>.</exception>
        void SetAliasValue(string alias, string assemblyName);

        /// <summary>
        /// Removes a resource object from the current <see cref="IExpandoResourceSet"/> with the specified <paramref name="name"/>.
        /// If this <see cref="IExpandoResourceSet"/> represents a hybrid resource set, then the original value of <paramref name="name"/>
        /// will be restored (if existed).
        /// </summary>
        /// <param name="name">Name of the resource value to remove.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// <para>If this <see cref="IExpandoResourceSet"/> instance is a hybrid resource set, and there is a binary resource
        /// defined for <paramref name="name"/>, then after this call the originally defined value will be returned by <see cref="GetObject"/> method.
        /// If you want to force <see cref="GetObject"/> to return always <see langword="null"/> for this resource set, then use the <see cref="SetObject"/> method with a <see langword="null"/> value</para>
        /// </remarks>
        void RemoveObject(string name);

        /// <summary>
        /// Removes a metadata object in the current <see cref="IExpandoResourceSet"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the metadata value to remove.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        void RemoveMetaObject(string name);

        /// <summary>
        /// Removes an assembly alias value in the current <see cref="IExpandoResourceSet"/>.
        /// </summary>
        /// <param name="alias">The alias, which should be removed.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="IExpandoResourceSet"/> is already disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="alias"/> is <see langword="null"/>.</exception>
        void RemoveAliasValue(string alias);

        /// <summary>
        /// Saves the <see cref="IExpandoResourceSet"/> to the specified file. If the current <see cref="IExpandoResourceSet"/> instance
        /// represents a hybrid resource set, saves the expando-part (.resx content) only.
        /// </summary>
        /// <param name="fileName">The location of the file where you want to save the resources.</param>
        /// <param name="compatibleFormat">If set to <c>true</c>, the result .resx file can be read by the system <a href="https://msdn.microsoft.com/en-us/library/system.resources.resxresourcereader.aspx" target="_blank">ResXResourceReader</a> class
        /// and the Visual Studio Resource Editor. If set to <c>false</c>, the result .resx is often shorter, and the values can be deserialized with better accuracy (see the remarks at <see cref="ResXResourceWriter"/>), but the result can be read only by <see cref="ResXResourceReader"/>
        /// <br/>Default value: <c>false</c>.</param>
        /// <param name="forceEmbeddedResources">If set to <c>true</c> the resources using a file reference (<see cref="ResXFileRef"/>) will be replaced into embedded resources.
        /// <br/>Default value: <c>false</c></param>
        /// <param name="basePath">A new base path for the file paths specified in the <see cref="ResXFileRef"/> objects. If <see langword="null"/>,
        /// the original base path will be used. The file paths in the saved .resx file will be relative to <paramref name="basePath"/>.
        /// Applicable if <paramref name="forceEmbeddedResources"/> is <c>false</c>.
        /// <br/>Default value: <c>false</c></param>
        /// <seealso cref="ResXResourceWriter"/>
        /// <seealso cref="ResXResourceWriter.CompatibleFormat"/>
        void Save(string fileName, bool compatibleFormat = false, bool forceEmbeddedResources = false, string basePath = null);

        /// <summary>
        /// Saves the <see cref="IExpandoResourceSet"/> to the specified <paramref name="stream"/>. If the current <see cref="IExpandoResourceSet"/> instance
        /// represents a hybrid resource set, saves the expando-part (.resx content) only.
        /// </summary>
        /// <param name="stream">The stream to which you want to save.</param>
        /// <param name="compatibleFormat">If set to <c>true</c>, the result .resx file can be read by the system <a href="https://msdn.microsoft.com/en-us/library/system.resources.resxresourcereader.aspx" target="_blank">ResXResourceReader</a> class
        /// and the Visual Studio Resource Editor. If set to <c>false</c>, the result .resx is often shorter, and the values can be deserialized with better accuracy (see the remarks at <see cref="ResXResourceWriter"/>), but the result can be read only by <see cref="ResXResourceReader"/>.
        /// <br/>Default value: <c>false</c></param>
        /// <param name="forceEmbeddedResources">If set to <c>true</c> the resources using a file reference (<see cref="ResXFileRef"/>) will be replaced into embedded resources.
        /// <br/>Default value: <c>false</c></param>
        /// <param name="basePath">A new base path for the file paths specified in the <see cref="ResXFileRef"/> objects. If <see langword="null"/>,
        /// the original base path will be used. The file paths in the saved .resx file will be relative to <paramref name="basePath"/>.
        /// Applicable if <paramref name="forceEmbeddedResources"/> is <c>false</c>.
        /// <br/>Default value: <c>false</c></param>
        /// <seealso cref="ResXResourceWriter"/>
        /// <seealso cref="ResXResourceWriter.CompatibleFormat"/>
        void Save(Stream stream, bool compatibleFormat = false, bool forceEmbeddedResources = false, string basePath = null);

        /// <summary>
        /// Saves the <see cref="IExpandoResourceSet"/> to the specified <paramref name="textWriter"/>. If the current <see cref="IExpandoResourceSet"/> instance
        /// represents a hybrid resource set, saves the expando-part (.resx content) only.
        /// </summary>
        /// <param name="textWriter">The text writer to which you want to save.</param>
        /// <param name="compatibleFormat">If set to <c>true</c>, the result .resx file can be read by the system <a href="https://msdn.microsoft.com/en-us/library/system.resources.resxresourcereader.aspx" target="_blank">ResXResourceReader</a> class
        /// and the Visual Studio Resource Editor. If set to <c>false</c>, the result .resx is often shorter, and the values can be deserialized with better accuracy (see the remarks at <see cref="ResXResourceWriter"/>), but the result can be read only by <see cref="ResXResourceReader"/>.
        /// <br/>Default value: <c>false</c></param>
        /// <param name="forceEmbeddedResources">If set to <c>true</c> the resources using a file reference (<see cref="ResXFileRef"/>) will be replaced into embedded resources.
        /// <br/>Default value: <c>false</c></param>
        /// <param name="basePath">A new base path for the file paths specified in the <see cref="ResXFileRef"/> objects. If <see langword="null"/>,
        /// the original base path will be used. The file paths in the saved .resx file will be relative to <paramref name="basePath"/>.
        /// Applicable if <paramref name="forceEmbeddedResources"/> is <c>false</c>.
        /// <br/>Default value: <c>false</c></param>
        /// <seealso cref="ResXResourceWriter"/>
        /// <seealso cref="ResXResourceWriter.CompatibleFormat"/>
        void Save(TextWriter textWriter, bool compatibleFormat = false, bool forceEmbeddedResources = false, string basePath = null);

        #endregion
    }
}