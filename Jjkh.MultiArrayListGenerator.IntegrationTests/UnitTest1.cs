using Jjkh.Generators;

namespace IntegrationTests
{
    [TestClass]
    public partial class UnitTest1
    {
        [MultiArrayList]
        private record struct GreetingRecord
        {
            public string Greeting;
            public bool Casual;
        }

        [TestMethod]
        public void TestMultiRecord()
        {
            GreetingRecordMultiArray xma = new([
                new GreetingRecord { Greeting = "hi", Casual = true },
                new GreetingRecord { Greeting = "greetings", Casual = false },
                new GreetingRecord { Greeting = "hello", Casual = true },
                new GreetingRecord { Greeting = "salutations", Casual = false },
                new GreetingRecord { Greeting = "howdy", Casual = true },
            ]);

            CollectionAssert.AreEqual(new[] { "hi", "greetings", "hello", "salutations", "howdy" }, xma.Greeting.ToArray());
            CollectionAssert.AreEqual(new[] { true, false, true, false, true }, xma.Casual.ToArray());
            Assert.AreEqual(new GreetingRecord { Greeting = "hello", Casual = true }, xma[2]);
        }

        [MultiArrayList]
        public class GreetingClass
        {
            public string? Greeting;
            public bool Casual;
        }

        [TestMethod]
        public void TestMultiClass()
        {
            GreetingClassMultiArray xma = new([
                new GreetingClass { Greeting = "hi", Casual = true },
                new GreetingClass { Greeting = "greetings", Casual = false },
                new GreetingClass { Greeting = "hello", Casual = true },
                new GreetingClass { Greeting = "salutations", Casual = false },
                new GreetingClass { Greeting = "howdy", Casual = true },
            ]);

            CollectionAssert.AreEqual(new[] { "hi", "greetings", "hello", "salutations", "howdy" }, xma.Greeting.ToArray());
            CollectionAssert.AreEqual(new[] { true, false, true, false, true }, xma.Casual.ToArray());
            var x2 = xma[2];
            Assert.AreEqual("hello", x2.Greeting);
            Assert.AreEqual(true, x2.Casual);
        }
    }
}