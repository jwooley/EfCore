using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Recipe.Xunit
{
    public class XUnitConsoleLogConverter : TextWriter
    {
        readonly ITestOutputHelper _output;
        public XUnitConsoleLogConverter(ITestOutputHelper output)
        {
            _output = output;
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
        public override void WriteLine(string message)
        {
            try
            {
                _output.WriteLine(message);
            }
            catch { }
        }
        public override void WriteLine(string format, params object[] args)
        {
            try
            {
                _output.WriteLine(format, args);
            }
            catch { }
        }
    }
}
