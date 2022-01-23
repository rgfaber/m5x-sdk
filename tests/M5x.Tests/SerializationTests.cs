using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using M5x.Streams;
using Xunit;


namespace M5x.Tests;

public static class SerializationTests
{
    public record MyData
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public int Height { get; set; } 
    }
    
    public class Json
    {
        [Fact]
        public void SerializeAsJson()
        {
            var d = new MyData
            {
                Birthday = DateTime.UtcNow, Height = 187, Name = "John"
            };
            File.WriteAllText("./my_data.json", JsonSerializer.Serialize(d));
        }

        [Fact]
        public void DeserializeWithAddedField()
        {
            const string s = "{ \"Name\":\"John\", \"Birthday\":\"2022-01-23T08:38:55.2501243Z\", \"Height\":187, \"Hair\":\"none\" }";
            File.WriteAllText("./my_added_data.json", s);
            var d = JsonSerializer.Deserialize<MyData>(File.ReadAllText("./my_added_data.json"));
        }
    }

    public class Xml
    {
        [Fact]
        public void SerializeAsXml()
        {
            var d = new MyData
            {
                Birthday = DateTime.UtcNow, Height = 187, Name = "John"
            };
            var s = new XmlSerializer(typeof(MyData));
            s.Serialize(new XmlTextWriter("./my_data.xml", Encoding.UTF8), d);
        }

        [Fact]
        public void DeserializeXmlWithAddedField()
        {
            var s = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<MyData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                    "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                    "<Name>John</Name>" +
                    "<Birthday>2022-01-23T09:08:37.558729Z</Birthday>" +
                    "<Height>187</Height>" +
                    "<Hair>none</Hair>" +
                    "</MyData>";
            var ser = new XmlSerializer(typeof(MyData));
            var d = ser.Deserialize(new XmlTextReader(s.ToStream()));
            Assert.IsType<MyData>(d);
        }
        
    }
    
    
    
    
    
    
    
    
    
}