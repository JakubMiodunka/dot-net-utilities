using System;
using System.Xml.Linq;


namespace DotNetUtilities
{
    /// <summary>
    /// Set of utilities related to diagnostics and error reporting.
    /// </summary>
    public static class DiagnosticsUtilities
    {
        /// <summary>
        /// Generates XML-based representation of provided exception.
        /// </summary>
        /// <param name="exception">
        /// Exception, which representation shall be generated.
        /// </param>
        /// <param name="elementName">
        /// Desired name of generated XML element.
        /// </param>
        /// <returns>
        /// XML-based representation of provided exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown, when at least one argument will be considered as invalid.
        /// </exception>
        public static XElement AsXmlElement(Exception exception, string elementName = "Exception")
        {
            #region Arguments validation
            if (exception is null)
            {
                string argumentName = nameof(exception);
                const string ErrorMessage = "Provided exception is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            if (string.IsNullOrWhiteSpace(elementName))
            {
                string argumentName = nameof(exception);
                string errorMessage = $"Provided element name is invalid: {elementName}";
                throw new ArgumentException(errorMessage, argumentName);
            }
            #endregion

            string exceptionType = exception.GetType().FullName;

            var exceptionElement = new XElement(elementName,
                new XAttribute("Type", exceptionType));

            var messageElement = new XElement("Message",
                new XAttribute("Content", exception.Message));

            exceptionElement.Add(messageElement);

            string application = exception.Source ?? "Unknown";
            string methodName = (exception.TargetSite is null) ? "Unknown" : exception.TargetSite.ToString();
            string stackTrace = exception.StackTrace ?? string.Empty;

            var sourceElement = new XElement("Source",
                new XAttribute("Application", application),
                new XAttribute("Method", methodName),
                new XAttribute("StackTrace", stackTrace));

            exceptionElement.Add(sourceElement);

            XElement innerExceptionElement;

            if (exception.InnerException is null)
            {
                innerExceptionElement = new XElement("InnerException");
            }
            else
            {
                innerExceptionElement = AsXmlElement(exception.InnerException, "InnerException");
            }

            exceptionElement.Add(innerExceptionElement);

            return exceptionElement;
        }
    }
}
