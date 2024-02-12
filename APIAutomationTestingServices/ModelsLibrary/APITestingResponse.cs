namespace ModelsLibrary
{
    public class APITestingResponse
    {
        public int TotalTestedObjects { get; set; }
        public int SuccessCalls { get; set; }
        public int FailureCalls { get; set; }
        public List<TestobjectInfo> TestedObjectInfos { get; set; }
    }
}
