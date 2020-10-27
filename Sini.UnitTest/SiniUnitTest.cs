using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sini.UnitTest
{
    [TestClass]
    public class SiniUnitTest
    {
        [TestMethod]
        public void BasicTypes()
        {
            // open ini file
            var ini = new Sini.IniFile("ok_file.ini");

            // get global var
            Assert.AreEqual(ini.GetStr(null, "global_val"), "foo");
            Assert.AreEqual(ini.GetStr(string.Empty, "global_val"), "foo");

            // test basic values from section
            Assert.AreEqual(ini.GetStr("section1", "str_val"), "val1");
            Assert.AreEqual(ini.GetInt("section1", "int_val"), 123);
            Assert.AreEqual(ini.GetLong("section1", "long_val"), 12345678901234);
            Assert.AreEqual(ini.GetULong("section1", "ulong_val"), 12345678901234567890);
            Assert.AreEqual(ini.GetFloat("section1", "float_val"), 1.2f);
            Assert.AreEqual(ini.GetDouble("section1", "double_val"), 4.51524141252152111);
            Assert.AreEqual(ini.GetInt("section1", "negative_int_val"), -123);
            Assert.AreEqual(ini.GetLong("section1", "negative_long_val"), -12345678901234);
            Assert.AreEqual(ini.GetFloat("section1", "negative_float_val"), -1.2f);
            Assert.AreEqual(ini.GetDouble("section1", "negative_double_val"), -4.51524141252152111);
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

            // getting from another section + comment in line
            Assert.AreEqual(ini.GetStr("section2", "hello"), "world");
        }


        [TestMethod]
        public void DefaultValues()
        {
            // open ini file
            var ini = new Sini.IniFile("ok_file.ini");

            // test basic values from section
            foreach (var section in new string[] { "section1", "", "blaaa" })
            {
                Assert.AreEqual(ini.GetStr(section, "bla_str_val", "default_val"), "default_val");
                Assert.AreEqual(ini.GetInt(section, "bla_int_val", 10), 10);
                Assert.AreEqual(ini.GetLong(section, "bla_long_val", 100), (long)100);
                Assert.AreEqual(ini.GetULong(section, "bla_ulong_val", 123), (ulong)123);
                Assert.AreEqual(ini.GetFloat(section, "bla_float_val", 54.3f), 54.3f);
                Assert.AreEqual(ini.GetDouble(section, "bla_double_val", 754.3), 754.3);
                Assert.AreEqual(ini.GetInt(section, "bla_negative_int_val", -100), -100);
                Assert.AreEqual(ini.GetLong(section, "bla_negative_long_val", -53), (long)-53);
                Assert.AreEqual(ini.GetFloat(section, "bla_negative_float_val", -64.4f), -64.4f);
                Assert.AreEqual(ini.GetDouble(section, "bla_negative_double_val", -444214.13), -444214.13);
                Assert.IsTrue(ini.GetBool(section, "bla_bool_val_pos1", true));
                Assert.IsFalse(ini.GetBool(section, "bla_bool_val_neg1", false));
                Assert.AreEqual(ini.GetStr(section, "bla_key.with.dot"), null);
                Assert.AreEqual(ini.GetStr(section, "bla_key.with.dot", "default_val"), "default_val");
            }

        }
    }
}
