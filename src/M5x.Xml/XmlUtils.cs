using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using M5x.Streams;
using Serilog;

namespace M5x.Xml
{
    public static class XmlUtils
    {
        public static string TransformXmltoHtml(string xml, string xslt, bool debug = false)
        {
            var transform = new XslCompiledTransform(debug);
            using (var reader = XmlReader.Create(new StringReader(xslt)))
            {
                transform.Load(reader);
            }

            var results = new StringWriter();
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                transform.Transform(reader, null, results);
            }

            return results.ToString();
        }


        public static Stream XsltTransform(string xml, string xslt)
        {
            try
            {
                // Load documents 
                var sOut = new MemoryStream();
                var xmlDocument = new XPathDocument(XmlReader.Create(xml.ToStream()));
                var xsltTx = new XslCompiledTransform();

                xsltTx.Load(XmlReader.Create(xslt.ToStream()));

                // Apply transformation and output results to console
                var outWriter = new StreamWriter(sOut) { AutoFlush = true };
                xsltTx.Transform(xmlDocument, null, outWriter);
                var s = outWriter.BaseStream;
                s.Position = 0;
                return s;
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        public static Stream XsltTransform(Stream xml, Stream xslt)
        {
            try
            {
                // Load documents 
                var sOut = new MemoryStream();
                var outSettings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true
                };
                var outWriter = new StreamWriter(sOut, Encoding.UTF8);
                var xmlDocument = new XPathDocument(XmlReader.Create(xml));
                var xsltTx = new XslCompiledTransform();
                var rdr = XmlReader.Create(xslt);
                while (rdr.Read())
                {
                    xsltTx.Load(rdr);
                    xsltTx.Transform(xmlDocument, null, outWriter);
                    //xsltTx.Transform(xmlDocument, null, sOut);
                }

                //var s = outWriter.;
                //s.Position = 0;
                //return s;
                outWriter.BaseStream.Position = 0;
                return outWriter.BaseStream;
//                sOut.Position = 0;
//                return sOut;
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        public static string XmlToHtml(string inputXml, string transformXsl, bool debug)
        {
            var xslTransform = new XslCompiledTransform(debug);
            using (var sr = new StringReader(transformXsl))
            {
                using (var xr = XmlReader.Create(sr))
                {
                    xslTransform.Load(xr);
                }
            }

            var result = string.Empty;
            using (var sr = new StringReader(inputXml))
            {
                using (var xr = XmlReader.Create(sr))
                {
                    while (xr.Read())
                        using (var sw = new StringWriter())
                        {
                            try
                            {
                                xslTransform.Transform(xr, null, sw);
                                result = sw.ToString();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                }
            }

            return result;
        }


        public static XmlReader ToXmlReader(this string sIn)
        {
            return XmlReader.Create(new StringReader(sIn));
        }


        //public static Stream XsltTransform(string xmlIn, string xsltIn)
        //{
        //    var styleSheet = xsltIn.ToXmlReader();
        //    styleSheet.Read();
        //    var sIn = xmlIn.ToStream();
        //    var outStream = new MemoryStream();
        //    var xPathDoc = new XPathDocument(sIn);
        //    var xslTrans = new XslCompiledTransform();
        //    xslTrans.Load(styleSheet);
        //    var writer = new XmlTextWriter(outStream, Encoding.UTF8);
        //    xslTrans.Transform(xPathDoc, null, writer);
        //    outStream.Seek(0, 0);
        //    return outStream;

        //}
    }
}