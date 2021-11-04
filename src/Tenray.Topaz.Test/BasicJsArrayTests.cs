﻿using NUnit.Framework;
using System;
using System.Collections;
using System.Text.Json;
using Tenray.Topaz.API;
using Tenray.Topaz.Utility;

namespace Tenray.Topaz.Test
{
    public class BasicJsArrayTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArray1(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = []
var b = [4,5,6]
a[0] = 'abc'
a[3] = 3
model.js = a
model.x = a[55]
model.y = a.at(3)
model.z = a.concat(b)
model.p = model.z.concat()
model.p.push(7,8,9,10)
model.p.push(11)
model.q = model.p.pop()
model.p.push(12,13)
model.r = model.p.shift()
");
            var js = model.js;
            var json = JsonSerializer.Serialize<JsArray>(js);
            Assert.IsTrue(json.StartsWith("["));
            Console.WriteLine(json);
            dynamic deserialized;
            if (useThreadSafeJsObjects)
            {
                deserialized = JsonSerializer.Deserialize<JsArray>(json);
                Assert.AreEqual(js, deserialized);
            }
            else
            {
                deserialized = JsonSerializer.Deserialize<JsArray>(json);
                Assert.AreEqual(js, deserialized);
            }

            Assert.AreEqual("abc", js[0]);
            Assert.AreEqual(null, js[1]);
            Assert.AreEqual(null, js[2]);
            Assert.AreEqual(3, js[3]);
            Assert.AreEqual(Undefined.Value, js[4]);
            Assert.AreEqual(null, model.x);
            Assert.AreEqual(3, model.y);
            Assert.AreEqual("[\"abc\",null,null,3,4,5,6]", 
                JsonSerializer.Serialize<JsArray>(model.z));
            Assert.AreEqual("[null,null,3,4,5,6,7,8,9,10,12,13]",
                JsonSerializer.Serialize<JsArray>(model.p));
            Assert.AreEqual(11, model.q);
            Assert.AreEqual("abc", model.r);

            engine.ExecuteScript(@"
model.u = model.p.length
model.p.length = 9
model.p.shift()
model.p.shift()
");
            Assert.AreEqual(12, model.u);
            Assert.AreEqual("[3,4,5,6,7,8,9]",
                JsonSerializer.Serialize<JsArray>(model.p));

            engine.ExecuteScript(@"
model.p.reverse()
");
            Assert.AreEqual("[9,8,7,6,5,4,3]",
                JsonSerializer.Serialize<JsArray>(model.p));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArray2(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = [{nested: 4}, [9,8,[7,6]], 4]
model.z = a.concat([1,2,3], [4,5,6])
");
            var json = JsonSerializer.Serialize<JsArray>(model.z);
            Console.WriteLine(json);
            var deserialized = JsonSerializer.Deserialize<JsArray>(json);
            Assert.AreEqual(typeof(JsObject), deserialized[0].GetType());
            Assert.AreEqual(typeof(JsArray), deserialized[1].GetType());
            Assert.AreEqual(typeof(JsArray), deserialized[1][2].GetType());
            Assert.AreEqual(model.z, deserialized);
            Assert.AreEqual("[{\"nested\":4},[9,8,[7,6]],4,1,2,3,4,5,6]",
                json);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArrayIndexOf(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = [1,2,3,4,5,2]
model.b = a.indexOf(3)
model.c = a.indexOf(3,3)
model.d = a.indexOf(2)
model.e = a.indexOf(2, 2)
model.f = a.indexOf(2, -2)
");
            Assert.AreEqual(2, model.b);
            Assert.AreEqual(-1, model.c);
            Assert.AreEqual(1, model.d);
            Assert.AreEqual(5, model.e);
            Assert.AreEqual(5, model.f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArrayLastIndexOf(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = [1,2,3,4,5,2]
model.b = a.lastIndexOf(3)
model.c = a.lastIndexOf(3,3)
model.d = a.lastIndexOf(2)
model.e = a.lastIndexOf(2, 2)
model.f = a.lastIndexOf(2, -2)
");
            Assert.AreEqual(2, model.b);
            Assert.AreEqual(2, model.c);
            Assert.AreEqual(5, model.d);
            Assert.AreEqual(1, model.e);
            Assert.AreEqual(1, model.f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArrayIncludes(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = [1,2,3,4,5,2]
model.b = a.includes(3)
model.c = a.includes(3,3)
model.d = a.includes(2)
model.e = a.includes(2, 2)
model.f = a.includes(2, -2)
");
            Assert.IsTrue(model.b);
            Assert.IsFalse(model.c);
            Assert.IsTrue(model.d);
            Assert.IsTrue(model.e);
            Assert.IsTrue(model.f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArrayUnshift(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = [3]
a.unshift(2)
a.unshift(1)
a.unshift(6,5,4)
model.a = a
model.b = a.toString()
");
            Assert.AreEqual("[6,5,4,1,2,3]",
                JsonSerializer.Serialize<JsArray>(model.a));
            Assert.AreEqual("6,5,4,1,2,3", model.b);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArrayValuesIterator(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
const array1 = ['a', 'b', 'c']
const iterator = array1.values()
let a = []
for (const value of iterator) {
  a.push(value)
}
model.a = a
");
            Assert.AreEqual("[\"a\",\"b\",\"c\"]",
                JsonSerializer.Serialize<JsArray>(model.a));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestJsArrayValuesSort(bool useThreadSafeJsObjects)
        {
            var engine = new TopazEngine();
            engine.Options.NoUndefined = true;
            engine.Options.UseThreadSafeJsObjects = useThreadSafeJsObjects;
            dynamic model = new CaseSensitiveDynamicObject();
            engine.SetValue("model", model);
            engine.ExecuteScript(@"
var a = [4, 2, 5, 1, 3];
a.sort(function(a, b) {
  return a - b;
});
model.a = a
");
            Assert.AreEqual("[1,2,3,4,5]",
                JsonSerializer.Serialize<JsArray>(model.a));
        }
    }
}