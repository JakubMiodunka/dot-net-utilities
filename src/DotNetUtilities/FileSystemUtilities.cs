using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DotNetUtilities
{
    /// <summary>
    /// Set of utilities related to file system.
    /// </summary>
    public static class FileSystemUtilities
    {
        /// <summary>
        /// Checks if specified file system entry exists and is a directory.
        /// If either of above cases would not be fulfilled, according exception will be thrown.
        /// </summary>
        /// <param name="path">
        /// Path to file system entry, which shall be validated.
        /// </param>
        /// <param name="shallExist">
        /// Specifies if provided file system entry shall exists within file system as directory.
        /// Useful, when there is a need to check if specified path is not already
        /// in use by other directory in file system.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown, when at least one argument will be considered as invalid.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown, when provided file system entry does not exist in file system as directory.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown, when provided file system entry exists within file system as directory but shall not.
        /// </exception>
        public static void ValidateDirectory(string path, bool shallExist = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                string argumentName = nameof(path);
                string errorMessage = $"Provided path is invalid: {path}";
                throw new ArgumentException(argumentName, errorMessage);
            }

            if (shallExist)
            {
                if (!Directory.Exists(path))
                {
                    if (File.Exists(path))
                    {
                        string errorMessage = $"Given entry is a file: {path}";
                        throw new DirectoryNotFoundException(errorMessage);
                    }
                    else
                    {
                        string errorMessage = $"Directory does not exist: {path}";
                        throw new DirectoryNotFoundException(errorMessage);
                    }
                }
            }
            else
            {
                if (Directory.Exists(path))
                {
                    string errorMessage = $"Directory already exist: {path}";
                    throw new IOException(errorMessage);
                }
            }
        }

        /// <summary>
        /// Checks if extension of specified file system entry is equal to expected one.
        /// If validation condition would not be fulfilled, according exception will be thrown.
        /// </summary>
        /// <remarks>
        /// Check is based entirely on provided path - method does not check
        /// if specified entry exists within file system.
        /// </remarks>
        /// <param name="path">
        /// Path to file system entry, which shall be validated.
        /// </param>
        /// <param name="validExtensions">
        /// Collection of extensions, which shall be considered as valid for validated file.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown, when at least one argument will be considered as invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown, when specified file system entry has invalid extension.
        /// </exception>
        public static void ValidateExtension(string path, IEnumerable<string> validExtensions)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                string argumentName = nameof(path);
                string errorMessage = $"Provided path is invalid: {path}";
                throw new ArgumentException(argumentName, errorMessage);
            }

            if (validExtensions is null)
            {
                string argumentName = nameof(validExtensions);
                const string ErrorMessage = "Provided extensions collection is null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }

            if (validExtensions.Any(string.IsNullOrWhiteSpace))
            {
                string argumentName = nameof(path);
                const string ErrorMessage = "Provided extensions collection contains invalid extension:";
                throw new ArgumentException(argumentName, ErrorMessage);
            }

            string actualExtension = Path.GetExtension(path);

            if (!validExtensions.Contains(actualExtension))
            {
                string errorMessage = $"Invalid extension: {actualExtension}";
                throw new IOException(errorMessage);
            }
        }

        /// <summary>
        /// Checks if extension of specified file system entry is equal to expected one.
        /// If validation condition would not be fulfilled, according exception will be thrown.
        /// </summary>
        /// <remarks>
        /// Check is based entirely on provided path - method does not check
        /// if specified entry exists within file system.
        /// </remarks>
        /// <param name="path">
        /// Path to file system entry, which shall be validated.
        /// </param>
        /// <param name="validExtension">
        /// Extension, which shall be considered as valid for validated file.
        /// </param>
        public static void ValidateExtension(string path, string validExtension)
        {
            string[] validExtensions = { validExtension };

            ValidateExtension(path, validExtensions);
        }

        /// <summary>
        /// Checks if specified file system entry exists, is a file and its extension
        /// matches the expected one.
        /// If either validation condition would not be fulfilled, according exception will be thrown.
        /// </summary>
        /// <param name="path">
        /// Path to file system entry, which shall be validated.
        /// </param>
        /// <param name="shallExist">
        /// Specifies if provided file system entry shall exists within file system as file.
        /// Useful, when there is a need to check if specified path is not already
        /// in use by other file in file system.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown, when at least one argument will be considered as invalid.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown, when provided file system entry does not exist in file system as file.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown, when provided file system entry exists within file system as file but shall not.
        /// </exception>
        public static void ValidateFile(string path, bool shallExist = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                string argumentName = nameof(path);
                string errorMessage = $"Provided file path is invalid: {path}";
                throw new ArgumentException(argumentName, errorMessage);
            }

            if (shallExist)
            {
                if (!File.Exists(path))
                {
                    if (Directory.Exists(path))
                    {
                        string errorMessage = $"Given entry is a directory: {path}";
                        throw new FileNotFoundException(errorMessage, path);
                    }
                    else
                    {
                        string errorMessage = $"File does not exist: {path}";
                        throw new FileNotFoundException(errorMessage, path);
                    }
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    string errorMessage = $"File already exist: {path}";
                    throw new IOException(errorMessage);
                }
            }
        }

        /// <summary>
        /// Checks if specified file system entry exists, is a file and its extension
        /// matches the expected one.
        /// If either validation condition would not be fulfilled, according exception will be thrown.
        /// </summary>
        /// <param name="path">
        /// Path to file system entry, which shall be validated.
        /// </param>
        /// <param name="validExtensions">
        /// Collection of extensions, which shall be considered as valid for validated file.
        /// </param>
        /// <param name="shallExist">
        /// Specifies if provided file system entry shall exists within file system as file.
        /// Useful, when there is a need to check if specified path is not already
        /// in use by other file in file system.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown, when at least one argument will be considered as invalid.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown, when provided file system entry does not exist in file system as file.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown, either when provided file system entry exists within file system but shall not.
        /// </exception>
        public static void ValidateFile(string path, IEnumerable<string> validExtensions, bool shallExist = true)
        {
            ValidateFile(path, shallExist);
            ValidateExtension(path, validExtensions);
        }

        /// <summary>
        /// Checks if specified file system entry exists, is a file and its extension
        /// matches the expected one.
        /// If either validation condition would not be fulfilled, according exception will be thrown.
        /// </summary>
        /// <param name="path">
        /// Path to file system entry, which shall be validated.
        /// </param>
        /// <param name="validExtension">
        /// Extension, which shall be conceited as valid for validated file.
        /// </param>
        /// <param name="shallExist">
        /// Specifies if provided file system entry shall exists within file system as file.
        /// Useful, when there is a need to check if specified path is not already
        /// in use by other file in file system.
        /// </param>
        public static void ValidateFile(string path, string validExtension, bool shallExist = true)
        {
            string[] validExtensions = { validExtension };

            ValidateFile(path, validExtensions, shallExist);
        }
    }
}
