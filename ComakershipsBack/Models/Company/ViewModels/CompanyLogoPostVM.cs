using Dahomey.Json.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// Viewmodel used for POSTing a logo.
    /// </summary>
    public class CompanyLogoPostVM
    {
        /// <summary>
        /// Contains the image of the company as base64
        /// </summary>
        public string LogoAsBase64 { get; set; }
    }
}
