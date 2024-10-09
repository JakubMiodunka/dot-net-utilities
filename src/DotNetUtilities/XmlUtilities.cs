using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace DotNetUtilities
{
    /// <summary>
    /// Set of utilities related to XML format.
    /// </summary>
    public static class XmlUtilities
    {
        #region Auxiliary methods
        /// <summary>
        /// Validates given XML document against provided schema.
        /// </summary>
        /// <param name="xmlDocument">
        /// XML document, which shall be validated.
        /// </param>
        /// <param name="xmlSchema">
        /// XML schema, against which provided XML document shall be validated against.
        /// </param>
        /// <param name="eventHandler">
        /// Callback invoked, when validation error will be detected.
        /// If null will be provided, first detection of validation error would result in exception throw.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        private static void PerformXmlValidation(XDocument xmlDocument, XmlSchema xmlSchema, ValidationEventHandler eventHandler)
        {
            #region Arguments validation
            if (xmlDocument is null)
            {
                string argumentName = nameof(xmlDocument);
                const string ErrorMessage = "Provided XML document is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            if (xmlSchema is null)
            {
                string argumentName = nameof(xmlSchema);
                const string ErrorMessage = "Provided XML schema is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }
            #endregion

            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(xmlSchema);

            xmlDocument.Validate(schemaSet, eventHandler);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Loads XML schema stored under specified path into object instance.
        /// </summary>
        /// <param name="schemaPath">
        /// Path to *.xsd schema, which shall be loaded to memory as object instance.
        /// </param>
        /// <returns>
        /// Object corresponding to specified *.xsd file.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown, when at least one argument will be considered as invalid.
        /// </exception>
        public static XmlSchema LoadXmlSchema(string schemaPath)
        {
            #region Arguments validation
            FileSystemUtilities.ValidateFile(schemaPath, ".xsd");
            #endregion

            using (var schemaReader = XmlReader.Create(schemaPath))
            {
                try
                {
                    return XmlSchema.Read(schemaReader, null);
                }
                catch (XmlSchemaException exception)
                {
                    string argumentName = nameof(schemaPath);
                    string errorMessage = $"Provided XML schema is invalid: {schemaPath}";
                    throw new ArgumentException(errorMessage, argumentName, exception);
                }
            }
        }

        /// <summary>
        /// Checks if given XML document matches provided schema.
        /// </summary>
        /// <param name="xmlDocument">
        /// XML document, which shall be validated.
        /// </param>
        /// <param name="xmlSchema">
        /// XML schema, against which provided XML document shall be validated against.
        /// </param>
        /// <returns>
        /// True if provided XML document matches provided *.xsd schema, false otherwise.
        /// </returns>
        public static bool IsMatchingToSchema(XDocument xmlDocument, XmlSchema xmlSchema)
        {
            bool isDocumentMatching = true;

            ValidationEventHandler eventHandler = (sender, eventData) => isDocumentMatching = false;
            PerformXmlValidation(xmlDocument, xmlSchema, eventHandler);

            return isDocumentMatching;
        }

        /// <summary>
        /// Checks if given XML document matches provided schema.
        /// If validation will fail, according exception will be thrown.
        /// </summary>
        /// <param name="xmlDocument">
        /// XML document, which shall be validated.
        /// </param>
        /// <param name="xmlSchema">
        /// XML schema, against which provided XML document shall be validated against.
        /// </param>
        /// <exception cref="FormatException">
        /// Thrown, when given XML document does not match provided schema.
        /// </exception>
        public static void ValidateXmlDocument(XDocument xmlDocument, XmlSchema xmlSchema)
        {
            if (!IsMatchingToSchema(xmlDocument, xmlSchema))
            {
                string errorMessage = $"XML document does not match the schema: {xmlSchema.SourceUri}";
                throw new FormatException(errorMessage);
            }
        }

        /// <summary>
        /// Generates XML-based report from given XML document validation against provided schema.
        /// </summary>
        /// <param name="xmlDocument">
        /// XML document, which shall be validated.
        /// <param name="xmlSchema">
        /// XML schema, against which provided XML document shall be validated against.
        /// </param>
        /// <returns>
        /// XML element containing generated report.
        /// </returns>
        public static XElement GenerateValidationReport(XDocument xmlDocument, XmlSchema xmlSchema)
        {
            var deviations = new List<XElement>();

            void EventHandler(object sender, ValidationEventArgs eventArgs)
            {
                var deviationElement = new XElement("Deviation",
                    new XAttribute("Index", deviations.Count() + 1),
                    new XAttribute("LineNumber", eventArgs.Exception.LineNumber),
                    new XAttribute("LinePosition", eventArgs.Exception.LinePosition),
                    new XAttribute("Message", eventArgs.Message));

                deviations.Add(deviationElement);
            }

            PerformXmlValidation(xmlDocument, xmlSchema, EventHandler);

            var deviationsElement = new XElement("Deviations",
                new XAttribute("Quantity", deviations.Count()));

            deviations.ForEach(deviationsElement.Add);

            return deviationsElement;
        }
       #endregion
    }
}
