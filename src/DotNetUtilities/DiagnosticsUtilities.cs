using System;
using System.Xml.Linq;


namespace DotNetUtilities
{
    /// <summary>
    /// Set of utilities related to diagnostics and error reporting.
    /// </summary>
    public static class DiagnosticsUtilities
    {
        #region Utilities
        /// <summary>
        /// Generates XML-based representation of provided exception.
        /// </summary>
        /// <param name="exception">
        /// Exception, which representation shall be generated.
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
        /// <example>
        /// Generated XML element matches the 'Exception' complex type
        /// defined in belows XML schema.
        /// <code>
        /// <xs:complexType name="ExceptionSource">
        ///     <xs:attribute name = "Application" type="xs:string" use="required"/>
        ///     <xs:attribute name = "Method" type="xs:string" use="required"/>
        ///     <xs:attribute name = "StackTrace" type="xs:string" use="required"/>
        /// </xs:complexType>
        /// <xs:complexType name="Exception">
        ///     <xs:sequence>
        ///         <xs:element name = "Message" type="Message"/>
        ///         <xs:element name = "Source" type="ExceptionSource"/>
        ///         <xs:element name = "InnerException" type="InnerException"/>
        ///     </xs:sequence>
        ///     <xs:attribute name = "Type" type="xs:string" use="required"/>
        /// </xs:complexType>
        /// <xs:complexType name="InnerException">
        ///     <xs:sequence>
        ///         <xs:element name = "Exception" type="Exception" minOccurs="0"/>
        ///     </xs:sequence>
        /// </xs:complexType>
        /// </code>
        /// </example>
        public static XElement AsXmlElement(Exception exception)
        {
            #region Arguments validation
            if (exception is null)
            {
                string argumentName = nameof(exception);
                const string ErrorMessage = "Provided exception is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }
            #endregion

            string exceptionType = exception.GetType().FullName;

            var exceptionElement = new XElement("Exception",
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
                innerExceptionElement = new XElement("InnerException",
                    AsXmlElement(exception.InnerException));
            }

            exceptionElement.Add(innerExceptionElement);

            return exceptionElement;
        }
        #endregion
    }
}
