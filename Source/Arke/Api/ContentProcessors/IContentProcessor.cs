using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api.ContentProcessors
{
    /// <summary>
    /// Represents a content processor that can be used to process HttpContent objects from web requests and responses.
    /// </summary>
    /// <remarks>The Arke framework expects a content processor to be thread safe.</remarks>
    public interface IContentProcessor
    {
        /// <summary>
        /// Converts an object into an <see cref="HttpContent"/>
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The <see cref="HttpContent"/> that represents the object.</returns>
        Task<HttpContent> ProcessObject(Object obj);

        /// <summary>
        /// Converts an <see cref="HttpContent"/> into the an object of the given type.
        /// </summary>
        /// <typeparam name="T">The type of object to convert the <see cref="HttpContent"/> into.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> to convert.</param>
        /// <returns>An instance of the given type converted from the given <see cref="HttpContent"/></returns>
        Task<T> ProcessContent<T>(HttpContent content);
    }
}
