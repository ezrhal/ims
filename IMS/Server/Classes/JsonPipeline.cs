using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class JsonPipeline
{
    public static List<BsonDocument> ConvertJsonToPipeline(string jsonString)
    {
        var pipeline = new List<BsonDocument>();

        // Parse the JSON string into a JObject
        var json = JObject.Parse(jsonString);

        foreach (var property in json.Properties())
        {
            var stageName = property.Name;
            var stageValue = property.Value as JObject;

            if (stageValue != null)
            {
                var stage = new BsonDocument();
                foreach (var subProperty in stageValue.Properties())
                {
                    var operatorName = subProperty.Name;
                    var operatorValue = subProperty.Value;
                    stage[operatorName] = BsonValue.Create(operatorValue.ToObject<object>());
                }

                var stageDocument = new BsonDocument
                {
                    { stageName, stage }
                };

                pipeline.Add(stageDocument);
            }
            else
            {
                Console.WriteLine($"Skipping invalid stage: {property}");
            }
        }

        return pipeline;
    }
}