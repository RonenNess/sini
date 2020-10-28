"""
A quick utility to generate all the specific GetXXX methods code.
"""

base_code = """
        /// <summary>
        /// Get value as __type_lower__.
        /// </summary>
        /// <param name="section">Section to get from, or null for global section.</param>
        /// <param name="key">Key to get value for.</param>
        /// <param name="defaultValue">Value to return if key not found.</param>
        /// <returns>Value as __type_lower__ if found, or defaultValue if not set. May raise exception on invalid format.</returns>
        public __type_lower__ Get__type_title__(string section, string key, __type_lower__ defaultValue = 0)
        {
            return GetPrimitive<__type_lower__>(section, key, defaultValue);
        }"""

for i in "char byte short ushort int uint long ulong float double".split(' '):
    type = i
    typeTitle = i.title()
    if typeTitle.startswith('U'):
        typeTitle = 'U' + typeTitle[1:].title()
    print (base_code.replace('__type_lower__', type).replace('__type_title__', typeTitle))