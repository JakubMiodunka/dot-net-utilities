using System;
using System.Xml.Linq;
using System.Xml.Schema;


namespace DotNetUtilities
{
    /// <summary>
    /// Set of utilities related to XML format.
    /// </summary>
    public static class XmlUtilities
    {
        /// <summary>
        /// Checks if given XML document matches specified schema.
        /// </summary>
        /// <param name="xmlDocument">
        /// XML document, which shall be validated.
        /// </param>
        /// <param name="schemaPath">
        /// Path to *.xsd schema, against which provided XML document shall be validated against.
        /// </param>
        /// <returns>
        /// True if provided XML document matches provided *.xsd schema, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        public static bool IsMatchingToSchema(XDocument xmlDocument, string schemaPath)
        {
            #region Arguments validation
            if (xmlDocument is null)
            {
                string argumentName = nameof(xmlDocument);
                const string ErrorMessage = "Provided XML document is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }
            #endregion

            FileSystemUtilities.ValidateFile(schemaPath, ".xsd");

            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, schemaPath);

            bool isDocumentValid = true;
            xmlDocument.Validate(schemaSet, (sender, eventData) => isDocumentValid = false);

            return isDocumentValid;
        }

        /// <summary>
        /// Checks if given XML file matches specified schema.
        /// </summary>
        /// <param name="filePath">
        /// Path to *.xml file, which shall be validated.
        /// </param>
        /// <param name="schemaPath">
        /// Path to *.xsd schema, against which provided XML file shall be validated against.
        /// </param>
        /// <returns>
        /// True if provided XML file matches provided *.xsd schema, false otherwise.
        /// </returns>
        public static bool IsMatchingToSchema(string filePath, string schemaPath)
        {
            #region Arguments validation
            FileSystemUtilities.ValidateFile(filePath, ".xml");
            #endregion

            var xmlDocument = XDocument.Load(filePath);

            return IsMatchingToSchema(xmlDocument, schemaPath);
        }

        /// <summary>
        /// Checks if given XML document matches specified schema.
        /// If validation will fail, according exception will be thrown.
        /// </summary>
        /// <param name="xmlDocument">
        /// XML document, which shall be validated.
        /// </param>
        /// <param name="schemataPath">
        /// Path to *.xsd schema, against which provided XML document shall be validated against.
        /// </param>
        /// <exception cref="FormatException">
        /// Thrown, when provided XML document does not match specified schema.
        /// </exception>
        public static void ValidateXmlDocument(XDocument xmlDocument, string schemataPath)
        {
            if (!IsMatchingToSchema(xmlDocument, schemataPath))
            {
                const string ErrorMessage = "XML document does not match the schema:";
                throw new FormatException(ErrorMessage);
            }
        }

        /// <summary>
        /// Checks if given XML file matches specified schema.
        /// If validation will fail, according exception will be thrown.
        /// </summary>
        /// <param name="filePath">
        /// Path to *.xml file, which shall be validated.
        /// </param>
        /// <param name="schemaPath">
        /// Path to *.xsd schema, against which provided XML file shall be validated against.
        /// </param>
        /// <exception cref="FormatException">
        /// Thrown, when provided XML file does not match specified schema.
        /// </exception>
        public static void ValidateXmlFile(string filePath, string schemaPath)
        {
            if (!IsMatchingToSchema(filePath, schemaPath))
            {
                string errorMessage = $"XML file doesn't match the schema: {filePath}";
                throw new FormatException(errorMessage);
            }
        }
    }
}
