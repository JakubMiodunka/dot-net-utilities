# dot-net-utilities

## Description

Project is set as *.NET Framework* class library, which contains various of commonly needed functionalities.
Used code syntax is compatible both with *.NET Framework* and *.NET Core*, so library can be used by applications
build around either of those.

## Repository Structure

* */src* - Contains source code of the library.
* */doc* - Contains project documentation.

## Documentation

To view project documentation containing more details about
utilities implemented within the library please open */doc/html/index.html* file.

## Usage

1. Clone the repository to Your machine.
2. Build the project (ex. using *Visual Studio*) to ensure,
that clone was successful and library is ready to be utilized.
3. Copy-paste required class/classes to Your project - used code syntax is compatible both with
*.NET Framework* and *.NET Core*.
Alternatively, if Your project is based on *.NET Framework*, You can reference
built library to Your project directly.

## Functionalities

* *DiagnosticsUtilities* - General diagnostics and error reporting.
* *FileSystemUtilities* - Basic operations within machine filesystem,
such as basic files/directories validation, coping/moving directories etc.
* *ProgressBar* - Advanced, but not overengineered CLI progress bar.
* *XmlUtilities* - Operations related with XML format,
such as validation XML documents against the schemas.

## Used Tools

* IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
* Documentation generator: [DoxyGen 1.12.0](https://www.doxygen.nl/)

## Authors

* Jakub Miodunka
  * [GitHub](https://github.com/JakubMiodunka)
  * [LinkedIn](https://www.linkedin.com/in/jakubmiodunka/)

## License

This project is licensed under the MIT License - see the *LICENSE.md* file for details.
