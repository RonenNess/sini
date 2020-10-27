using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sini.UnitTest
{
    [TestClass]
    public class SiniUnitTest
    {
        // test enum values.
        enum MyEnum
        {
            Foo,
            Bar,
        }

        [TestMethod]
        public void BasicTypes()
        {
            // open ini file
            var ini = new Sini.IniFile("ok_file.ini");

            // get global var
            Assert.AreEqual(ini.GetStr(null, "global_val"), "foo");
            Assert.AreEqual(ini.GetStr(string.Empty, "global_val"), "foo");

            // test basic values from section
            Assert.AreEqual("val1",                     ini.GetStr("section1", "str_val"));
            Assert.AreEqual(123,                        ini.GetInt("section1", "int_val"));
            Assert.AreEqual(12345678901234,             ini.GetLong("section1", "long_val"));
            Assert.AreEqual(12345678901234567890,       ini.GetULong("section1", "ulong_val"));
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
        public void DefaultValues()
        {
            // open ini file
            var ini = new Sini.IniFile("ok_file.ini");

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
        public void WrongFormats()
        {
            // open ini file
            var ini = new Sini.IniFile("ok_file.ini");
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
        public void CustomTypes()
        {
            // register cusom type parser
            IniFile.DefaultConfig.CustomParsers[typeof(MyPoint)] = (string val) => 
            {
                var parts = val.Split(',');
                return new MyPoint() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
            };

            // open ini file
            var ini = new Sini.IniFile("ok_file.ini");

            // read point 
            var mypoint = ini.GetCustomType("section1", "point_val", new MyPoint());
            Assert.AreEqual(5, mypoint.X);
            Assert.AreEqual(-7, mypoint.Y);
        }

        // custom struct to test custom parsers
        public struct MyPoint
        {
            public int X;
            public int Y;
        }
    }
}
