# Kotas utilities [![Build Status](https://travis-ci.org/Marfusios/kotas-utils.svg?branch=master)](https://travis-ci.org/Marfusios/kotas-utils) [![NuGet version](https://badge.fury.io/nu/Kotas.Utils.Common.svg)](https://badge.fury.io/nu/Kotas.Utils.Common)

These libraries contain reusable building components for the .NET Standard 2.0 applications. 

### License: 
    Apache License 2.0

### Features

* **Kotas.Utils.Common** - a base common library
    * preconfigured json serialization
    * unix datetime
    * validations
    * comparers (int/string array)
    * is primitive method
    * other utils (reflection, string, enum)
* **Kotas.Utils.Data** - a base library for abstract data access
    * the unit of work pattern
* **Kotas.Utils.Data.Native** - a library for native data access
    * implementation of the unit of work pattern
    * base classes: `NativeQuery` and `NativeStore`
    * SQL server via `System.Data.SqlClient`
    * PostgreSQL via `Npgsql`
* **Kotas.Utils.Data.EntityFramework** - a library for Entity Framework data access
    * implementation of the unit of work pattern
    * base classes: `EntityFrameworkQuery` and `EntityFrameworkStore`
    * SQL server via `Microsoft.EntityFrameworkCore.SqlServer`
    * PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`

### Usage

```csharp


```
