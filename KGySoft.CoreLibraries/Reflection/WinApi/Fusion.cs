﻿#if NETFRAMEWORK
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Fusion.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace KGySoft.Reflection.WinApi
{
    /// <summary>
    /// The fusion API enables a runtime host to access the properties of an application's resources in order to locate
    /// the correct versions of those resources for the application.
    /// </summary>
    [SecurityCritical]
    internal static class Fusion
    {
        #region Nested classes

        private static class NativeMethods
        {
            #region Methods

            /// <summary>
            /// Gets a pointer to a new <see cref="IAssemblyCache"/> instance that represents the global assembly cache.
            /// </summary>
            /// <param name="ppAsmCache">The returned <see cref="IAssemblyCache"/> pointer.</param>
            /// <param name="dwReserved">Reserved for future extensibility. dwReserved must be 0 (zero).</param>
            /// <returns>HRESULT</returns>
            [DllImport("fusion.dll")]
            [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "False alarm, SecurityCriticalAttribute is applied to container class")]
            internal static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int dwReserved);

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the path for an assembly if it is in the GAC. Returns the path of the newest available version.
        /// </summary>
        internal static string GetGacPath(string name)
        {
            const int bufSize = 1024;

            if (NativeMethods.CreateAssemblyCache(out IAssemblyCache assemblyCache, 0) >= 0)
            {
                var aInfo = new ASSEMBLY_INFO
                {
                    cchBuf = bufSize,
                    currentAssemblyPath = new String('\0', bufSize)
                };

                int hresult = assemblyCache.QueryAssemblyInfo(0, name, ref aInfo);
                if (hresult >= 0)
                    return aInfo.currentAssemblyPath;
            }

            return null;
        }

        #endregion
    }
}
#endif