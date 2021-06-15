# Requirements
.Net core Web project (Blazor/Angular/React) to read the excel and attached xml data and display on screen (if we can display with the style in excel).

The template that needs to be read is “F 20.04” in the sheet and when merged with the data from the xml it should display the sheet as in “Final” tab.

Consideration needs to be given to Unit test, DI, logging, change to template etc.

Less hard coding so that solution is robust enough to handle a new template

_Disclaimer: The main focus was on the back-end developement and to deliver a solution to avoid hard-coding when dealing with the files and to generate the report dynamically. Also, to follow TDD and keep the code coverage high. The UI representation may not look idea though_

# Assumptions
The solution assumes that the Template excel file _always_ comes with the user-defined indexes and based on which it merges the report values and generates the final report. As per assignment requirement. If the user-defined indexes are not provided, the report will not be generated

# Solution overview

## The idea behind
When dealing with excel files and mergin the report values into the template, there is no hard-coded values as to represent the columns or properties from the excel sheet. In fact, the oeveral idea is: 
1. The solution first reads the report values from the xml file
2. It then reads the template and calculates the target cells wherein the report values must be places
3. It writes the values into the target cells identified from the previous steps and generates a new excel file to represent the merged version or final result
4. It reads the final result and converts it to a data table
5. It generates a raw html out of the final report dynamically
6. Finally, the blazor page renders the generated html 

# Tech spec

## Project type
The project is a Blazor .net core project hostted on aspnet core. 

## IDE
Visual Studion 2019

## Dependencies
- [EPPlus](https://www.nuget.org/packages/EPPlus) nuget package (non-commercial version)
- [Oledb](https://www.nuget.org/packages/System.Data.OleDb/)
- [Moq](https://www.nuget.org/packages/moq/) & [Fluent Assertions](https://www.nuget.org/packages/FluentAssertions/)

## Overal structure
This project follows the default Visual Studio solution structure for blazor projects consisting of the followings:
- **Client prject** - the front-end
- **Server project** - the services and controllers
- **Shared project** - the model classes 
- **Test project** - MS Unit test for .net core

## How to run
1. Clone the source code
2. Build the solution in Visual Studio 2019
3. Hit F5 to run the solution

# Code Coverage against the server project where all the services and logics are hosted
![image](https://user-images.githubusercontent.com/7995157/120728525-066fcb00-c4d5-11eb-981f-227d32084dbd.png)

# A screenshot from the generated report
Un-handled UI part: The report does not ignore the repeated cells such as references. This can be handled by having a hash set to keep track of visited nodes when generating the raw html dynamically in service `DataTableToRawHtmlParser`.
![image](https://user-images.githubusercontent.com/7995157/120767344-08587f00-c513-11eb-80e5-915921c3b2be.png)

