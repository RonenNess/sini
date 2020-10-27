# SINI

SINI (Simple INI) is an ini files wrapper lib with easy-to-use methods to parse C# primitives as well as enums and custom objects. 
It also provides an API to convert an ini file into an object instance, similar to how C# allow to read XMLs as objects. 

SINI support comments and sections, and highly configurable.

# Install

`Install-Package Sini`

Or visit [https://www.nuget.org/packages/sini/](https://www.nuget.org/packages/sini/) for more details.

# Usage

Using SINI is quite simple:

```cs
// open ini file named 'my_conf_file.ini'.
// you can also create ini file from string array, where every string = line.
var ini = new Sini.IniFile("my_conf_file.ini");

// read key 'some_key' as string from global section. if not set, will return "default value if not found" instead.
var someVal = ini.GetStr(null, "some_key", "default value if not found");

// read key 'int_key' under section 'section1' as int. if not set, will return 0 instead.
var intVal = ini.GetInt("section1", "int_key", 0);

// read key 'bool_key' under section 'section1' as boolean. if not set, will return false instead.
var boolVal = ini.GetBool("section1", "bool_key", false);

// read key 'enum_key' under section 'section1' as enum of type MyEnum. if not set, will return MyEnum.Foo instead.
// MyEnum has either 'Foo' or 'Bar' values.
var enumVal = ini.GetEnum("section1", "enum_key", MyEnum.Foo);
```

And a valid ini file to match the example above would be something like:

```ini
; in global section
some_key = hello world

[section1]
int_key = 5
bool_key = true
enum_key = Bar  ; possible values = Foo / Bar
```

## Basic Types

SINI has convinient methods to read the following primitive types:

- GetStr()
- GetInt()
- GetLong()
- GetULong()
- GetFloat()
- GetDouble()
- GetBool()

If you need a built-in C# type that doesn't have a convinient wrapper, you can use `GetPrimitive()` instead. For example, lets try with Short:

```cs
short value = GetPrimitive<short>("section", "some_key", -1);
```

### Exceptions

If you try to read a wrong format, for example you try to read int but value is not a valid number, a `FormatException` exception will be thrown. This exception is used by any Get*() method that can fail on parsing.


# Advanced Stuff

## Custom Types

Lets say you have a custom type and you want to be able to read it from ini files:

```cs
public struct MyPoint
{
    public int X;
    public int Y;
}
```

To do so, you can register a custom parser method:

```cs
// register cusom type parser for MyPoint
IniFile.DefaultConfig.CustomParsers[typeof(MyPoint)] = (string val) => 
{
    var parts = val.Split(',');
    return new MyPoint() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
};
```

And later you can use it like this:

```cs
MyPoint point = ini.GetCustomType("section1", "point_value", new MyPoint());
```

Note that if you get exception inside your parsing method, SINI will capture it and raise a `FormatException` instead, which is the exception you get on any invalid format.


## Booleans

By default, the following values will be considered as 'true' for boolean fields: ```1, true, yes, on```, while the following as false: ```0, false, no, off```. Any other value will throw `FormatException`. 

These values are defined by the ini files config, and you can change the accepted values. For example, lets add 'yeah' and 'nah' to the possible values:

```cs
IniFile.DefaultConfig.BoolPositiveValues.Add("yeah");
IniFile.DefaultConfig.BoolNegativeValues.Add("nah");
```

Note that before parsing booleans SINI will lowercase the value, meaning that 'TrUe' is the same as 'true'. You can also modify this behavior:

```cs
IniFile.DefaultConfig.LowercaseBoolValues = false;
```

## Key & Section validation

By default SINI will only accept English characters, underscore, digits and dots for sections and key names. For example, the following keys will throw an exception:

```
bad key1 = val
bad-key2 = val
badkêy = val
```

The keys / section validation is done by a `RegEx` expression. You can change it by changing `IniFile.DefaultConfig.KeyValidationRegex`. For example, lets make it only accept English characters:

```cs
IniFile.DefaultConfig.KeyValidationRegex = @"^[a-zA-Z]+$";
```

## Changing Delimiter

By default SINI will use '=' as delimiter between key and value. If you want to change that for whatever reason, for example lets say you want to use '|' instead of '=', you can set it in DefaultConfig:

```cs
IniFile.DefaultConfig.Delimiter = '|';
```

## Comments

By default SINI will use ';' and '#' as comments. You can change these characters. For example, lets say we want to use '&' instead:

```cs
IniFile.DefaultConfig.CommentCharacters = new char[] { '&' };
```

In addition you can decide if you want to allow comments only at the begining of a line, or if you want to allow comments after the value part (default behavior):

```cs
IniFile.DefaultConfig.AllowCommentsAfterValue = true;
```

## Per-file Config

As you probably noticed, there's a static config instance which is used to determine the behavior of SINI: `IniFile.DefaultConfig`. Note that you can also provide a per-file config in the IniFile constructor:

```cs
var customConf = IniConfig.CreateDefaults();
customConf.AllowCommentsAfterValue = false;           // <-- change something
var ini = new Sini.IniFile("ok_file.ini", customConf);      // <-- use custom configuration just for this file
```


# License

SINI is distributed with the permissive MIT license.