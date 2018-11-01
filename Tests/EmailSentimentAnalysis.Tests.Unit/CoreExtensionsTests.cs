using Core;
using Core.Extensions;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class CoreExtensionsTests
    {
        private CoreDependencyInstances _coreDependencies;
        public CoreExtensionsTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }
        [Fact]
        public void IntConversionExtensionShouldSafelyConvertAllValuesAndNotThrow()
        {
            Assert.Equal(1, "1".ToInt());
            Assert.Equal(99, "99".ToInt());
            Assert.Equal(0, "you_smell".ToInt());
            Assert.Equal(0, "".ToInt());
            Assert.Equal(0, "  ".ToInt());
            Assert.Equal(-1, "you_smell_more".ToInt(-1));
        }

        [Fact]
        public void BoolConversionExtensionShouldSafelyConvertAllValuesAndNotThrow()
        {
            Assert.True("true".ToBool());
            Assert.True("1".ToBool());
            Assert.True("yes".ToBool());
            Assert.True("TrUe".ToBool());
            Assert.True("YeS".ToBool());
            Assert.False ("0".ToBool());
            Assert.False("whatever".ToBool());
            Assert.False("".ToBool());
            Assert.False(" ".ToBool());
            Assert.True("make me true".ToBool(true));





        }


    }

}
