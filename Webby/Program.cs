using System;
using System.Linq;
using System.IO;
using System.Xml.Xsl;
using System.Xml;
using System.Diagnostics;



namespace Webby
{
    class Program
    {

        /// <summary>
        /// Returns a FileInfo object, using the directory where the exe file is executing from as a base path.
        /// </summary>
        /// <param name="transformName">the name of the XSLT file</param>
        /// <returns></returns>
        private static FileInfo TransformFileInfo(string transformName)
        {
            DirectoryInfo myExeDir = (new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).Directory;
            string transformFullName = System.IO.Path.Combine(myExeDir.FullName, "Transformations", transformName + ".xslt");
            FileInfo result = new FileInfo(transformFullName);
            Console.Out.WriteLine("Transform located at " + result.FullName);
            return result;
        }

        public static void ExceptionHandler(Exception ex)
        {
            Console.Out.WriteLine(" ==== AN ERROR OCCURRED ====");

            Console.Out.WriteLine(ex.ToString());
            Exception innerException = ex.InnerException;

            //Iterate through remaining inner exceptions
            while (innerException != null)
            {
                Console.Out.WriteLine(innerException.ToString());
                innerException = innerException.InnerException;
            }

            //Return a failure code
            Fail();
        }

        //Return a failure exit status code so automated build systems such as Jenkins can receive this as an error condition
        private static void Fail()
        {


            //If Running from Visual Studio debugger, give a chance to read the console output first.
            if (Debugger.IsAttached)
            {
                Console.Out.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            Environment.Exit(1);
        }


        //Successfully exit.
        private static void Success()
        {
            //If Running from Visual Studio debugger, give a chance to read the console output first.
            if (Debugger.IsAttached)
            {
                Console.Out.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            Environment.Exit(0);
        }


        //Main Method
        static void Main(string[] args)
        {

            const int SOURCE_DOC_ARGUMENT = 0; //Args[0] = SourceDocument (Web.Config)
            const int OUTPUT_DOC_ARGUMENT = 1; //Args[1] = Filename for output document (Web.Config.Transformed)
            const int TEMPLATE_DOC_ARGUMENT = 2;//Args[2] = Filename of xslt, no path, no extension (UpdateConnectionStrings)
            const int XSLVARS_ARUGMENTS = 3; //Args[3+] = XSL Variables in the form VarName:VarData. (All optional)

            
            try
            {
                //Check to make sure usage has the minimum number of arguments.
                if (args.Length < XSLVARS_ARUGMENTS)
                {
                    Console.Out.WriteLine(@"USAGE: Webby.exe C:\My\web.config C:\My\Web.config.transformed UpdateConnectionStrings");
                    Fail();
                }


                string sourceDocument = args[SOURCE_DOC_ARGUMENT];
                string outputName = args[OUTPUT_DOC_ARGUMENT];
                string templateDocument = args[TEMPLATE_DOC_ARGUMENT];
                


                //Check to see if the source and template documents exist
                FileInfo sDoc = new FileInfo(sourceDocument);
                FileInfo tDoc = TransformFileInfo(templateDocument);

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


                //Begin our XSL Transormations
                var myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(tDoc.FullName);


                XsltArgumentList xslArgs = new XsltArgumentList();
                for (int i = XSLVARS_ARUGMENTS; i < args.Length; i++)
                {
                    String argName = args[i].Split(':')[0];
                    String argValue = args[i].Split(':')[1];
                    xslArgs.AddParam(argName, "", argValue);
                }


                
                // Create an XmlWriter to write the output.             
                XmlWriter writer = XmlWriter.Create(outputName);

                //Perform the transformation
                myXslTrans.Transform(sourceDocument, xslArgs, writer);
                writer.Close();
                Success();
                
            }


                //Broad exception handling to make sure we never pass exceptions upstream to Jenkins 
                // We will fail with System Exit Code 1 though!
            catch (Exception ex)
            {
                //Pass the exception to our exception handler.
                ExceptionHandler(ex);
            }
            


        }
    }
}
