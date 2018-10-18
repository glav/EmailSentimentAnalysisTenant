using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class TestDataHelper
    {
        public string GetFileDataEmbeddedInAssembly(string filename)
        {
            var asm = this.GetType().GetTypeInfo().Assembly;
            using (var stream = asm.GetManifestResourceStream($"EmailSentimentAnalysis.Tests.Unit.TestData.{filename}"))
            {
                using (var sr = new System.IO.StreamReader(stream))
                {
                    var testData = sr.ReadToEnd();
                    return testData;
                }
            }
        }
    }
}
