using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Xml;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Reflection;


namespace Webby
{
    class Program
    {

        static FileInfo TransformFileInfo(string TransformName)
        {


            DirectoryInfo myExeDir = (new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).Directory;


            
            string TransformFullName = System.IO.Path.Combine(myExeDir.FullName, "Transformations", TransformName + ".xslt");
            FileInfo result = new FileInfo(TransformFullName);
            Console.Out.WriteLine("Transform located at " + result.FullName);
            return result;

        }

        static void Fail()
        {
            //If Running from Visual Studio debugger, give a chance to read the console output first.
            if (Debugger.IsAttached)
            {
                Console.Out.WriteLine("Debugger attached. Press any key to exit.");
                Console.ReadKey();
            }
            Environment.Exit(1);
        }

        static void Success()
        {
            //If Running from Visual Studio debugger, give a chance to read the console output first.
            if (Debugger.IsAttached)
            {
                Console.Out.WriteLine("Debugger attached. Press any key to exit.");
                Console.ReadKey();
            }
            Environment.Exit(0);
        }


        static void Main(string[] args)
        {

            try
            {
                if (args.Length < 3)
                {
                    Console.Out.WriteLine(@"USAGE: Webby.exe C:\My\web.config C:\My\Web.config.transformed UpdateConnectionStrings.xslt");
                    Fail();
                }
                string SourceDocument = args[0];
                string OutputName = args[1];
                string TemplateDocument = args[2];
                


                //Check to see if the source and template documents exist
                FileInfo sDoc = new FileInfo(SourceDocument);
                FileInfo tDoc = TransformFileInfo(TemplateDocument);

                if (!sDoc.Exists)
                {
                    Console.Out.WriteLine("Source Document " + sDoc.FullName + " does not exist.");
                    Fail();
                }

                if (!tDoc.Exists)
                {
                    Console.Out.WriteLine("Template " + tDoc.FullName + " does not exist.");
                    Fail();
                }

                var myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(tDoc.FullName);


                XsltArgumentList xslArgs = new XsltArgumentList();
                for (int i = 3; i < args.Length; i++)
                {
                    String ArgName = args[i].Split(':')[0];
                    String ArgValue = args[i].Split(':')[1];
                    xslArgs.AddParam(ArgName, "", ArgValue);
                }


                // Create an XmlWriter to write the output.             
                XmlWriter writer = XmlWriter.Create(OutputName);
                myXslTrans.Transform(SourceDocument, xslArgs, writer);
                writer.Close();
                Success();
            }


                //Broad exception handling to make sure we never pass exceptions upstream to Jenkins
                // We will fail with System Exit Code 1 though!
            catch (Exception ex)
            {
                Console.Out.WriteLine("===FAILURE WITH XSL TRANSOFMRATION ===");
                Console.Out.WriteLine(ex.ToString());
                Fail();
            }
            


        }
    }
}
