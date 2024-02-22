﻿namespace ModelsLibrary
{
    public class APITestingModel
    {
        public string APIUrl { get; set; }
        public string MethodType { get; set; }
        public string JsonSchema { get; set; }
        public List<KeyValue> Headers { get; set; }
        public APITestingModel()
        {
            Headers = new List<KeyValue>();
        }
    }
}
