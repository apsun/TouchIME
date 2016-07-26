using System;
using System.Linq;
using Microsoft.Ink;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Helper class for obtaining <see cref="Recognizer"/> objects.
    /// </summary>
    public static class RecognizerHelper
    {
        private static readonly Recognizers Proxy = new Recognizers();

        /// <summary>
        /// Gets all available recognizers on the computer.
        /// </summary>
        public static Recognizer[] GetAll()
        {
            return Proxy.Cast<Recognizer>().Where(x => x.Languages.Length > 0).ToArray();
        }

        /// <summary>
        /// Gets the default recognizer for the current language,
        /// or null if none is available.
        /// </summary>
        public static Recognizer GetDefault()
        {
            return Proxy.GetDefaultRecognizer();
        }

        /// <summary>
        /// Gets the recognizer with the specified GUID, or null
        /// if it does not exist.
        /// </summary>
        /// <param name="guid">The GUID that identifies the recognizer.</param>
        public static Recognizer GetByGuid(Guid guid)
        {
            return Proxy.Cast<Recognizer>().FirstOrDefault(x => x.Id == guid);
        }
    }
}
