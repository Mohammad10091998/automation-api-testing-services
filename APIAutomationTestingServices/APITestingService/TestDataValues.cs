
namespace APITestingService
{
    public static class TestDataValues
    {
        public static List<int> IntegerValues { get; } = new List<int> { -10000000, 0, 1000000 };
        public static List<long> LongValues { get; } = new List<long> { -100000000, 0, 100000000 };
        public static List<double> DoubleValues { get; } = new List<double> { -10000.720, 0.0, 10000.720 };
        public static List<float> FloatValues { get; } = new List<float> { -10000.11f, 0.0f, 10000.11f };
        public static List<decimal> DecimalValues { get; } = new List<decimal> { -10000.11m, 0.0m, 10000.11m };
        public static List<bool> BoolValues { get; } = new List<bool> { true, false };
        public static List<DateTime> DateTimeValues { get; } = new List<DateTime> { DateTime.Now.AddYears(-100), DateTime.Now.AddYears(100) };
        public static List<Guid> GuidValues { get; } = new List<Guid> { Guid.NewGuid(), Guid.Empty };
        public static List<char> CharValues { get; } = new List<char> { 'a', '1', '@' };
        public static List<string> StringValues { get; } = new List<string> { null, "", "abc123", "!@#$%^", "&*()_ " };
    }
}

