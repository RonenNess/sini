using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sini;
using System;
using System.Collections.Generic;


namespace SiniTest.UnitTest
{
    [TestClass]
    public class SiniUnitTest
    {

        [TestMethod]
        public void BasicTypes()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // get global var
            Assert.AreEqual(ini.GetStr(null, "global_val"), "foo");
            Assert.AreEqual(ini.GetStr(string.Empty, "global_val"), "foo");

            // test basic values from section
            Assert.AreEqual("val1",                     ini.GetStr("section1", "str_val"));
            Assert.AreEqual(123,                        ini.GetInt("section1", "int_val"));
            Assert.AreEqual((uint)123,                  ini.GetUInt("section1", "int_val"));
            Assert.AreEqual((ushort)123,                ini.GetUShort("section1", "int_val"));
            Assert.AreEqual(123,                        ini.GetShort("section1", "int_val"));
            Assert.AreEqual((byte)123,                  ini.GetByte("section1", "int_val"));
            Assert.AreEqual('a',                        ini.GetChar("section1", "char_val"));
            Assert.AreEqual(12345678901234,             ini.GetLong("section1", "long_val"));
            Assert.AreEqual((ulong)12345678901234567890,ini.GetULong("section1", "ulong_val"));
            Assert.AreEqual(1.2f,                       ini.GetFloat("section1", "float_val"));
            Assert.AreEqual(4.51524141252152111,        ini.GetDouble("section1", "double_val"));
            Assert.AreEqual(-123,                       ini.GetInt("section1", "negative_int_val"));
            Assert.AreEqual(-12345678901234,            ini.GetLong("section1", "negative_long_val"));
            Assert.AreEqual(-1.2f,                      ini.GetFloat("section1", "negative_float_val"));
            Assert.AreEqual(-4.51524141252152111,       ini.GetDouble("section1", "negative_double_val"));
            Assert.AreEqual(MyEnum.Bar,                 ini.GetEnum("section1", "enum_val", MyEnum.Foo));
            Assert.IsTrue(ini.GetBool("section1", "bool_val_pos1"));
            Assert.IsTrue(ini.GetBool("section1", "bool_val_pos2"));
            Assert.IsTrue(ini.GetBool("section1", "bool_val_pos3"));
            Assert.IsTrue(ini.GetBool("section1", "bool_val_pos4"));
            Assert.IsTrue(ini.GetBool("section1", "bool_val_pos5"));
            Assert.IsFalse(ini.GetBool("section1", "bool_val_neg1"));
            Assert.IsFalse(ini.GetBool("section1", "bool_val_neg2"));
            Assert.IsFalse(ini.GetBool("section1", "bool_val_neg3"));
            Assert.IsFalse(ini.GetBool("section1", "bool_val_neg4"));
            Assert.IsFalse(ini.GetBool("section1", "bool_val_neg5"));
            Assert.AreEqual(ini.GetStr("section1", "key.with.dot"), "val");
            Assert.AreEqual(5, ini.GetPrimitive<short>("section1", "short_val", -1));

            // getting from another section + comment in line
            Assert.AreEqual("world", ini.GetStr("section2", "hello"));
        }

        [TestMethod]
        public void SectionWithSpace()
        {
            var config = new IniConfig();
            config.KeyValidationRegex = @"^[a-zA-Z_\.0-9 ]+$";
            var ini = new IniFile("ok_file_section_space.ini", config);
            Assert.AreEqual("world", ini.GetStr("section with spaces", "hello", "default_val"));
        }

        [TestMethod]
        public void CustomMultilineContinuation()
        {
            var config = new IniConfig();
            config.MultilineContinuation = "|=";
            var ini = new IniFile("custom_multiline.ini", config);
            Assert.AreEqual("val1\nval2\nval3", ini.GetStr("section1", "str_val", "default_val"));
        }

        [TestMethod]
        public void DefaultValues()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // test basic values from section
            foreach (var section in new string[] { "section1", "", "blaaa" })
            {
                Assert.AreEqual("default_val", ini.GetStr(section, "bla_str_val", "default_val"));
                Assert.AreEqual(null, ini.GetStr(section, "bla_str_val"));
                Assert.AreEqual(10, ini.GetInt(section, "bla_int_val", 10));
                Assert.AreEqual((long)100, ini.GetLong(section, "bla_long_val", 100));
                Assert.AreEqual((ulong)123, ini.GetULong(section, "bla_ulong_val", 123));
                Assert.AreEqual(54.3f, ini.GetFloat(section, "bla_float_val", 54.3f));
                Assert.AreEqual(754.3, ini.GetDouble(section, "bla_double_val", 754.3));
                Assert.AreEqual(-100, ini.GetInt(section, "bla_negative_int_val", -100));
                Assert.AreEqual((long)-53, ini.GetLong(section, "bla_negative_long_val", -53));
                Assert.AreEqual(-64.4f, ini.GetFloat(section, "bla_negative_float_val", -64.4f));
                Assert.AreEqual(-444214.13, ini.GetDouble(section, "bla_negative_double_val", -444214.13));
                Assert.IsTrue(ini.GetBool(section, "bla_bool_val_pos1", true));
                Assert.IsFalse(ini.GetBool(section, "bla_bool_val_neg1", false));
                Assert.AreEqual(null, ini.GetStr(section, "bla_key.with.dot"));
                Assert.AreEqual("default_val", ini.GetStr(section, "bla_key.with.dot", "default_val"));
            }

        }


        [TestMethod]
        public void Exists()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check multiline value
            Assert.AreEqual("this is\na multiline\nvalue.", ini.GetStr("section1", "multiline", null));
            Assert.AreEqual("this is\n\na double multiline\nvalue.", ini.GetStr("section1", "multiline_2", null));
        }


        [TestMethod]
        public void Sections()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check multiline value
            var expected = new List<string>("section1,section2,invalids,special".Split(','));
            expected.Sort();
            var values = new List<string>(ini.Sections);
            values.Sort();
            CollectionAssert.AreEqual(expected, values);
        }

        [TestMethod]
        public void MultilineValue()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check existing and non-existing keys in global section
            Assert.IsTrue(ini.ContainsKey(null, "global_val"));
            Assert.IsFalse(ini.ContainsKey(null, "global_val_nope"));

            // check existing and non-existing keys in a section
            Assert.IsTrue(ini.ContainsKey("section1", "str_val"));
            Assert.IsFalse(ini.ContainsKey("section1", "str_val_nope"));
        }

        [TestMethod]
        public void ContainsKey()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check existing and non-existing keys in global section
            Assert.IsTrue(ini.ContainsKey("section1", "str_val"));
            Assert.IsFalse(ini.ContainsKey("section1", "foobar"));
            Assert.IsTrue(ini.ContainsKey(null, "global_val"));
            Assert.IsFalse(ini.ContainsKey(null, "foobar"));
        }

        [TestMethod]
        public void ParseComments()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check parsed comments
            Assert.AreEqual("comment line", ini.GetComment("section2", "hello"));
            Assert.AreEqual("comment line 2", ini.GetComment("section2", "value_with_comment"));
            Assert.AreEqual("section comment", ini.GetComment("special", null));
            Assert.AreEqual("global value comment", ini.GetComment(null, "global_val"));
            Assert.AreEqual(String.Join('\n', "but this part of the multiline comment", "will remain."), ini.GetComment("special", "point_val"));
        }

        [TestMethod]
        public void ToFullString()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // build full text, make sure comments are kept (except the ones that were suppoed to be removed)
            var fullText = ini.ToFullString();
            var expected = System.IO.File.ReadAllText("ok_file_parsed_and_written.ini").Replace(Environment.NewLine, "\n");
            Assert.AreEqual(expected, fullText);
        }

        [TestMethod]
        public void ClearComments()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // make sure comments are removed
            ini.ClearComments();
            var fullText = ini.ToFullString();
            var expected = System.IO.File.ReadAllText("ok_file_parsed_and_written_no_comments.ini").Replace(Environment.NewLine, "\n");
            Assert.AreEqual(expected, fullText);
        }

        [TestMethod]
        public void ContainsSection()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check existing and non-existing keys in global section
            Assert.IsTrue(ini.ContainsSection("section1"));
            Assert.IsFalse(ini.ContainsSection("foobar"));
        }

        [TestMethod]
        public void GetKeys()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check existing and non-existing keys in global section
            CollectionAssert.AreEquivalent(ini.GetKeys(null), "global_val".Split(','));
            CollectionAssert.AreEquivalent(ini.GetKeys("section2"), "hello,value_with_comment".Split(','));
        }

        [TestMethod]
        public void AsDictionary()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // check existing and non-existing keys in global section
            var expected = new Dictionary<string, string>();
            expected["global_val"] = "foo";
            CollectionAssert.AreEquivalent(ini.AsDictionary(null), expected);

            expected = new Dictionary<string, string>();
            expected["hello"] = "world";
            expected["value_with_comment"] = "hello";
            CollectionAssert.AreEquivalent(ini.AsDictionary("section2"), expected);
        }

        [TestMethod]
        public void WrongFormats()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");
            Assert.ThrowsException<FormatException>(() => { ini.GetInt("section1", "str_val"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetLong("section1", "str_val"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetULong("section1", "str_val"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetFloat("section1", "str_val"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetDouble("section1", "str_val"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetBool("section1", "str_val"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetBool("invalids", "bad_bool"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetInt("invalids", "bad_int"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetULong("invalids", "bad_ulong"); });
            Assert.ThrowsException<FormatException>(() => { ini.GetEnum("invalids", "bad_enum1", MyEnum.Foo); });
            Assert.ThrowsException<FormatException>(() => { ini.GetEnum("invalids", "bad_enum2", MyEnum.Foo); });
        }

        [TestMethod]
        public void SetValues()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // replace value and validate it was changed
            Assert.AreEqual("val1", ini.GetStr("section1", "str_val"));
            ini.SetValue("section1", "str_val", "new_val");
            Assert.AreEqual("new_val", ini.GetStr("section1", "str_val"));

            // replace value without section and validate it was changed
            Assert.AreEqual("foo", ini.GetStr(null, "global_val"));
            ini.SetValue(null, "global_val", "new_value_set");
            Assert.AreEqual("new_value_set", ini.GetStr(null, "global_val"));

            // add a new value and validate it was created
            Assert.AreEqual(null, ini.GetStr("section1", "not_existing"));
            ini.SetValue("section1", "not_existing", "abcd");
            Assert.AreEqual("abcd", ini.GetStr("section1", "not_existing"));

            // add a new value and validate it was created, under global section
            Assert.AreEqual(null, ini.GetStr(null, "not_existing"));
            ini.SetValue(null, "not_existing", "abcd_2");
            Assert.AreEqual("abcd_2", ini.GetStr(null, "not_existing"));

            // add a new value and validate it was created, under a newly created section
            Assert.AreEqual(null, ini.GetStr("non_existing_section", "str_val"));
            ini.SetValue("non_existing_section", "str_val", "qwerty");
            Assert.AreEqual("qwerty", ini.GetStr("non_existing_section", "str_val"));
        }

        [TestMethod]
        public void DeleteValues()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // delete a simple value and make sure no longer exists
            Assert.AreEqual("val1", ini.GetStr("section1", "str_val"));
            ini.DeleteKey("section1", "str_val");
            Assert.AreEqual(null, ini.GetStr("section1", "str_val"));
            Assert.IsFalse(ini.ContainsKey("section1", "str_val"));
            Assert.IsTrue(ini.ContainsKey("section1", "int_val"));

            // delete all keys from a section and make sure section is removed
            Assert.IsTrue(ini.ContainsSection("special"));
            Assert.AreEqual("15,25", ini.GetStr("special", "point_val"));
            ini.DeleteKey("special", "point_val");
            Assert.AreEqual(null, ini.GetStr("special", "point_val"));
            Assert.IsFalse(ini.ContainsSection("special"));
        }

        [TestMethod]
        public void DeleteSection()
        {
            // open ini file
            var ini = new IniFile("ok_file.ini");

            // delete all keys from a section and make sure section is removed
            Assert.IsTrue(ini.ContainsSection("special"));
            Assert.AreEqual("15,25", ini.GetStr("special", "point_val"));
            ini.DeleteSection("special");
            Assert.AreEqual(null, ini.GetStr("special", "point_val"));
            Assert.IsFalse(ini.ContainsSection("special"));


            // delete global section and make sure its empty after
            Assert.IsTrue(ini.ContainsKey(null, "global_val"));
            ini.DeleteSection(null);
            Assert.IsFalse(ini.ContainsKey(null, "global_val"));
        }

        [TestMethod]
        public void CreateEmpty()
        {
            var ini = IniFile.CreateEmpty();
            ini.SetValue("foo", "bar", "hello");
            Assert.AreEqual("hello", ini.GetStr("foo", "bar"));
        }

        [TestMethod]
        public void CustomTypes()
        {
            // register cusom type parser
            IniFile.DefaultConfig.CustomParsers[typeof(MyPoint)] = (string val) => 
            {
                var parts = val.Split(',');
                return new MyPoint() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
            };

            // open ini file
            var ini = new IniFile("ok_file.ini");

            // read point 
            var mypoint = ini.GetCustomType("section1", "point_val", new MyPoint());
            Assert.AreEqual(5, mypoint.X);
            Assert.AreEqual(-7, mypoint.Y);
        }


        [TestMethod]
        public void IniToObject()
        {
            // register cusom type parser for point
            IniFile.DefaultConfig.CustomParsers[typeof(MyPoint)] = (string val) =>
            {
                var parts = val.Split(',');
                return new MyPoint() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
            };

            // parse object
            TestObject ret = IniFile.ToObject<TestObject>("test_object.ini");

            // validate fields
            Assert.AreEqual(5, ret.Foo);
            Assert.AreEqual("hello", ret.Bar);
            Assert.IsTrue(ret.FooBar);
            Assert.AreEqual(MyEnum.Bar, ret.EnumVal);
            Assert.AreEqual(7, ret.Point.X);
            Assert.AreEqual(3, ret.Point.Y);
            Assert.IsTrue(ret.BoolWithYes);

            // validate nested fields
            Assert.AreEqual(5, ret.Nested.ThisIsNested);
            Assert.AreEqual(10, ret.Nested.OtherField);
            Assert.AreEqual(MyEnum.Foo, ret.Nested.EnumVal);
            Assert.AreEqual(-1, ret.Nested.NestedPoint.X);
            Assert.AreEqual(-5, ret.Nested.NestedPoint.Y);

            // deeper nested
            Assert.AreEqual("ok", ret.Nested.Deeper.Value);
            Assert.AreEqual("wohoo", ret.Nested.Deeper.EvenDeeper.Value);

            // read two objects from the same file
            TestObjectMulti obj1 = IniFile.ToObject<TestObjectMulti>("test_object_multi.ini", section: "obj1");
            TestObjectMulti obj2 = IniFile.ToObject<TestObjectMulti>("test_object_multi.ini", section: "obj2");

            // check the multi objects read
            Assert.AreEqual("bar", obj1.Foo);
            Assert.AreEqual("world", obj1.Hello);
            Assert.AreEqual("rab", obj2.Foo);
            Assert.AreEqual("bye", obj2.Hello);

            // parse object
            TestObjectNestedDict nestedAsDict = IniFile.ToObject<TestObjectNestedDict>("test_object.ini");

            // validate fields
            Assert.AreEqual(5, nestedAsDict.Foo);
            Assert.AreEqual("hello", nestedAsDict.Bar);
            Assert.IsTrue(nestedAsDict.FooBar);
            Assert.AreEqual(MyEnum.Bar, nestedAsDict.EnumVal);
            Assert.AreEqual(7, nestedAsDict.Point.X);
            Assert.AreEqual(3, nestedAsDict.Point.Y);

            // validate nested fields as dictionary
            Assert.AreEqual("5", nestedAsDict.Nested["this_is_nested"]);
            Assert.AreEqual("10", nestedAsDict.Nested["other_field"]);
            Assert.AreEqual("Foo", nestedAsDict.Nested["enum_val"]);
            Assert.AreEqual("-1,-5", nestedAsDict.Nested["nested_point"]);

        }

        [TestMethod]
        public void IniToObjectExampleFromReadme()
        {
            // parse object
            Config ret = IniFile.ToObject<Config>("test_object_readme_example.ini");

            // validate fields
            Assert.IsTrue(ret.Graphics.Fullscreen);
            Assert.AreEqual(1280, ret.Graphics.Resolution.Width);
            Assert.AreEqual(1024, ret.Graphics.Resolution.Height);
            Assert.AreEqual(100, ret.Sound.Volume);
        }

        [TestMethod]
        public void IniFromObject()
        {
            // remove the default parser
            if (IniFile.DefaultConfig.CustomParsers.ContainsKey(typeof(MyPoint)))
            {
                IniFile.DefaultConfig.CustomParsers.Remove(typeof(MyPoint));
            }

            // create an object
            TestObject original = new TestObject()
            {
                Bar = "aaa",
                EnumVal = MyEnum.Bar,
                Foo = 15,
                FooBar = false,
                Point = new MyPoint() { X = 5, Y = 100 },
                Nested = new NestedTestObject()
                {
                    EnumVal = MyEnum.Foo,
                    OtherField = 1.234f
                }
            };

            // test without custom parsers / serializers
            {
                // create ini
                var ini = IniFile.FromObject(original);

                // now create copy object from ini
                var copy = new TestObject();
                IniFile.ToObject(ref copy, ini, null, IniFile.ParseObjectFlags.DefaultFalgs | IniFile.ParseObjectFlags.AllowMissingFields | IniFile.ParseObjectFlags.AllowAdditionalKeys);

                // make sure original and copy are the same
                Assert.AreEqual(original.Bar, copy.Bar);
                Assert.AreEqual(original.EnumVal, copy.EnumVal);
                Assert.AreEqual(original.Foo, copy.Foo);
                Assert.AreEqual(original.FooBar, copy.FooBar);
                Assert.AreEqual(original.Point.X, copy.Point.X);
                Assert.AreEqual(original.Point.Y, copy.Point.Y);
                Assert.AreEqual(original.Nested.EnumVal, copy.Nested.EnumVal);
                Assert.AreEqual(original.Nested.OtherField, copy.Nested.OtherField);
            }

            // test with custom parsers / serializers
            {
                IniFile.DefaultConfig.CustomParsers[typeof(MyPoint)] = (string val) =>
                {
                    var parts = val.Split(',');
                    return new MyPoint() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
                };
                IniFile.DefaultConfig.CustomSerializers[typeof(MyPoint)] = (object val) =>
                {
                    var point = (MyPoint)val;
                    return point.X.ToString() + "," + point.Y.ToString();
                };

                // create ini
                var ini = IniFile.FromObject(original);

                // now create copy object from ini
                var copy = new TestObject();
                IniFile.ToObject(ref copy, ini, null, IniFile.ParseObjectFlags.DefaultFalgs | IniFile.ParseObjectFlags.AllowMissingFields | IniFile.ParseObjectFlags.AllowAdditionalKeys);

                // make sure original and copy are the same
                Assert.AreEqual(original.Bar, copy.Bar);
                Assert.AreEqual(original.EnumVal, copy.EnumVal);
                Assert.AreEqual(original.Foo, copy.Foo);
                Assert.AreEqual(original.FooBar, copy.FooBar);
                Assert.AreEqual(original.Point.X, copy.Point.X);
                Assert.AreEqual(original.Point.Y, copy.Point.Y);
                Assert.AreEqual(original.Nested.EnumVal, copy.Nested.EnumVal);
                Assert.AreEqual(original.Nested.OtherField, copy.Nested.OtherField);
            }
        }

        #region classes for BasicTypes

        // test enum values.
        public enum MyEnum
        {
            Foo,
            Bar,
        }

        #endregion

        #region classes for CustomTypes

        // custom struct to test custom parsers
        public struct MyPoint
        {
            public int X;
            public int Y;
        }

        #endregion

        #region classes For IniToObject

        /// <summary>
        /// Test object for ini-to-object API with specific section.
        /// </summary>
        public class TestObjectMulti
        {
            public string Foo;
            public string Hello;
        }

        /// <summary>
        /// Test object for ini-to-object API.
        /// </summary>
        public class TestObject
        {
            public int Foo;
            public string Bar;
            public bool FooBar { get; set; }

            public string NotReadProp { get; private set; }
            protected string NotReadField;

            public bool BoolWithYes;

            public MyEnum EnumVal;
            public MyPoint Point;

            public NestedTestObject Nested;

            public int GetterOnly => 5;

            public int SetterOnly { set { } }
        }

        /// <summary>
        /// Test object for ini-to-object API - with dictionary as nested.
        /// </summary>
        public class TestObjectNestedDict
        {
            public int Foo;
            public string Bar;
            public bool FooBar { get; set; }
            public int SetterOnly { set { } }
            public bool BoolWithYes;

            public string NotReadProp { get; private set; }
            protected string NotReadField;

            public MyEnum EnumVal;
            public MyPoint Point;

            public Dictionary<string, string> Nested;
        }

        /// <summary>
        /// Nested object to parse from inside TestObject.
        /// </summary>
        public class NestedTestObject
        {
            public int ThisIsNested;
            public float OtherField;
            public MyEnum EnumVal;
            public MyPoint NestedPoint;
            public DeeperNestedTestObject Deeper;
        }

        /// <summary>
        /// Deeper nested object to parse from inside NestedTestObject.
        /// </summary>
        public class DeeperNestedTestObject
        {
            public string Value;
            public DeeperDeeperNestedTestObject EvenDeeper;
        }

        /// <summary>
        /// Deeper deeper nested object to parse from inside DeeperNestedTestObject.
        /// </summary>
        public class DeeperDeeperNestedTestObject
        {
            public string Value;
        }

        #endregion

        #region classes For IniToObjectExampleFromReadme

        /// <summary>
        /// Config for readme test
        /// </summary>
        class Config
        {
            public GraphicsConfig Graphics;
            public SoundConfig Sound;
        }
        class GraphicsConfig
        {
            public bool Fullscreen;
            public class ResolutionConfig
            {
                public int Width;
                public int Height;
            }
            public ResolutionConfig Resolution;
        }
        class SoundConfig
        {
            public int Volume;
        }

        #endregion
    }
}
