
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


using System.Collections.ObjectModel;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;




namespace TFSclient
{


    public class TfsApplication
    {


        public static string FaultTolerantRelativePath(string absolutePath, string basePath)
        {
            if(absolutePath == null || basePath == null)
                return null;

            absolutePath = absolutePath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            basePath = basePath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            
            if (!basePath.EndsWith("/"))
                basePath += "/";

            if (!absolutePath.EndsWith("/"))
                absolutePath += "/";

            if (absolutePath.Length < basePath.Length)
                throw new ArgumentException("absolutePath.Length < basePath.Length ? This can't be. You mixed up absolute and base path.");

            string resultingPath = absolutePath.Substring(basePath.Length);
            resultingPath = resultingPath.Replace('/', System.IO.Path.DirectorySeparatorChar);

            return resultingPath;
        } // End Function FaultTolerantRelativePath


        public static string BuildRelativePath(string absolutePath, string basePath)
        {
            // string relativePath = String.Empty; string[] basePaths = basePath.Split(System.IO.Path.DirectorySeparatorChar);
            // string[] absolutePaths = absolutePath.Split(System.IO.Path.DirectorySeparatorChar); 
            // return [...]
            Uri fullPath = new Uri(absolutePath, UriKind.Absolute);
            Uri relRoot = new Uri(basePath, UriKind.Absolute);

            string relPath = relRoot.MakeRelativeUri(fullPath).ToString();
            // relPath == @"MoreSubFolder\LastFolder\SomeFile.txt"
            return relPath;
        } // End Function BuildRelativePath



        
        // http://stackoverflow.com/questions/1827651/how-do-you-get-the-latest-version-of-source-code-using-the-team-foundation-serve
        public static void ListChangesInProject()
        {
            // Connect to the team project collection and the server that hosts the version-control repository. 
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(
                new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion")
            );


            //var tfsUrl = "http://myTfsServer:8080/tfs/defaultcollection";
            string tfsUrl = "http://corfoundation:8080/tfs/COR-DEV-Produktion";
            /////////var sourceControlRootPath = "$/MyTeamProject";
            TfsTeamProjectCollection tfsConnection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsUrl));
            VersionControlServer vcs = tfsConnection.GetService<VersionControlServer>();

            // vcs.QueryHistory("path", VersionSpec.Latest, )

            // vcs: versionControlService
            // var changeSets = vcs.QueryHistory(sourceControlRootPath, RecursionType.Full);

            //string path = "http://corfoundation:8080/tfs/web/UI/Pages/Scc/History.aspx?path=%24%2FCOR-Basic%2FCOR-Basic%2FBasic";
            string path = "$/COR-Basic/COR-Basic/Basic";
            bool boolIncludeChanges = true;
            bool boolIncludeDLInfo = true;


            // https://stackoverflow.com/questions/24491196/how-to-get-history-of-checkins-changsets-for-specific-team-project
            // System.Collections.IEnumerable changeSets = vcs.QueryHistory(path, RecursionType.Full); // V12


            // VersionSpec.ParseSingleSpec("C100", null), // starting from changeset 100
            // VersionSpec.ParseSingleSpec("C200", null), // ending with changeset 200

            // V10:
            // https://stackoverflow.com/questions/26641638/queryhistory-for-a-range-of-changeset-in-tfs-for-specific-info
            System.Collections.IEnumerable changeSets = vcs.QueryHistory(
                path,
                VersionSpec.Latest,
                0,
                RecursionType.Full, //look into all subfolders
                null, // User
                null, //version from - first
                null, //version to - latest => all
                //1, //Return at maximum 1 item
                Int32.MaxValue, //Return maximum items
                boolIncludeChanges, // Include information on changes done
                false, // SlotMode
                boolIncludeDLInfo,
                //true //sort ascending - C1, C2, .. CLatest
                false
            );

            int insertHere = 0;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" />
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <meta charset=""utf-8"" /> 
    
    <meta http-equiv=""cache-control"" content=""max-age=0"" />
    <meta http-equiv=""cache-control"" content=""no-cache"" />
    <meta http-equiv=""expires"" content=""0"" />
    <meta http-equiv=""expires"" content=""Tue, 01 Jan 1980 1:00:00 GMT"" />
    <meta http-equiv=""pragma"" content=""no-cache"" />
    
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
    <!--<meta name = ""viewport"" content = ""width=device-width, initial-scale=0.90, minimum-scale=0.90, maximum-scale=0.90"" />-->
    <title>Changeset-List</title>
    <style type=""text/css"" media=""all"">
        table{border-collapse: collapse;}
        table, tr, td{border: 1px solid gray; color: black;font-weight: normal; text-transform: none; text-align: left;}
        table, tr, th{border: 1px solid gray; background-color: black; color: white; font-weight: bold; text-transform: uppercase; text-align: left; padding-left: 0.25cm; padding-right: 0.25cm; padding-top: 0.15cm; padding-bottom: 0.15cm;}

        #foo tr:nth-child(even) {background: #EDEDED}
        #foo tr:nth-child(odd) {background: #FFF}
        .dt{white-space: nowrap; width: 3cm; padding-left: 0.25cm; padding-right: 0.25cm;}
        .ct{width: 2.5cm; padding-left: 0.25cm; padding-right: 0.25cm;}
        #bar{border: none; width: 100%;}
        #bar tr:nth-child(even) {background: #BDD7EE}
        #bar tr:nth-child(odd) {background: #DDEBF7}
    </style>
</head>
<body>
");

            insertHere = sb.Length;

            

            sb.Append(@"
    <table id=""foo"">
        <thead>
            <tr>
                <th>Changeset</th>
                <th>Benutzer</th>
                <th>Ge&auml;ndert</th>
                <th>Kommentar</th>
                <th>&Auml;nderungen</th>
            </tr>
        </thead>
        <tbody>
");
            

            System.Collections.Generic.Dictionary<string, int> dictCommitStats = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            bool bCountCommits = false;
            
            foreach (Changeset c in changeSets)
            {
                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append(c.ChangesetId);
                sb.Append("</td>");


                if (bCountCommits)
                {
                    if (dictCommitStats.ContainsKey(c.CommitterDisplayName))
                        dictCommitStats[c.CommitterDisplayName] += 1;
                    else
                        dictCommitStats[c.CommitterDisplayName] = 1;
                }
                else
                {
                    if (dictCommitStats.ContainsKey(c.CommitterDisplayName))
                        dictCommitStats[c.CommitterDisplayName] += c.Changes.Length;
                    else
                        dictCommitStats[c.CommitterDisplayName] = c.Changes.Length;
                }
                


                sb.Append("<td class=\"white-space: nowrap;\">");
                sb.Append(HtmlEncode(c.CommitterDisplayName));
                sb.Append("</td>");

                sb.Append("<td class=\"dt\">");
                sb.Append(HtmlEncode(c.CreationDate.ToString("dddd, dd.MM.yyyy HH:mm")));
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(HtmlEncode(c.Comment));
                sb.Append("</td>");


                sb.Append("<td>");

                sb.Append("<table id=\"bar\"><tbody>");

                foreach (Change change in c.Changes)
                {
                    sb.Append("<tr>");


                    sb.Append("<td class=\"ct\">");
                    sb.Append(HtmlEncode(change.ChangeType.ToString()));
                    sb.Append("</td>");

                    sb.Append("<td class=\"dt\">");
                    sb.Append(HtmlEncode(change.Item.CheckinDate.ToString("dd.MM.yyyy HH:mm")));
                    sb.Append("</td>");

                    //sb.Append("<td>");
                    //sb.Append(change.Item.DownloadUrl);
                    //sb.Append("</td>");

                    //sb.Append("<td>");
                    //sb.Append(change.Item.ItemType);
                    //sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(HtmlEncode(change.Item.ServerItem));
                    sb.Append("</td>");

                    sb.Append("</tr>");

                }
                sb.Append("</tbody></table>");
                sb.Append("</td>");
                sb.Append("</tr>");

                // Stupid...
                //Changeset changeSet = vcs.GetChangeset(c.ChangesetId);
                //foreach (Change change in changeSet.Changes)
                //{
                //    // All sorts of juicy data in here
                //    System.Console.WriteLine(change);
                //}

                

            }


            sb.Append(@"
        </tbody>
    </table>
</body>
</html>");
            
            // System.Console.WriteLine(html);

            // dictCommitStats.Add("foo", Int32.MaxValue);
            // dictCommitStats.Add("bar", Int32.MinValue);
            // dictCommitStats.Add("Test", 1);
            
            System.Collections.Generic.List<KeyValuePair<string, int>> ls = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<string, int> kvp in dictCommitStats)
            {
                ls.Add(kvp);
            }

            //ls.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            //ls.Sort(delegate(KeyValuePair<string, int> pair1, KeyValuePair<string, int> pair2) { return -1 * pair1.Value.CompareTo(pair2.Value); });
            ls.Sort(delegate(KeyValuePair<string, int> pair1, KeyValuePair<string, int> pair2) { return pair1.Value.CompareTo(pair2.Value); });


            sb.Insert(insertHere, "    </ul>\r\n");
            foreach (KeyValuePair<string, int> pair in ls)
            {
                System.Globalization.NumberFormatInfo s_SwissNumberFormat = CreateSwissNumberFormatInfo();
                // System.Console.WriteLine("{0} commits by {1}.", pair.Value.ToString("N0", s_SwissNumberFormat).PadLeft(14, ' '), pair.Key);
                // System.Console.WriteLine("{0,14} commits by {1}.", pair.Value.ToString("N0", s_SwissNumberFormat), pair.Key);
                // System.Console.WriteLine(string.Format(s_SwissNumberFormat, "{0,14:N0} commits by {1}.", pair.Value, pair.Key));

                string changeType = bCountCommits ? "commits" : "changes";
                if(pair.Value == 1)
                    changeType = bCountCommits ? "commit" : "change";

                sb.Insert(insertHere, "        <li>" + HtmlEncode(string.Format(s_SwissNumberFormat, "{0,14:N0} {1} by {2}.", pair.Value, changeType, pair.Key)).Replace(" ", "&nbsp;") + "</li>\r\n");
            }
            sb.Insert(insertHere, "    <ul>\r\n");

            string html = sb.ToString();
            System.IO.File.WriteAllText(@"d:\commits.htm", html, System.Text.Encoding.UTF8);
            System.Console.WriteLine("Fertig");
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string HtmlEncode(string input)
        {
            return System.Web.HttpUtility.HtmlEncode(input);
        }


        private static System.Globalization.NumberFormatInfo CreateSwissNumberFormatInfo()
        {
            //System.Globalization.NumberFormatInfo nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
            System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
            nfi.NumberGroupSeparator = "'";
            nfi.NumberDecimalSeparator = ".";

            nfi.CurrencyGroupSeparator = "'";
            nfi.CurrencyDecimalSeparator = ".";
            nfi.CurrencySymbol = "CHF";

            return nfi;
        } // SetupNumberFormatInfo


        // http://stackoverflow.com/questions/1827651/how-do-you-get-the-latest-version-of-source-code-using-the-team-foundation-serve
        public static void DownloadFiles(string sourcePath, string targetPath)
        {
            // Connect to the team project collection and the server that hosts the version-control repository. 
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(
                new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion")
            );

            VersionControlServer sourceControl = tpc.GetService<VersionControlServer>();

            ItemSet items = sourceControl.GetItems(sourcePath, VersionSpec.Latest, RecursionType.Full);

            foreach (Item item in items.Items)
            {
                // build relative path
                string relativePath = FaultTolerantRelativePath(item.ServerItem, sourcePath);
                
                switch (item.ItemType)
                {
                    case ItemType.Any:
                        throw new ArgumentOutOfRangeException("ItemType returned was Any; expected File or Folder.");
                    case ItemType.File:
                        item.DownloadFile(System.IO.Path.Combine(targetPath, relativePath));
                        break;
                    case ItemType.Folder:
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(targetPath, relativePath));
                        break;
                } // End switch (item.ItemType)

            } // Next item

        } // End Sub DownloadFiles


        // http://msdn.microsoft.com/en-us/library/bb130331.aspx
        public static void WorkspaceGetter()
        {
            // Connect to the team project collection and the server that hosts the version-control repository. 
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(
                //new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion")
                new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion/")
            );

            VersionControlServer vcServer = tpc.GetService<VersionControlServer>();


            // Your workspace is a local copy of your team’s codebase. 
            // Workspace: Folder

            // Get the workspace that is mapped to c:\BuildProcessTemplate
            WorkspaceInfo wsInfo = Workstation.Current.GetLocalWorkspaceInfo(
               vcServer, @"WorkspaceName", @"UserName");
            Workspace ws = vcServer.GetWorkspace(wsInfo);

            // Update the workspace with most recent version of the files from the repository.
            GetStatus status = ws.Get();
            Console.Write("Conflicts: ");
            Console.WriteLine(status.NumConflicts);
        } // End Sub WorkspaceGetter


        // http://msdn.microsoft.com/en-us/library/bb130331.aspx
        public static void TfsItemList()
        {
            // Connect to the team project collection and the server that hosts the version-control repository. 
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(
                //new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion")
                new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion/")
            );
            VersionControlServer vcServer = tpc.GetService<VersionControlServer>();


            // List all of the .xaml files.
            ItemSet items = vcServer.GetItems("$/*.xaml", RecursionType.Full);
            foreach (Item item in items.Items)
            {
                Console.Write(item.ItemType.ToString());
                Console.Write(": ");
                Console.WriteLine(item.ServerItem.ToString());
            } // Next item

        } // End Sub TfsItemList


        public static void TfsMain()
        {
            // Connect to Team Foundation Server
            //     Server is the name of the server that is running the application tier for Team Foundation.
            //     Port is the port that Team Foundation uses. The default port is 8080.
            //     VDir is the virtual path to the Team Foundation application. The default path is tfs.
            //Uri tfsUri = (args.Length < 1) ? new Uri("http://corfoundation:8080/tfs") : new Uri(args[0]);
            //Uri tfsUri = (args.Length < 1) ? new Uri("http://Server:Port/VDir") : new Uri(args[0]);

            Uri tfsUri = new Uri("http://corfoundation:8080/tfs");


            TfsConfigurationServer configurationServer =
                TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);

            // Get the catalog of team project collections
            ReadOnlyCollection<CatalogNode> collectionNodes = configurationServer.CatalogNode.QueryChildren
            (
                new[] { CatalogResourceTypes.ProjectCollection }
                , false
                , CatalogQueryOptions.None
            );

            // List the team project collections
            foreach (CatalogNode collectionNode in collectionNodes)
            {
                // Use the InstanceId property to get the team project collection
                Guid collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection teamProjectCollection = configurationServer.GetTeamProjectCollection(collectionId);

                // Print the name of the team project collection
                Console.WriteLine("Collection: " + teamProjectCollection.Name);

                // Get a catalog of team projects for the collection
                ReadOnlyCollection<CatalogNode> projectNodes = collectionNode.QueryChildren(
                    new[] { CatalogResourceTypes.TeamProject }
                    ,false
                    ,CatalogQueryOptions.None
                );

                // List the team projects in the collection
                foreach (CatalogNode projectNode in projectNodes)
                {
                    Console.WriteLine(" Team Project: " + projectNode.Resource.DisplayName);
                } // Next projectNode

            } // Next collectionNode

        } // End Sub TfsMain 


        public static void SoundsInteresting()
        {
            // http://stackoverflow.com/questions/6764883/programatically-checkout-a-file-in-tfs-2010
            // http://msdn.microsoft.com/en-us/library/bb130331.aspx

            // http://stackoverflow.com/questions/618203/tfs-client-api-query-to-get-work-items-linked-to-a-specific-file


            // http://msdn.microsoft.com/en-us/magazine/jj553516.aspx


            // http://sortedbits.com/example-add-workitem-to-team-foundation-server/

            // Workspace (Arbeitsbereich):
            // http://msdn.microsoft.com/en-us/library/ms181383.aspx

        }


        // Crappy
        // http://www.codeproject.com/Questions/499216/ReadingplusaplusdocumentplusfromplusTFSplusserverp
        public static void CrappyKeepTrack()
        {
            Uri uri = new Uri("http://corfoundation:8080/tfs/COR-DEV-Produktion/");

            using (Microsoft.TeamFoundation.Client.TeamFoundationServer tfs = Microsoft.TeamFoundation.Client.TeamFoundationServerFactory.GetServer(uri))
            {
                Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore wit = (Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore)tfs.GetService(typeof(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore));

                Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemCollection result = wit.Query("SELECT * FROM WorkItems");
                foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem wi in result)
                {
                    foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Attachment attachment in wi.Attachments)
                    {
                        //do something
                        Console.WriteLine(attachment.Name);
                    } // Next attachment

                } // Next wi

            } // End Using tfs

        } // End Sub CrappyKeepTrack


    } // End Class TfsApplication


} // End Namespace TFSclient
