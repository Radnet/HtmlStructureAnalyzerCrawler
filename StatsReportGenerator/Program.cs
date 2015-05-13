using BDCExcelManager;
using NLog;
using OfficeOpenXml;
using SharedLibrary;
using SharedLibrary.SimpleHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsReportGenerator
{
    class Program
    {
        // MongoDB Helpers
        private static MongoDBWrapper mongoDB = new MongoDBWrapper();

        public static FlexibleOptions ProgramOptions { get; private set; }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        static void Main(string[] args)
        {
            // set error exit code
            System.Environment.ExitCode = -50;
            try
            {
                // load configurations
                ProgramOptions = ConsoleUtils.Initialize(args, true);

                // start execution
                Execute(ProgramOptions);

                // check before ending for waitForKeyBeforeExit option
                if (ProgramOptions.Get ("waitForKeyBeforeExit", false))
                    ConsoleUtils.WaitForAnyKey();
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Fatal(ex);

                // check before ending for waitForKeyBeforeExit option
                if (ProgramOptions.Get ("waitForKeyBeforeExit", false))
                    ConsoleUtils.WaitForAnyKey();

                ConsoleUtils.CloseApplication(-60, true);
            }
            // set success exit code
            ConsoleUtils.CloseApplication(0, false);
        }

        static Logger logger = LogManager.GetCurrentClassLogger();
        static DateTime started = DateTime.UtcNow;

        private static void Execute(FlexibleOptions options)
        {
            logger.Info ("Worker Started");

            Console.WriteLine ("Open MongoDB connection...");

            // Configuring MongoDB Wrapper
            //Consts consts = new Consts();
            string fullServerAddress = String.Join (":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase (Consts.MONGO_USER, Consts.MONGO_PSW, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_PROCESSED_URLS_COLLECTION);

            // Initialize excel object
            ExcelManager excel = new ExcelManager (new FileInfo (ConsoleUtils.ProgramOptions.Get ("REPORT_MODEL_PATH") ) );
        
            // Acessing WorkSheet itself
            var dataWorkSheet = excel.OpenWorksheet ("DATA");

            // Sanity Check
            if (dataWorkSheet == null)
            {
                // Problem to find workSheet
                logger.Info ("Erro ao abrir XLSX");
                return;
            }
            
            Console.WriteLine("Writing Name...");
            // Write name
            dataWorkSheet.Write (3,  2, ConsoleUtils.ProgramOptions.Get("MONGO_DATABASE") + " in Numbers");

            // Writing Date
            dataWorkSheet.Write (4, 2, DateTime.Now.ToString("dd MM yyyy"));

            Console.WriteLine("Writing Pages Count...");
            // Write total of pages
            dataWorkSheet.Write (6,  2,  mongoDB.CountStatsCollection () );

            Console.WriteLine("Writing Links Count...");
            //Write internal and external links
            dataWorkSheet.Write (10, 3,  mongoDB.CountInternalLinks   () );
            dataWorkSheet.Write (11, 3,  mongoDB.CountExternalLinks   () );

            Console.WriteLine("Writing TEXT & STYLERS...");
            // Write Tags Count for TEXT & STYLERS
            dataWorkSheet.Write (29, 3,  mongoDB.CountTag ("title"  )    );
            dataWorkSheet.Write (30, 3,  mongoDB.CountTag ("header" )    );
            dataWorkSheet.Write (31, 3,  mongoDB.CountTag ("h1"     )    );
            dataWorkSheet.Write (32, 3,  mongoDB.CountTag ("h2"     )    );
            dataWorkSheet.Write (33, 3,  mongoDB.CountTag ("h3"     )    );
            dataWorkSheet.Write (34, 3,  mongoDB.CountTag ("h4"     )    );
            dataWorkSheet.Write (35, 3,  mongoDB.CountTag ("h5"     )    );
            dataWorkSheet.Write (36, 3,  mongoDB.CountTag ("h6"     )    );
            dataWorkSheet.Write (37, 3,  mongoDB.CountTag ("p"      )    );
            dataWorkSheet.Write (38, 3,  mongoDB.CountTag ("br"     )    );
            dataWorkSheet.Write (39, 3,  mongoDB.CountTag ("footer" )    );
            dataWorkSheet.Write (40, 3,  mongoDB.CountTag ("article")    );
            dataWorkSheet.Write (41, 3,  mongoDB.CountTag ("aside"  )    );
            dataWorkSheet.Write (42, 3,  mongoDB.CountTag ("font"   )    );
            dataWorkSheet.Write (43, 3,  mongoDB.CountTag ("em"     )    );
            dataWorkSheet.Write (44, 3,  mongoDB.CountTag ("strong" )    );
            dataWorkSheet.Write (45, 3,  mongoDB.CountTag ("i"      )    );
            dataWorkSheet.Write (46, 3,  mongoDB.CountTag ("u"      )    );
            dataWorkSheet.Write (47, 3,  mongoDB.CountTag ("ins"    )    );
            dataWorkSheet.Write (48, 3,  mongoDB.CountTag ("del"    )    );
            dataWorkSheet.Write (49, 3,  mongoDB.CountTag ("mark"   )    );
            dataWorkSheet.Write (50, 3,  mongoDB.CountTag ("dfn"    )    );
            dataWorkSheet.Write (51, 3,  mongoDB.CountTag ("code"   )    );
            dataWorkSheet.Write (52, 3,  mongoDB.CountTag ("samp"   )    );
            dataWorkSheet.Write (53, 3,  mongoDB.CountTag ("kbd"    )    );
            dataWorkSheet.Write (54, 3,  mongoDB.CountTag ("var"    )    );
            dataWorkSheet.Write (55, 3,  mongoDB.CountTag ("pre"    )    );
            dataWorkSheet.Write (56, 3,  mongoDB.CountTag ("q"      )    );
            dataWorkSheet.Write (57, 3,  mongoDB.CountTag ("small"  )    );
            dataWorkSheet.Write (58, 3,  mongoDB.CountTag ("sub"    )    );
            dataWorkSheet.Write (59, 3,  mongoDB.CountTag ("sup"    )    );
            dataWorkSheet.Write (60, 3,  mongoDB.CountTag ("summary")    );
            dataWorkSheet.Write (61, 3,  mongoDB.CountTag ("time"   )    );

            Console.WriteLine("Writing INTERACTION...");                                              
            // Write Tags Count for INTERACTION           
            dataWorkSheet.Write (29, 5,  mongoDB.CountTag ("audio")      );
            dataWorkSheet.Write (30, 5,  mongoDB.CountTag ("track")      );
            dataWorkSheet.Write (31, 5,  mongoDB.CountTag ("button")     );
            dataWorkSheet.Write (32, 5,  mongoDB.CountTag ("input")      );
            dataWorkSheet.Write (33, 5,  mongoDB.CountTag ("datalist")   );
            dataWorkSheet.Write (34, 5,  mongoDB.CountTag ("dl")         );
            dataWorkSheet.Write (35, 5,  mongoDB.CountTag ("dt")         );
            dataWorkSheet.Write (36, 5,  mongoDB.CountTag ("dd")         );
            dataWorkSheet.Write (37, 5,  mongoDB.CountTag ("option")     );
            dataWorkSheet.Write (38, 5,  mongoDB.CountTag ("keygen")     );
            dataWorkSheet.Write (39, 5,  mongoDB.CountTag ("output")     );

            Console.WriteLine("Writing VISUAL...");
            // Write Tags Count for VISUAL                
            dataWorkSheet.Write (29, 7,  mongoDB.CountTag ("img")        );
            dataWorkSheet.Write (30, 7,  mongoDB.CountTag ("figure")     );
            dataWorkSheet.Write (31, 7,  mongoDB.CountTag ("figcaption") );
            dataWorkSheet.Write (32, 7,  mongoDB.CountTag ("canvas")     );
            dataWorkSheet.Write (33, 7,  mongoDB.CountTag ("map")        );
            dataWorkSheet.Write (34, 7,  mongoDB.CountTag ("meter")      );
            dataWorkSheet.Write (35, 7,  mongoDB.CountTag ("progress")   );
            dataWorkSheet.Write (36, 7,  mongoDB.CountTag ("source")     );

            Console.WriteLine("Writing FORM...");                                              
            // Write Tags Count for FORM                  
            dataWorkSheet.Write (29, 9,  mongoDB.CountTag ("form")       );
            dataWorkSheet.Write (30, 9,  mongoDB.CountTag ("fieldset")   );
            dataWorkSheet.Write (31, 9,  mongoDB.CountTag ("legend")     );
            dataWorkSheet.Write (32, 9,  mongoDB.CountTag ("li")         );
            dataWorkSheet.Write (33, 9,  mongoDB.CountTag ("ol")         );
            dataWorkSheet.Write (34, 9,  mongoDB.CountTag ("ul")         );
            dataWorkSheet.Write (35, 9,  mongoDB.CountTag ("menu")       );
            dataWorkSheet.Write (36, 9,  mongoDB.CountTag ("menuitem")   );

            Console.WriteLine("Writing TABLE...");                                  
            // Write Tags Count for TABLE                 
            dataWorkSheet.Write (29, 11, mongoDB.CountTag ("table")      );
            dataWorkSheet.Write (30, 11, mongoDB.CountTag ("td")         );
            dataWorkSheet.Write (31, 11, mongoDB.CountTag ("th")         );
            dataWorkSheet.Write (32, 11, mongoDB.CountTag ("tfoot")      );
            dataWorkSheet.Write (33, 11, mongoDB.CountTag ("col")        );
            dataWorkSheet.Write (34, 11, mongoDB.CountTag ("colgroup")   );

            Console.WriteLine("Writing ORGANIZERS...");  
            // Write Tags Count for PAGE ORGANIZERS       
            dataWorkSheet.Write (29, 13, mongoDB.CountTag ("head")       );
            dataWorkSheet.Write (30, 13, mongoDB.CountTag ("div")        );
            dataWorkSheet.Write (31, 13, mongoDB.CountTag ("frameset")   );
            dataWorkSheet.Write (32, 13, mongoDB.CountTag ("frame")      );
            dataWorkSheet.Write (33, 13, mongoDB.CountTag ("link")       );
            dataWorkSheet.Write (34, 13, mongoDB.CountTag ("nav")        );

            Console.WriteLine("Writing LINKS...");                                              
            // Write Tags Count for LINK                  
            dataWorkSheet.Write (29, 15, mongoDB.CountTag ("a")          );

            Console.WriteLine("Writing ADDRESS...");
            // Write Tags Count for ADDRESS
            dataWorkSheet.Write (29, 17, mongoDB.CountTag ("address")    );

            Console.WriteLine("Writing EXTERNAL APP...");                                             
            // Write Tags Count for EXTERNAL APLICATION  
            dataWorkSheet.Write (29, 19, mongoDB.CountTag ("embed")      );

            Console.WriteLine("Writing Seting FORMULAS for DATA...");

            // Get Epplus WorkSheet
            ExcelWorksheet ws = dataWorkSheet.EPPlusSheet;

            // Set formulas for DATA WorkSheet
            // TOTAL for TAGS types
            ws.Cells["S30"].Formula = "=SUM(S29)";
            ws.Cells["Q30"].Formula = "=SUM(Q29)";
            ws.Cells["O30"].Formula = "=SUM(O29)";
            ws.Cells["M35"].Formula = "=SUM(M29:M34)";
            ws.Cells["K35"].Formula = "=SUM(K29:K34)";
            ws.Cells["I37"].Formula = "=SUM(I29:I36)";
            ws.Cells["G37"].Formula = "=SUM(G29:G36)";
            ws.Cells["E40"].Formula = "=SUM(E29:E39)";
            ws.Cells["C62"].Formula = "=SUM(C29:C61)";

            // TAGS types RESUME COUNTS
            ws.Cells["C16"].Formula = "=C62";
            ws.Cells["C17"].Formula = "=E40";
            ws.Cells["C18"].Formula = "=G37";
            ws.Cells["C19"].Formula = "=I37";
            ws.Cells["C20"].Formula = "=K35";
            ws.Cells["C21"].Formula = "=M35";
            ws.Cells["C22"].Formula = "=O30";
            ws.Cells["C23"].Formula = "=Q30";
            ws.Cells["C24"].Formula = "=S30";

            // TAGS types GRAN TOTAL
            ws.Cells["C25"].Formula = "=SUM(C16:C24)";

            // TAGS types RESUME PERCENTAGE
            ws.Cells["D16"].Formula = "=C16/C25";
            ws.Cells["D17"].Formula = "=C17/C25";
            ws.Cells["D18"].Formula = "=C18/C25";
            ws.Cells["D19"].Formula = "=C19/C25";
            ws.Cells["D20"].Formula = "=C20/C25";
            ws.Cells["D21"].Formula = "=C21/C25";
            ws.Cells["D22"].Formula = "=C22/C25";
            ws.Cells["D23"].Formula = "=C23/C25";
            ws.Cells["D24"].Formula = "=C24/C25";

            // Internal and External LINKS GRAN TOTAL
            ws.Cells["C12"].Formula = "=SUM(C10:C11)";

            // Internal and External LINKS PERCENTAGE
            ws.Cells["D10"].Formula = "=C10/C12";
            ws.Cells["D11"].Formula = "=C11/C12";

            // Set formulas for SUMMARY WorkSheet
            // Acessing WorkSheet itself
            dataWorkSheet = excel.OpenWorksheet("SUMMARY");

            // Sanity Check
            if (dataWorkSheet == null)
            {
                // Problem to find workSheet
                logger.Info("Erro ao abrir SUMMARY WorkSheet");
                return;
            }

            // Get Epplus WorkSheet
            ws = dataWorkSheet.EPPlusSheet;

            dataWorkSheet.Write(3, 3, "Data Mined from " + ConsoleUtils.ProgramOptions.Get("MONGO_DATABASE") + " domain");
            
            ws.Cells["A1"].Formula  = "=DATA!B4";
            ws.Cells["C4"].Formula  = "=DATA!B3";
            ws.Cells["A6"].Formula  = "=DATA!B6";
            ws.Cells["A12"].Formula = "=DATA!C12";
            ws.Cells["A18"].Formula = "=DATA!C12/A6";

            // File props
            string fileName    = "Report_" + ConsoleUtils.ProgramOptions.Get("MONGO_DATABASE") + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            string outFilePath = ConsoleUtils.ProgramOptions.Get("OUT_REPORT_DIR");

            Console.WriteLine("Saving file...");
            // Save File
            excel.SaveAs (new FileInfo (Path.Combine (outFilePath, fileName) ) );

            logger.Info ("End");
        }
    }
}
