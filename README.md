# SINI

SINI (Simple INI) is an ini files wrapper lib with easy-to-use methods to parse C# primitives as well as enums and custom objects. 
It also provides an API to convert an ini file into an object instance, similar to how C# allow to read XMLs as objects. 

SINI support comments and sections, and is highly configurable.

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

## Reading Primitives

SINI has convenient methods to read the following primitive types:

- GetStr()
- GetChar()
- GetByte()
- GetShort()
- GetUShort()
- GetInt()
- GetUInt()
- GetLong()
- GetULong()
- GetFloat()
- GetDouble()
- GetBool()
- GetEnum()

If you need a built-in C# type that doesn't have a wrapper, you can use `GetPrimitive<T>()` instead. For example, lets try with short:

```cs
short value = GetPrimitive<short>("section", "some_key", -1);
```

### Exceptions

If you try to read a wrong format, for example you try to read int but value is not a valid number, a `FormatException` exception will be thrown. This exception is used by any Get*() method that can fail on parsing.


## Reading Custom Types

Lets say you have a custom type and you want to be able to read it from ini files as if it was a primitive type. For example, a point struct:

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

Note that if you get an exception inside your parser method, SINI will capture it and raise a `FormatException` instead, which is the exception you get on any invalid format.


## INI to Object

C# have a wonderful functionality to convert XML files into object instances. If you didn't know about it, [check it out](https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?view=netcore-3.1) now - it's extremely useful.

SINI provides a similar functionality, but with INI files instead. For example, lets say you have the following object:

```cs
public class MyObj
{
    public int Foo;
    public string Bar;
}
```

You can write a corresponding ini file for it that looks like this:

```ini
foo = 5
bar = hello
```

And then read it directly into an instance, with the `IniFile.ToObject()` method:

```cs
MyObj obj1 = IniFile.ToObject<MyObj>("my_ini_file.ini");
```

When using ToObject(), SINI will attempt to read any Field and Property with public setters from the ini file (using reflection).
Note that the name of the fields turns into *snake_case* while in the ini file. You can control this behavior, but more on that later.

### Nesting

Sometimes you have nested objects in your class, for example if your class looks like this:

```cs
public class MyObj
{
    public int Foo;
    public string Bar;
    public MyObjNested Nested;
}

public class MyObjNested
{
    public bool FooBar;
}
```

In this case, when you try to read an ini file into the object, the nested object will attempt to read itself from a section with the same name (but in snake case). So in this case, your ini file should look something like this:

```ini
foo = 5
bar = hello

[nested]
foo_bar = true
```

Note that ToObject() only supports *one level of nesting*. This is because ini files don't support nesting, and you can't tell if a new section is contained inside the previous section, or under root.

### Custom Parsers

If you defined any custom type parsers prior to calling `ToObject()`, for example like with `MyPoint` [in the example above](#reading-custom-types), they will be used when attempting to read this type instead of the default behavior (which is calling `ToObject()` internally on a section with the same name).

### Multiple Objects in a single file

The `ToObject` methods accept an optional `section` parameter. 
Providing it will only read data from the given section, and treat it as the global scope. You can use this to store multiple objects in a single file, separated by sections.

For example, we can create this ini file:

```ini
[obj1]
foo = bar
hello = world

[obj2]
foo = rab
hello = bye
```

And then read two objects from it:

```cs
public class MyObj
{
    public string Foo;
    public string Hello;
}

MyObj obj1 = IniFile.ToObject<MyObj>("my_ini_file.ini", section:"obj1");
MyObj obj2 = IniFile.ToObject<MyObj>("my_ini_file.ini", section:"obj2");
```

Needless to say, in this case we can't have any nested fields, since we already begin in a section.

### ToObject Flags

You may have noticed that the `ToObject()` method also accepts an optional `flags` parameter. These flags determine the behavior while loading the ini file into an object. Let's list them here:

#### AllowMissingFields

If set, SINI wouldn't mind if not all public properties are loaded from ini file.
If not set, you'll get an exception unless the ini file populates all public fields.

#### AllowAdditionalKeys

If set, SINI wouldn't mind if there are extra keys in the ini file that don't match any of the object's fields.
If not set, you'll get an exception for any unused key.

Note that this validation only runs if you read the whole file, and not just a specific section in it.

#### LowercaseKeysAndSections

If set, when searching for a field in the ini file its name will be lowercased.
For example, a field named 'Foo' will be taken from a key named 'foo', and 'FooBar' from 'foobar'. This also affect section names.

#### SnakecaseKeysAndSections

If set (default), when searching for a field in the ini file its name will be snake_cased.
For example, a field named 'Foo' will be taken from a key named 'foo', and 'FooBar' from 'foo_bar'. This also affect section names.


# Advanced Stuff

## Booleans

By default, the following values will be considered as 'true' for boolean fields: ```1, true, yes, on```, while the following as false: ```0, false, no, off```. Any other value will throw `FormatException`. 

These values are defined by the ini files config, and you can change the accepted values. For example, lets add 'yeah' and 'nah' to the possible values:

```cs
IniFile.DefaultConfig.BoolPositiveValues.Add("yeah");
IniFile.DefaultConfig.BoolNegativeValues.Add("nah");
```

Note that before parsing booleans SINI will lowercase the value, meaning that 'TrUe' will be treated as 'true'. You can also modify this behavior if you don't want it:

```cs
IniFile.DefaultConfig.LowercaseBoolValues = false;
```

If you disable lowering case of booleans, values like these:

```ini
broken_val = True
```

Will raise `FormatException`, because `True` != `true`.

## Keys & Section validation

By default SINI will only accept English characters, underscore, digits and dots for sections and key names. For example, the following keys will throw an exception:

```
bad key1 = val
bad-key2 = val
badkêy = val
```

The keys / section validation is done by a `RegEx` match. You can use a different regex and change the naming rules by setting `IniFile.DefaultConfig.KeyValidationRegex`. For example, lets make it only accept English characters, no digits, underscore or dots:

```cs
IniFile.DefaultConfig.KeyValidationRegex = @"^[a-zA-Z]+$";
```

You can also set it to null, if you don't want any validations.

## Changing Delimiter

By default SINI will use '=' as delimiter between key and value. If you want to change that for whatever reason, you can set it in the ini config:

```cs
IniFile.DefaultConfig.Delimiter = '|';
```

The example above now expect ini file to look like this:

```ini
some_key | some_val
```

## Comments

By default SINI will use ';' and '#' as comments, but you can change these characters. For example, lets say we want to use '&' instead:

```cs
IniFile.DefaultConfig.CommentCharacters = new char[] { '&' };
```

In addition you can decide if you want to allow comments only at the begining of a line, or if you want to allow comments after the value part (default behavior). To disable comments after value, set `AllowCommentsAfterValue` to false:

```cs
IniFile.DefaultConfig.AllowCommentsAfterValue = false;
```

Once set to false, lines like these will not be accepted:

```ini
some_key = some_val ; this is broken
```

## Per-file Config

In the examples above we only used a static config instance, `IniFile.DefaultConfig`, which is used to determine the behavior of SINI when no specific config is provided. Note that you can provide custom configurations per ini file you open, as a constructor parameter. For example:

```cs
var customConf = IniConfig.CreateDefaults();
customConf.AllowCommentsAfterValue = false;           // <-- change something
var ini = new Sini.IniFile("ok_file.ini", customConf);      // <-- use custom configuration just for this file
```


# License

SINI is distributed with the permissive MIT license.