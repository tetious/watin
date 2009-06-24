#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;

namespace WatiN.Core
{
    /// <summary>
    /// Associates a description with a component.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// public class HomePage : Page
    /// {
    ///     [FindBy(Name = "signIn")]
    ///     [Description("Sign in button.")]
    ///     public Button SignInButton;
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DescriptionAttribute : ComponentDecoratorAttribute
    {
        private readonly string description;

        /// <summary>
        /// Associates a description with a component.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> is null</exception>
        public DescriptionAttribute(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            this.description = description;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return description; }
        }
        
        /// <inheritdoc />
        public override void Decorate(Component component)
        {
            component.Description = description;
        }
    }
}
