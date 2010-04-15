using System.Linq;
using System.Reflection;
using FubuCore.Binding;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BasicTypeConverterTester
    {
        private ValueConverterRegistry _registry;
        private BasicTypeConverter _basicTypeConverter;
        private PropertyInfo _property;
        private IBindingContext _context;
        private string _propertyValue;

        private class PropertyHolder{public string Property { get; set; }}

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0]);
            _property = typeof(PropertyHolder).GetProperty("Property");
            _basicTypeConverter = _registry.Families.SingleOrDefault(cf =>
                cf.Matches(_property)) as BasicTypeConverter;
            _basicTypeConverter.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IBindingContext>();
            _propertyValue = "some value";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(3);
        }

        [Test]
        public void should_match_property()
        {
            _basicTypeConverter.Matches(_property).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_on_exception()
        {
            _basicTypeConverter.Matches(null).ShouldBeFalse();
        }

        [Test]
        public void should_build()
        {
            ValueConverter converter = _basicTypeConverter.Build(_registry, _property);
            converter(_context).ShouldEqual(_propertyValue);
            _context.VerifyAllExpectations();
        }
    }
}