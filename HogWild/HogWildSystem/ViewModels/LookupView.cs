// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : James Thompson
// Created          : 03-11-2025
//
// Last Modified By : James Thompson
// Last Modified On : 11-13-2024
// ***********************************************************************
// <copyright file="LookupView.cs" company="HogWildSystem">
//     Copyright (c) NAIT. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.ViewModels
{
    /// <summary>
    /// Class LookupView.
    /// </summary>
    public class LookupView
    {
        /// <summary>
        /// Gets or sets the lookup identifier.
        /// </summary>
        /// <value>The lookup identifier.</value>
        public int LookupID { get; set; }
        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        /// <value>The category identifier.</value>
        public int CategoryID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [remove from view flag].
        /// </summary>
        /// <value><c>true</c> if [remove from view flag]; otherwise, <c>false</c>.</value>
        public bool RemoveFromViewFlag { get; set; }
    }
}
